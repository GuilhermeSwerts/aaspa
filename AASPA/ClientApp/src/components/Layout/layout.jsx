import React, { useState, useContext, useEffect } from 'react';
import { AuthContext } from '../../context/AuthContext';
import './layout.css';
import Logo from '../../assets/Logo.png';
import { Dropdown, DropdownToggle, DropdownItem, DropdownMenu, Collapse, Container } from 'reactstrap';
import { FaChartArea, FaChartPie, FaFileDownload, FaFileImport, FaRegArrowAltCircleDown, FaRegArrowAltCircleUp } from "react-icons/fa";
import { FaUsers } from "react-icons/fa";
import { FaHandHoldingUsd } from "react-icons/fa";
import { GiReceiveMoney } from "react-icons/gi";
import { RiContactsBookUploadLine } from "react-icons/ri";
import { IoMdExit } from "react-icons/io";
import { FaGear, FaGears, FaUsersGear, FaUser, FaKey } from "react-icons/fa6";
import { FaUserGear } from "react-icons/fa6";
import { FaFileInvoiceDollar } from "react-icons/fa";

function NavBar({ children, pagina_atual, usuario_nome, usuario_tipo }) {
    const { handdleLogout, usuario, handdleUsuarioLogado } = useContext(AuthContext);
    const [toggle, setToggle] = useState(false);
    const [user, setUser] = useState({ sigla: '', nome: '' });

    useEffect(() => {
        setUser({
            sigla: usuario?.usuario_nome.split(' ').map(nome => nome[0]).join(''),
            nome: usuario?.usuario_nome
        }, [])
    });

    return (
        <section>
            <header className="header-container">
                <nav className="header-nav">
                    <div className={pagina_atual === "CLIENTES" ? "link link-active" : "link"}>
                        <a href="/">
                            <FaUsers size={15} /> CLIENTES
                            <span className="nav-tooltip">CLIENTES</span>
                        </a>
                    </div>
                    <span>/</span>
                    <div className={pagina_atual === "CARGA CLIENTES" ? "link link-active" : "link"}>
                        <a href="/ccliente">
                            <FaUsers size={15} /> C. CLIENTES
                            <span className="nav-tooltip">CARGA CLIENTES</span>
                        </a>
                    </div>
                    <span>/</span>
                    <div className={pagina_atual === "CONTATOS/OCORRÊNCIA" ? "link link-active" : "link"}>
                        <a href="/hstcocontatoocorrencia">
                            <RiContactsBookUploadLine size={15} /> CONTATOS
                            <span className="nav-tooltip">CONTATOS/OCORRÊNCIA</span>
                        </a>
                    </div>
                    <span>/</span>
                    <div className={pagina_atual === "BENEFÍCIOS" ? "link link-active" : "link"}>
                        <a href="/beneficios">
                            <FaHandHoldingUsd size={15} /> BENEFÍCIOS
                            <span className="nav-tooltip">BENEFÍCIOS</span>
                        </a>
                    </div>
                    <span>/</span>
                    <div className={pagina_atual === "PAGAMENTOS" ? "link link-active" : "link"}>
                        <a href="/pagamentos">
                            <GiReceiveMoney size={15} /> PAGAMENTOS
                            <span className="nav-tooltip">PAGAMENTOS</span>
                        </a>
                    </div>
                    <span>/</span>
                    <div className={pagina_atual === "GERENCIAR MOTIVO CONTATO" ? "link link-active" : "link"}>
                        <a href="/gmotivocontato">
                            <FaGears size={15} /> MOTIVO CONTATO
                            <span className="nav-tooltip">GERENCIAR MOTIVO CONTATO</span>
                        </a>
                    </div>
                    <span>/</span>
                    <div className={pagina_atual === "GERENCIAR ORIGEM" ? "link link-active" : "link"}>
                        <a href="/gorigem">
                            <FaGear size={15} /> ORIGFEM
                            <span className="nav-tooltip">GERENCIAR ORIGEM</span>
                        </a>
                    </div>
                    <span>/</span>
                    <div className={pagina_atual === "GERENCIAR STATUS" ? "link link-active" : "link"}>
                        <a href="/gstatus">
                            <FaUserGear size={15} /> STATUS
                            <span className="nav-tooltip">GERENCIAR STATUS</span>
                        </a>
                    </div>
                    <span>/</span>
                    <div className={pagina_atual === "GERENCIAR BENEFICIOS" ? "link link-active" : "link"}>
                        <a href="/gbeneficio">
                            <FaGear size={15} /> BENEFICIOS
                            <span className="nav-tooltip">GERENCIAR BENEFICIOS</span>
                        </a>
                    </div>
                    <span>/</span>
                    <div className={pagina_atual === "GERENCIAR CAPTADOR" ? "link link-active" : "link"}>
                        <a href="/gcaptador">
                            <FaUsersGear size={15} /> CAPTADOR
                            <span className="nav-tooltip">GERENCIAR CAPTADOR</span>
                        </a>
                    </div>
                    <span>/</span>
                    <div className={pagina_atual === "REMESSA" ? "link link-active" : "link"}>
                        <a href='/rremessa'>
                            <FaFileDownload size={15} /> REMESSA
                            <span className="nav-tooltip">REMESSA</span>
                        </a>
                    </div>
                    <span>/</span>
                    <div className={pagina_atual === "RETORNO" ? "link link-active" : "link"}>
                        <a href='/rretorno'>
                            <FaFileImport size={15} /> RETORNO
                            <span className="nav-tooltip">RETORNO</span>
                        </a>
                    </div>
                    <span>/</span>
                    <div className={pagina_atual === "REPASSE" ? "link link-active" : "link"}>
                        <a href='/rrepassefinanceiro'>
                            <FaFileInvoiceDollar size={15} /> REPASSE
                            <span className="nav-tooltip">REPASSE</span>
                        </a>
                    </div>
                    <span>/</span>
                    <div className={pagina_atual === "AVERBAÇÃO" ? "link link-active" : "link"}>
                        <a href='/rrelatorio'>
                            <FaChartPie size={15} /> AVERBAÇÃO
                            <span className="nav-tooltip">AVERBAÇÃO</span>
                        </a>
                    </div>
                    <span>/</span>
                    <div className={pagina_atual === "CARTEIRA" ? "link link-active" : "link"}>
                        <a href='/relatoriocarteira'>
                            <FaChartArea size={15} /> CARTEIRA
                            <span className="nav-tooltip">CARTEIRA</span>
                        </a>
                    </div>
                </nav>
                <div className="header-tools" style={{ display: toggle ? 'flex' : 'none' }}>
                    <div style={{ display: 'flex', justifyContent: "center", alignItems: 'center', gap: 20 }}>
                        <img src={Logo} alt="logo" width={100} />
                        {user.nome}
                    </div>
                    <div style={{ borderBottom: '1px solid #c2c2c2', height: '1px', width: '100%' }}></div>
                    <div className='links'>
                        {/* <a href="">Ver Perfil <FaUser /></a> */}
                        {/* <a href="">Alterar Senha <FaKey /></a> */}
                        <a onClick={handdleLogout}>Sair <IoMdExit /></a>
                    </div>
                </div>
                <div className="header-profile" onClick={e => setToggle(!toggle)}>
                    {user.sigla}
                </div>
            </header >
            <hr />
            <div className="dashboard-container" style={{ padding: '1rem', borderRadius: '20px 20px 0 0 ' }}>
                {children}
            </div>
        </section>
    );
}

export { NavBar };
