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
  final _cpfAlunoController = TextEditingController();
  final _dataNascController = TextEditingController();
  final _descNecessidadeController = TextEditingController();
  final _nomeRespController = TextEditingController();
  final _cpfRespController = TextEditingController();
  final _telefoneRespController = TextEditingController();
  final _emailRespController = TextEditingController();
  final _parentescoController = TextEditingController();
  final _enderecoController = TextEditingController();
  final _bairroController = TextEditingController();
  final _comunidadeController = TextEditingController();
  final _qtdMoradoresController = TextEditingController();
  final _rendaController = TextEditingController();
  final _beneficioDescController = TextEditingController();
  final _tipoInternetController = TextEditingController();
  
  final _anoSerieController = TextEditingController();
  final _meioTransporteController = TextEditingController();
  final _tempoDeslocamentoController = TextEditingController();
  final _frequenciaController = TextEditingController();

  String _sexo = 'M';
  String _turno = 'Matutino';
  
  bool _recebeBeneficio = false;
  bool _necessidadeEspecial = false;
  bool _possuiInternet = false;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.grey[50],
      appBar: AppBar(
        title: const Text('Nova Coleta', style: TextStyle(fontWeight: FontWeight.bold)),
        elevation: 0,
        flexibleSpace: Container(
          decoration: const BoxDecoration(
            gradient: LinearGradient(
              colors: [Colors.deepPurple, Colors.indigo],
              begin: Alignment.topLeft,
              end: Alignment.bottomRight,
            ),
          ),
        ),
        foregroundColor: Colors.white,
        actions: [
          IconButton(
            icon: const Icon(Icons.auto_fix_high),
            tooltip: 'Preencher com Mock',
            onPressed: () {
              setState(() {
                final rng = Random();
                _nomeAlunoController.text = 'Aluno Mock ${rng.nextInt(1000)}';
                _dataNascController.text = '2015-05-10';
                _nomeRespController.text = 'Responsável Mock ${rng.nextInt(1000)}';
                _cpfRespController.text = '${rng.nextInt(900) + 100}${rng.nextInt(900) + 100}${rng.nextInt(900) + 100}${rng.nextInt(90) + 10}';
                _parentescoController.text = 'Pai';
                _enderecoController.text = 'Rua Teste HTTP';
                _bairroController.text = 'Bairro Mock';
                _comunidadeController.text = 'Comunidade Y';
                _rendaController.text = '1500';
                _recebeBeneficio = true;
                _cpfAlunoController.text = '${rng.nextInt(900) + 100}${rng.nextInt(900) + 100}${rng.nextInt(900) + 100}11';
                _descNecessidadeController.text = '';
                _telefoneRespController.text = '92999999999';
                _emailRespController.text = 'mock@email.com';
                _qtdMoradoresController.text = '4';
                _beneficioDescController.text = 'Bolsa Família';
                _tipoInternetController.text = 'Wi-Fi / Fibra';
                _anoSerieController.text = '5º Ano Fundamental';
                _meioTransporteController.text = 'Ônibus';
                _tempoDeslocamentoController.text = '30';
                _frequenciaController.text = '90';
                _sexo = 'M';
                _turno = 'Matutino';
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
                        _buildTextField(_cpfAlunoController, 'CPF do Aluno', Icons.badge),
                        _buildDropdown('Sexo', _sexo, ['M', 'F'], (val) => setState(() => _sexo = val!)),
                        _buildTextField(_dataNascController, 'Data de Nascimento (YYYY-MM-DD)', Icons.calendar_today),
                        SwitchListTile(
                          contentPadding: EdgeInsets.zero,
                          title: const Text('Necessidade Educacional Especial?'),
                          value: _necessidadeEspecial,
                          onChanged: (v) => setState(() => _necessidadeEspecial = v),
                          activeColor: Colors.deepPurple,
                        ),
                        if (_necessidadeEspecial)
                          _buildTextField(_descNecessidadeController, 'Descrição da Necessidade', Icons.healing),
                      ],
                    ),
                    const SizedBox(height: 16),
                    _buildSectionCard(
                      title: 'Escolaridade e Transporte',
                      icon: Icons.school,
                      children: [
                        _buildTextField(_anoSerieController, 'Ano/Série', Icons.grade),
                        _buildDropdown('Turno', _turno, ['Matutino', 'Vespertino', 'Noturno', 'Integral'], (val) => setState(() => _turno = val!)),
                        _buildTextField(_frequenciaController, 'Frequência Escolar (%)', Icons.percent, keyboardType: TextInputType.number),
                        _buildTextField(_meioTransporteController, 'Meio de Transporte', Icons.directions_bus),
                        _buildTextField(_tempoDeslocamentoController, 'Tempo Deslocamento (Min)', Icons.timer, keyboardType: TextInputType.number),
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
                        _buildTextField(_telefoneRespController, 'Telefone', Icons.phone, keyboardType: TextInputType.phone),
                        _buildTextField(_emailRespController, 'Email', Icons.email, keyboardType: TextInputType.emailAddress, isRequired: false),
                      ],
                    ),
                    const SizedBox(height: 16),
                    _buildSectionCard(
                      title: 'Dados da Família',
                      icon: Icons.home,
                      children: [
                        _buildTextField(_enderecoController, 'Endereço', Icons.location_on),
                        _buildTextField(_bairroController, 'Bairro', Icons.map),
                        _buildTextField(_comunidadeController, 'Comunidade', Icons.location_city, isRequired: false),
                        _buildTextField(_qtdMoradoresController, 'Qtd de Moradores na Casa', Icons.people, keyboardType: TextInputType.number),
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
                          activeColor: Colors.deepPurple,
                        ),
                        if (_recebeBeneficio)
                          _buildTextField(_beneficioDescController, 'Qual Benefício?', Icons.card_giftcard),
                        SwitchListTile(
                          contentPadding: EdgeInsets.zero,
                          title: const Text('Possui Internet em Casa?'),
                          value: _possuiInternet,
                          onChanged: (v) => setState(() => _possuiInternet = v),
                          activeColor: Colors.deepPurple,
                        ),
                        if (_possuiInternet)
                          _buildTextField(_tipoInternetController, 'Tipo de Acesso (Wi-Fi, 4G, etc)', Icons.wifi),
                      ],
                    ),
                    const SizedBox(height: 32),
                    ElevatedButton.icon(
                      style: ElevatedButton.styleFrom(
                        padding: const EdgeInsets.symmetric(vertical: 16),
                        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
                        backgroundColor: Colors.deepPurple,
                        foregroundColor: Colors.white,
                        elevation: 4,
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
                  style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold, color: Colors.deepPurple),
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

  Widget _buildDropdown(String label, String value, List<String> items, Function(String?) onChanged) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 16.0),
      child: DropdownButtonFormField<String>(
        value: value,
        decoration: InputDecoration(
          labelText: label,
          border: OutlineInputBorder(
            borderRadius: BorderRadius.circular(16),
            borderSide: BorderSide.none,
          ),
          filled: true,
          fillColor: Colors.grey.shade100,
        ),
        items: items.map((e) => DropdownMenuItem(value: e, child: Text(e))).toList(),
        onChanged: onChanged,
      ),
    );
  }

  Widget _buildTextField(TextEditingController controller, String label, IconData icon, {TextInputType? keyboardType, bool isRequired = true}) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 16.0),
      child: TextFormField(
        controller: controller,
        decoration: InputDecoration(
          labelText: label,
          prefixIcon: Icon(icon, color: Colors.grey),
          border: OutlineInputBorder(
            borderRadius: BorderRadius.circular(16),
            borderSide: BorderSide.none,
          ),
          filled: true,
          fillColor: Colors.grey.shade100,
        ),
        keyboardType: keyboardType,
        validator: (val) {
          if (isRequired && (val == null || val.trim().isEmpty)) return 'Obrigatório';
          return null;
        },
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
        'qtdMoradores': int.tryParse(_qtdMoradoresController.text) ?? 1,
        'rendaFamiliarMensal': double.tryParse(_rendaController.text) ?? 0.0,
        'recebeBeneficioSocial': _recebeBeneficio ? 1 : 0,
        'beneficioSocial': _recebeBeneficio ? _beneficioDescController.text : null,
        'possuiInternetCasa': _possuiInternet ? 1 : 0,
        'tipoAcessoInternet': _possuiInternet ? _tipoInternetController.text : null,
      });

      // Responsavel
      batch.insert('responsaveis', {
        'idResponsavel': idResponsavel,
        'nomeResponsavel': _nomeRespController.text,
        'cpfResponsavel': _cpfRespController.text,
        'telefoneResponsavel': _telefoneRespController.text,
        'emailResponsavel': _emailRespController.text,
      });

      // Aluno
      batch.insert('alunos', {
        'idAluno': idAluno,
        'idFamilia': idFamilia,
        'nomeAluno': _nomeAlunoController.text,
        'dataNascimento': _dataNascController.text,
        'sexo': _sexo,
        'cpfAluno': _cpfAlunoController.text,
        'necessidadeEducacionalEspecial': _necessidadeEspecial ? 1 : 0,
        'descricaoNecessidade': _necessidadeEspecial ? _descNecessidadeController.text : '',
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
        'anoSerie': _anoSerieController.text,
        'turno': _turno,
        'frequenciaEscolarPct': double.tryParse(_frequenciaController.text) ?? 0.0,
        'meioTransporteEscola': _meioTransporteController.text,
        'tempoDeslocamentoMin': int.tryParse(_tempoDeslocamentoController.text) ?? 0,
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
