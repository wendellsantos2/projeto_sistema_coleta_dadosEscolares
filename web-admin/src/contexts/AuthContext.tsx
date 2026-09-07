import React, { createContext, useState, useEffect } from 'react';
import api from '../services/api';

interface AuthContextData {
  signed: boolean;
  user: any;
  login: (email: string, senha: string) => Promise<void>;
  logout: () => void;
  loading: boolean;
}

export const AuthContext = createContext<AuthContextData>({} as AuthContextData);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<any>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const token = localStorage.getItem('@ColetaEscolar:token');
    if (token) {
      // Decode JWT ou apenas considerar logado. Vamos considerar logado
      setUser({ token });
    }
    setLoading(false);
  }, []);

  const login = async (email: string, senha: string) => {
    const response = await api.post('/auth/login', { email, senha });
    const { token } = response.data;
    localStorage.setItem('@ColetaEscolar:token', token);
    setUser({ token });
  };

  const logout = () => {
    localStorage.removeItem('@ColetaEscolar:token');
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ signed: !!user, user, login, logout, loading }}>
      {children}
    </AuthContext.Provider>
  );
};
