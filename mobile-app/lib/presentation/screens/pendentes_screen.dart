import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../providers/sync_provider.dart';

class PendentesScreen extends StatelessWidget {
  const PendentesScreen({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    final syncProvider = context.watch<SyncProvider>();

    return Scaffold(
      backgroundColor: Colors.grey[100],
      appBar: AppBar(
        title: const Text('Coletas Pendentes', style: TextStyle(fontWeight: FontWeight.bold)),
        elevation: 0,
      ),
      body: FutureBuilder<List<Map<String, dynamic>>>(
        future: syncProvider.getPendingRecordsList(),
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(child: CircularProgressIndicator());
          }

          if (snapshot.hasError) {
            return Center(child: Text('Erro ao carregar dados: ${snapshot.error}'));
          }

          final pendentes = snapshot.data ?? [];

          if (pendentes.isEmpty) {
            return Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Icon(Icons.check_circle_outline, size: 64, color: Colors.green[300]),
                  const SizedBox(height: 16),
                  const Text('Nenhuma coleta pendente!', style: TextStyle(fontSize: 18, color: Colors.blueGrey)),
                ],
              ),
            );
          }

          return ListView.builder(
            padding: const EdgeInsets.all(16.0),
            itemCount: pendentes.length,
            itemBuilder: (context, index) {
              final record = pendentes[index];
              // Format dataColeta if needed, right now it's ISO string
              final dataColeta = record['dataColeta'] as String;
              final nomeAluno = record['nomeAluno'] as String;

              return Card(
                elevation: 2,
                shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                margin: const EdgeInsets.only(bottom: 12),
                child: ListTile(
                  leading: const CircleAvatar(
                    backgroundColor: Colors.orange,
                    child: Icon(Icons.sync_problem, color: Colors.white),
                  ),
                  title: Text(nomeAluno, style: const TextStyle(fontWeight: FontWeight.bold)),
                  subtitle: Text('Coletado em: ${dataColeta.split('T').first}'),
                  trailing: const Icon(Icons.chevron_right, color: Colors.grey),
                ),
              );
            },
          );
        },
      ),
    );
  }
}
