import React, { useContext, useState, useEffect } from 'react';
import { NavBar } from '../../components/Layout/layout';
import { AuthContext } from '../../context/AuthContext';
import { api } from '../../api/api';
import { Mascara } from '../../util/mascara';
import { ButtonTooltip } from '../../components/Inputs/ButtonTooltip';
import { TbZoomMoney } from "react-icons/tb";
import NovoPagamento from '../../components/Modal/NovoPagamento';
import * as Enum from '../../util/enum';
import { FaSearch } from 'react-icons/fa';
import { Alert } from '../../util/alertas';
import { Paginacao } from '../../components/Paginacao/Paginacao';
import { Size } from '../../util/size';

function Pagamentos() {
    const { usuario, handdleUsuarioLogado } = useContext(AuthContext)
    const [clientes, setClientes] = useState([]);
    const [clientesFiltro, setClientesFiltro] = useState([]);
    const [filtroNome, setFiltroNome] = useState(true);

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

    const BuscarTodosClientes = () => {
        api.get(`BuscarTodosClientes?dateInit=${dateInit}&dateEnd=${dateEnd}`, res => {
            setClientes([]);
            setClientesFiltro([]);
            setClientes(res.data.clientes);
            setClientesFiltro(res.data.clientes);
        }, err => {
            Alert("Houve um erro ao buscar clientes.", false)
        })
    }

    useEffect(() => {
        handdleUsuarioLogado();
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

    //**paginação**
    const [limit, setLimit] = useState(8);
    const [offset, setOffset] = useState(0);
    const endIndex = offset + limit;
    const currentData = clientes.slice(offset, endIndex);


    return (
        <NavBar pagina_atual='PAGAMENTOS' usuario_tipo={usuario && usuario.usuario_tipo} usuario_nome={usuario && usuario.usuario_nome} >
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
                    <button style={{ width: '100%' }} onClick={BuscarTodosClientes} className='btn btn-primary'>BUSCAR <FaSearch size={Size.IconeTabela} /></button>
                </div>
            </div>
            <br />
            <table className='table table-striped'>
                <thead>
                    <tr>
                        <th>CPF</th>
                        <th>Nome</th>
                        <th>Status Atual</th>
                        <th>Captador</th>
                        <th>Ações</th>
                    </tr>
                </thead>
                <tbody>
                    {currentData.map(cliente => {

                        if (cliente.statusAtual.status_id != Enum.EStatus.Deletado)
                            return (
                                <tr>
                                    <td>{Mascara.cpf(cliente.cliente.cliente_cpf)}</td>
                                    <td>{cliente.cliente.cliente_nome}</td>
                                    <td>{cliente.statusAtual.status_nome}</td>
                                    <td>{cliente.captador.captador_nome}</td>
                                    <td style={{ display: 'flex', gap: 5 }}>
                                        <ButtonTooltip
                                            onClick={() => window.location.href = `/historicopagamento?clienteId=${cliente.cliente.cliente_id}`}
                                            className='btn btn-danger'
                                            text={'Historico De Pagamentos'}
                                            top={true}
                                            textButton={<TbZoomMoney size={Size.IconeTabela} />}
                                        />
                                        {cliente.statusAtual.status_id !== Enum.EStatus.Deletado && cliente.statusAtual.status_id !== Enum.EStatus.Inativo
                                            && <NovoPagamento ClienteId={cliente.cliente.cliente_id} ClienteNome={cliente.cliente.cliente_nome} />}
                                    </td>
                                </tr>
                            )
                    })}
                    {clientes.length == 0 && <span>Nenhum cliente foi encontrado...</span>}
                </tbody>
            </table>
            <Paginacao
                limit={limit}
                setLimit={setLimit}
                offset={offset}
                total={clientes.length}
                setOffset={setOffset}
            />
        </NavBar>
    );
}

export default Pagamentos;