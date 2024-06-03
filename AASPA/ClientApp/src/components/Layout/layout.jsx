import React, { useState, useContext } from 'react';
import { AuthContext } from '../../context/AuthContext';
import './layout.css';
import Logo from '../../assets/Logo.png';
import {
    Dropdown,
    DropdownToggle,
    DropdownMenu,
    DropdownItem
} from 'reactstrap';
import { FaGears } from "react-icons/fa6";

function NavBar({ children, pagina_atual, usuario_nome, usuario_tipo }) {
    const { handdleLogout } = useContext(AuthContext);
    const [dropdownOpen, setDropdownOpen] = useState(false);
    const toggleDropdown = () => setDropdownOpen((prevState) => !prevState);
    const IrPara = (pargina) => window.location.href = `/${pargina}`;

    return (
        <section>
            <nav className='nav-container'>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                    <img style={{ marginLeft: 20 }} src={Logo} alt="logo" width={80} />
                    <ul style={{ display: 'flex', justifyContent: 'center', alignItems: 'center' }}>
                        <li onClick={() => IrPara('')} className={pagina_atual === 'CLIENTES' ? 'selected' : ''} >CLIENTES</li>
                        <li  onClick={() => IrPara('hstcocontatoocorrencia')} className={pagina_atual === 'HISTORICO CONTATOS' ? 'selected' : ''}>HISTORICO CONTATOS/OCORRENCIA</li>
                        <li onClick={() => IrPara("beneficios")} className={pagina_atual === 'BENEFICIOS' ? 'selected' : ''} >BENEFICIOS</li>
                        <li onClick={() => IrPara("pagamentos")} className={pagina_atual === 'PAGAMENTOS' ? 'selected' : ''} >PAGAMENTOS</li>

                        {usuario_tipo === 1 && <Dropdown style={{ background: 'none', border: 'none' }} isOpen={dropdownOpen} toggle={toggleDropdown} direction={'left'}>
                            <DropdownToggle caret color='' style={{ color: '#Fff' }}><FaGears size={25} /></DropdownToggle>
                            <DropdownMenu>
                                <DropdownItem header>Ferramentas</DropdownItem>
                                <DropdownItem onClick={() => IrPara("gmotivocontato")}>GERENCIAR MOTIVO CONTATO</DropdownItem>
                                <DropdownItem onClick={() => IrPara("gorigem")}>GERENCIAR ORIGEM</DropdownItem>
                                <DropdownItem onClick={() => IrPara("gstatus")}>GERENCIAR STATUS</DropdownItem>
                                <DropdownItem onClick={() => IrPara("gbeneficio")}>GERENCIAR BENEFICIOS</DropdownItem>
                            </DropdownMenu>
                        </Dropdown>}

                        <li onClick={handdleLogout}>SAIR</li>
                    </ul>
                </div>
            </nav>
            <br />
            <div style={{margin:'2rem'}}>
                {children}
            </div>
        </section>
    );
}

export { NavBar };
