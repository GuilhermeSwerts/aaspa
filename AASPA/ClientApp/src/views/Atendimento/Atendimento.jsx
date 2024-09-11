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

function Atendimento() {
    const ref = useRef();
    const { usuario, handdleUsuarioLogado } = useContext(AuthContext)
    const [showFiltro, setShowFiltro] = useState(true);
    const [filtro, setFiltro] = useState({
        cpf: '',
        matricula: ''
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

        api.get(`BuscarFiltroClientes?&cpf=${filtro.cpf}&beneficio=${filtro.matricula}&paginaAtual=${pPagina}&QtdPorPagina=${limit}`, res => {
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
        window.open(`${api.ambiente}/DownloadContatoFiltro?cpf=${filtro.cpf}&beneficio=${filtro.matricula}`)
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
                    <div className="col-md-1" style={{ marginTop: '1.5rem' }}>
                        <button style={{ width: '100%' }} onClick={e => { BuscarTodosClientes(1) }} className='btn btn-primary'><FaSearch size={17} /></button>
                    </div>
                    <div className="col-md-6"></div>
                    <div className="col-md-1" style={{ marginTop: '1.5rem' }}>
                        <button style={{ width: '100%' }} onClick={DownloadFiltro} className='btn btn-primary'>Extrair <FaDownload size={17} /></button>
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