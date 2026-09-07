import 'package:sqflite/sqflite.dart';
import '../core/database_helper.dart';
import 'api_client.dart';
import 'models/coleta_sync_dto.dart';

class SyncService {
  final ApiClient _apiClient = ApiClient();
  final DatabaseHelper _dbHelper = DatabaseHelper.instance;

  Future<void> syncPendingRecords() async {
    final db = await _dbHelper.database;
    
    // Pega os registros pendentes
    final registros = await db.query(
      'registros_coleta',
      where: 'statusSincronizacao = ?',
      whereArgs: ['PENDENTE'],
    );

    if (registros.isEmpty) return;

    List<Map<String, dynamic>> payload = [];

    for (var registro in registros) {
      final idRegistro = registro['idRegistro'] as String;
      final idAluno = registro['idAluno'] as String;
      final idFamilia = registro['idFamilia'] as String;
      final observacao = registro['observacao'] as String?;

      // Busca Familia
      final familiaMap = (await db.query('familias', where: 'idFamilia = ?', whereArgs: [idFamilia])).first;
      
      // Busca Aluno
      final alunoMap = (await db.query('alunos', where: 'idAluno = ?', whereArgs: [idAluno])).first;

      // Busca Vínculo Aluno-Responsável
      final vinculoMap = (await db.query('alunos_responsaveis', where: 'idAluno = ?', whereArgs: [idAluno])).first;
      final idResponsavel = vinculoMap['idResponsavel'] as String;
      
      // Busca Responsável
      final responsavelMap = (await db.query('responsaveis', where: 'idResponsavel = ?', whereArgs: [idResponsavel])).first;

      // Busca Matrícula (pode não existir em registros antigos)
      final matriculaList = await db.query('matriculas', where: 'idAluno = ?', whereArgs: [idAluno]);
      final matriculaMap = matriculaList.isNotEmpty ? matriculaList.first : null;

      final dto = ColetaSyncDto(
        idRegistro: idRegistro,
        idFamilia: idFamilia,
        idAluno: idAluno,
        nomeAluno: alunoMap['nomeAluno'] as String,
        dataNascimento: alunoMap['dataNascimento'] as String,
        sexo: alunoMap['sexo'] as String? ?? '',
        cpfAluno: alunoMap['cpfAluno'] as String? ?? '',
        necessidadeEducacionalEspecial: (alunoMap['necessidadeEducacionalEspecial'] as int? ?? 0) == 1,
        descricaoNecessidade: alunoMap['descricaoNecessidade'] as String? ?? '',
        nomeResponsavel: responsavelMap['nomeResponsavel'] as String,
        parentescoResponsavel: vinculoMap['parentescoResponsavel'] as String,
        cpfResponsavel: responsavelMap['cpfResponsavel'] as String? ?? '',
        telefoneResponsavel: responsavelMap['telefoneResponsavel'] as String? ?? '',
        emailResponsavel: responsavelMap['emailResponsavel'] as String? ?? '',
        endereco: familiaMap['endereco'] as String,
        bairro: familiaMap['bairro'] as String,
        comunidade: familiaMap['comunidade'] as String? ?? '',
        qtdMoradores: familiaMap['qtdMoradores'] as int? ?? 0,
        rendaFamiliarMensal: (familiaMap['rendaFamiliarMensal'] as num?)?.toDouble() ?? 0.0,
        recebeBeneficioSocial: (familiaMap['recebeBeneficioSocial'] as int? ?? 0) == 1,
        beneficioSocial: familiaMap['beneficioSocial'] as String? ?? '',
        possuiInternetCasa: (familiaMap['possuiInternetCasa'] as int? ?? 0) == 1,
        tipoAcessoInternet: familiaMap['tipoAcessoInternet'] as String? ?? '',
        meioTransporteEscola: matriculaMap?['meioTransporteEscola'] as String? ?? '',
        tempoDeslocamentoMin: matriculaMap?['tempoDeslocamentoMin'] as int? ?? 0,
        frequenciaEscolarPct: (matriculaMap?['frequenciaEscolarPct'] as num?)?.toDouble() ?? 0.0,
        anoSerie: matriculaMap?['anoSerie'] as String? ?? '',
        turno: matriculaMap?['turno'] as String? ?? '',
        observacao: observacao ?? '',
      );

      payload.add(dto.toJson());
    }

    try {
      await _apiClient.postList('/sync', payload);
      
      // Se sucesso, atualiza o status para SINCRONIZADO no banco local
      Batch batch = db.batch();
      for (var p in payload) {
        batch.update(
          'registros_coleta',
          {'statusSincronizacao': 'SINCRONIZADO'},
          where: 'idRegistro = ?',
          whereArgs: [p['idRegistro']],
        );
      }
      await batch.commit(noResult: true);
    } catch (e) {
      print('Erro ao sincronizar: $e');
      rethrow;
    }
  }
}
