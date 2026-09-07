import React, { useEffect, useState } from 'react';
import api from '../services/api';
import { ArrowLeft, Edit, Trash2, X } from 'lucide-react';
import { useNavigate } from 'react-router-dom';

export default function Alunos() {
  const navigate = useNavigate();
  const [alunos, setAlunos] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  
  // Modal State
  const [editingAluno, setEditingAluno] = useState<any | null>(null);
  const [formData, setFormData] = useState({
    nomeAluno: '',
    dataNascimento: '',
    sexo: '',
    cpfAluno: '',
    necessidadeEducacionalEspecial: false,
    descricaoNecessidade: ''
  });
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    fetchAlunos();
  }, []);

  async function fetchAlunos() {
    setLoading(true);
    try {
      const response = await api.get('/Alunos');
      setAlunos(response.data);
      setError('');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Erro ao carregar alunos.');
    } finally {
      setLoading(false);
    }
  }

  async function handleDelete(id: string) {
    if (window.confirm('Tem certeza que deseja excluir este aluno? Esta ação não pode ser desfeita.')) {
      try {
        await api.delete(`/Alunos/${id}`);
        fetchAlunos(); // recarrega a lista
      } catch (err: any) {
        if (err.response?.status === 403) {
          alert('Acesso Negado: Apenas Administradores podem excluir alunos.');
        } else {
          alert('Erro ao excluir aluno.');
        }
      }
    }
  }

  function handleEditClick(aluno: any) {
    setEditingAluno(aluno);
    setFormData({
      nomeAluno: aluno.nomeAluno,
      dataNascimento: aluno.dataNascimento ? aluno.dataNascimento.split('T')[0] : '',
      sexo: aluno.sexo,
      cpfAluno: aluno.cpfAluno || '',
      necessidadeEducacionalEspecial: aluno.necessidadeEducacionalEspecial,
      descricaoNecessidade: aluno.descricaoNecessidade || ''
    });
  }

  async function handleUpdateFormSubmit(e: React.FormEvent) {
    e.preventDefault();
    setSaving(true);
    try {
      await api.put(`/Alunos/${editingAluno.idAluno}`, formData);
      setEditingAluno(null);
      fetchAlunos(); // recarrega
    } catch (err: any) {
      alert(err.response?.data?.message || 'Erro ao atualizar aluno.');
    } finally {
      setSaving(false);
    }
  }

  if (loading && alunos.length === 0) {
    return <div className="min-h-screen flex items-center justify-center bg-slate-50">Carregando alunos...</div>;
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
            <h1 className="text-3xl font-bold text-slate-800">Gerenciar Alunos</h1>
            <p className="text-slate-500">Listagem, edição e exclusão de alunos cadastrados</p>
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
                  <th className="p-4">Nome do Aluno</th>
                  <th className="p-4">CPF</th>
                  <th className="p-4">Nascimento</th>
                  <th className="p-4">Sexo</th>
                  <th className="p-4 text-center">Ações</th>
                </tr>
              </thead>
              <tbody>
                {alunos.length === 0 ? (
                  <tr>
                    <td colSpan={5} className="p-8 text-center text-slate-500">
                      Nenhum aluno encontrado.
                    </td>
                  </tr>
                ) : (
                  alunos.map((aluno) => (
                    <tr key={aluno.idAluno} className="border-b border-slate-100 hover:bg-slate-50 transition-colors">
                      <td className="p-4 text-sm font-medium text-slate-800">{aluno.nomeAluno}</td>
                      <td className="p-4 text-sm text-slate-700">{aluno.cpfAluno || 'Não informado'}</td>
                      <td className="p-4 text-sm text-slate-700">
                        {aluno.dataNascimento ? new Date(aluno.dataNascimento).toLocaleDateString('pt-BR') : '-'}
                      </td>
                      <td className="p-4 text-sm text-slate-700">{aluno.sexo}</td>
                      <td className="p-4 text-center">
                        <button 
                          onClick={() => handleEditClick(aluno)}
                          className="p-2 text-blue-600 hover:bg-blue-50 rounded-full transition-colors mr-2"
                          title="Editar"
                        >
                          <Edit className="w-4 h-4" />
                        </button>
                        <button 
                          onClick={() => handleDelete(aluno.idAluno)}
                          className="p-2 text-red-600 hover:bg-red-50 rounded-full transition-colors"
                          title="Excluir"
                        >
                          <Trash2 className="w-4 h-4" />
                        </button>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        </div>

        {/* Edit Modal */}
        {editingAluno && (
          <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
            <div className="bg-white rounded-xl shadow-xl w-full max-w-lg overflow-hidden">
              <div className="flex justify-between items-center p-6 border-b border-slate-100 bg-slate-50">
                <h3 className="text-xl font-bold text-slate-800">Editar Aluno</h3>
                <button 
                  onClick={() => setEditingAluno(null)} 
                  className="text-slate-400 hover:text-slate-600"
                >
                  <X className="w-6 h-6" />
                </button>
              </div>
              <form onSubmit={handleUpdateFormSubmit} className="p-6">
                <div className="space-y-4">
                  <div>
                    <label className="block text-sm font-medium text-slate-700 mb-1">Nome do Aluno</label>
                    <input 
                      type="text" 
                      required
                      className="w-full p-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none"
                      value={formData.nomeAluno}
                      onChange={e => setFormData({...formData, nomeAluno: e.target.value})}
                    />
                  </div>
                  
                  <div className="grid grid-cols-2 gap-4">
                    <div>
                      <label className="block text-sm font-medium text-slate-700 mb-1">CPF</label>
                      <input 
                        type="text" 
                        maxLength={11}
                        className="w-full p-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none"
                        value={formData.cpfAluno}
                        onChange={e => setFormData({...formData, cpfAluno: e.target.value})}
                      />
                    </div>
                    <div>
                      <label className="block text-sm font-medium text-slate-700 mb-1">Data Nascimento</label>
                      <input 
                        type="date" 
                        required
                        className="w-full p-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none"
                        value={formData.dataNascimento}
                        onChange={e => setFormData({...formData, dataNascimento: e.target.value})}
                      />
                    </div>
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-slate-700 mb-1">Sexo</label>
                    <select 
                      required
                      className="w-full p-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none"
                      value={formData.sexo}
                      onChange={e => setFormData({...formData, sexo: e.target.value})}
                    >
                      <option value="">Selecione...</option>
                      <option value="M">Masculino</option>
                      <option value="F">Feminino</option>
                      <option value="Outro">Outro</option>
                    </select>
                  </div>

                  <div className="flex items-center gap-2 mt-4">
                    <input 
                      type="checkbox" 
                      id="nee"
                      className="w-4 h-4 text-blue-600 rounded"
                      checked={formData.necessidadeEducacionalEspecial}
                      onChange={e => setFormData({...formData, necessidadeEducacionalEspecial: e.target.checked})}
                    />
                    <label htmlFor="nee" className="text-sm font-medium text-slate-700">
                      Possui Necessidade Educacional Especial?
                    </label>
                  </div>

                  {formData.necessidadeEducacionalEspecial && (
                    <div>
                      <label className="block text-sm font-medium text-slate-700 mb-1">Descrição da Necessidade</label>
                      <input 
                        type="text" 
                        required
                        className="w-full p-2 border border-slate-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 outline-none"
                        value={formData.descricaoNecessidade}
                        onChange={e => setFormData({...formData, descricaoNecessidade: e.target.value})}
                      />
                    </div>
                  )}
                </div>

                <div className="mt-8 flex justify-end gap-3">
                  <button 
                    type="button"
                    onClick={() => setEditingAluno(null)}
                    className="px-4 py-2 text-slate-600 hover:bg-slate-100 rounded-lg font-medium transition-colors"
                  >
                    Cancelar
                  </button>
                  <button 
                    type="submit"
                    disabled={saving}
                    className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg font-medium shadow-sm transition-colors disabled:opacity-50"
                  >
                    {saving ? 'Salvando...' : 'Salvar Alterações'}
                  </button>
                </div>
              </form>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
