import React, { useContext, useEffect, useState } from 'react';
import { NavBar } from '../../components/Layout/layout';
import { AuthContext } from '../../context/AuthContext';
import { api } from '../../api/api';
import { Mascara } from '../../util/mascara';
import ModalVincularBeneficios from '../../components/Modal/ModalVincularBeneficios';
import ModalLogBeneficios from '../../components/Modal/ModalLogBeneficios';
import * as Enum from '../../util/enum';
import { FaSearch } from 'react-icons/fa';
import { Alert, Pergunta } from '../../util/alertas';

function Beneficios() {
    const { usuario, handdleUsuarioLogado } = useContext(AuthContext);
    const [clientes, setClientes] = useState([]);
    const [clientesFiltro, setClientesFiltro] = useState([]);
    const [filtroNome, setFiltroNome] = useState(true);

    const [qtdPaginas, setQtdPaginas] = useState(0);
    const [paginaAtual, setPaginaAtual] = useState(1);

    function get1DiaDoMes() {
        const today = new Date();
        const year = today.getFullYear();
        const month = String(today.getMonth() + 1).padStart(2, '0'); // Janeiro é 0!

        return `${year}-${month}-01`;
    }

    function getDataDeHoje() {
        const today = new Date();
        const year = today.getFullYear();
        const month = String(today.getMonth() + 1).padStart(2, '0'); // Janeiro é 0!
        const day = String(today.getDate()).padStart(2, '0');

        return `${year}-${month}-${day}`;
    }

    const [dateInit, setDateInit] = useState(get1DiaDoMes());
    const [dateEnd, setDateEnd] = useState(getDataDeHoje());

    const BuscarTodosClientes = (ppagina) => {
        if (!ppagina)
            ppagina = paginaAtual;

        api.get(`BuscarTodosClientes?dateInit=${dateInit}&dateEnd=${dateEnd}&&paginaAtual=${ppagina}`, res => {
            setClientes([]);
            setClientesFiltro([]);
            setClientes(res.data.clientes);
            setClientesFiltro(res.data.clientes);

            setQtdPaginas(res.data.qtdPaginas);

            if (res.data.qtdPaginas < paginaAtual)
                setPaginaAtual(1);

        }, err => {
            Alert("Houve um erro ao buscar clientes.", false)
        })
    }

    useEffect(() => {
        BuscarTodosClientes();
    }, [])

    const onChangeFiltro = ({ target }) => {
        const { value } = target;
        if (value && value != "" && filtroNome)
            setClientesFiltro(clientes.filter(x => x.cliente.cliente_nome.toUpperCase().includes(value.toUpperCase())));
        else if (value && value != "" && !filtroNome)
            setClientesFiltro(clientes.filter(x => x.cliente.cliente_cpf.includes(value.replace('.', '').replace('.', '').replace('-', ''))));
        else
            setClientesFiltro(clientes);
    }

    const AlterarPagina = async (pagina, isProxima) => {
        let buscar = false;

        if (isProxima && (paginaAtual + pagina) > qtdPaginas) {
            if (await Pergunta("Numero da página digitada maior que quantidade de paginas\nDeseja buscar pelo numero maximo?")) {
                setPaginaAtual(qtdPaginas);
                buscar = true;
                BuscarTodosClientes(qtdPaginas)
            } else {
                return;
            }
        } else if (!isProxima && (paginaAtual - pagina) <= 0) {
            if (await Pergunta("Numero da página digitada menor que 1\nDeseja buscar pelo numero minimo?")) {
                setPaginaAtual(1);
                buscar = true;
                BuscarTodosClientes(1)
            } else {
                return;
            }
        } else {
            if (isProxima) {
                setPaginaAtual(paginaAtual + pagina);
                BuscarTodosClientes(paginaAtual + pagina)
            } else {
                setPaginaAtual(paginaAtual - pagina);
                BuscarTodosClientes(paginaAtual - pagina)
            }
        }
    }

    return (
        <NavBar pagina_atual='BENEFÍCIOS' usuario_tipo={usuario && usuario.usuario_tipo} usuario_nome={usuario && usuario.usuario_nome} >
            <div className='row'>
                <div className="col-md-2">
                    <span>Tipo de Filtro</span>
                    <select className='form-control' onChange={e => setFiltroNome(e.target.value == 1)}>
                        <option value={1}>NOME</option>
                        <option value={2}>CPF</option>
                    </select>
                </div>
                {<div className="col-md-8">
                    <span>{!filtroNome ? 'Pesquisar pelo CPF' : 'Pesquisar pelo nome'} </span>
                    <input type="text"
                        onChange={onChangeFiltro}
                        maxLength={!filtroNome ? 14 : 255}
                        className='form-control'
                        placeholder={!filtroNome ? 'CPF do cliente' : 'Nome do cliente'} />
                </div>}
                <div style={{ marginTop: '22px' }} className="col-md-2">
                    <button type='button' onClick={() => window.location.href = '/cliente'} className='btn btn-primary'>Novo Cliente</button>
                </div>
            </div>
            <div className="row">
                <div className="col-md-2">
                    <span>Data De Cadastro De:</span>
                    <input type="date" value={dateInit} onChange={e => setDateInit(e.target.value)} name="dateInit" id="dateInit" className='form-control' />
                </div>
                <div className="col-md-2">
                    <span>Até:</span>
                    <input type="date" value={dateEnd} onChange={e => setDateEnd(e.target.value)} name="dateEnd" id="dateEnd" className='form-control' />
                </div>
                <div className="col-md-2" style={{ marginTop: '20px' }}>
                    <button style={{ width: '100%' }} onClick={BuscarTodosClientes} className='btn btn-primary'>BUSCAR <FaSearch size={17} /></button>
                </div>
            </div>
            <br />
            <table className='table table-striped'>
                <thead>
                    <tr>
                        <th>CPF</th>
                        <th>Nome</th>
                        <th>Telefone(Celular)</th>
                        <th>Data Cadastro</th>
                        <th>Status Atual</th>
                        <th>Captador</th>
                        <th>Beneficios Ativos</th>
                        <th>Ações</th>
                    </tr>
                </thead>
                <tbody>
                    {clientesFiltro.map(cliente => (
                        <tr>
                            <td>{Mascara.cpf(cliente.cliente.cliente_cpf)}</td>
                            <td>{cliente.cliente.cliente_nome}</td>
                            <td>{Mascara.telefone(cliente.cliente.cliente_telefoneCelular)}</td>
                            <td>{Mascara.data(cliente.cliente.cliente_dataCadastro)}</td>
                            <td>{cliente.statusAtual.status_nome}</td>
                            <td>{cliente.captador.captador_nome}</td>
                            <td><select className='form-control'>
                                {cliente.beneficios.map(beneficio => (
                                    <option value={beneficio.beneficio_id}>{beneficio.beneficio_nome_beneficio}</option>
                                ))}
                            </select></td>
                            <td style={{ display: 'flex', gap: 5 }}>
                                {cliente.statusAtual.status_id !== Enum.EStatus.Deletado && cliente.statusAtual.status_id !== Enum.EStatus.Inativo
                                    && <ModalVincularBeneficios BuscarTodosClientes={BuscarTodosClientes} ClienteId={cliente.cliente.cliente_id} />}
                                <ModalLogBeneficios ClienteId={cliente.cliente.cliente_id} ClienteNome={cliente.cliente.cliente_nome} />
                            </td>
                        </tr>
                    ))}
                    {clientes.length == 0 && <span>Nenhum cliente foi encontrado...</span>}
                </tbody>
            </table>
            <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', gap: 20, color: '#000' }}>
                <button onClick={() => { AlterarPagina(10, false) }} disabled={paginaAtual === 1} className='btn btn-primary'>-10</button>
                <button onClick={() => { AlterarPagina(5, false) }} disabled={paginaAtual === 1} className='btn btn-primary'>-5</button>
                <button onClick={() => { AlterarPagina(1, false) }} disabled={paginaAtual === 1} className='btn btn-primary'>Anterior</button>
                <span>{paginaAtual} de {qtdPaginas}</span>
                <button onClick={() => { AlterarPagina(1, true) }} disabled={paginaAtual >= qtdPaginas} className='btn btn-primary'>Próxima</button>
                <button onClick={() => { AlterarPagina(5, true) }} disabled={paginaAtual >= qtdPaginas} className='btn btn-primary'>+5</button>
                <button onClick={() => { AlterarPagina(10, true) }} disabled={paginaAtual >= qtdPaginas} className='btn btn-primary'>+10</button>
            </div>
        </NavBar>
    );
}

export default Beneficios;