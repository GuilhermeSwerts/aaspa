import { createContext, useEffect, useState } from "react";
import { api } from '../api/api';

export const AuthContext = createContext();

export function AuthProvider({ children }) {
    const [usuario, setUsuario] = useState(null);

    const handdleLogout = () => {
        window.localStorage.removeItem("usuario_logado");
        window.location.href = '/login';
    }

    const handdleUsuarioLogado = () => {
        if (usuario) return;

        var usuario_logado = window.localStorage.getItem("usuario_logado");
        var access_token = window.localStorage.getItem("access_token");
        if (!usuario_logado) window.location.href = '/login';
        const usuario_logado_obj = JSON.parse(usuario_logado);

        if (access_token && usuario_logado) {
            setUsuario(usuario_logado_obj);
            return;
        }
        window.location.href = '/login';
    }

    useEffect(() => {
        if (window.location.pathname !== '/login')
            handdleUsuarioLogado();
        
    }, [usuario]);

    return (
        <AuthContext.Provider value={{ usuario, handdleLogout, handdleUsuarioLogado }}>
            {children}
        </AuthContext.Provider>
    );
}