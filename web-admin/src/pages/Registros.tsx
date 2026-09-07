import React, { useEffect, useState } from 'react';
import api from '../services/api';
import { ArrowLeft, CheckCircle, Clock } from 'lucide-react';
import { useNavigate } from 'react-router-dom';

export default function Registros() {
  const navigate = useNavigate();
  const [registros, setRegistros] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

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
                  <th className="p-4">Código Família</th>
                  <th className="p-4">Aluno</th>
                  <th className="p-4">Pesquisador</th>
                  <th className="p-4">Status</th>
                </tr>
              </thead>
              <tbody>
                {registros.length === 0 ? (
                  <tr>
                    <td colSpan={5} className="p-8 text-center text-slate-500">
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
                      <td className="p-4 text-sm font-medium text-slate-800">{reg.codigoFamilia}</td>
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
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  );
}
