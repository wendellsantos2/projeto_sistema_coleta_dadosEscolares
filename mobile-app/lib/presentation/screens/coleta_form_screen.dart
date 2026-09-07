import 'package:flutter/material.dart';
import 'package:uuid/uuid.dart';
import 'package:sqflite/sqflite.dart';
import 'dart:math';
import '../../core/database_helper.dart';

class ColetaFormScreen extends StatefulWidget {
  const ColetaFormScreen({Key? key}) : super(key: key);

  @override
  State<ColetaFormScreen> createState() => _ColetaFormScreenState();
}

class _ColetaFormScreenState extends State<ColetaFormScreen> {
  final _formKey = GlobalKey<FormState>();
  final _uuid = const Uuid();
  bool _isSaving = false;

  // Form controllers
  final _nomeAlunoController = TextEditingController();
  final _dataNascController = TextEditingController();
  final _nomeRespController = TextEditingController();
  final _cpfRespController = TextEditingController();
  final _parentescoController = TextEditingController();
  final _enderecoController = TextEditingController();
  final _bairroController = TextEditingController();
  final _comunidadeController = TextEditingController();
  final _rendaController = TextEditingController();
  
  bool _recebeBeneficio = false;
  bool _necessidadeEspecial = false;
  bool _possuiInternet = false;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.grey[100],
      appBar: AppBar(
        title: const Text('Nova Coleta', style: TextStyle(fontWeight: FontWeight.bold)),
        elevation: 0,
        actions: [
          IconButton(
            icon: const Icon(Icons.auto_fix_high),
            tooltip: 'Preencher com Mock',
            onPressed: () {
              setState(() {
                _nomeAlunoController.text = 'Joaozinho Mock';
                _dataNascController.text = '2015-05-10';
                _nomeRespController.text = 'Joao Mock Pai';
                final rng = Random();
                _cpfRespController.text = '${rng.nextInt(900) + 100}${rng.nextInt(900) + 100}${rng.nextInt(900) + 100}${rng.nextInt(90) + 10}';
                _parentescoController.text = 'Pai';
                _enderecoController.text = 'Rua Teste HTTP';
                _bairroController.text = 'Bairro Mock';
                _comunidadeController.text = 'Comunidade Y';
                _rendaController.text = '1500';
                _recebeBeneficio = true;
                _necessidadeEspecial = false;
                _possuiInternet = true;
              });
              ScaffoldMessenger.of(context).showSnackBar(
                const SnackBar(content: Text('Formulario preenchido com mock!')),
              );
            },
          )
        ],
      ),
      body: _isSaving
          ? const Center(child: CircularProgressIndicator())
          : SingleChildScrollView(
              padding: const EdgeInsets.all(16.0),
              child: Form(
                key: _formKey,
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    _buildSectionCard(
                      title: 'Dados do Aluno',
                      icon: Icons.person,
                      children: [
                        _buildTextField(_nomeAlunoController, 'Nome do Aluno', Icons.badge),
                        _buildTextField(_dataNascController, 'Data de Nascimento (YYYY-MM-DD)', Icons.calendar_today),
                        SwitchListTile(
                          contentPadding: EdgeInsets.zero,
                          title: const Text('Necessidade Educacional Especial?'),
                          value: _necessidadeEspecial,
                          onChanged: (v) => setState(() => _necessidadeEspecial = v),
                          activeColor: Colors.blue,
                        ),
                      ],
                    ),
                    const SizedBox(height: 16),
                    _buildSectionCard(
                      title: 'Dados do Responsável',
                      icon: Icons.supervisor_account,
                      children: [
                        _buildTextField(_nomeRespController, 'Nome do Responsável', Icons.person_outline),
                        _buildTextField(_cpfRespController, 'CPF do Responsável', Icons.credit_card),
                        _buildTextField(_parentescoController, 'Parentesco (Ex: Mãe, Pai)', Icons.family_restroom),
                      ],
                    ),
                    const SizedBox(height: 16),
                    _buildSectionCard(
                      title: 'Dados da Família',
                      icon: Icons.home,
                      children: [
                        _buildTextField(_enderecoController, 'Endereço', Icons.location_on),
                        _buildTextField(_bairroController, 'Bairro', Icons.map),
                        _buildTextField(_comunidadeController, 'Comunidade', Icons.location_city),
                        _buildTextField(
                          _rendaController,
                          'Renda Familiar Mensal (R\$)',
                          Icons.attach_money,
                          keyboardType: TextInputType.number,
                        ),
                        SwitchListTile(
                          contentPadding: EdgeInsets.zero,
                          title: const Text('Recebe Benefício Social?'),
                          value: _recebeBeneficio,
                          onChanged: (v) => setState(() => _recebeBeneficio = v),
                          activeColor: Colors.blue,
                        ),
                        SwitchListTile(
                          contentPadding: EdgeInsets.zero,
                          title: const Text('Possui Internet em Casa?'),
                          value: _possuiInternet,
                          onChanged: (v) => setState(() => _possuiInternet = v),
                          activeColor: Colors.blue,
                        ),
                      ],
                    ),
                    const SizedBox(height: 32),
                    ElevatedButton.icon(
                      style: ElevatedButton.styleFrom(
                        padding: const EdgeInsets.symmetric(vertical: 16),
                        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                        backgroundColor: Colors.green,
                        foregroundColor: Colors.white,
                        elevation: 2,
                      ),
                      onPressed: _salvarColetaLocal,
                      icon: const Icon(Icons.save),
                      label: const Text('SALVAR COLETA LOCALMENTE', style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold)),
                    ),
                    const SizedBox(height: 24),
                  ],
                ),
              ),
            ),
    );
  }

  Widget _buildSectionCard({required String title, required IconData icon, required List<Widget> children}) {
    return Card(
      elevation: 2,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(16)),
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Icon(icon, color: Colors.blueGrey, size: 28),
                const SizedBox(width: 12),
                Text(
                  title,
                  style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold, color: Colors.blueGrey),
                ),
              ],
            ),
            const Divider(height: 24, thickness: 1),
            ...children,
          ],
        ),
      ),
    );
  }

  Widget _buildTextField(TextEditingController controller, String label, IconData icon, {TextInputType? keyboardType}) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 16.0),
      child: TextFormField(
        controller: controller,
        decoration: InputDecoration(
          labelText: label,
          prefixIcon: Icon(icon, color: Colors.grey),
          border: OutlineInputBorder(
            borderRadius: BorderRadius.circular(12),
          ),
          filled: true,
          fillColor: Colors.grey[50],
        ),
        keyboardType: keyboardType,
        validator: (val) => val == null || val.trim().isEmpty ? 'Obrigatório' : null,
      ),
    );
  }

  Future<void> _salvarColetaLocal() async {
    if (!_formKey.currentState!.validate()) return;
    
    setState(() => _isSaving = true);

    try {
      final db = await DatabaseHelper.instance.database;
      final batch = db.batch();

      final idFamilia = _uuid.v4();
      final idAluno = _uuid.v4();
      final idResponsavel = _uuid.v4();
      final idRegistro = _uuid.v4();
      final idMatricula = _uuid.v4();
      final dataAtual = DateTime.now().toIso8601String();

      // Familia
      batch.insert('familias', {
        'idFamilia': idFamilia,
        'endereco': _enderecoController.text,
        'bairro': _bairroController.text,
        'comunidade': _comunidadeController.text,
        'qtdMoradores': 3, // mock
        'rendaFamiliarMensal': double.tryParse(_rendaController.text) ?? 0.0,
        'recebeBeneficioSocial': _recebeBeneficio ? 1 : 0,
        'beneficioSocial': _recebeBeneficio ? 'Bolsa Familia' : null, // mock
        'possuiInternetCasa': _possuiInternet ? 1 : 0,
        'tipoAcessoInternet': _possuiInternet ? 'Banda Larga' : null, // mock
      });

      // Responsavel
      batch.insert('responsaveis', {
        'idResponsavel': idResponsavel,
        'nomeResponsavel': _nomeRespController.text,
        'cpfResponsavel': _cpfRespController.text,
        'telefoneResponsavel': '00000000000', // mock
        'emailResponsavel': '',
      });

      // Aluno
      batch.insert('alunos', {
        'idAluno': idAluno,
        'idFamilia': idFamilia,
        'nomeAluno': _nomeAlunoController.text,
        'dataNascimento': _dataNascController.text,
        'sexo': 'M', // mock
        'cpfAluno': '${Random().nextInt(900) + 100}${Random().nextInt(900) + 100}${Random().nextInt(900) + 100}${Random().nextInt(90) + 10}',
        'necessidadeEducacionalEspecial': _necessidadeEspecial ? 1 : 0,
        'descricaoNecessidade': '',
      });

      // Vinculo Aluno Responsavel
      batch.insert('alunos_responsaveis', {
        'idAluno': idAluno,
        'idResponsavel': idResponsavel,
        'parentescoResponsavel': _parentescoController.text,
      });

      // Matricula
      batch.insert('matriculas', {
        'idMatricula': idMatricula,
        'idAluno': idAluno,
        'anoLetivo': 2026,
        'anoSerie': '1 ano Ensino Medio', // mock
        'turno': 'Matutino', // mock
        'frequenciaEscolarPct': 100.0, // mock
        'meioTransporteEscola': 'Onibus Escolar', // mock
        'tempoDeslocamentoMin': 30, // mock
      });

      // Registro de Coleta
      batch.insert('registros_coleta', {
        'idRegistro': idRegistro,
        'idAluno': idAluno,
        'idFamilia': idFamilia,
        'observacao': 'Coletado offline',
        'dataColeta': dataAtual,
        'statusSincronizacao': 'PENDENTE',
      });

      await batch.commit(noResult: true);
      
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Coleta salva offline com sucesso!')),
        );
        Navigator.pop(context);
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Erro: $e')),
        );
        setState(() => _isSaving = false);
      }
    }
  }
}
