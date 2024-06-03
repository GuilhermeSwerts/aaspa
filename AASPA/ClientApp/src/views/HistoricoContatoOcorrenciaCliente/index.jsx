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

function HistoricoOcorrenciaCliente() {
    const { usuario, handdleUsuarioLogado } = useContext(AuthContext)
    const [clientId, setClienteId] = useState();
    const [historicoOcorrenciaCliente, setHistoricoOcorrenciaCliente] = useState([]);
    const [clienteData, setClienteData] = useState({
        cliente: { cliente_nome: '', cliente_cpf: '', cliente_id: 0 }
    });
    const descRef = useRef();

    const BuscarHistoricoOcorrenciaCliente = (id) => {
        api.get(`BuscarTodosContatoOcorrencia/${id}`, res => {
            setHistoricoOcorrenciaCliente(res.data);
        }, erro => {
            alert('Houve um erro ao buscar historico ocorrencia cliente.')
        })
    }

    useEffect(() => {
        handdleUsuarioLogado();
        const id = GetParametro('clienteId');
        if (!id) {
            window.history.back();
        }
        setClienteId(id);
        api.get(`BuscarClienteID/${id}`, res => {
            setClienteData(res.data);
            BuscarHistoricoOcorrenciaCliente(id);
        }, erro => {
            alert('Houve um erro ao buscar o cliente.')
        })
    }, [])

    const AbrirDescricao = (desc) => {
        descRef.current.AbrirModal(desc);
    }

    const ExcluirOcorrencia = async (id) => {
        var res = await window.confirm("Deseja realmente excluir essa ocorrencia?")
        if (res) {
            api.delete(`DeletarContatoOcorrencia/${id}`, res => {
                BuscarHistoricoOcorrenciaCliente(clientId);
                alert('Ocorrencia excluida com sucesso!');
            }, err => {
                alert('Houve um erro ao excluido a ocorrencia.')
            })
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
                <ModalContatoOcorrencia BuscarHistoricoOcorrenciaCliente={BuscarHistoricoOcorrenciaCliente} cliente={clienteData.cliente} />
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
                    {historicoOcorrenciaCliente.map(historico => (
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
                                    onClick={() => ExcluirOcorrencia(historico.id)}
                                    className='btn btn-danger'
                                    text={'Excluir Ocorrencia'}
                                    top={true}
                                    textButton={<FaTrash size={20} />}
                                />
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </NavBar>
    );
}

export default HistoricoOcorrenciaCliente;