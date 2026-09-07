import 'package:flutter/material.dart';
import 'package:sqflite/sqflite.dart';
import 'dart:async';
import '../../data/sync_service.dart';
import '../../core/database_helper.dart';

class SyncProvider extends ChangeNotifier {
  final SyncService _syncService = SyncService();
  bool _isSyncing = false;
  int _pendingCount = 0;
  int _totalCount = 0;
  bool _autoSyncEnabled = false;
  Timer? _syncTimer;

  bool get isSyncing => _isSyncing;
  int get pendingCount => _pendingCount;
  int get totalCount => _totalCount;
  bool get autoSyncEnabled => _autoSyncEnabled;

  Future<void> checkPending() async {
    final db = await DatabaseHelper.instance.database;
    final count = Sqflite.firstIntValue(await db.rawQuery(
      "SELECT COUNT(*) FROM registros_coleta WHERE statusSincronizacao = 'PENDENTE'"
    ));
    _pendingCount = count ?? 0;

    final countTotal = Sqflite.firstIntValue(await db.rawQuery(
      "SELECT COUNT(*) FROM registros_coleta"
    ));
    _totalCount = countTotal ?? 0;
    
    notifyListeners();
  }

  Future<List<Map<String, dynamic>>> getPendingRecordsList() async {
    final db = await DatabaseHelper.instance.database;
    final List<Map<String, dynamic>> result = await db.rawQuery('''
      SELECT r.idRegistro, r.dataColeta, a.nomeAluno, r.statusSincronizacao 
      FROM registros_coleta r
      INNER JOIN alunos a ON r.idAluno = a.idAluno
      WHERE r.statusSincronizacao = 'PENDENTE'
      ORDER BY r.dataColeta DESC
    ''');
    return result;
  }

  Future<List<Map<String, dynamic>>> getTotalRecordsList() async {
    final db = await DatabaseHelper.instance.database;
    final List<Map<String, dynamic>> result = await db.rawQuery('''
      SELECT r.idRegistro, r.dataColeta, a.nomeAluno, r.statusSincronizacao 
      FROM registros_coleta r
      INNER JOIN alunos a ON r.idAluno = a.idAluno
      ORDER BY r.dataColeta DESC
    ''');
    return result;
  }

  Future<void> syncData() async {
    if (_pendingCount == 0) return;

    _isSyncing = true;
    notifyListeners();

    try {
      await _syncService.syncPendingRecords();
    } catch (e) {
      print('Erro no sync: $e');
      rethrow;
    } finally {
      _isSyncing = false;
      await checkPending();
    }
  }

  Future<void> clearAllData() async {
    final db = await DatabaseHelper.instance.database;
    await db.transaction((txn) async {
      await txn.delete('registros_coleta');
      await txn.delete('matriculas');
      await txn.delete('alunos_responsaveis');
      await txn.delete('responsaveis');
      await txn.delete('alunos');
      await txn.delete('familias');
    });
    await checkPending();
  }

  void toggleAutoSync(bool value) {
    _autoSyncEnabled = value;
    if (_autoSyncEnabled) {
      _syncTimer?.cancel();
      _syncTimer = Timer.periodic(const Duration(seconds: 30), (timer) {
        if (!_isSyncing && _pendingCount > 0) {
          syncData();
        }
      });
    } else {
      _syncTimer?.cancel();
      _syncTimer = null;
    }
    notifyListeners();
  }

  @override
  void dispose() {
    _syncTimer?.cancel();
    super.dispose();
  }
}
