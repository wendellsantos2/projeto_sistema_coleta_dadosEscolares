import React, { useEffect, useState } from 'react';
import api from '../services/api';
import { ArrowLeft, CheckCircle, Clock, Eye, X, FileText } from 'lucide-react';
import { useNavigate } from 'react-router-dom';

export default function Registros() {
  const navigate = useNavigate();
  const [registros, setRegistros] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [selectedRegistro, setSelectedRegistro] = useState<any>(null);

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
          <div>
            <h1 className="text-3xl font-bold text-slate-800">Registros Coletados</h1>
            <p className="text-slate-500">Listagem de todas as pesquisas realizadas em campo</p>
          </div>
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

      {/* Modal de Detalhes */}
      {selectedRegistro && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4">
          <div className="bg-white rounded-xl shadow-2xl w-full max-w-3xl max-h-[90vh] overflow-y-auto">
            <div className="sticky top-0 bg-white border-b border-slate-100 p-6 flex justify-between items-center z-10">
              <div className="flex items-center gap-3">
                <div className="bg-indigo-100 text-indigo-600 p-2 rounded-lg">
                  <FileText className="w-6 h-6" />
                </div>
                <h2 className="text-2xl font-bold text-slate-800">Detalhes da Coleta</h2>
              </div>
              <button 
                onClick={() => setSelectedRegistro(null)} 
                className="text-slate-400 hover:text-slate-600 hover:bg-slate-100 p-2 rounded-full transition-colors"
              >
                <X className="w-6 h-6" />
              </button>
            </div>
            
            <div className="p-6">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                
                {/* Aluno Info */}
                <div className="bg-slate-50 p-4 rounded-lg border border-slate-100">
                  <h3 className="text-sm font-bold text-slate-500 uppercase tracking-wider mb-4 border-b pb-2">Dados do Aluno</h3>
                  <div className="space-y-3">
                    <div>
                      <p className="text-xs text-slate-500">Nome do Aluno</p>
                      <p className="text-slate-800 font-medium">{selectedRegistro.nomeAluno}</p>
                    </div>
                    <div className="flex justify-between">
                      <div>
                        <p className="text-xs text-slate-500">Código do Aluno</p>
                        <p className="text-slate-800 font-medium">ALU-{selectedRegistro.idAluno.substring(0, 8).toUpperCase()}</p>
                      </div>
                      <div>
                        <p className="text-xs text-slate-500">CPF</p>
                        <p className="text-slate-800 font-medium">{selectedRegistro.cpfAluno || 'Não informado'}</p>
                      </div>
                    </div>
                    <div className="flex justify-between">
                      <div>
                        <p className="text-xs text-slate-500">Data de Nasc.</p>
                        <p className="text-slate-800 font-medium">
                          {selectedRegistro.dataNascimento 
                            ? new Date(selectedRegistro.dataNascimento).toLocaleDateString('pt-BR') 
                            : 'Não informado'}
                        </p>
                      </div>
                      <div>
                        <p className="text-xs text-slate-500">Nec. Especial?</p>
                        <p className="text-slate-800 font-medium">{selectedRegistro.necessidadeEducacionalEspecial ? 'Sim' : 'Não'}</p>
                      </div>
                    </div>
                  </div>
                </div>

                {/* Familia Info */}
                <div className="bg-slate-50 p-4 rounded-lg border border-slate-100">
                  <h3 className="text-sm font-bold text-slate-500 uppercase tracking-wider mb-4 border-b pb-2">Dados da Família</h3>
                  <div className="space-y-3">
                    <div>
                      <p className="text-xs text-slate-500">Endereço Completo</p>
                      <p className="text-slate-800 font-medium">
                        {selectedRegistro.endereco}, {selectedRegistro.bairro} 
                        {selectedRegistro.comunidade ? ` - ${selectedRegistro.comunidade}` : ''}
                      </p>
                    </div>
                    <div className="flex justify-between">
                      <div>
                        <p className="text-xs text-slate-500">Renda Mensal</p>
                        <p className="text-slate-800 font-medium">
                          {new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(selectedRegistro.rendaFamiliarMensal || 0)}
                        </p>
                      </div>
                      <div>
                        <p className="text-xs text-slate-500">Benefício Social</p>
                        <p className="text-slate-800 font-medium">
                          {selectedRegistro.recebeBeneficioSocial ? selectedRegistro.beneficioSocial : 'Não recebe'}
                        </p>
                      </div>
                    </div>
                    <div>
                      <p className="text-xs text-slate-500">Acesso à Internet</p>
                      <p className="text-slate-800 font-medium">
                        {selectedRegistro.possuiInternetCasa 
                          ? `Sim (${selectedRegistro.tipoAcessoInternet || 'Banda Larga'})` 
                          : 'Não possui'}
                      </p>
                    </div>
                  </div>
                </div>

                {/* Coleta Info */}
                <div className="bg-slate-50 p-4 rounded-lg border border-slate-100">
                  <h3 className="text-sm font-bold text-slate-500 uppercase tracking-wider mb-4 border-b pb-2">Informações da Coleta</h3>
                  <div className="space-y-3">
                    <div>
                      <p className="text-xs text-slate-500">Pesquisador Responsável</p>
                      <p className="text-slate-800 font-medium">{selectedRegistro.pesquisadorNome}</p>
                    </div>
                    <div>
                      <p className="text-xs text-slate-500">Data da Coleta</p>
                      <p className="text-slate-800 font-medium">
                        {new Date(selectedRegistro.dataColeta).toLocaleString('pt-BR')}
                      </p>
                    </div>
                    <div>
                      <p className="text-xs text-slate-500">Data de Sincronização</p>
                      <p className="text-slate-800 font-medium">
                        {selectedRegistro.sincronizadoEm 
                          ? new Date(selectedRegistro.sincronizadoEm).toLocaleString('pt-BR') 
                          : 'Pendente'}
                      </p>
                    </div>
                  </div>
                </div>

                {/* Observações */}
                <div className="col-span-1 md:col-span-2 bg-slate-50 p-4 rounded-lg border border-slate-100">
                  <h3 className="text-sm font-bold text-slate-500 uppercase tracking-wider mb-4 border-b pb-2">Observações</h3>
                  <p className="text-slate-700 whitespace-pre-wrap">
                    {selectedRegistro.observacao || 'Nenhuma observação registrada.'}
                  </p>
                </div>

              </div>
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
