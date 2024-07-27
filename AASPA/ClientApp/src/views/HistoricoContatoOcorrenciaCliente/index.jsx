import React, { useContext, useEffect, useRef, useState } from 'react';
import { AuthContext } from '../../context/AuthContext';
import { GetParametro } from '../../util/parametro';
import { NavBar } from '../../components/Layout/layout';
import { Mascara } from '../../util/mascara';
import { api } from '../../api/api';
import ModalContatoOcorrencia from '../../components/Modal/novaContatoOcorrencia';
import DescricaoModal from '../../components/Modal/descricaoModal';
import { FaEye, FaTrash } from 'react-icons/fa6';
import { ButtonTooltip } from '../../components/Inputs/ButtonTooltip';
import ModalEditarContatoOcorrencia from '../../components/Modal/editarContatoOcorrencia';
import axios from 'axios';
import * as Enum from '../../util/enum';
import { Alert, Pergunta } from '../../util/alertas';

function HistoricoOcorrenciaCliente(props) {
    const { usuario, handdleUsuarioLogado } = useContext(AuthContext);
    const [clientId, setClienteId] = useState();
    const [historicoOcorrenciaCliente, setHistoricoOcorrenciaCliente] = useState([]);
    const [clienteData, setClienteData] = useState({
        cliente: { cliente_nome: '', cliente_cpf: '', cliente_id: 0 }
    });
    const descRef = useRef();
    const [qtdPaginas, setQtdPaginas] = useState(0);
    const [paginaAtual, setPaginaAtual] = useState(1);

    const ExcluirOcorrencia = async (id) => {
        if (await Pergunta("Deseja realmente excluir essa ocorrência?")) {
            try {
                var res = await axios.delete(`${api.urlBase}/DeletarContatoOcorrencia/${id}`, {
                    headers: {
                        "Authorization": `Bearer ${api.access_token ? api.access_token : ""}`
                    }
                })
                if (res.status === 200) {
                    BuscarHistoricoOcorrenciaCliente(clientId);
                    Alert('Ocorrência excluída com sucesso!');
                }
            } catch (error) {

                Alert('Houve um erro ao excluir a ocorrência.', false);
            }
        }
    };

    const BuscarHistoricoOcorrenciaCliente = async (id) => {
        try {
            api.get(`${api.urlBase}/BuscarTodosContatoOcorrencia/${id}`, res => {
                setHistoricoOcorrenciaCliente(res.data);
            })
        } catch (error) {
            console.clear();
            console.log(error);
            Alert('Houve um erro ao buscar o Histórico!', false);
        }
    };

    useEffect(() => {
        const id = GetParametro('clienteId');
        if (!id) {
            window.history.back();
        } else {
            setClienteId(id);
            api.get(`BuscarClienteID/${id}`,
                async (res) => {
                    setClienteData(res.data);
                    BuscarHistoricoOcorrenciaCliente(id);
                },
                (erro) => {
                    Alert('Houve um erro ao buscar o cliente.', false);
                }
            );
        }
    }, []);

    const AbrirDescricao = (desc) => {
        descRef.current.AbrirModal(desc);
    };

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
        <NavBar usuario_tipo={usuario && usuario.usuario_tipo} usuario_nome={usuario && usuario.usuario_nome}>
            <DescricaoModal ref={descRef} />
            <button className='btn btn-link' onClick={() => window.history.back()}>Voltar</button>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <div>
                    <h4>Contato: {clienteData.cliente.cliente_nome}</h4>
                    <h4>CPF: {Mascara.cpf(clienteData.cliente.cliente_cpf)}</h4>
                </div>
                {clienteData.statusAtual && clienteData.statusAtual.status_id !== Enum.EStatus.Deletado && clienteData.statusAtual.status_id !== Enum.EStatus.Inativo
                    && <ModalContatoOcorrencia BuscarHistoricoOcorrenciaCliente={BuscarHistoricoOcorrenciaCliente} cliente={clienteData.cliente} />}
            </div>
            <table className='table table-striped'>
                <thead>
                    <th>Origem</th>
                    <th>Data / hora da ocorrência</th>
                    <th>Motivo do contato</th>
                    <th>Situação ocorrência</th>
                    <th>Descricao da Ocorrência </th>
                    <th>Ações</th>
                </thead>
                <tbody>
                    {historicoOcorrenciaCliente && historicoOcorrenciaCliente.length > 0 && historicoOcorrenciaCliente.map(historico => (
                        <tr>
                            <td>{historico.origem}</td>
                            <td>{historico.dataHoraOcorrencia}</td>
                            <td>{historico.motivoDoContato}</td>
                            <td>{historico.situacaoOcorrencia}</td>
                            <td><button style={{ width: '100%' }} className='btn btn-success' type="button" onClick={() => AbrirDescricao(historico.descricaoDaOcorrência)}><FaEye color='#fff' size={20} /></button></td>
                            <td style={{ display: 'flex', gap: 20 }}>
                                <ModalEditarContatoOcorrencia
                                    ClienteId={clientId}
                                    BuscarHistoricoOcorrenciaCliente={BuscarHistoricoOcorrenciaCliente}
                                    ContatoOcorrenciaId={historico.id}
                                />
                                <ButtonTooltip
                                    onClick={async () => await ExcluirOcorrencia(historico.id)}
                                    className='btn btn-danger'
                                    text={'Excluir Ocorrencia'}
                                    top={true}
                                    textButton={<FaTrash size={20} />}
                                />
                            </td>
                        </tr>
                    ))}
                    {historicoOcorrenciaCliente.length === 0 && <span>Nenhuma Contato/Ocorrencia Cadastrada</span>}
                </tbody>
            </table>
            {/* <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', gap: 20, color: '#000' }}>
                <button onClick={() => { AlterarPagina(10, false) }} disabled={paginaAtual === 1} className='btn btn-primary'>-10</button>
                <button onClick={() => { AlterarPagina(5, false) }} disabled={paginaAtual === 1} className='btn btn-primary'>-5</button>
                <button onClick={() => { AlterarPagina(1, false) }} disabled={paginaAtual === 1} className='btn btn-primary'>Anterior</button>
                <span>{paginaAtual} de {qtdPaginas}</span>
                <button onClick={() => { AlterarPagina(1, true) }} disabled={paginaAtual >= qtdPaginas} className='btn btn-primary'>Próxima</button>
                <button onClick={() => { AlterarPagina(5, true) }} disabled={paginaAtual >= qtdPaginas} className='btn btn-primary'>+5</button>
                <button onClick={() => { AlterarPagina(10, true) }} disabled={paginaAtual >= qtdPaginas} className='btn btn-primary'>+10</button>
            </div> */}
        </NavBar>
    );
}

export default HistoricoOcorrenciaCliente;