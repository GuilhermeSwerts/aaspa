import React, { useState } from 'react';
import './login.css'
import Logo from '../../assets/logo_login.png'
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
        <div className="backgroud-login">
            <div className="login-page">
                <div className="left-container-login">
                    <img src={Logo} width={200} alt="logo" />
                    <div style={{ width: '100%' }}>
                        <form className="login-form" onSubmit={handdleLogin}>
                            <div className="input-email">
                                <input className='form-control' required type="text" placeholder="Usuário" value={usuario} onChange={e => setUsuario(e.target.value.toUpperCase())} />
                            </div>
                            <br />
                            <div className="input-email">
                                <input className='form-control' required type={showSenha ? "text" : "password"} placeholder="Senha" value={senha} onChange={e => setDenha(e.target.value)} />
                            </div>
                            <br />
                            <div style={{ display: 'flex', justifyContent: 'start', alignItems: 'center', gap: 10 }}>
                                <input value={showSenha} onChange={e => setShowSenha(!showSenha)} type='checkbox' />
                                <span style={{ color: '#000' }} for="password">Visualizar Senha</span>
                            </div>
                            <br />
                            <button style={{ width: '100%' }} className='btn btn-primary' type="submit">ENTRAR</button>
                        </form>
                    </div>
                </div>
                <div className="right-container-login">
                    <div>
                        <h1>Bem-vindo!</h1><br />
                        <h1>Unidos, conquistamos mais</h1>
                    </div>
                </div>
            </div>
        </div>
    );
}

{/* // <div id="login-page">
        //     <div className="version">
        //         <small>Versão 1.0.0</small>
        //     </div>
        //     <div className="login">
        //         <h2 className="login-title">Login</h2>
        //         <p className="notice">Faça login para acessar o sistema</p>
        //         <form className="login-form" onSubmit={handdleLogin}>
        //             <div className="input-email">
        //                 <input required type="text" placeholder="Usuário" value={usuario} onChange={e => setUsuario(e.target.value.toUpperCase())} //>
        //             </div>
        //             <div className="input-email">
        //                 <input required type={showSenha ? "text" : "password"} placeholder="Senha" value={senha} onChange={e => setDenha(e.target.value)} //>
        //             </div>
        //             <div style={{ display: 'flex', justifyContent: 'start', alignItems: 'center', gap: 10 }}>
        //                 <input value={showSenha} onChange={e => setShowSenha(!showSenha)} type='checkbox' //>
        //                 <span style={{ color: '#000' }} for="password">Visualizar Senha</span>
        //             </div>
        //             <button type="submit">ENTRAR</button>
        //         </form>
        //     </div>
        //     <div className="background">
        //        <div>
        //             <h1>Bem-vindo!</h1><br />
        //             <h1>Unidos, conquistamos MAIS!</h1>
        //         </div>
        //     </div>
        // </div> */}