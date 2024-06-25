import React, { useState, useContext } from 'react';
import { AuthContext } from '../../context/AuthContext';
import './layout.css';
import Logo from '../../assets/Logo.png';
import { Dropdown, DropdownToggle, DropdownItem, DropdownMenu, Collapse } from 'reactstrap';
import { FaChartPie, FaFileDownload, FaFileImport, FaRegArrowAltCircleLeft, FaRegArrowAltCircleRight } from "react-icons/fa";
import { FaUsers } from "react-icons/fa";
import { FaHandHoldingUsd } from "react-icons/fa";
import { GiReceiveMoney } from "react-icons/gi";
import { RiContactsBookUploadLine } from "react-icons/ri";

import { IoIosCloseCircle, IoMdMenu } from "react-icons/io";
import { FaGear, FaGears, FaUsersGear } from "react-icons/fa6";
import { FaUserGear } from "react-icons/fa6";
import { FaFileInvoiceDollar } from "react-icons/fa";
import { AiOutlineMenuFold, AiOutlineMenuUnfold } from 'react-icons/ai';

function NavBar({ children, pagina_atual, usuario_nome, usuario_tipo }) {
    const [showMenu, setShowMenu] = useState(false);
    const { handdleLogout } = useContext(AuthContext);
    const IrPara = (pargina) => window.location.href = `/${pargina}`;
    const toggleMenu = () => {
        setShowMenu(!showMenu)


        document.querySelector('.sidebar').classList.toggle("open");
        document.querySelector('.btn-toggle-menu').classList.toggle('btn-toggle-menu-open');
        document.querySelector('.dashboard').classList.toggle('opening');
    }

    return (
        <section className='layout-container'>
            <div className="sidebar">
                <div className="logo">
                    <div className=""></div>
                    <img width={100} src={Logo} alt="logo" />
                    <button onClick={toggleMenu} className={'btn-toggle-menu btn-toggle-menu-open'}>{showMenu ? <AiOutlineMenuFold size={25} color='#fff' /> : <AiOutlineMenuUnfold size={25} color='#fff' />}</button>
                </div>
                <DropdownItem header>PAGINAS</DropdownItem>
                <div className="menu">
                    <div onClick={() => IrPara('')} className={pagina_atual === "CLIENTES" ? "menu-item menu-item-active" : "menu-item"}>CLIENTES <FaUsers size={25} color='#fff' /></div>
                    <div onClick={() => IrPara('hstcocontatoocorrencia')} className={pagina_atual === "CONTATOS/OCORRÊNCIA" ? "menu-item menu-item-active" : "menu-item"}>CONTATOS/OCORRÊNCIA<RiContactsBookUploadLine size={25} color='#fff' /></div>
                    <div onClick={() => IrPara("beneficios")} className={pagina_atual === "BENEFÍCIOS" ? "menu-item menu-item-active" : "menu-item"}>BENEFÍCIOS <FaHandHoldingUsd size={25} color='#fff' /></div>
                    <div onClick={() => IrPara("pagamentos")} className={pagina_atual === "PAGAMENTOS" ? "menu-item menu-item-active" : "menu-item"}>PAGAMENTOS <GiReceiveMoney size={25} color='#fff' /></div>
                </div>
                <DropdownItem header>ADMINISTRAÇÃO</DropdownItem>
                <div className="menu">
                    <div className={pagina_atual === "GERENCIAR MOTIVO CONTATO" ? "menu-item menu-item-active" : "menu-item"} onClick={() => IrPara("gmotivocontato")}>GERENCIAR MOTIVO CONTATO <FaGears size={30} color='#fff' /></div>
                    <div className={pagina_atual === "GERENCIAR ORIGEM" ? "menu-item menu-item-active" : "menu-item"} onClick={() => IrPara("gorigem")}>GERENCIAR ORIGEM <FaGear size={25} color='#fff' /></div>
                    <div className={pagina_atual === "GERENCIAR STATUS" ? "menu-item menu-item-active" : "menu-item"} onClick={() => IrPara("gstatus")}>GERENCIAR STATUS <FaUserGear size={25} color='#fff' /></div>
                    <div className={pagina_atual === "GERENCIAR BENEFICIOS" ? "menu-item menu-item-active" : "menu-item"} onClick={() => IrPara("gbeneficio")}>GERENCIAR BENEFICIOS <FaGear size={25} color='#fff' /></div>
                    <div className={pagina_atual === "GERENCIAR CAPTADOR" ? "menu-item menu-item-active" : "menu-item"} onClick={() => IrPara("gcaptador")}>GERENCIAR CAPTADOR <FaUsersGear size={25} color='#fff' /></div>
                </div>
                <DropdownItem header>ARQUIVOS</DropdownItem>
                <div className="menu">
                    <div className={pagina_atual === "REMESSA" ? "menu-item menu-item-active" : "menu-item"} onClick={() => IrPara("rremessa")}>REMESSA <FaFileDownload size={25} color='#fff' /></div>
                    <div className={pagina_atual === "RETORNO" ? "menu-item menu-item-active" : "menu-item"} onClick={() => IrPara("rretorno")}>RETORNO <FaFileImport size={25} color='#fff' /></div>
                    <div className={pagina_atual === "REPASSE" ? "menu-item menu-item-active" : "menu-item"} onClick={() => IrPara("rrepassefinanceiro")}>REPASSE <FaFileInvoiceDollar size={25} color='#fff' /></div>
                </div>
                <DropdownItem header>RELATÓRIOS</DropdownItem>
                <div className="menu">
                    <div className={pagina_atual === "AVERBAÇÃO" ? "menu-item menu-item-active" : "menu-item"} onClick={() => IrPara("rrelatorio")}>AVERBAÇÃO <FaChartPie size={25} color='#fff' /></div>
                </div>
            </div>
            <div className="dashboard">
                <div className="top-bar">
                    <div className="user-info">
                        <span className="hello">Bem-Vindo {usuario_nome}</span>
                    </div>
                    <a className='btn-loggout' onClick={handdleLogout}>Sair</a>
                </div>
                <br />
                <div className="dashboard-container">
                    {children}
                </div>
            </div>
        </section>
    );
}

export { NavBar };
