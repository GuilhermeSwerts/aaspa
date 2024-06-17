import React, { useState, useContext } from 'react';
import { AuthContext } from '../../context/AuthContext';
import './layout.css';
import Logo from '../../assets/Logo.png';
import { Dropdown, DropdownToggle, DropdownItem, DropdownMenu, } from 'reactstrap';
import { FaRegArrowAltCircleLeft, FaRegArrowAltCircleRight } from "react-icons/fa";
import { FaUsers } from "react-icons/fa";
import { FaHandHoldingUsd } from "react-icons/fa";
import { GiReceiveMoney } from "react-icons/gi";
import { RiContactsBookUploadLine } from "react-icons/ri";

function NavBar({ children, pagina_atual, usuario_nome, usuario_tipo }) {
    const [dropdownOpen, setDropdownOpen] = useState(false);
    const toggleDropdown = () => setDropdownOpen((prevState) => !prevState);
    const [dropdownOpenTools, setDropdownOpenTools] = useState(false);
    const toggleDropdownTools = () => setDropdownOpenTools((prevState) => !prevState);

    const [showMenu, setShowMenu] = useState(true);
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
            <div className="btn-container-toggle-menu btn-toggle-menu-open">
                <button onClick={toggleMenu} className={'btn-toggle-menu'}>{!showMenu ? <FaRegArrowAltCircleRight /> : <FaRegArrowAltCircleLeft />}</button>
            </div>
            <div className="sidebar open">
                <div className="logo"><img width={100} src={Logo} alt="logo" /></div>
                <DropdownItem header>Paginas</DropdownItem>
                <div className="menu">
                    <div onClick={() => IrPara('')} className="menu-item">CLIENTES <FaUsers size={25} color='#fff' /></div>
                    <div onClick={() => IrPara('hstcocontatoocorrencia')} className="menu-item">CONTATOS/OCORRÊNCIA <RiContactsBookUploadLine size={25} color='#fff' /></div>
                    <div onClick={() => IrPara("beneficios")} className="menu-item">BENEFÍCIOS <FaHandHoldingUsd size={25} color='#fff' /></div>
                    <div onClick={() => IrPara("pagamentos")} className="menu-item">PAGAMENTOS <GiReceiveMoney size={25} color='#fff' /></div>
                </div>
                <DropdownItem header>Tools</DropdownItem>
                <div className="settings">
                    {usuario_tipo === 1 && <Dropdown style={{ background: 'none', border: 'none' }} isOpen={dropdownOpen} toggle={toggleDropdown} direction={'right'}>
                        <DropdownToggle caret color='' style={{
                            padding: '15px 20px',
                            cursor: 'pointer',
                            textAlign: 'left',
                            display: 'flex',
                            width: '100%',
                            color: '#fff',
                            justifyContent: 'space-between',
                            alignItems: 'center',
                        }}>FERRAMENTAS</DropdownToggle>
                        <DropdownMenu>
                            <DropdownItem header>Ferramentas</DropdownItem>
                            <DropdownItem onClick={() => IrPara("gmotivocontato")}>GERENCIAR MOTIVO CONTATO</DropdownItem>
                            <DropdownItem onClick={() => IrPara("gorigem")}>GERENCIAR ORIGEM</DropdownItem>
                            <DropdownItem onClick={() => IrPara("gstatus")}>GERENCIAR STATUS</DropdownItem>
                            <DropdownItem onClick={() => IrPara("gbeneficio")}>GERENCIAR BENEFICIOS</DropdownItem>
                        </DropdownMenu>
                    </Dropdown>}
                    {usuario_tipo === 1 && <Dropdown style={{ background: 'none', border: 'none' }} isOpen={dropdownOpenTools} toggle={toggleDropdownTools} direction={'right'}>
                        <DropdownToggle caret color='' style={{
                            padding: '15px 20px',
                            cursor: 'pointer',
                            textAlign: 'left',
                            display: 'flex',
                            width: '100%',
                            color: '#fff',
                            justifyContent: 'space-between',
                            alignItems: 'center',
                        }}>RELATÓRIOS</DropdownToggle>
                        <DropdownMenu>
                            <DropdownItem header>Relatórios</DropdownItem>
                            <DropdownItem onClick={() => IrPara("rremessa")}>REMESSA</DropdownItem>
                            <DropdownItem onClick={() => IrPara("rretorno")}>RETORNO</DropdownItem>
                            <DropdownItem onClick={()=> IrPara("rrepassefinanceiro")}>REPASSE | FINANCEIRO</DropdownItem>
                        </DropdownMenu>
                    </Dropdown>}
                </div>
            </div>
            <div className="dashboard opening">
                <div className="top-bar">
                    <div className="user-info">
                        <span className="hello">Bem-Vindo {usuario_nome}</span>
                    </div>
                    <a className='btn-loggout' onClick={handdleLogout}>Sair</a>
                </div>
                <br />
                {children}
            </div>
        </section>
    );
}

export { NavBar };
