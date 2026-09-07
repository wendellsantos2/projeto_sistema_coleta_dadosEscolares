import React, { useEffect, useState } from 'react';
import api from '../services/api';
import { ArrowLeft, CheckCircle, Clock, Eye, X, FileText, Download, Table } from 'lucide-react';
import { useNavigate } from 'react-router-dom';
import * as XLSX from 'xlsx';

function buildExportRow(r: any) {
  return {
    'id_registro': r.idRegistro,
    'id_familia': r.idFamilia || r.codigoFamilia || '',
    'id_aluno': 'ALU-' + r.idAluno.substring(0, 8).toUpperCase(),
    'nome_aluno': r.nomeAluno,
    'data_nascimento': r.dataNascimento ? new Date(r.dataNascimento).toLocaleDateString('pt-BR') : '',
    'sexo': r.sexo || '',
    'cpf_aluno': r.cpfAluno || '',
    'nome_responsavel': r.nomeResponsavel || '',
    'parentesco_responsavel': r.parentescoResponsavel || '',
    'cpf_responsavel': r.cpfResponsavel || '',
    'telefone_responsavel': r.telefoneResponsavel || '',
    'email_responsavel': r.emailResponsavel || '',
    'endereco': r.endereco || '',
    'bairro': r.bairro || '',
    'comunidade': r.comunidade || '',
    'qtd_moradores': r.qtdMoradores || 0,
    'renda_familiar_mensal': `R$ ${(r.rendaFamiliarMensal || 0).toFixed(2).replace('.', ',')}`,
    'recebe_beneficio_social': r.recebeBeneficioSocial ? 'Sim' : 'Não',
    'beneficio_social': r.beneficioSocial || '',
    'possui_internet_casa': r.possuiInternetCasa ? 'Sim' : 'Não',
    'tipo_acesso_internet': r.tipoAcessoInternet || '',
    'meio_transporte_escola': r.meioTransporteEscola || '',
    'tempo_deslocamento_min': r.tempoDeslocamentoMin || 0,
    'frequencia_escolar_pct': `${r.frequenciaEscolarPct || 0}%`,
    'ano_serie': r.anoSerie || '',
    'turno': r.turno || '',
    'necessidade_educacional_especial': r.necessidadeEducacionalEspecial ? 'Sim' : 'Não',
    'descricao_necessidade': r.descricaoNecessidade || '',
    'observacao': r.observacao || '',
  };
}

export default function Registros() {
  const navigate = useNavigate();
  const [registros, setRegistros] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [selectedRegistro, setSelectedRegistro] = useState<any>(null);
  const [showPreview, setShowPreview] = useState(false);

  const previewColumns = [
    'id_aluno', 'nome_aluno', 'sexo', 'data_nascimento', 'bairro',
    'turno', 'ano_serie', 'recebe_beneficio_social', 'possui_internet_casa',
  ];

  useEffect(() => {
    async function fetchRegistros() {
      try {
        const response = await api.get('/RegistrosColeta');
        setRegistros(response.data);
      } catch (err) {
        setError('Erro ao carregar registros.');
      } finally {
        setLoading(false);
      }
    }
    fetchRegistros();
  }, []);

  function exportToExcel() {
    const dataToExport = registros.map(buildExportRow);
    const ws = XLSX.utils.json_to_sheet(dataToExport);
    const wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'Registros');
    XLSX.writeFile(wb, 'Registros_Coleta.xlsx');
    setShowPreview(false);
  }

  if (loading) {
    return <div className="min-h-screen flex items-center justify-center bg-slate-50">Carregando...</div>;
  }

  return (
    <div className="min-h-screen bg-slate-50 p-8">
      <div className="max-w-6xl mx-auto">
        <div className="flex items-center gap-4 mb-8">
          <button 
            onClick={() => navigate('/')}
            className="p-2 hover:bg-slate-200 rounded-full transition-colors"
          >
            <ArrowLeft className="w-6 h-6 text-slate-700" />
          </button>
          <div className="flex-1">
            <h1 className="text-3xl font-bold text-slate-800">Registros Coletados</h1>
            <p className="text-slate-500">Listagem de todas as pesquisas realizadas em campo</p>
          </div>
          <button
            onClick={() => setShowPreview(true)}
            className="flex items-center gap-2 bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded-lg font-medium shadow-sm transition-colors"
          >
            <Download className="w-5 h-5" />
            Exportar Excel
          </button>
        </div>

        {error && (
          <div className="bg-red-50 text-red-600 p-4 rounded-lg mb-6">
            {error}
          </div>
        )}

        <div className="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden">
          <div className="overflow-x-auto">
            <table className="w-full text-left border-collapse">
              <thead>
                <tr className="bg-slate-50 border-b border-slate-200 text-sm font-semibold text-slate-600">
                  <th className="p-4">Data da Coleta</th>
                  <th className="p-4">Código Aluno</th>
                  <th className="p-4">Aluno</th>
                  <th className="p-4">Pesquisador</th>
                  <th className="p-4">Status</th>
                  <th className="p-4">Ações</th>
                </tr>
              </thead>
              <tbody>
                {registros.length === 0 ? (
                  <tr>
                    <td colSpan={6} className="p-8 text-center text-slate-500">
                      Nenhum registro encontrado.
                    </td>
                  </tr>
                ) : (
                  registros.map((reg) => (
                    <tr key={reg.idRegistro} className="border-b border-slate-100 hover:bg-slate-50 transition-colors">
                      <td className="p-4 text-sm text-slate-700">
                        {new Date(reg.dataColeta).toLocaleDateString('pt-BR')} <br/>
                        <span className="text-xs text-slate-400">{new Date(reg.dataColeta).toLocaleTimeString('pt-BR')}</span>
                      </td>
                      <td className="p-4 text-sm font-medium text-slate-800">
                        ALU-{reg.idAluno.substring(0, 8).toUpperCase()}
                      </td>
                      <td className="p-4 text-sm text-slate-700">{reg.nomeAluno}</td>
                      <td className="p-4 text-sm text-slate-700">{reg.pesquisadorNome}</td>
                      <td className="p-4">
                        {reg.statusSincronizacao === 'SINCRONIZADO' ? (
                          <span className="inline-flex items-center gap-1 bg-green-100 text-green-700 px-2 py-1 rounded-full text-xs font-medium">
                            <CheckCircle className="w-3 h-3" /> Sincronizado
                          </span>
                        ) : (
                          <span className="inline-flex items-center gap-1 bg-yellow-100 text-yellow-700 px-2 py-1 rounded-full text-xs font-medium">
                            <Clock className="w-3 h-3" /> Pendente
                          </span>
                        )}
                      </td>
                      <td className="p-4">
                        <button 
                          onClick={() => setSelectedRegistro(reg)}
                          title="Ver Informações"
                          className="p-2 text-indigo-600 hover:bg-indigo-50 hover:text-indigo-800 rounded-full transition-colors"
                        >
                          <Eye className="w-5 h-5" />
                        </button>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        </div>
      </div>

      {/* ===== MODAL PRÉ-VISUALIZAÇÃO EXCEL ===== */}
      {showPreview && registros.length > 0 && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 p-4">
          <div className="bg-white rounded-xl shadow-2xl w-full max-w-6xl max-h-[90vh] flex flex-col">
            <div className="sticky top-0 bg-white border-b border-slate-100 p-5 flex justify-between items-center rounded-t-xl">
              <div className="flex items-center gap-3">
                <div className="bg-green-100 text-green-600 p-2 rounded-lg">
                  <FileText className="w-6 h-6" />
                </div>
                <div>
                  <h2 className="text-xl font-bold text-slate-800">Pré-visualização da Planilha</h2>
                  <p className="text-xs text-slate-400">
                    {registros.length} registros &bull; {Object.keys(buildExportRow(registros[0])).length} colunas no arquivo final
                  </p>
                </div>
              </div>
              <button onClick={() => setShowPreview(false)} className="text-slate-400 hover:text-slate-600 hover:bg-slate-100 p-2 rounded-full transition-colors">
                <X className="w-6 h-6" />
              </button>
            </div>

            <div className="overflow-auto flex-1 p-4">
              <p className="text-xs text-slate-500 mb-3 italic">
                Prévia com colunas principais. O arquivo Excel final conterá <strong>todas as {Object.keys(buildExportRow(registros[0])).length} colunas</strong>.
              </p>
              <table className="w-full text-xs border border-slate-200 rounded-lg overflow-hidden">
                <thead>
                  <tr className="bg-indigo-50">
                    {previewColumns.map(col => (
                      <th key={col} className="p-2 border border-slate-200 text-left text-indigo-700 font-semibold whitespace-nowrap">
                        {col}
                      </th>
                    ))}
                  </tr>
                </thead>
                <tbody>
                  {registros.map((r, i) => {
                    const row = buildExportRow(r);
                    return (
                      <tr key={i} className={i % 2 === 0 ? 'bg-white' : 'bg-slate-50'}>
                        {previewColumns.map(col => (
                          <td key={col} className="p-2 border border-slate-100 text-slate-700 whitespace-nowrap max-w-[200px] overflow-hidden text-ellipsis">
                            {String((row as any)[col] ?? '')}
                          </td>
                        ))}
                      </tr>
                    );
                  })}
                </tbody>
              </table>
            </div>

            <div className="bg-slate-50 p-4 border-t border-slate-200 flex justify-end gap-3 rounded-b-xl">
              <button
                onClick={() => setShowPreview(false)}
                className="border border-slate-300 text-slate-700 px-5 py-2 rounded-lg font-medium hover:bg-slate-100 transition-colors"
              >
                Cancelar
              </button>
              <button
                onClick={exportToExcel}
                className="flex items-center gap-2 bg-green-600 hover:bg-green-700 text-white px-6 py-2 rounded-lg font-bold shadow-sm transition-colors"
              >
                <Download className="w-5 h-5" />
                Confirmar e Baixar Excel
              </button>
            </div>
          </div>
        </div>
      )}

      {/* ===== MODAL DETALHES COMPLETO ===== */}
      {selectedRegistro && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4">
          <div className="bg-white rounded-xl shadow-2xl w-full max-w-3xl max-h-[90vh] overflow-y-auto">
            <div className="sticky top-0 bg-white border-b border-slate-100 p-6 flex justify-between items-center z-10">
              <div className="flex items-center gap-3">
                <div className="bg-indigo-100 text-indigo-600 p-2 rounded-lg">
                  <FileText className="w-6 h-6" />
                </div>
                <div>
                  <h2 className="text-xl font-bold text-slate-800">Detalhes da Coleta</h2>
                  <p className="text-xs text-slate-400">
                    ALU-{selectedRegistro.idAluno.substring(0, 8).toUpperCase()} &bull; {selectedRegistro.pesquisadorNome}
                  </p>
                </div>
              </div>
              <button onClick={() => setSelectedRegistro(null)} className="text-slate-400 hover:text-slate-600 hover:bg-slate-100 p-2 rounded-full transition-colors">
                <X className="w-6 h-6" />
              </button>
            </div>

            <div className="p-6 space-y-5">

              <DetailSection title="👤 Dados do Aluno">
                <DetailRow label="Nome" value={selectedRegistro.nomeAluno} />
                <DetailRow label="Código" value={`ALU-${selectedRegistro.idAluno.substring(0, 8).toUpperCase()}`} />
                <DetailRow label="CPF" value={selectedRegistro.cpfAluno} />
                <DetailRow label="Sexo" value={selectedRegistro.sexo === 'M' ? 'Masculino' : selectedRegistro.sexo === 'F' ? 'Feminino' : selectedRegistro.sexo} />
                <DetailRow label="Data de Nasc." value={selectedRegistro.dataNascimento ? new Date(selectedRegistro.dataNascimento).toLocaleDateString('pt-BR') : null} />
                <DetailRow label="Nec. Especial?" value={selectedRegistro.necessidadeEducacionalEspecial ? 'Sim' : 'Não'} />
                {selectedRegistro.necessidadeEducacionalEspecial && (
                  <DetailRow label="Descrição NEE" value={selectedRegistro.descricaoNecessidade} />
                )}
              </DetailSection>

              <DetailSection title="👨‍👩‍👧 Dados do Responsável">
                <DetailRow label="Nome" value={selectedRegistro.nomeResponsavel} />
                <DetailRow label="Parentesco" value={selectedRegistro.parentescoResponsavel} />
                <DetailRow label="CPF" value={selectedRegistro.cpfResponsavel} />
                <DetailRow label="Telefone" value={selectedRegistro.telefoneResponsavel} />
                <DetailRow label="E-mail" value={selectedRegistro.emailResponsavel} />
              </DetailSection>

              <DetailSection title="🏠 Dados da Família">
                <DetailRow label="Endereço" value={selectedRegistro.endereco} />
                <DetailRow label="Bairro" value={selectedRegistro.bairro} />
                <DetailRow label="Comunidade" value={selectedRegistro.comunidade} />
                <DetailRow label="Qtd. Moradores" value={selectedRegistro.qtdMoradores} />
                <DetailRow label="Renda Mensal" value={new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(selectedRegistro.rendaFamiliarMensal || 0)} />
                <DetailRow label="Benefício Social" value={selectedRegistro.recebeBeneficioSocial ? (selectedRegistro.beneficioSocial || 'Sim') : 'Não recebe'} />
                <DetailRow label="Internet em Casa" value={selectedRegistro.possuiInternetCasa ? `Sim (${selectedRegistro.tipoAcessoInternet || 'não especificado'})` : 'Não possui'} />
              </DetailSection>

              <DetailSection title="🎓 Escolaridade e Transporte">
                <DetailRow label="Ano/Série" value={selectedRegistro.anoSerie} />
                <DetailRow label="Turno" value={selectedRegistro.turno} />
                <DetailRow label="Frequência Escolar" value={selectedRegistro.frequenciaEscolarPct != null ? `${selectedRegistro.frequenciaEscolarPct}%` : null} />
                <DetailRow label="Meio de Transporte" value={selectedRegistro.meioTransporteEscola} />
                <DetailRow label="Tempo Deslocamento" value={selectedRegistro.tempoDeslocamentoMin != null ? `${selectedRegistro.tempoDeslocamentoMin} min` : null} />
              </DetailSection>

              <DetailSection title="📋 Informações da Coleta">
                <DetailRow label="Pesquisador" value={selectedRegistro.pesquisadorNome} />
                <DetailRow label="Data da Coleta" value={new Date(selectedRegistro.dataColeta).toLocaleString('pt-BR')} />
                <DetailRow label="Status" value={selectedRegistro.statusSincronizacao} />
                <DetailRow label="Sincronizado Em" value={selectedRegistro.sincronizadoEm ? new Date(selectedRegistro.sincronizadoEm).toLocaleString('pt-BR') : 'Pendente'} />
                {selectedRegistro.observacao && <DetailRow label="Observação" value={selectedRegistro.observacao} />}
              </DetailSection>

            </div>

            <div className="bg-slate-50 p-4 border-t border-slate-200 flex justify-end rounded-b-xl">
              <button
                onClick={() => setSelectedRegistro(null)}
                className="bg-white border border-slate-300 text-slate-700 px-6 py-2 rounded-lg font-medium hover:bg-slate-50 transition-colors"
              >
                Fechar
              </button>
            </div>
          </div>
        </div>
      )}

    </div>
  );
}

function DetailSection({ title, children }: { title: string; children: React.ReactNode }) {
  return (
    <div className="bg-slate-50 p-4 rounded-xl border border-slate-100">
      <h3 className="text-sm font-bold text-slate-600 uppercase tracking-wider mb-3 pb-2 border-b border-slate-200">{title}</h3>
      <div className="grid grid-cols-1 sm:grid-cols-2 gap-x-8 gap-y-3">
        {children}
      </div>
    </div>
  );
}

function DetailRow({ label, value }: { label: string; value: any }) {
  if (value === null || value === undefined || value === '') return null;
  return (
    <div>
      <p className="text-xs text-slate-400 font-medium">{label}</p>
      <p className="text-slate-800 font-medium text-sm">{String(value)}</p>
    </div>
  );
}
