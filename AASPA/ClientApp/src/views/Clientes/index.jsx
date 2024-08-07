import React, { useContext, useEffect, useState } from 'react';
import { AuthContext } from '../../context/AuthContext';
import { NavBar } from '../../components/Layout/layout';
import { ButtonTooltip } from '../../components/Inputs/ButtonTooltip';
import IconHistoricoPagamento from '../../assets/paymenthistory.png';
import { RiChatHistoryLine } from "react-icons/ri";
import { FaDownload, FaFilter, FaPlus, FaSearch, FaUserEdit } from "react-icons/fa";
import { Mascara } from '../../util/mascara';
import { api } from '../../api/api';
import ModalEditarStatusAtual from '../../components/Modal/editarStatusAtual';
import ModalLogStatus from '../../components/Modal/LogStatus';
import ModalLogBeneficios from '../../components/Modal/ModalLogBeneficios';
import { TbZoomMoney } from 'react-icons/tb';
import * as Enum from '../../util/enum';
import ModalVisualizarCliente from '../../components/Modal/visualizarDadosCliente';
import { Alert, Info, Pergunta } from '../../util/alertas';
import { Collapse } from 'reactstrap'
import { Paginacao } from '../../components/Paginacao/Paginacao';

export default () => {
    const { usuario } = useContext(AuthContext);

    const [statusCliente, setStatusCliente] = useState(0);
    const [statusRemessa, setStatusRemessa] = useState(0);
    const [statusIntegraall, setStatusIntegraall] = useState(0);

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
    const [showFiltro, setShowFiltro] = useState(false);

    const [beneficio, setBeneficio] = useState('');

    const [dateInit, setDateInit] = useState('');
    const [dateEnd, setDateEnd] = useState('');

    const [dateInitAverbacao, setDateInitAverbacao] = useState(get1DiaDoMes());
    const [dateEndAverbacao, setDateEndAverbacao] = useState(getDataDeHoje());

    const [clientes, setClientes] = useState([]);
    const [clientesFiltro, setClientesFiltro] = useState([]);
    const [filtroNome, setFiltroNome] = useState(1);

    const [paginaAtual, setPaginaAtual] = useState(1);
    const [qtdPaginas, setQtdPaginas] = useState(0);
    const [cadastroExterno, setcadastroExterno] = useState(0);
    const [nome, setNome] = useState('');
    const [cpf, setCpf] = useState('');

    //**paginação**
    const [totalClientes, setTotalClientes] = useState(0);
    const [limit, setLimit] = useState(8);
    const [offset, setOffset] = useState(0);
    const endIndex = offset + limit;
    const currentData = clientes.slice(offset, endIndex);

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
        // paginaAtual=${pPagina}
        api.get(`BuscarTodosClientes?statusCliente=${sCliente}&statusRemessa=${sRemessa}&dateInit=${dateInit}&dateEnd=${dateEnd}&cadastroExterno=${cadastroExterno}&nome=${nome}&cpf=${cpf}&dateInitAverbacao=${dateInitAverbacao}&dateEndAverbacao=${dateEndAverbacao}&beneficio=${beneficio}&statusIntegraall=${statusIntegraall}`, res => {
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
        BuscarTodosClientes();
    }, [])

    const onChangeFiltro = ({ target }) => {
        const { value } = target;
        const tipoFiltro = typeof (filtroNome) == 'string' ? parseInt(filtroNome) : filtroNome;

        switch (tipoFiltro) {
            case 1:
                setNome(value);
                setCpf('');
                setBeneficio('');
                break;
            case 2:
                setCpf(value)
                setNome('');
                setBeneficio('');
                break;
            case 3:
                setBeneficio(value);
                setNome('');
                setCpf('');
                break;
            default:
                break;
        }
    }

    const DownloadClienteFiltro = () => {
        window.open(`${api.ambiente}/DownloadClienteFiltro?statusCliente=${statusCliente}&statusRemessa=${statusCliente}&dateInit=${dateInit}&dateEnd=${dateEnd}&paginaAtual=${paginaAtual}&cadastroExterno=${cadastroExterno}&nome=${nome}&cpf=${cpf}&dateInitAverbacao=${dateInitAverbacao}&dateEndAverbacao=${dateEndAverbacao}&statusIntegraall=${statusIntegraall}`)
    }

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
            <div className="row">
                <div className="col-md-2">
                    <button style={{ width: '100%' }} className='btn btn-info' onClick={e => setShowFiltro(!showFiltro)}>{showFiltro ? 'Esconder' : 'Mostrar'} Filtro<FaFilter /></button>
                </div>
                <div className="col-md-8"></div>
                <div className="col-md-2" style={{ display: 'flex', justifyContent: 'end' }}>
                    <button type='button' onClick={() => window.location.href = '/cliente'} className='btn btn-primary'><FaPlus /></button>
                </div>
            </div>
            <br />
            <hr />
            <Collapse isOpen={showFiltro}>
                <div className="row">
                    <div className="col-md-2">
                        <span>Tipo de Filtro</span>
                        <select className='form-control' onChange={e => setFiltroNome(parseInt(e.target.value))}>
                            <option value={1}>NOME</option>
                            <option value={2}>CPF</option>
                            <option value={3}>BENEFÍCIO</option>
                        </select>
                    </div>
                    {<div className="col-md-6">
                        <span>{filtroNome == 1 ? 'Pesquisar pelo nome' : filtroNome == 2 ? 'Pesquisar pelo CPF' : 'Pesquisar N° Benefício'} </span>
                        <input type="text"
                            onChange={onChangeFiltro}
                            maxLength={filtroNome == 1 ? 255 : filtroNome == 2 ? 14 : 10}
                            className='form-control'
                            value={filtroNome === 1 ? nome : filtroNome === 2 ? Mascara.cpf(cpf) : beneficio}
                            placeholder={filtroNome == 1 ? 'Nome do cliente' : filtroNome == 2 ? 'CPF do cliente' : 'Pesquisar N° Benefício'}

                        />
                    </div>}
                    <div className="col-md-4">
                        <span>Status:</span>
                        <select className='form-control' onChange={e => { setStatusCliente(e.target.value); BuscarTodosClientes(e.target.value, statusRemessa) }}>
                            <option value={0}>TODOS</option>
                            <option value={1}>ATIVOS</option>
                            <option value={2}>INATIVOS</option>
                            <option value={3}>EXCLUIDOS</option>
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
                        <span>Data Da Averbação De:</span>
                        <input type="date" value={dateInitAverbacao} onChange={e => setDateInitAverbacao(e.target.value)} name="dateInitAverbacao" id="dateInitAverbacao" className='form-control' />
                    </div>
                    <div className="col-md-2">
                        <span>Até:</span>
                        <input type="date" value={dateEndAverbacao} onChange={e => setDateEndAverbacao(e.target.value)} name="dateEndAverbacao" id="dateEndAverbacao" className='form-control' />
                    </div>
                    <div className="col-md-2">
                        <span>N° Benefício</span>
                        <input placeholder='N° Benefício' type="text" value={beneficio} onChange={e => setBeneficio(e.target.value)} name="beneficio" id="beneficio" className='form-control' />
                    </div>
                    <div className="col-md-2">
                        <span>Status Integraall</span>
                        <select className='form-control' value={statusIntegraall} onChange={e => { setStatusIntegraall(e.target.value) }}>
                            <option value={0}>Todos</option>
                            <option value={11}>Aguardando Averbação</option>
                            <option value={12}>Enviado Averbação</option>
                            <option value={15}>Averbado</option>
                        </select>
                    </div>
                    <div className="col-md-10" />
                    <div className="col-md-1" style={{ marginTop: '1.5rem' }}>
                        <button style={{ width: '100%' }} onClick={() => BuscarTodosClientes(statusCliente, statusRemessa, 1)} className='btn btn-primary'><FaSearch size={17} /></button>
                    </div>
                    <div className="col-md-1" style={{ marginTop: '1.5rem' }}>
                        <button style={{ width: '100%' }} onClick={DownloadClienteFiltro} className='btn btn-primary'><FaDownload size={17} /></button>
                    </div>
                </div>
            </Collapse>

            <span>Total Clientes: {totalClientes}</span>
            <br />
            <table className='table table-striped'>
                <thead>
                    <tr>
                        <th>#</th>
                        <th>CPF</th>
                        <th>Nome</th>
                        <th>Telefone(Celular)</th>
                        <th>Data Averbação</th>
                        <th>Data Cadastro</th>
                        <th>Status Atual</th>
                        <th>Status Integraall</th>
                        <th>Ações</th>
                    </tr>
                </thead>
                <tbody>
                    {currentData.map(cliente => {
                        return (
                            <tr className='selecao'>
                                <td>{cliente.cliente.cliente_id}</td>
                                <td>{Mascara.cpf(cliente.cliente.cliente_cpf)}</td>
                                <td>{cliente.cliente.cliente_nome}</td>
                                <td>{Mascara.telefone(cliente.cliente.cliente_telefoneCelular)}</td>
                                <td>{Mascara.data(cliente.cliente.cliente_DataAverbacao)}</td>
                                <td>{Mascara.data(cliente.cliente.cliente_dataCadastro)}</td>
                                <td>{cliente.statusAtual.status_nome}</td>
                                <td>{cliente.cliente.cliente_StatusIntegral}</td>
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
                                            textButton={<TbZoomMoney size={10} />}
                                        />
                                        <ButtonTooltip
                                            backgroundColor={'#006600'}
                                            onClick={() => window.location.href = `/historicoocorrenciacliente?clienteId=${cliente.cliente.cliente_id}`}
                                            className='btn btn-success button-container-item'
                                            text={'Historico Contatos/Ocorrências'}
                                            top={true}
                                            textButton={<RiChatHistoryLine size={10} />}
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
                                            textButton={<FaUserEdit color='#fff' size={10} />}
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
                                        textButton={<FaUserEdit color='#fff' size={10} />}
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
                                        textButton={<FaUserEdit color='#fff' size={10} />}
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
            <Paginacao
                limit={limit}
                setLimit={setLimit}
                offset={offset}
                total={totalClientes}
                setOffset={setOffset}
            />
        </NavBar >
    );
}

