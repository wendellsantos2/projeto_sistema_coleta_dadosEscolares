import React, { useEffect, useState, useContext } from 'react';
import { AuthContext } from '../contexts/AuthContext';
import api from '../services/api';
import { Users, Home, TrendingUp, AlertTriangle, LogOut, FileText } from 'lucide-react';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, PieChart, Pie, Cell, Legend } from 'recharts';
import { useNavigate } from 'react-router-dom';

const COLORS = ['#0088FE', '#00C49F', '#FFBB28', '#FF8042', '#8884d8'];

export default function Dashboard() {
  const { logout } = useContext(AuthContext);
  const navigate = useNavigate();
  const [data, setData] = useState<any>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    async function loadData() {
      try {
        const response = await api.get('/RegistrosColeta/dashboard');
        setData(response.data);
      } catch (err) {
        setError('Erro ao carregar dados do dashboard.');
      } finally {
        setLoading(false);
      }
    }
    loadData();
  }, []);

  if (loading) {
    return <div className="min-h-screen flex items-center justify-center bg-slate-50 text-slate-600">Carregando painel...</div>;
  }

  if (error) {
    return <div className="min-h-screen flex items-center justify-center bg-slate-50 text-red-500 font-bold">{error}</div>;
  }

  return (
    <div className="min-h-screen bg-slate-100 p-8">
      {/* Header */}
      <div className="flex justify-between items-center mb-8">
        <div>
          <h1 className="text-3xl font-bold text-slate-800">Visão Geral</h1>
          <p className="text-slate-500">Indicadores consolidados da pesquisa escolar</p>
        </div>
        <div className="flex gap-4">
          <button 
            onClick={() => navigate('/alunos')}
            className="flex items-center gap-2 bg-indigo-50 px-4 py-2 rounded-lg shadow text-indigo-700 hover:bg-indigo-100 transition-colors"
          >
            <Users className="w-5 h-5" />
            Gerenciar Alunos
          </button>
          <button 
            onClick={() => navigate('/registros')}
            className="flex items-center gap-2 bg-white px-4 py-2 rounded-lg shadow text-slate-600 hover:text-blue-600 transition-colors"
          >
            <FileText className="w-5 h-5" />
            Ver Todas Informações
          </button>
          <button 
            onClick={logout}
            className="flex items-center gap-2 bg-red-50 px-4 py-2 rounded-lg text-red-600 hover:bg-red-100 transition-colors"
          >
            <LogOut className="w-5 h-5" />
            Sair
          </button>
        </div>
      </div>

      {/* KPI Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <KPICard icon={<Users />} title="Total de Alunos" value={data.totalAlunos} color="text-blue-600" bg="bg-blue-100" />
        <KPICard icon={<Home />} title="Famílias Pesquisadas" value={data.totalFamilias} color="text-green-600" bg="bg-green-100" />
        <KPICard icon={<TrendingUp />} title="Freq. Média Geral" value={`${data.frequenciaMediaGeral}%`} color="text-purple-600" bg="bg-purple-100" />
        <KPICard icon={<AlertTriangle />} title="Alunos com NEE" value={data.alunosComNecessidadeEspecial} color="text-orange-600" bg="bg-orange-100" />
      </div>

      {/* Charts Row 1 */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-6">
        
        {/* Gráfico de Meio de Transporte */}
        <div className="bg-white p-6 rounded-xl shadow-sm border border-slate-200">
          <h2 className="text-lg font-bold text-slate-700 mb-4">Meio de Transporte Utilizado</h2>
          <div className="h-[300px] w-full">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={data.distribuicaoTransporte}>
                <CartesianGrid strokeDasharray="3 3" vertical={false} />
                <XAxis dataKey="tipo" />
                <YAxis />
                <Tooltip cursor={{fill: 'transparent'}} />
                <Bar dataKey="quantidade" fill="#3b82f6" radius={[4, 4, 0, 0]} />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Gráfico de Renda */}
        <div className="bg-white p-6 rounded-xl shadow-sm border border-slate-200">
          <h2 className="text-lg font-bold text-slate-700 mb-4">Distribuição de Renda Familiar</h2>
          <div className="h-[300px] w-full">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={data.distribuicaoRenda} layout="vertical">
                <CartesianGrid strokeDasharray="3 3" horizontal={false} />
                <XAxis type="number" />
                <YAxis dataKey="faixa" type="category" width={120} />
                <Tooltip cursor={{fill: 'transparent'}} />
                <Bar dataKey="quantidade" fill="#10b981" radius={[0, 4, 4, 0]} />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </div>
      </div>

      {/* Charts Row 2 */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        
        {/* Gráfico de Turno (Pie) */}
        <div className="bg-white p-6 rounded-xl shadow-sm border border-slate-200">
          <h2 className="text-sm font-bold text-slate-700 mb-4 text-center">Turnos de Estudo</h2>
          <div className="h-[200px] w-full">
            <ResponsiveContainer width="100%" height="100%">
              <PieChart>
                <Pie
                  data={data.distribuicaoTurno}
                  cx="50%"
                  cy="50%"
                  innerRadius={50}
                  outerRadius={80}
                  dataKey="quantidade"
                  nameKey="turno"
                  labelLine={false}
                >
                  {data.distribuicaoTurno.map((entry: any, index: number) => (
                    <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                  ))}
                </Pie>
                <Tooltip />
                <Legend />
              </PieChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Gráfico de Benefícios Sociais (Pie) */}
        <div className="bg-white p-6 rounded-xl shadow-sm border border-slate-200">
          <h2 className="text-sm font-bold text-slate-700 mb-4 text-center">Benefícios Sociais</h2>
          <div className="h-[200px] w-full">
            <ResponsiveContainer width="100%" height="100%">
              <PieChart>
                <Pie
                  data={data.distribuicaoBeneficios}
                  cx="50%"
                  cy="50%"
                  innerRadius={50}
                  outerRadius={80}
                  dataKey="quantidade"
                  nameKey="beneficio"
                  labelLine={false}
                >
                  {data.distribuicaoBeneficios.map((entry: any, index: number) => (
                    <Cell key={`cell-${index}`} fill={COLORS[(index + 2) % COLORS.length]} />
                  ))}
                </Pie>
                <Tooltip />
                <Legend />
              </PieChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Gráfico Acesso à Internet (Pie) */}
        <div className="bg-white p-6 rounded-xl shadow-sm border border-slate-200">
          <h2 className="text-sm font-bold text-slate-700 mb-4 text-center">Acesso à Internet</h2>
          <div className="h-[200px] w-full">
            <ResponsiveContainer width="100%" height="100%">
              <PieChart>
                <Pie
                  data={[
                    { name: 'Sem Internet', value: data.familiasSemInternet },
                    { name: 'Com Internet', value: data.totalFamilias - data.familiasSemInternet }
                  ]}
                  cx="50%"
                  cy="50%"
                  innerRadius={50}
                  outerRadius={80}
                  dataKey="value"
                  labelLine={false}
                >
                  <Cell fill="#ef4444" />
                  <Cell fill="#3b82f6" />
                </Pie>
                <Tooltip />
                <Legend />
              </PieChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Gráfico Necessidade Especial (Pie) */}
        <div className="bg-white p-6 rounded-xl shadow-sm border border-slate-200">
          <h2 className="text-sm font-bold text-slate-700 mb-4 text-center">Necessidades Especiais</h2>
          <div className="h-[200px] w-full">
            <ResponsiveContainer width="100%" height="100%">
              <PieChart>
                <Pie
                  data={[
                    { name: 'Possui NEE', value: data.alunosComNecessidadeEspecial },
                    { name: 'Não Possui', value: data.totalAlunos - data.alunosComNecessidadeEspecial }
                  ]}
                  cx="50%"
                  cy="50%"
                  innerRadius={50}
                  outerRadius={80}
                  dataKey="value"
                  labelLine={false}
                >
                  <Cell fill="#f59e0b" />
                  <Cell fill="#10b981" />
                </Pie>
                <Tooltip />
                <Legend />
              </PieChart>
            </ResponsiveContainer>
          </div>
        </div>

      </div>

      {/* Full width button to see all records */}
      <div className="mt-8">
        <button 
          onClick={() => navigate('/registros')}
          className="w-full bg-indigo-600 hover:bg-indigo-700 text-white font-bold py-4 px-6 rounded-xl shadow-md transition-colors flex items-center justify-center gap-3 text-lg"
        >
          <FileText className="w-6 h-6" />
          Ver todas informações e registros detalhados
        </button>
      </div>
    </div>
  );
}

// Subcomponente de Card
const KPICard = ({ icon, title, value, color, bg }: any) => (
  <div className="bg-white p-6 rounded-xl shadow-sm border border-slate-200 flex items-center gap-4">
    <div className={`w-14 h-14 rounded-full flex items-center justify-center ${bg} ${color}`}>
      {React.cloneElement(icon, { className: 'w-7 h-7' })}
    </div>
    <div>
      <p className="text-sm font-medium text-slate-500">{title}</p>
      <h3 className="text-2xl font-bold text-slate-800">{value}</h3>
    </div>
  </div>
);
