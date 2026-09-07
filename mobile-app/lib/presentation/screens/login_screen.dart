import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../providers/auth_provider.dart';

class LoginScreen extends StatefulWidget {
  const LoginScreen({Key? key}) : super(key: key);

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  final _emailController = TextEditingController();
  final _passwordController = TextEditingController();

  @override
  Widget build(BuildContext context) {
    final auth = context.watch<AuthProvider>();

    return Scaffold(
      body: Center(
        child: SingleChildScrollView(
          child: Padding(
            padding: const EdgeInsets.all(24.0),
            child: Column(
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
              const Icon(Icons.school, size: 80, color: Colors.blue),
              const SizedBox(height: 20),
              const Text(
                'Coleta Escolar',
                style: TextStyle(fontSize: 24, fontWeight: FontWeight.bold),
              ),
              const SizedBox(height: 40),
              TextField(
                controller: _emailController,
                decoration: const InputDecoration(
                  labelText: 'Email',
                  border: OutlineInputBorder(),
                ),
                keyboardType: TextInputType.emailAddress,
              ),
              const SizedBox(height: 16),
              TextField(
                controller: _passwordController,
                decoration: const InputDecoration(
                  labelText: 'Senha',
                  border: OutlineInputBorder(),
                ),
                obscureText: true,
              ),
              const SizedBox(height: 24),
              SizedBox(
                width: double.infinity,
                height: 50,
                child: ElevatedButton(
                  onPressed: auth.isLoading
                      ? null
                      : () async {
                          final success = await context.read<AuthProvider>().login(
                                _emailController.text,
                                _passwordController.text,
                              );
                          if (!success && mounted) {
                            ScaffoldMessenger.of(context).showSnackBar(
                              const SnackBar(content: Text('Login falhou. Verifique as credenciais e a conexao.')),
                            );
                          }
                        },
                  child: auth.isLoading
                      ? const CircularProgressIndicator()
                      : const Text('Entrar'),
                ),
              ),
              const SizedBox(height: 30),
              const Divider(),
              const SizedBox(height: 10),
              const Text('Login Rápido para Testes', style: TextStyle(color: Colors.grey)),
              const SizedBox(height: 10),
              Wrap(
                spacing: 10,
                runSpacing: 10,
                alignment: WrapAlignment.center,
                children: [
                  OutlinedButton(
                    onPressed: () {
                      _emailController.text = 'admin@coleta.com';
                      _passwordController.text = 'admin123';
                    },
                    child: const Text('ADMIN'),
                  ),
                  OutlinedButton(
                    onPressed: () {
                      _emailController.text = 'gestor@coleta.com';
                      _passwordController.text = 'admin123';
                    },
                    child: const Text('GESTOR'),
                  ),
                  OutlinedButton(
                    onPressed: () {
                      _emailController.text = 'pesquisador@coleta.com';
                      _passwordController.text = 'admin123';
                    },
                    child: const Text('PESQUISADOR'),
                  ),
                ],
              ),
            ],
          ),
        ),
      ),
    ),
  );
}
}
