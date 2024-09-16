import React, { useState, useContext, useEffect } from 'react';
import { AuthContext } from '../../context/AuthContext';
import './layout.css';
import Logo from '../../assets/logo_original.png';
import { IoMenu } from "react-icons/io5";
import { IoMdCloseCircle } from "react-icons/io";
import { IoMdExit } from "react-icons/io";
import { Size } from '../../util/size';
import { PublicaRotas } from '../../router/Rotas';
import { FaCubes } from 'react-icons/fa';

function NavBar({ children, usuario_nome, usuario_tipo }) {
    const { handdleLogout, usuario, handdleUsuarioLogado } = useContext(AuthContext);
    const [toggle, setToggle] = useState(false);
    const [user, setUser] = useState({ sigla: '', nome: '' });
    const rota_atual = window.location.pathname

    const pagina_atual = PublicaRotas.find(x => x.path === rota_atual);

    useEffect(() => {
        if (usuario?.usuario_nome) {
            setUser({
                sigla: usuario.usuario_nome.split(' ').map(nome => nome[0]).join(''),
                nome: usuario.usuario_nome
            });
        }
    }, [usuario]);

    const toggleMenu = () => {
        document.querySelector('.container-nav').classList.toggle('open-nav')
    }

    return (
        <section>
            <header className="header-container">
                <button className='btn-menu' onClick={toggleMenu}><IoMenu size={Size.IconeMenu} /></button>
                <img style={{ width: '100px' }} src={Logo} alt="logo" />
                <nav className="container-nav">
                    <div className="flex-align-end" style={{ padding: '1rem' }}>
                        <button className='btn-menu' onClick={toggleMenu}><IoMdCloseCircle size={Size.IconeMenu} /></button>
                    </div>
                    <div style={{ width: '100%' }}>
                        {PublicaRotas.map((rota, i) => (
                            <div key={`${rota.path}-${i}`} className={rota_atual === rota.path ? "link link-active" : "link"}>
                                <a href={rota.path}>
                                    {rota.Icon && <rota.Icon size={Size.IconeMenu} />} {rota.nome}
                                </a>
                            </div>
                        ))}
                    </div>

                </nav>
                <div className="header-tools" style={{ display: toggle ? 'flex' : 'none' }}>
                    <div style={{ display: 'flex', justifyContent: "center", alignItems: 'center', gap: 20 }}>
                        <img src={Logo} alt="logo" width={100} />
                        {user.nome}
                    </div>
                    <div style={{ borderBottom: '1px solid #c2c2c2', height: '1px', width: '100%' }}></div>
                    <div className='links'>
                        <a onClick={handdleLogout}>Sair <IoMdExit /></a>
                    </div>
                </div>
                <div className="header-profile" onClick={e => setToggle(!toggle)}>
                    {user.sigla}
                </div>
            </header >
            <div style={{ background: "#f2f2f2", padding: '2rem', width: '100%', minHeight: '100vh', height: "auto" }}>
                <div className='header-simples'>
                    <h5><FaCubes color='#fff' /> {pagina_atual && pagina_atual.nome}</h5>
                    <button onClick={e => window.history.back()} style={{ color: '#fff' }} className='btn btn-link'>Voltar</button>
                </div>
                <br />
                {children}
            </div>
        </section>
    );
}

export { NavBar };
