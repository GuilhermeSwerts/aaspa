import React, { useContext, useEffect, useState, useRef } from 'react';
import { NavBar } from '../../components/Layout/layout'
import { Collapse } from 'reactstrap';
import { Mascara } from '../../util/mascara';
import { Paginacao } from '../../components/Paginacao/Paginacao';
import { AuthContext } from '../../context/AuthContext';
import { FaDownload, FaFilter, FaSearch } from 'react-icons/fa';
import { api } from '../../api/api';
import { Alert } from '../../util/alertas';
import CrudAtendimento from './CrudAtendimento';
import { Size } from '../../util/size';
import Captador from '../Gerenciamento/Captador/index';
import Dropdown from '../../components/Inputs/Dropdown';
import { TbZoomMoney } from 'react-icons/tb';
import { ButtonTooltip } from '../../components/Inputs/ButtonTooltip';

function Atendimento() {
    const ref = useRef();
    const { usuario, handdleUsuarioLogado } = useContext(AuthContext)
    const [situacaoOcorrencias, setSituacaoOcorrencias] = useState([]);
    const [selectedOptions, setSelectedOptions] = useState([]);
    const [showFiltro, setShowFiltro] = useState(true);
    const [dateInitAtendimento, setDateInitAtendimento] = useState('');
    const [dateEndAtendimento, setDateEndAtendimento] = useState('');
    const [situacao, setSituacao] = useState('');
    const [filtro, setFiltro] = useState({
        cpf: '',
        matricula: '',
        situacao: '',
    });
    const [clientes, setClientes] = useState([]);
    //**paginação**
    const [totalClientes, setTotalClientes] = useState(0);
    const [limit, setLimit] = useState(8);
    const [offset, setOffset] = useState(0);
    const [paginaAtual, SetPaginaAtual] = useState(1);

    const onChangeFiltro = (e) => {
        setFiltro({
            ...filtro,
            [e.target.name]: e.target.value,
        });
    };

    const BuscarSituacaoOcorrencias = () => {
        api.get("api/SituacaoOcorrencia", res => {
            setSituacaoOcorrencias(res.data);
        }, err => {
            Alert('Houve um erro ao buscar os Situação Ocorrencias', false)
        })
    }

    const BuscarTodosClientes = (pPagina) => {

        if (!pPagina) pPagina = paginaAtual;

        let situacoesOcorrencias = "";

        if (selectedOptions.length > 0) {
            selectedOptions.forEach((x, i) => {
                situacoesOcorrencias += `SituacoesOcorrencias=${x}`
                if (i !== selectedOptions.length - 1) {
                    situacoesOcorrencias += '&';
                }
            })
        }

        api.get(`BuscarFiltroClientes?&cpf=${filtro.cpf}&beneficio=${filtro.matricula}&dataInitAtendimento=${dateInitAtendimento}&dataEndAtendimento=${dateEndAtendimento}&paginaAtual=${pPagina}&QtdPorPagina=${limit}&${situacoesOcorrencias}`, res => {
            setClientes([]);
            setClientes(res.data.clientes);
            setTotalClientes(res.data.totalClientes);
        }, err => {
            Alert("Houve um erro ao buscar clientes.", false)
        })
    }

    useEffect(() => {
        handdleUsuarioLogado();
        BuscarSituacaoOcorrencias();
        BuscarTodosClientes();
    }, [])

    const DownloadFiltro = () => {
        let situacoesOcorrencias = "";

        if (selectedOptions.length > 0) {
            selectedOptions.forEach((x, i) => {
                situacoesOcorrencias += `SituacoesOcorrencias=${x}`
                if (i !== selectedOptions.length - 1) {
                    situacoesOcorrencias += '&';
                }
            })
        }
        window.open(`${api.ambiente}/DownloadContatoFiltro?cpf=${filtro.cpf}&beneficio=${filtro.matricula}&dataInitAtendimento=${dateInitAtendimento}&dataEndAtendimento=${dateEndAtendimento}&situacaoOcorrencia=${situacao}&${situacoesOcorrencias}`)
    }

    return (
        <NavBar pagina_atual='ATENDIMENTO' usuario_tipo={usuario && usuario.usuario_tipo} usuario_nome={usuario && usuario.usuario_nome} >
            <CrudAtendimento situacaoOcorrencias={situacaoOcorrencias} ref={ref} isUsuarioMaster={usuario && usuario.usuario_tipo === 1} />
            <div className="row">
                <div className="col-md-2">
                    <button style={{ width: '100%' }} className='btn btn-info' onClick={e => setShowFiltro(!showFiltro)}>{showFiltro ? 'Esconder' : 'Mostrar'} Filtro<FaFilter /></button>
                </div>
                <div className="col-md-8"></div>
            </div>
            <br />
            <Collapse isOpen={showFiltro}>
                <div className="row">
                    <div className="col-md-2">
                        <span>N° CPF</span>
                        <input type="text"
                            name='cpf'
                            onChange={onChangeFiltro}
                            maxLength={14}
                            className='form-control'
                            value={Mascara.cpf(filtro.cpf)}
                            placeholder={'CPF do cliente'}
                        />
                    </div>
                    <div className="col-md-2">
                        <span>N° Matrícula</span>
                        <input
                            placeholder='N° Matrícula'
                            type="text"
                            value={filtro.matricula}
                            onChange={onChangeFiltro}
                            name="matricula"
                            id="matricula"
                            className='form-control' />
                    </div>
                    <div className="col-md-2">
                        <span>Data Atendimento De:</span>
                        <input
                            type="date"
                            value={dateInitAtendimento}
                            onChange={e => setDateInitAtendimento(e.target.value)}
                            name="dateInit"
                            id="dateInit"
                            className='form-control' />
                    </div>
                    <div className="col-md-2">
                        <span>Até:</span>
                        <input
                            type="date"
                            value={dateEndAtendimento}
                            onChange={e => setDateEndAtendimento(e.target.value)}
                            name="dateEnd"
                            id="dateEnd"
                            className='form-control' />
                    </div>
                    <div className="col-md-2">
                        <span>Situação Ocorrencia</span>
                        <Dropdown options={
                            situacaoOcorrencias.map(item => ({ value: item.nome, text: item.nome }))
                        }
                            selectedOptions={selectedOptions}
                            setSelectedOptions={setSelectedOptions}
                        />
                        {/* <select name='status' value={situacao} id="status" onChange={e => { setSituacao(e.target.value) }} required className='form-control'>
                            <option value="TODOS">TODOS</option>
                            <option value="ATENDIDA">ATENDIDA</option>
                            <option value="EM TRATAMENTO">EM TRATAMENTO</option>
                            <option value="CANCELADA">CANCELADA</option>
                            <option value="FINALIZADO">FINALIZADO</option>
                            <option value="REEMBOLSO AGENDADO">REEMBOLSO AGENDADO</option>
                            <option value="DADOS INVALIDOS">DADOS INVALIDOS</option>
                            <option value="EM PROCESSAMENTO">EM PROCESSAMENTO</option>
                        </select> */}
                    </div>
                    <div className="col-md-1" style={{ marginTop: '1.5rem' }}>
                        <button style={{ width: '100%' }} onClick={e => { BuscarTodosClientes(1) }} className='btn btn-primary'><FaSearch size={Size.IconeTabela} /></button>
                    </div>
                    <div className="col-md-1"></div>
                    <div className="col-md-1" style={{ marginTop: '1.5rem' }}>
                        <button style={{ width: '100%' }} onClick={DownloadFiltro} className='btn btn-primary'>Extrair <FaDownload size={Size.IconeTabela} /></button>
                    </div>
                </div>
            </Collapse>
            <br />
            <table className='table table-striped'>
                <thead>
                    <tr>
                        <th>#</th>
                        <th>CPF</th>
                        <th>Matrícula</th>
                        <th>Nome</th>
                        <th>Telefone(Celular)</th>
                        <th>Data Cadastro</th>
                        <th>Captador</th>
                        <th>Status Atual</th>
                        <th>Ações</th>
                    </tr>
                </thead>
                <tbody>
                    {clientes.map(cliente => {
                        return (
                            <tr className='selecao'>
                                <td onClick={e => ref.current.AbrirCrud(cliente.cliente.cliente_id)}>{cliente.cliente.cliente_id}</td>
                                <td onClick={e => ref.current.AbrirCrud(cliente.cliente.cliente_id)}>{Mascara.cpf(cliente.cliente.cliente_cpf)}</td>
                                <td onClick={e => ref.current.AbrirCrud(cliente.cliente.cliente_id)}>{cliente.cliente.cliente_matriculaBeneficio}</td>
                                <td onClick={e => ref.current.AbrirCrud(cliente.cliente.cliente_id)}>{cliente.cliente.cliente_nome}</td>
                                <td onClick={e => ref.current.AbrirCrud(cliente.cliente.cliente_id)}>{Mascara.telefone(cliente.cliente.cliente_telefoneCelular)}</td>
                                <td onClick={e => ref.current.AbrirCrud(cliente.cliente.cliente_id)}>{Mascara.data(cliente.cliente.cliente_dataCadastro)}</td>
                                <td onClick={e => ref.current.AbrirCrud(cliente.cliente.cliente_id)}>{cliente.captador.captador_nome}</td>
                                <td onClick={e => ref.current.AbrirCrud(cliente.cliente.cliente_id)}>{cliente.statusAtual.status_nome}</td>
                                <td>
                                    <ButtonTooltip
                                        backgroundColor={'#004d00'}
                                        onClick={() => window.location.href = `/historicopagamento?clienteId=${cliente.cliente.cliente_id}`}
                                        className='btn btn-success button-container-item'
                                        text={'Historico De Pagamentos'}
                                        top={true}
                                        textButton={<TbZoomMoney size={Size.IconeTabela} />}
                                    />
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
                total={totalClientes}
                setOffset={setOffset}
                onChange={BuscarTodosClientes}
                setCurrentPage={SetPaginaAtual}
            />
        </NavBar>
    );
}

export default Atendimento;