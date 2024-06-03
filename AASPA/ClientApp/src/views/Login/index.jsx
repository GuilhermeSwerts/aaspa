import React, { useState } from 'react';
import './login.css'
import Logo from '../../assets/Logo.png'
import { api } from '../../api/api';

export default () => {
    const [usuario, setUsuario] = useState('');
    const [senha, setDenha] = useState('');

    const handdleLogin = (event) => {
        event.preventDefault();

        api.get(`LoginUsuario?usuario=${usuario}&senha=${senha}`, res => {

            window.localStorage.setItem("usuario_logado", JSON.stringify(res.data.usuario));
            window.localStorage.setItem("access_token", res.data.token);

            window.location.href = '/';
        }, erro => {
            alert(erro.response.data)
        })
    }

    return (
        <form onSubmit={handdleLogin} className='container-login'>
            <div class="login-container">
                <div class="logo-container">
                    <img src={Logo} width={150} alt="Logo" class="logo" />
                </div>
                <div class="form-group">
                    <label for="username">Usuário</label>
                    <input value={usuario} onChange={e => setUsuario(e.target.value.toUpperCase())} type="text" placeholder='OPERADOR.NOME' id="username" name="username" required />
                </div>
                <div class="form-group">
                    <label for="password">Senha</label>
                    <input value={senha} onChange={e => setDenha(e.target.value)} type="password" id="password" placeholder='********' name="password" required />
                </div>
                <button type="submit" class="login-button">Entrar</button>
            </div>
        </form >
    );
}
