import React, { useContext } from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, AuthContext } from './contexts/AuthContext';
import Login from './pages/Login';
import Dashboard from './pages/Dashboard';
import Registros from './pages/Registros';
import Alunos from './pages/Alunos';

const PrivateRoute = ({ children }: { children: React.ReactNode }) => {
  const { signed, loading } = useContext(AuthContext);

  if (loading) return <div>Carregando...</div>;

  return signed ? <>{children}</> : <Navigate to="/login" />;
};

function RoutesComponent() {
  return (
    <Routes>
      <Route path="/login" element={<Login />} />
      <Route 
        path="/" 
        element={
          <PrivateRoute>
            <Dashboard />
          </PrivateRoute>
        } 
      />
      <Route 
        path="/registros" 
        element={
          <PrivateRoute>
            <Registros />
          </PrivateRoute>
        } 
      />
      <Route 
        path="/alunos" 
        element={
          <PrivateRoute>
            <Alunos />
          </PrivateRoute>
        } 
      />
    </Routes>
  );
}

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <RoutesComponent />
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;
