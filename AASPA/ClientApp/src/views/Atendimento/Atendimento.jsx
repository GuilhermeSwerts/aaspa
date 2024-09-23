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

function Atendimento() {
    const ref = useRef();
    const { usuario, handdleUsuarioLogado } = useContext(AuthContext)
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

    const BuscarTodosClientes = (pPagina) => {

        if (!pPagina) pPagina = paginaAtual;

        api.get(`BuscarFiltroClientes?&cpf=${filtro.cpf}&beneficio=${filtro.matricula}&dataInitAtendimento=${dateInitAtendimento}&dataEndAtendimento=${dateEndAtendimento}&situacaoOcorrencia=${situacao}&paginaAtual=${pPagina}&QtdPorPagina=${limit}`, res => {
            setClientes([]);
            setClientes(res.data.clientes);
            setTotalClientes(res.data.totalClientes);
        }, err => {
            Alert("Houve um erro ao buscar clientes.", false)
        })
    }

    useEffect(() => {
        handdleUsuarioLogado();
        BuscarTodosClientes();
    }, [])

    const DownloadFiltro = () => {

        console.log(api.ambiente);
        window.open(`${api.ambiente}/DownloadContatoFiltro?cpf=${filtro.cpf}&beneficio=${filtro.matricula}&dataInitAtendimento=${dateInitAtendimento}&dataEndAtendimento=${dateEndAtendimento}&situacaoOcorrencia=${situacao}`)
    }

    return (
        <NavBar pagina_atual='ATENDIMENTO' usuario_tipo={usuario && usuario.usuario_tipo} usuario_nome={usuario && usuario.usuario_nome} >
            <CrudAtendimento ref={ref} />
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
                        <select name='status' value={situacao} id="status" onChange={e => { setSituacao(e.target.value )}} required className='form-control'>
                            <option value="TODOS">TODOS</option>
                            <option value="ATENDIDA">ATENDIDA</option>
                            <option value="EM TRATAMENTO">EM TRATAMENTO</option>
                            <option value="CANCELADA">CANCELADA</option>
                            <option value="FINALIZADO">FINALIZADO</option>
                            <option value="REEMBOLSO AGENDADO">REEMBOLSO AGENDADO</option>
                        </select>                       
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
                        <th>Status Atual</th>
                    </tr>
                </thead>
                <tbody>
                    {clientes.map(cliente => {
                        return (
                            <tr className='selecao' onClick={e => ref.current.AbrirCrud(cliente.cliente.cliente_id)}>
                                <td>{cliente.cliente.cliente_id}</td>
                                <td>{Mascara.cpf(cliente.cliente.cliente_cpf)}</td>
                                <td>{cliente.cliente.cliente_matriculaBeneficio}</td>
                                <td>{cliente.cliente.cliente_nome}</td>
                                <td>{Mascara.telefone(cliente.cliente.cliente_telefoneCelular)}</td>
                                <td>{Mascara.data(cliente.cliente.cliente_dataCadastro)}</td>
                                <td>{cliente.statusAtual.status_nome}</td>
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