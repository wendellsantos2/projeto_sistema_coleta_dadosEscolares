import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../providers/sync_provider.dart';

class TotalLocalScreen extends StatelessWidget {
  const TotalLocalScreen({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    final syncProvider = context.watch<SyncProvider>();

    return Scaffold(
      backgroundColor: Colors.grey[100],
      appBar: AppBar(
        title: const Text('Todos os Registros', style: TextStyle(fontWeight: FontWeight.bold)),
        elevation: 0,
      ),
      body: FutureBuilder<List<Map<String, dynamic>>>(
        future: syncProvider.getTotalRecordsList(),
        builder: (context, snapshot) {
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(child: CircularProgressIndicator());
          }

          if (snapshot.hasError) {
            return Center(child: Text('Erro ao carregar dados: ${snapshot.error}'));
          }

          final registros = snapshot.data ?? [];

          if (registros.isEmpty) {
            return Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Icon(Icons.folder_open, size: 64, color: Colors.grey[400]),
                  const SizedBox(height: 16),
                  const Text('Nenhum registro local encontrado!', style: TextStyle(fontSize: 18, color: Colors.blueGrey)),
                ],
              ),
            );
          }

          return ListView.builder(
            padding: const EdgeInsets.all(16.0),
            itemCount: registros.length,
            itemBuilder: (context, index) {
              final record = registros[index];
              final dataColeta = record['dataColeta'] as String;
              final nomeAluno = record['nomeAluno'] as String;
              final status = record['statusSincronizacao'] as String;

              final isPendente = status == 'PENDENTE';

              return Card(
                elevation: 2,
                shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                margin: const EdgeInsets.only(bottom: 12),
                child: ListTile(
                  leading: CircleAvatar(
                    backgroundColor: isPendente ? Colors.orange : Colors.green,
                    child: Icon(
                      isPendente ? Icons.sync_problem : Icons.check, 
                      color: Colors.white
                    ),
                  ),
                  title: Text(nomeAluno, style: const TextStyle(fontWeight: FontWeight.bold)),
                  subtitle: Text('Coletado em: ${dataColeta.split('T').first}'),
                  trailing: Container(
                    padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                    decoration: BoxDecoration(
                      color: isPendente ? Colors.orange[100] : Colors.green[100],
                      borderRadius: BorderRadius.circular(12),
                    ),
                    child: Text(
                      isPendente ? 'Pendente' : 'Sincronizado',
                      style: TextStyle(
                        fontSize: 12,
                        fontWeight: FontWeight.bold,
                        color: isPendente ? Colors.orange[800] : Colors.green[800],
                      ),
                    ),
                  ),
                ),
              );
            },
          );
        },
      ),
    );
  }
}
