class ColetaSyncDto {
  final String idRegistro;
  final String idFamilia;
  final String idAluno;
  
  // Aluno
  final String nomeAluno;
  final String dataNascimento;
  final String sexo;
  final String? cpfAluno;
  final bool necessidadeEducacionalEspecial;
  final String? descricaoNecessidade;

  // Responsavel
  final String nomeResponsavel;
  final String parentescoResponsavel;
  final String cpfResponsavel;
  final String telefoneResponsavel;
  final String? emailResponsavel;

  // Familia
  final String endereco;
  final String bairro;
  final String comunidade;
  final int qtdMoradores;
  final double rendaFamiliarMensal;
  final bool recebeBeneficioSocial;
  final String? beneficioSocial;
  final bool possuiInternetCasa;
  final String? tipoAcessoInternet;

  // Matricula
  final String meioTransporteEscola;
  final int tempoDeslocamentoMin;
  final double frequenciaEscolarPct;
  final String anoSerie;
  final String turno;

  // Registro
  final String? observacao;

  ColetaSyncDto({
    required this.idRegistro,
    required this.idFamilia,
    required this.idAluno,
    required this.nomeAluno,
    required this.dataNascimento,
    required this.sexo,
    this.cpfAluno,
    required this.necessidadeEducacionalEspecial,
    this.descricaoNecessidade,
    required this.nomeResponsavel,
    required this.parentescoResponsavel,
    required this.cpfResponsavel,
    required this.telefoneResponsavel,
    this.emailResponsavel,
    required this.endereco,
    required this.bairro,
    required this.comunidade,
    required this.qtdMoradores,
    required this.rendaFamiliarMensal,
    required this.recebeBeneficioSocial,
    this.beneficioSocial,
    required this.possuiInternetCasa,
    this.tipoAcessoInternet,
    required this.meioTransporteEscola,
    required this.tempoDeslocamentoMin,
    required this.frequenciaEscolarPct,
    required this.anoSerie,
    required this.turno,
    this.observacao,
  });

  Map<String, dynamic> toJson() {
    return {
      'idRegistro': idRegistro,
      'idFamilia': idFamilia,
      'idAluno': idAluno,
      'nomeAluno': nomeAluno,
      'dataNascimento': dataNascimento,
      'sexo': sexo,
      'cpfAluno': cpfAluno,
      'necessidadeEducacionalEspecial': necessidadeEducacionalEspecial,
      'descricaoNecessidade': descricaoNecessidade,
      'nomeResponsavel': nomeResponsavel,
      'parentescoResponsavel': parentescoResponsavel,
      'cpfResponsavel': cpfResponsavel,
      'telefoneResponsavel': telefoneResponsavel,
      'emailResponsavel': emailResponsavel,
      'endereco': endereco,
      'bairro': bairro,
      'comunidade': comunidade,
      'qtdMoradores': qtdMoradores,
      'rendaFamiliarMensal': rendaFamiliarMensal,
      'recebeBeneficioSocial': recebeBeneficioSocial,
      'beneficioSocial': beneficioSocial,
      'possuiInternetCasa': possuiInternetCasa,
      'tipoAcessoInternet': tipoAcessoInternet,
      'meioTransporteEscola': meioTransporteEscola,
      'tempoDeslocamentoMin': tempoDeslocamentoMin,
      'frequenciaEscolarPct': frequenciaEscolarPct,
      'anoSerie': anoSerie,
      'turno': turno,
      'observacao': observacao,
    };
  }
}
