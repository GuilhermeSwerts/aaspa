import React, { useContext, useEffect, useState } from 'react';
import { AuthContext } from '../../context/AuthContext';
import { NavBar } from '../../components/Layout/layout';
import { ButtonTooltip } from '../../components/Inputs/ButtonTooltip';
import { RiChatHistoryLine } from "react-icons/ri";
import { FaDownload, FaFilter, FaPlus, FaSearch, FaUserEdit, FaTimes } from "react-icons/fa";
import { Mascara } from '../../util/mascara';
import { api } from '../../api/api';
import ModalEditarStatusAtual from '../../components/Modal/editarStatusAtual';
import ModalLogStatus from '../../components/Modal/LogStatus';
import ModalLogBeneficios from '../../components/Modal/ModalLogBeneficios';
import { TbZoomMoney } from 'react-icons/tb';
import ModalVisualizarCliente from '../../components/Modal/visualizarDadosCliente';
import { Alert } from '../../util/alertas';
import ImportarCLientesIntegral from '../../components/Modal/importarClientesIntegral';
import { Collapse } from 'reactstrap'
import { Paginacao } from '../../components/Paginacao/Paginacao';
import { Size } from '../../util/size';
import { GetParametro } from '../../util/parametro';
import ExcluirCliente from '../../components/Modal/excluirCliente';

const Cclientes = () => {
    const { usuario, handdleUsuarioLogado } = useContext(AuthContext);
    const [statusCliente, setStatusCliente] = useState(0);
    const [statusRemessa, setStatusRemessa] = useState(0);
    const [statusIntegraall, setStatusIntegraall] = useState(0);
    const [showFiltro, setShowFiltro] = useState(false);
    const [beneficio, setBeneficio] = useState('');
    const [dateInit, setDateInit] = useState('');
    const [dateEnd, setDateEnd] = useState('');
    const [dateInitAverbacao, setDateInitAverbacao] = useState('');
    const [dateEndAverbacao, setDateEndAverbacao] = useState('');
    const [clientes, setClientes] = useState([]);
    const [filtroNome, setFiltroNome] = useState(1);
    const [paginaAtual, setPaginaAtual] = useState(1);
    const [totalClientes, setTotalClientes] = useState(0);
    const [cadastroExterno, setcadastroExterno] = useState(0);
    const [nome, setNome] = useState('');
    const [cpf, setCpf] = useState('');
    const [qtdPaginas, setQtdPaginas] = useState(0);
    const [isSimples, setIsSimpes] = useState(false);
    const [ModalExcluir, setModalExcluir] = useState(false);
    const [clienteSelecionado, setClienteSelecionado] = useState(null);
    const [isLoading, setIsLoading] = useState(false);


    //**paginação**
    const [limit, setLimit] = useState(10);
    const [offset, setOffset] = useState(0);

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

        api.get(`BuscarTodosClientes?QtdPorPagina=${limit}&PaginaAtual=${pPagina}&statusCliente=${sCliente}&statusRemessa=${sRemessa}&dateInit=${dateInit}&dateEnd=${dateEnd}&cadastroExterno=${cadastroExterno}&nome=${nome}&cpf=${cpf}&dateInitAverbacao=${dateInitAverbacao}&dateEndAverbacao=${dateEndAverbacao}&beneficio=${beneficio}&statusIntegraall=${statusIntegraall}`, res => {
            setClientes([]);

            setClientes(res.data.clientes);
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
        const buscaSimples = GetParametro("BuscaSimples")
        if (buscaSimples && buscaSimples !== null) {
            setIsSimpes(true);
        } else {
            setIsSimpes(false);
        }
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

    const handleOpenModal = (cliente) => {
        setClienteSelecionado(cliente);
        setModalExcluir(true);
    };

    const handleConfirmExclusion = ({ cancelamento = '', motivoCancelamento = '' } = {}) => {
        var formData = new FormData();
        addCampos(formData, cancelamento, motivoCancelamento);

        api.post("/CancelarClienteIntegraall", formData, res => {
            if (res.data === "Proposta não encontrada!") {
                Alert(res.data, false);
            } else {
                Alert(res.data, true);
            }

            setModalExcluir(false);
        }, err => {
            Alert(err.response ? err.response.data : 'Erro desconhecido', false);
        });
    };


    const handleCloseModal = () => {
        setModalExcluir(false);
    }

    const addCampos = (formData, cancelamento, motivoCancelamento) => {
        formData.append("clienteid", clienteSelecionado.cliente.cliente_id);
        formData.append('cancelamento', cancelamento)
        formData.append('motivoCancelamento', motivoCancelamento);
    }

    const DownloadClienteFiltro = () => {
        window.open(`${api.ambiente}/DownloadClienteFiltro?statusCliente=${statusCliente}&statusRemessa=${statusCliente}&dateInit=${dateInit}&dateEnd=${dateEnd}&paginaAtual=${paginaAtual}&cadastroExterno=${cadastroExterno}&nome=${nome}&cpf=${cpf}&dateInitAverbacao=${dateInitAverbacao}&dateEndAverbacao=${dateEndAverbacao}&statusIntegraall=${statusIntegraall}`)
    }

    return (
        <NavBar pagina_atual={'CARGA CLIENTES'} usuario_tipo={usuario && usuario.usuario_tipo} usuario_nome={usuario && usuario.usuario_nome}>
            <div className="row">
                <div className="col-md-2">
                    <button style={{ width: '100%' }} className='btn btn-info' onClick={e => setShowFiltro(!showFiltro)}>{showFiltro ? 'Esconder' : 'Mostrar'} Filtro<FaFilter /></button>
                </div>
                {!isSimples && <div className="col-md-2">
                    <ImportarCLientesIntegral BuscarTodosClientes={BuscarTodosClientes} />
                </div>}
                <div className="col-md-6"></div>
                <div className="col-md-2" style={{ display: 'flex', justifyContent: 'end' }}>
                    <button type='button' onClick={() => window.location.href = '/cliente'} className='btn btn-primary'><FaPlus /></button>
                </div>
            </div>
            <br />
            <hr />
            <Collapse isOpen={showFiltro}>
                {!isSimples && <>
                    <div className='row'>
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
                            <span>FOI GERADO REMESSA:</span>
                            <select className='form-control' onChange={e => { setStatusRemessa(e.target.value); BuscarTodosClientes(statusCliente, e.target.value) }}>
                                <option value={0}>TODOS</option>
                                <option value={1}>SIM</option>
                                <option value={2}>NÃO</option>
                            </select>
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
                        <div className="col-md-8" />
                        <div className="col-md-1" style={{ marginTop: '1.5rem' }}>
                            <button style={{ width: '100%' }} onClick={() => BuscarTodosClientes(statusCliente, statusRemessa, 1)} className='btn btn-primary'><FaSearch size={Size.IconeTabela} /></button>
                        </div>
                        <div className="col-md-1" style={{ marginTop: '1.5rem' }}>
                            <button style={{ width: '100%' }} onClick={DownloadClienteFiltro} className='btn btn-primary'><FaDownload size={Size.IconeTabela} /></button>
                        </div>
                    </div>
                    <br />
                </>}
                {isSimples && <div className="row">
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
                        <button style={{ width: '100%' }} onClick={() => BuscarTodosClientes(statusCliente, statusRemessa, 1)} className='btn btn-primary'><FaSearch size={Size.IconeTabela} /></button>
                    </div>
                    <div className="col-md-1" style={{ marginTop: '1.5rem' }}>
                        <button style={{ width: '100%' }} onClick={DownloadClienteFiltro} className='btn btn-primary'><FaDownload size={Size.IconeTabela} /></button>
                    </div>
                </div>}
            </Collapse>

            <span>Total Clientes: {totalClientes}</span>
            <br />
            <table className='table table-striped'>
                <thead>
                    {!isSimples && <tr>
                        <th>#</th>
                        <th>CPF</th>
                        <th>Nome</th>
                        <th>Status Integraall</th>
                        <th>Data Averbação</th>
                        <th>Status Atual</th>
                        <th>Captador</th>
                        <th>Remessa</th>
                        <th>Cadastro Externo</th>
                        <th>Ações</th>
                    </tr>}
                    {isSimples && <tr>
                        <th>#</th>
                        <th>CPF</th>
                        <th>Nome</th>
                        <th>Telefone(Celular)</th>
                        <th>Data Averbação</th>
                        <th>Data Cadastro</th>
                        <th>Captador</th>
                        <th>Status Atual</th>
                        <th>Status Integraall</th>
                        <th>Ações</th>
                    </tr>}
                </thead>
                <tbody>
                    {clientes.map(cliente => {
                        return (
                            <>
                                {!isSimples && <tr className='selecao'>
                                    <td>{cliente.cliente.cliente_id}</td>
                                    <td>{Mascara.cpf(cliente.cliente.cliente_cpf)}</td>
                                    <td>{cliente.cliente.cliente_nome}</td>
                                    <td>{cliente.cliente.cliente_StatusIntegral == 11 ? "Aguardando Averbação" : cliente.cliente.cliente_StatusIntegral == 12 ? "Enviado para Averbação" : cliente.cliente.cliente_StatusIntegral == 13 ? "Cancelado" : cliente.cliente.cliente_StatusIntegral == 14 ? "Cancelado/Não averbado" : cliente.cliente.cliente_StatusIntegral == 15 ? "Averbado" : cliente.cliente.cliente_StatusIntegral == 16 ? "Ativo/Pago" : ""}</td>
                                    <td>{Mascara.data(cliente.cliente.cliente_DataAverbacao)}</td>
                                    <td>{cliente.statusAtual.status_nome}</td> {/*{cliente.statusAtual && }*/}
                                    <td>{cliente.captador.captador_nome}</td>
                                    <td>{cliente.cliente.cliente_remessa_id > 0 ? cliente.cliente.cliente_remessa_id : '-'}</td>
                                    <td>{cliente.cliente.clientes_cadastro_externo == true ? 'SIM' : 'NÃO'}</td>
                                    <td className='button-container-grid'>
                                        <ButtonTooltip
                                            backgroundColor={'#004d00'}
                                            onClick={() => window.location.href = `/historicopagamento?clienteId=${cliente.cliente.cliente_id}`}
                                            className='btn btn-success button-container-item'
                                            text={'Historico De Pagamentos'}
                                            top={true}
                                            textButton={<TbZoomMoney size={Size.IconeTabela} />}
                                        />
                                        <ButtonTooltip
                                            backgroundColor={'#006600'}
                                            onClick={() => window.location.href = `/historicoocorrenciacliente?clienteId=${cliente.cliente.cliente_id}`}
                                            className='btn btn-success button-container-item'
                                            text={'Historico Contatos/Ocorrências'}
                                            top={true}
                                            textButton={<RiChatHistoryLine size={Size.IconeTabela} />}
                                        />
                                        <ModalLogStatus ClienteId={cliente.cliente.cliente_id} ClienteNome={cliente.cliente.cliente_nome} />
                                        <ModalLogBeneficios ClienteId={cliente.cliente.cliente_id} ClienteNome={cliente.cliente.cliente_nome} />
                                        <ModalVisualizarCliente Cliente={cliente.cliente} Captador={cliente.captador} />
                                        <ButtonTooltip
                                            backgroundColor={'#00b300'}
                                            onClick={() => window.location.href = `/cliente?clienteId=${cliente.cliente.cliente_id}`}
                                            className='btn btn-warning button-container-item'
                                            text={'Editar Dados'}
                                            top={true}
                                            textButton={<FaUserEdit color='#fff' size={Size.IconeTabela} />}
                                        />
                                        <ButtonTooltip
                                            backgroundColor={'#ff0000'}
                                            onClick={() => handleOpenModal(cliente)}
                                            className='btn btn-danger button-container-item'
                                            text={'Cancelar'}
                                            top={true}
                                            textButton={<FaTimes color='#fff' size={Size.IconeTabela} />}
                                        />
                                        <ModalEditarStatusAtual BuscarTodosClientes={BuscarTodosClientes} ClienteId={cliente.cliente.cliente_id} StatusId={cliente.statusAtual.status_id} />
                                    </td>
                                </tr>}
                                {isSimples && <tr className='selecao'>
                                    <td>{cliente.cliente.cliente_id}</td>
                                    <td>{Mascara.cpf(cliente.cliente.cliente_cpf)}</td>
                                    <td>{cliente.cliente.cliente_nome}</td>
                                    <td>{Mascara.telefone(cliente.cliente.cliente_telefoneCelular)}</td>
                                    <td>{Mascara.data(cliente.cliente.cliente_DataAverbacao)}</td>
                                    <td>{Mascara.data(cliente.cliente.cliente_dataCadastro)}</td>
                                    <td>{cliente.captador.captador_nome}</td>
                                    <td>{cliente.statusAtual.status_nome}</td>
                                    <td>{cliente.cliente.cliente_StatusIntegral == 11 ? "Aguardando Averbação" : cliente.cliente.cliente_StatusIntegral == 12 ? "Enviado para Averbação" : cliente.cliente.cliente_StatusIntegral == 13 ? "Cancelado" : cliente.cliente.cliente_StatusIntegral == 14 ? "Cancelado/Não averbado" : cliente.cliente.cliente_StatusIntegral == 15 ? "Averbado" : cliente.cliente.cliente_StatusIntegral == 16 ? "Ativo/Pago" : ""}</td>
                                    <td className='button-container-grid'>
                                        <ButtonTooltip
                                            backgroundColor={'#004d00'}
                                            onClick={() => window.location.href = `/historicopagamento?clienteId=${cliente.cliente.cliente_id}`}
                                            className='btn btn-success button-container-item'
                                            text={'Historico De Pagamentos'}
                                            top={true}
                                            textButton={<TbZoomMoney size={Size.IconeTabela} />}
                                        />
                                        <ButtonTooltip
                                            backgroundColor={'#006600'}
                                            onClick={() => window.location.href = `/historicoocorrenciacliente?clienteId=${cliente.cliente.cliente_id}`}
                                            className='btn btn-success button-container-item'
                                            text={'Historico Contatos/Ocorrências'}
                                            top={true}
                                            textButton={<RiChatHistoryLine size={Size.IconeTabela} />}
                                        />
                                        <ModalLogStatus ClienteId={cliente.cliente.cliente_id} ClienteNome={cliente.cliente.cliente_nome} />
                                        <ModalLogBeneficios ClienteId={cliente.cliente.cliente_id} ClienteNome={cliente.cliente.cliente_nome} />
                                        <ModalVisualizarCliente Cliente={cliente.cliente} Captador={cliente.captador} />
                                        <ButtonTooltip
                                            backgroundColor={'#00b300'}
                                            onClick={() => window.location.href = `/cliente?clienteId=${cliente.cliente.cliente_id}`}
                                            className='btn btn-warning button-container-item'
                                            text={'Editar Dados'}
                                            top={true}
                                            textButton={<FaUserEdit color='#fff' size={Size.IconeTabela} />}
                                        />
                                        <ButtonTooltip
                                            backgroundColor={'#ff0000'}
                                            onClick={() => handleOpenModal(cliente)}
                                            className='btn btn-danger button-container-item'
                                            text={'Cancelar'}
                                            top={true}
                                            textButton={<FaTimes color='#fff' size={Size.IconeTabela} />}
                                        />
                                        <ModalEditarStatusAtual BuscarTodosClientes={BuscarTodosClientes} ClienteId={cliente.cliente.cliente_id} StatusId={cliente.statusAtual.status_id} />
                                    </td>
                                </tr>}
                            </>
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
                setCurrentPage={value => BuscarTodosClientes(null, null, value)}
            />
            <ExcluirCliente
                show={ModalExcluir}
                handleClose={() => setModalExcluir(false)}
                handleConfirm={handleConfirmExclusion}
            />
        </NavBar >
    );
}

export default Cclientes;