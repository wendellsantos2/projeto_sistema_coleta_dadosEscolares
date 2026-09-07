import React, { useState, useContext } from 'react';
import { AuthContext } from '../contexts/AuthContext';
import { LogIn, AlertCircle } from 'lucide-react';
import { useNavigate } from 'react-router-dom';

export default function Login() {
  const { login } = useContext(AuthContext);
  const navigate = useNavigate();
  const [email, setEmail] = useState('');
  const [senha, setSenha] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    try {
      await login(email, senha);
      navigate('/');
    } catch (err: any) {
      setError('Credenciais inválidas ou erro na conexão com a API.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-slate-100 flex items-center justify-center p-4">
      <div className="bg-white rounded-2xl shadow-xl p-8 w-full max-w-md">
        <div className="flex justify-center mb-8">
          <div className="bg-blue-600 p-4 rounded-full">
            <LogIn className="w-8 h-8 text-white" />
          </div>
        </div>
        
        <h2 className="text-2xl font-bold text-center text-slate-800 mb-8">
          Painel Gestor - Coleta Escolar
        </h2>

        {error && (
          <div className="mb-4 bg-red-50 p-4 rounded-lg flex items-center gap-3 text-red-700">
            <AlertCircle className="w-5 h-5 flex-shrink-0" />
            <p className="text-sm">{error}</p>
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-6">
          <div>
            <label className="block text-sm font-medium text-slate-700 mb-2">
              E-mail Administrativo
            </label>
            <input
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className="w-full px-4 py-3 rounded-lg border border-slate-300 focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
              placeholder="admin@coleta.com"
              required
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-slate-700 mb-2">
              Senha
            </label>
            <input
              type="password"
              value={senha}
              onChange={(e) => setSenha(e.target.value)}
              className="w-full px-4 py-3 rounded-lg border border-slate-300 focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors"
              placeholder="••••••••"
              required
            />
          </div>

          <button
            type="submit"
            disabled={loading}
            className="w-full bg-blue-600 hover:bg-blue-700 text-white font-semibold py-3 px-4 rounded-lg transition-colors flex justify-center items-center"
          >
            {loading ? 'Entrando...' : 'Acessar Painel'}
          </button>
        </form>

        <div className="mt-8 border-t border-slate-200 pt-6">
          <p className="text-sm text-slate-500 text-center mb-4">Login Rápido para Testes</p>
          <div className="flex flex-col gap-2">
            <button
              onClick={() => { setEmail('admin@coleta.com'); setSenha('admin123'); }}
              className="text-sm bg-slate-100 hover:bg-slate-200 text-slate-700 py-2 rounded transition-colors"
            >
              Preencher como <strong>ADMIN</strong>
            </button>
            <button
              onClick={() => { setEmail('gestor@coleta.com'); setSenha('admin123'); }}
              className="text-sm bg-slate-100 hover:bg-slate-200 text-slate-700 py-2 rounded transition-colors"
            >
              Preencher como <strong>GESTOR</strong>
            </button>
            <button
              onClick={() => { setEmail('pesquisador@coleta.com'); setSenha('admin123'); }}
              className="text-sm bg-slate-100 hover:bg-slate-200 text-slate-700 py-2 rounded transition-colors"
            >
              Preencher como <strong>PESQUISADOR</strong> (Acesso Negado na Web)
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
