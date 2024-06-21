import React, { useContext, useEffect, useState } from 'react';
import { AuthContext } from '../../context/AuthContext';
import { NavBar } from '../../components/Layout/layout';
import { ButtonTooltip } from '../../components/Inputs/ButtonTooltip';
import IconHistoricoPagamento from '../../assets/paymenthistory.png';
import { RiChatHistoryLine } from "react-icons/ri";
import { FaUserEdit } from "react-icons/fa";
import { Mascara } from '../../util/mascara';
import { api } from '../../api/api';
import ModalEditarStatusAtual from '../../components/Modal/editarStatusAtual';
import ModalLogStatus from '../../components/Modal/LogStatus';
import ModalLogBeneficios from '../../components/Modal/ModalLogBeneficios';
import { TbZoomMoney } from 'react-icons/tb';
import * as Enum from '../../util/enum';
import ModalVisualizarCliente from '../../components/Modal/visualizarDadosCliente';

export default () => {
    const { usuario, handdleUsuarioLogado } = useContext(AuthContext);

    const [statusCliente, setStatusCliente] = useState(0);
    const [statusRemessa, setStatusRemessa] = useState(0);

    const [clientes, setClientes] = useState([]);
    const [clientesFiltro, setClientesFiltro] = useState([]);
    const [filtroNome, setFiltroNome] = useState(true);

    const BuscarTodosClientes = (sCliente, sRemessa) => {
        if (!sCliente) {
            sCliente = statusCliente;
        }
        if (!sRemessa) {
            sRemessa = statusRemessa
        }

        api.get(`BuscarTodosClientes?statusCliente=${sCliente}&statusRemessa=${sRemessa}`, res => {
            setClientes([]);
            setClientesFiltro([]);
            setClientes(res.data);
            setClientesFiltro(res.data);
        }, err => {
            alert("Houve um erro ao buscar clientes.")
        })
    }

    useEffect(() => {
        handdleUsuarioLogado()
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

    return (
        <NavBar pagina_atual={'CLIENTES'} usuario_tipo={usuario && usuario.usuario_tipo} usuario_nome={usuario && usuario.usuario_nome}>

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
                    <span>Status:</span>
                    <select className='form-control' onChange={e => { setStatusCliente(e.target.value); BuscarTodosClientes(e.target.value, statusRemessa) }}>
                        <option value={0}>TODOS</option>
                        <option value={1}>ATIVOS</option>
                        <option value={2}>INATIVOS</option>
                        <option value={3}>DELETADOS</option>
                    </select>
                </div>
                <div className="col-md-2">
                    <span>FOI GERADO REMESSA:</span>
                    <select className='form-control' onChange={e => { setStatusRemessa(e.target.value); BuscarTodosClientes(statusCliente, e.target.value) }}>
                        <option value={0}>TODOS</option>
                        <option value={1}>SIM</option>
                        <option value={2}>NÃO</option>
                    </select>
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
                        <th>Remessa</th>
                        <th>Ações</th>
                    </tr>
                </thead>
                <tbody>
                    {clientesFiltro.map(cliente => {
                        return (
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
                                <td>{cliente.cliente.cliente_remessa_id > 0 ? cliente.cliente.cliente_remessa_id : '-'}</td>
                                {cliente.statusAtual.status_id !== Enum.EStatus.Deletado && cliente.statusAtual.status_id !== Enum.EStatus.Inativo
                                    && <td style={{ display: 'flex', gap: 5 }}>
                                        <ButtonTooltip
                                            backgroundColor={'#004d00'}
                                            onClick={() => window.location.href = `/historicopagamento?clienteId=${cliente.cliente.cliente_id}`}
                                            className='btn btn-success'
                                            text={'Historico De Pagamentos'}
                                            top={true}
                                            textButton={<TbZoomMoney size={25} />}
                                        />
                                        <ButtonTooltip
                                            backgroundColor={'#006600'}
                                            onClick={() => window.location.href = `/historicoocorrenciacliente?clienteId=${cliente.cliente.cliente_id}`}
                                            className='btn btn-success'
                                            text={'Historico Contatos/Ocorrências'}
                                            top={true}
                                            textButton={<RiChatHistoryLine size={25} />}
                                        />
                                        <ModalLogStatus ClienteId={cliente.cliente.cliente_id} ClienteNome={cliente.cliente.cliente_nome} />
                                        <ModalLogBeneficios ClienteId={cliente.cliente.cliente_id} ClienteNome={cliente.cliente.cliente_nome} />
                                        <ModalVisualizarCliente Cliente={cliente.cliente} />
                                        <ButtonTooltip
                                            backgroundColor={'#00b300'}
                                            onClick={() => window.location.href = `/cliente?clienteId=${cliente.cliente.cliente_id}`}
                                            className='btn btn-warning'
                                            text={'Editar Dados'}
                                            top={true}
                                            textButton={<FaUserEdit color='#fff' size={25} />}
                                        />
                                        <ModalEditarStatusAtual BuscarTodosClientes={BuscarTodosClientes} ClienteId={cliente.cliente.cliente_id} StatusId={cliente.statusAtual.status_id} />
                                    </td>}
                                {cliente.statusAtual.status_id == Enum.EStatus.Deletado && <td style={{ display: 'flex', gap: 5 }}>
                                    <ModalVisualizarCliente Cliente={cliente.cliente} />
                                    <ModalEditarStatusAtual BuscarTodosClientes={BuscarTodosClientes} ClienteId={cliente.cliente.cliente_id} StatusId={cliente.statusAtual.status_id} />
                                </td>}
                                {cliente.statusAtual.status_id == Enum.EStatus.ExcluidoAguardandoEnvio && <td style={{ display: 'flex', gap: 5 }}>
                                    <ModalVisualizarCliente Cliente={cliente.cliente} />
                                    <ModalEditarStatusAtual BuscarTodosClientes={BuscarTodosClientes} ClienteId={cliente.cliente.cliente_id} StatusId={cliente.statusAtual.status_id} />
                                </td>}
                                {cliente.statusAtual.status_id == Enum.EStatus.Inativo && <td style={{ display: 'flex', gap: 5 }}>
                                    <ModalVisualizarCliente Cliente={cliente.cliente} />
                                    <ModalEditarStatusAtual BuscarTodosClientes={BuscarTodosClientes} ClienteId={cliente.cliente.cliente_id} StatusId={cliente.statusAtual.status_id} />
                                </td>}
                            </tr>
                        )
                    })}
                    {clientes.length == 0 && <span>Nenhum cliente foi encontrado...</span>}
                </tbody>
            </table>
        </NavBar >
    );
}

