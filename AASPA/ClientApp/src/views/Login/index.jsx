import React, { useState } from 'react';
import './login.css'
import Logo from '../../assets/Logo.png'
import { api } from '../../api/api';
import { Alert } from '../../util/alertas';

export default () => {
    const [usuario, setUsuario] = useState('');
    const [senha, setDenha] = useState('');
    const [showSenha, setShowSenha] = useState(false);

    const handdleLogin = (event) => {
        event.preventDefault();

        api.get(`LoginUsuario?usuario=${usuario}&senha=${senha}`, res => {

            window.localStorage.setItem("usuario_logado", JSON.stringify(res.data.usuario));
            window.localStorage.setItem("access_token", res.data.token);

            window.location.href = '/';
        }, erro => {
            Alert(erro.response.data, false)
        })
    }

    return (
        <div class="login-card">
            <img src={Logo} width={60} alt="Logo" class="logo" />
            <h3>Insira suas credenciais</h3>
            <form class="login-form" onSubmit={handdleLogin}>
                <input required type="text" placeholder="Usuário" value={usuario} onChange={e => setUsuario(e.target.value.toUpperCase())} />
                <input required type={showSenha ? "text" : "password"} placeholder="Senha" value={senha} onChange={e => setDenha(e.target.value)} />
                <div style={{ display: 'flex', justifyContent: 'start', alignItems: 'center', gap: 10 }}>
                    <input value={showSenha} onChange={e => setShowSenha(!showSenha)} type='checkbox' />
                    <span style={{ color: '#fff' }} for="password">Visualizar Senha</span>
                </div>
                <button type="submit">ENTRAR</button>
            </form>
        </div>
    );
}
