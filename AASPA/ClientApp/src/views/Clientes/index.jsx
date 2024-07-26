import React, { useContext, useEffect, useState } from 'react';
import { AuthContext } from '../../context/AuthContext';
import { NavBar } from '../../components/Layout/layout';
import { ButtonTooltip } from '../../components/Inputs/ButtonTooltip';
import IconHistoricoPagamento from '../../assets/paymenthistory.png';
import { RiChatHistoryLine } from "react-icons/ri";
import { FaDownload, FaSearch, FaUserEdit } from "react-icons/fa";
import { Mascara } from '../../util/mascara';
import { api } from '../../api/api';
import ModalEditarStatusAtual from '../../components/Modal/editarStatusAtual';
import ModalLogStatus from '../../components/Modal/LogStatus';
import ModalLogBeneficios from '../../components/Modal/ModalLogBeneficios';
import { TbZoomMoney } from 'react-icons/tb';
import * as Enum from '../../util/enum';
import ModalVisualizarCliente from '../../components/Modal/visualizarDadosCliente';
import { Alert, Info, Pergunta } from '../../util/alertas';
import ImportarCLientesIntegral from '../../components/Modal/importarClientesIntegral';
import axios from 'axios';

export default () => {
    const { usuario, handdleUsuarioLogado } = useContext(AuthContext);

    const [statusCliente, setStatusCliente] = useState(0);
    const [statusRemessa, setStatusRemessa] = useState(0);

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

    const [dateInitAverbacao, setDateInitAverbacao] = useState('');
    const [dateEndAverbacao, setDateEndAverbacao] = useState('');

    const [clientes, setClientes] = useState([]);
    const [clientesFiltro, setClientesFiltro] = useState([]);
    const [filtroNome, setFiltroNome] = useState(true);

    const [paginaAtual, setPaginaAtual] = useState(1);
    const [qtdPaginas, setQtdPaginas] = useState(0);
    const [totalClientes, setTotalClientes] = useState(0);
    const [cadastroExterno, setcadastroExterno] = useState(0);
    const [nome, setNome] = useState('');
    const [cpf, setCpf] = useState('');

    const BuscarTodosClientes = (sCliente, sRemessa, pPagina) => {
        if (!pPagina) {
            pPagina = paginaAtual;
        }
        if (!sCliente) {
            sCliente = statusCliente;
        }
        if (!sRemessa) {
            sRemessa = statusRemessa;
        }

        api.get(`BuscarTodosClientes?statusCliente=${sCliente}&statusRemessa=${sRemessa}&dateInit=${dateInit}&dateEnd=${dateEnd}&paginaAtual=${pPagina}&cadastroExterno=${cadastroExterno}&nome=${nome}&cpf=${cpf}&dateInitAverbacao=${dateInitAverbacao}&dateEndAverbacao=${dateEndAverbacao}`, res => {
            setClientes([]);
            setClientesFiltro([]);

            setClientes(res.data.clientes);
            setClientesFiltro(res.data.clientes);
            setQtdPaginas(res.data.qtdPaginas);

            if (res.data.qtdPaginas < paginaAtual)
                setPaginaAtual(1);

            setTotalClientes(res.data.totalClientes);
        }, err => {
            Alert("Houve um erro ao buscar clientes.", false)
        })
    }

    useEffect(() => {
        handdleUsuarioLogado()
        BuscarTodosClientes();
    }, [])

    const onChangeFiltro = ({ target }) => {
        const { value } = target;
        if (filtroNome) {
            setNome(value);
        } else {
            setCpf(value);
        }
        setPaginaAtual(1);
        axios.get(`BuscarTodosClientes?statusCliente=${statusCliente}&statusRemessa=${statusRemessa}&dateInit=${dateInit}&dateEnd=${dateEnd}&paginaAtual=${paginaAtual}&cadastroExterno=${cadastroExterno}&nome=${nome}&cpf=${cpf}`, {
            baseURL: api.urlBase,
            headers: {
                "Authorization": `Bearer ${api.access_token ? api.access_token : ""}`
            }
        }).then(res => {
            setClientes([]);
            setClientesFiltro([]);
            let clt = res.data.clientes;
            setClientes(clt);
            setClientesFiltro(clt);
            setQtdPaginas(res.data.qtdPaginas);

            setTotalClientes(res.data.totalClientes);
        }).catch(err => {
            Alert("Houve um erro ao buscar clientes.", false)
        })

    }

    const DownloadClienteFiltro = () => {
        window.open(`${process.env.NODE_ENV === "production" ? "http://adm.aaspa.org.br" : "https://localhost:5001"}/DownloadClienteFiltro?statusCliente=${statusCliente}&statusRemessa=${statusRemessa}&dateInit=${dateInit}&dateEnd=${dateEnd}&paginaAtual=${null}&cadastroExterno=${cadastroExterno}&nome=${nome}&cpf=${cpf}&dateInitAverbacao=${dateInitAverbacao}&dateEndAverbacao=${dateEndAverbacao}`)
    }

    // const AlterarPagina = (e) => {
    //     if (e.target.value <= qtdPaginas) {
    //         setPaginaAtual(e.target.value)
    //     // } else {
    //         Info("Numero da página digitada maior que quantidade de paginas")
    //     }
    // }

    const AlterarPagina = async (pagina, isProxima) => {
        let buscar = false;

        if (isProxima && (paginaAtual + pagina) > qtdPaginas) {
            if (await Pergunta("Numero da página digitada maior que quantidade de paginas\nDeseja buscar pelo numero maximo?")) {
                setPaginaAtual(qtdPaginas);
                buscar = true;
                BuscarTodosClientes(statusCliente, statusRemessa, qtdPaginas)
            } else {
                return;
            }
        } else if (!isProxima && (paginaAtual - pagina) <= 0) {
            if (await Pergunta("Numero da página digitada menor que 1\nDeseja buscar pelo numero minimo?")) {
                setPaginaAtual(1);
                buscar = true;
                BuscarTodosClientes(statusCliente, statusRemessa, 1)
            } else {
                return;
            }
        } else {
            if (isProxima) {
                setPaginaAtual(paginaAtual + pagina);
                BuscarTodosClientes(statusCliente, statusRemessa, paginaAtual + pagina)
            } else {
                setPaginaAtual(paginaAtual - pagina);
                BuscarTodosClientes(statusCliente, statusRemessa, paginaAtual - pagina)
            }
        }
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
                {<div className="col-md-6">
                    <span>{!filtroNome ? 'Pesquisar pelo CPF' : 'Pesquisar pelo nome'} </span>
                    <input type="text"
                        onChange={onChangeFiltro}
                        maxLength={!filtroNome ? 14 : 255}
                        className='form-control'
                        value={!filtroNome ? cpf : nome}
                        placeholder={!filtroNome ? 'CPF do cliente' : 'Nome do cliente'}

                    />
                </div>}
                <div style={{ marginTop: '22px' }} className="col-md-2">
                    <ImportarCLientesIntegral />
                </div>
                <div style={{ marginTop: '22px' }} className="col-md-2">
                    <button style={{ width: '100%' }} type='button' onClick={() => window.location.href = '/cliente'} className='btn btn-primary'>Novo Cliente</button>
                </div>
            </div>
            <div className="row">
                <div className="col-md-2">
                    <span>Status:</span>
                    <select className='form-control' onChange={e => { setStatusCliente(e.target.value); BuscarTodosClientes(e.target.value, statusRemessa) }}>
                        <option value={0}>TODOS</option>
                        <option value={1}>ATIVOS</option>
                        <option value={2}>INATIVOS</option>
                        <option value={3}>EXCLUIDOS</option>
                    </select>
                </div>
                <br />
                <div className="col-md-3">
                    <span>FOI GERADO REMESSA:</span>
                    <select className='form-control' onChange={e => { setStatusRemessa(e.target.value); BuscarTodosClientes(statusCliente, e.target.value) }}>
                        <option value={0}>TODOS</option>
                        <option value={1}>SIM</option>
                        <option value={2}>NÃO</option>
                    </select>
                </div>
                <div className="col-md-2">
                    <span>Data De Cadastro De:</span>
                    <input type="date" value={dateInit} onChange={e => setDateInit(e.target.value)} name="dateInit" id="dateInit" className='form-control' />
                </div>
                <div className="col-md-2">
                    <span>Até:</span>
                    <input type="date" value={dateEnd} onChange={e => setDateEnd(e.target.value)} name="dateEnd" id="dateEnd" className='form-control' />
                </div>
                <div className="col-md-2">
                    <span>Cadastro Externo:</span>
                    <select className='form-control' value={cadastroExterno} onChange={e => setcadastroExterno(e.target.value)}>
                        <option value={0}>TODOS</option>
                        <option value={1}>SIM</option>
                        <option value={2}>NÃO</option>
                    </select>
                </div>
                <div className="col-md-2">
                    <span>Data Da Averbação De:</span>
                    <input type="date" value={dateInitAverbacao} onChange={e => setDateInitAverbacao(e.target.value)} name="dateInitAverbacao" id="dateInitAverbacao" className='form-control' />
                </div>
                <div className="col-md-2">
                    <span>Até:</span>
                    <input type="date" value={dateEndAverbacao} onChange={e => setDateEndAverbacao(e.target.value)} name="dateEndAverbacao" id="dateEndAverbacao" className='form-control' />
                </div>
                <div className="col-md-1" style={{ marginTop: '20px' }}>
                    <button style={{ width: '100%' }} onClick={() => BuscarTodosClientes(statusCliente, statusRemessa, 1)} className='btn btn-primary'><FaSearch size={17} /></button>
                </div>
                <div className="col-md-2" style={{ marginTop: '22px' }}>
                    <button style={{ width: '100%' }} onClick={DownloadClienteFiltro} className='btn btn-primary'>Extrair Clientes<FaDownload size={17} /></button>
                </div>
            </div>
            <span>Total Clientes: {totalClientes}</span>
            <br />
            <table className='table table-striped'>
                <thead>
                    <tr>
                        <th>CPF</th>
                        <th>Nome</th>
                        <th>Telefone(Celular)</th>
                        <th>Data Averbação</th>
                        <th>Status Atual</th>
                        <th>Captador</th>
                        <th>Beneficios Ativos</th>
                        <th>Remessa</th>
                        <th>Cadastro Externo</th>
                        <th>Ações</th>
                    </tr>
                </thead>
                <tbody>
                    {clientesFiltro.map(cliente => {
                        console.log(cliente)
                        return (
                            <tr>
                                <td>{Mascara.cpf(cliente.cliente.cliente_cpf)}</td>
                                <td>{cliente.cliente.cliente_nome}</td>
                                <td>{Mascara.telefone(cliente.cliente.cliente_telefoneCelular)}</td>
                                <td>{Mascara.data(cliente.cliente.cliente_DataAverbacao)}</td>
                                <td>{cliente.statusAtual.status_nome}</td>
                                <td>{cliente.captador.captador_nome}</td>
                                <td><select className='form-control'>
                                    {cliente.beneficios.map(beneficio => (
                                        <option value={beneficio.beneficio_id}>{beneficio.beneficio_nome_beneficio}</option>
                                    ))}
                                </select></td>
                                <td>{cliente.cliente.cliente_remessa_id > 0 ? cliente.cliente.cliente_remessa_id : '-'}</td>
                                <td>{cliente.cliente.clientes_cadastro_externo == true ? 'SIM' : 'NÃO'}</td>
                                {cliente.statusAtual.status_id !== Enum.EStatus.Deletado
                                    && cliente.statusAtual.status_id !== Enum.EStatus.ExcluidoAguardandoEnvio
                                    && cliente.statusAtual.status_id !== Enum.EStatus.Inativo
                                    && <td className='button-container-grid'>
                                        <ButtonTooltip
                                            backgroundColor={'#004d00'}
                                            onClick={() => window.location.href = `/historicopagamento?clienteId=${cliente.cliente.cliente_id}`}
                                            className='btn btn-success button-container-item'
                                            text={'Historico De Pagamentos'}
                                            top={true}
                                            textButton={<TbZoomMoney size={17} />}
                                        />
                                        <ButtonTooltip
                                            backgroundColor={'#006600'}
                                            onClick={() => window.location.href = `/historicoocorrenciacliente?clienteId=${cliente.cliente.cliente_id}`}
                                            className='btn btn-success button-container-item'
                                            text={'Historico Contatos/Ocorrências'}
                                            top={true}
                                            textButton={<RiChatHistoryLine size={17} />}
                                        />
                                        <ModalLogStatus ClienteId={cliente.cliente.cliente_id} ClienteNome={cliente.cliente.cliente_nome} />
                                        <ModalLogBeneficios ClienteId={cliente.cliente.cliente_id} ClienteNome={cliente.cliente.cliente_nome} />
                                        <ModalVisualizarCliente Cliente={cliente.cliente} />
                                        <ButtonTooltip
                                            backgroundColor={'#00b300'}
                                            onClick={() => window.location.href = `/cliente?clienteId=${cliente.cliente.cliente_id}`}
                                            className='btn btn-warning button-container-item'
                                            text={'Editar Dados'}
                                            top={true}
                                            textButton={<FaUserEdit color='#fff' size={17} />}
                                        />
                                        <ModalEditarStatusAtual BuscarTodosClientes={BuscarTodosClientes} ClienteId={cliente.cliente.cliente_id} StatusId={cliente.statusAtual.status_id} />
                                    </td>}
                                {cliente.statusAtual.status_id == Enum.EStatus.Deletado && <td className='button-container-grid'>
                                    <ButtonTooltip
                                        backgroundColor={'#00b300'}
                                        onClick={() => window.location.href = `/cliente?clienteId=${cliente.cliente.cliente_id}`}
                                        className='btn btn-warning button-container-item'
                                        text={'Editar Dados'}
                                        top={true}
                                        textButton={<FaUserEdit color='#fff' size={17} />}
                                    />
                                    <ModalVisualizarCliente Cliente={cliente.cliente} />
                                    <ModalEditarStatusAtual BuscarTodosClientes={BuscarTodosClientes} ClienteId={cliente.cliente.cliente_id} StatusId={cliente.statusAtual.status_id} />
                                </td>}
                                {cliente.statusAtual.status_id == Enum.EStatus.ExcluidoAguardandoEnvio && <td className='button-container-grid'>
                                    <ButtonTooltip
                                        backgroundColor={'#00b300'}
                                        onClick={() => window.location.href = `/cliente?clienteId=${cliente.cliente.cliente_id}`}
                                        className='btn btn-warning'
                                        text={'Editar Dados'}
                                        top={true}
                                        textButton={<FaUserEdit color='#fff' size={17} />}
                                    />
                                    <ModalVisualizarCliente Cliente={cliente.cliente} />
                                    <ModalEditarStatusAtual BuscarTodosClientes={BuscarTodosClientes} ClienteId={cliente.cliente.cliente_id} StatusId={cliente.statusAtual.status_id} />
                                </td>}
                                {cliente.statusAtual.status_id == Enum.EStatus.Inativo && <td className='button-container-grid'>
                                    <ModalVisualizarCliente Cliente={cliente.cliente} />
                                    <ModalEditarStatusAtual BuscarTodosClientes={BuscarTodosClientes} ClienteId={cliente.cliente.cliente_id} StatusId={cliente.statusAtual.status_id} />
                                </td>}
                            </tr>
                        )
                    })}
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


        </NavBar >
    );
}

