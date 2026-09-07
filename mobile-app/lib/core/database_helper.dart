import 'package:sqflite/sqflite.dart';
import 'package:path/path.dart';
import 'package:uuid/uuid.dart';

class DatabaseHelper {
  static final DatabaseHelper instance = DatabaseHelper._init();
  static Database? _database;

  DatabaseHelper._init();

  Future<Database> get database async {
    if (_database != null) return _database!;
    _database = await _initDB('coleta_escolar.db');
    return _database!;
  }

  Future<Database> _initDB(String filePath) async {
    final dbPath = await getDatabasesPath();
    final path = join(dbPath, filePath);

    return await openDatabase(
      path,
      version: 1,
      onCreate: _createDB,
    );
  }

  Future _createDB(Database db, int version) async {
    const idType = 'TEXT PRIMARY KEY';
    const textType = 'TEXT';
    const textNotNull = 'TEXT NOT NULL';
    const intType = 'INTEGER NOT NULL';
    const realType = 'REAL NOT NULL';
    const boolType = 'INTEGER NOT NULL';

    await db.execute('''
      CREATE TABLE familias (
        idFamilia $idType,
        endereco $textNotNull,
        bairro $textNotNull,
        comunidade $textNotNull,
        qtdMoradores $intType,
        rendaFamiliarMensal $realType,
        recebeBeneficioSocial $boolType,
        beneficioSocial $textType,
        possuiInternetCasa $boolType,
        tipoAcessoInternet $textType
      )
    ''');

    await db.execute('''
      CREATE TABLE alunos (
        idAluno $idType,
        idFamilia $textNotNull,
        nomeAluno $textNotNull,
        dataNascimento $textNotNull,
        sexo $textNotNull,
        cpfAluno $textType,
        necessidadeEducacionalEspecial $boolType,
        descricaoNecessidade $textType
      )
    ''');

    await db.execute('''
      CREATE TABLE responsaveis (
        idResponsavel $idType,
        nomeResponsavel $textNotNull,
        cpfResponsavel $textNotNull,
        telefoneResponsavel $textNotNull,
        emailResponsavel $textType
      )
    ''');

    await db.execute('''
      CREATE TABLE alunos_responsaveis (
        idAluno $textNotNull,
        idResponsavel $textNotNull,
        parentescoResponsavel $textNotNull,
        PRIMARY KEY (idAluno, idResponsavel)
      )
    ''');

    await db.execute('''
      CREATE TABLE matriculas (
        idMatricula $idType,
        idAluno $textNotNull,
        anoLetivo $intType,
        anoSerie $textNotNull,
        turno $textNotNull,
        frequenciaEscolarPct $realType,
        meioTransporteEscola $textNotNull,
        tempoDeslocamentoMin $intType
      )
    ''');

    await db.execute('''
      CREATE TABLE registros_coleta (
        idRegistro $idType,
        idAluno $textNotNull,
        idFamilia $textNotNull,
        observacao $textType,
        dataColeta $textNotNull,
        statusSincronizacao $textNotNull
      )
    ''');
  }

  Future close() async {
    final db = await instance.database;
    db.close();
  }

  Future<void> insertMockData() async {
    final db = await instance.database;
    
    final uuid = const Uuid();
    String idFamilia = uuid.v4();
    String idAluno = uuid.v4();
    String idResp = uuid.v4();
    String idMatricula = uuid.v4();
    String idRegistro = uuid.v4();

    await db.transaction((txn) async {
      await txn.insert('familias', {
        'idFamilia': idFamilia,
        'endereco': 'Rua Teste HTTP',
        'bairro': 'Bairro Mock',
        'comunidade': 'Comunidade Y',
        'qtdMoradores': 4,
        'rendaFamiliarMensal': 1500.0,
        'recebeBeneficioSocial': 1,
        'beneficioSocial': 'Bolsa Familia',
        'possuiInternetCasa': 1,
        'tipoAcessoInternet': 'Banda Larga'
      });

      await txn.insert('alunos', {
        'idAluno': idAluno,
        'idFamilia': idFamilia,
        'nomeAluno': 'Joaozinho Mock',
        'dataNascimento': '2015-05-10',
        'sexo': 'M',
        'cpfAluno': uuid.v4().replaceAll('-', '').substring(0, 11),
        'necessidadeEducacionalEspecial': 0,
        'descricaoNecessidade': ''
      });

      await txn.insert('responsaveis', {
        'idResponsavel': idResp,
        'nomeResponsavel': 'Joao Mock Pai',
        'cpfResponsavel': uuid.v4().replaceAll('-', '').substring(0, 11),
        'telefoneResponsavel': '999999999',
        'emailResponsavel': 'joaomock@coleta.com'
      });

      await txn.insert('alunos_responsaveis', {
        'idAluno': idAluno,
        'idResponsavel': idResp,
        'parentesco': 'Pai'
      });

      await txn.insert('matriculas', {
        'idMatricula': idMatricula,
        'idAluno': idAluno,
        'anoSerie': '5º Ano',
        'turno': 'Matutino',
        'frequenciaEscolarPct': 95.0,
        'meioTransporteEscola': 'A pe',
        'tempoDeslocamentoMin': 15
      });

      await txn.insert('registros_coleta', {
        'idRegistro': idRegistro,
        'idAluno': idAluno,
        'idFamilia': idFamilia,
        'observacao': 'Teste gerado pelo Mock Injector',
        'dataColeta': DateTime.now().toIso8601String(),
        'statusSincronizacao': 'PENDENTE'
      });
    });
  }
}
