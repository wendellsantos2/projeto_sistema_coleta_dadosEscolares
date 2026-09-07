import 'dart:convert';
import 'package:http/http.dart' as http;
import 'package:shared_preferences/shared_preferences.dart';
import '../../core/constants.dart';

class ApiClient {
  Future<String?> _getToken() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getString('jwt_token');
  }

  Future<Map<String, dynamic>> post(String endpoint, Map<String, dynamic> body, {bool requiresAuth = true}) async {
    final url = Uri.parse('${AppConstants.baseUrl}$endpoint');
    final headers = {'Content-Type': 'application/json'};

    if (requiresAuth) {
      final token = await _getToken();
      if (token != null) {
        headers['Authorization'] = 'Bearer $token';
      }
    }

    final response = await http.post(url, headers: headers, body: jsonEncode(body));
    return _processResponse(response);
  }

  Future<Map<String, dynamic>> postList(String endpoint, List<Map<String, dynamic>> body, {bool requiresAuth = true}) async {
    final url = Uri.parse('${AppConstants.baseUrl}$endpoint');
    final headers = {'Content-Type': 'application/json'};

    if (requiresAuth) {
      final token = await _getToken();
      if (token != null) {
        headers['Authorization'] = 'Bearer $token';
      }
    }

    final response = await http.post(url, headers: headers, body: jsonEncode(body));
    return _processResponse(response);
  }

  Map<String, dynamic> _processResponse(http.Response response) {
    if (response.statusCode >= 200 && response.statusCode < 300) {
      if (response.body.isNotEmpty) {
        return jsonDecode(response.body);
      }
      return {};
    } else {
      throw Exception('Erro ${response.statusCode}: ${response.body}');
    }
  }
}
