import React, { createRef, Component } from 'react'
import ModalEditarContatoOcorrencia from '../../components/Modal/editarContatoOcorrencia';
import { ButtonTooltip } from '../../components/Inputs/ButtonTooltip';
import DescricaoModal from '../../components/Modal/descricaoModal';
import { api } from '../../api/api';
import { Paginacao } from '../../components/Paginacao/Paginacao';
import { Alert, Pergunta } from '../../util/alertas';
import './crud.css';
import ModalContatoOcorrencia from '../../components/Modal/novaContatoOcorrencia';
import { FaEye, FaTrash } from 'react-icons/fa';
import ModalEditarAtendimento from '../../components/Modal/EditarAtendimento';
import axios from 'axios';

class CrudAtendimento extends Component {

    constructor(props) {
        super(props);
        this.state = {
            show: false,
            historicoOcorrenciaCliente: [],
            clienteData: { cliente: { cliente_nome: '', cliente_cpf: '', cliente_id: 0 } },
            clientId: 0,
            limit: 8,
            offset: 0
        }
        this.AbrirCrud = (clientId) => {

            api.get(`${api.urlBase}/BuscarClienteID/${clientId}`,
                async (res) => {
                    this.setState({ clienteData: res.data });
                },
                (erro) => {
                    console.clear()
                    console.log(erro)
                    Alert('Houve um erro ao buscar o cliente.', false);
                }
            );

            try {
                api.get(`${api.urlBase}/BuscarTodosContatoOcorrencia/${clientId}`, res => {
                    this.setState({ historicoOcorrenciaCliente: res.data });
                })
            } catch (error) {
                console.clear();
                console.log(error);
                Alert('Houve um erro ao buscar o Histórico!', false);
            }

            this.setState({ clientId, show: true })
        }
        this.descRef = createRef();
    }

    BuscarHistoricoOcorrenciaCliente = async (id) => {
        try {
            api.get(`${api.urlBase}/BuscarTodosContatoOcorrencia/${id}`, res => {
                this.setState({ historicoOcorrenciaCliente: res.data });
            })
        } catch (error) {
            console.clear();
            console.log(error);
            Alert('Houve um erro ao buscar o Histórico!', false);
        }
    };

    render() {
        const { show, historicoOcorrenciaCliente, clienteData, clientId } = this.state;
        const { descRef } = this;

        //**paginação**
        const limit = this.state.limit;

        const setLimit = (value) => {
            this.setState({ limit: value })
        }

        const offset = this.state.offset;

        const setOffset = (value) => {
            this.setState({ setOffset: value })
        }

        const endIndex = offset + limit;
        const currentData = historicoOcorrenciaCliente.slice(offset, endIndex);

        const AbrirDescricao = (desc) => {
            descRef.current.AbrirModal(desc);
        };

        const ExcluirOcorrencia = async (id) => {
            if (await Pergunta("Deseja realmente excluir essa ocorrência?")) {
                try {
                    var res = await axios.delete(`${api.urlBase}/DeletarContatoOcorrencia/${id}`, {
                        headers: {
                            "Authorization": `Bearer ${api.access_token ? api.access_token : ""}`
                        }
                    })
                    if (res.status === 200) {
                        this.BuscarHistoricoOcorrenciaCliente(clientId);
                        Alert('Ocorrência excluída com sucesso!');
                    }
                } catch (error) {
                    console.clear()
                    console.log(error)
                    Alert('Houve um erro ao excluir a ocorrência.', false);
                }
            }
        };

        const Fechar = () => {
            this.setState({ show: false });
        }

        return (
            <div style={{ display: show ? 'block' : 'none' }} className='modal-crud'>
                <DescricaoModal ref={descRef} />
                <h4>Cliente: {clienteData.cliente.cliente_nome}</h4>
                <br />
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', padding: 20 }}>
                    <button className='btn btn-link' onClick={Fechar}>Voltar</button>
                    <ModalContatoOcorrencia BuscarHistoricoOcorrenciaCliente={this.BuscarHistoricoOcorrenciaCliente} cliente={clienteData ? clienteData.cliente : { cliente_nome: '', cliente_cpf: '', cliente_id: 0 }} />
                </div>
                <table className='table table-striped'>
                    <thead>
                        <th>Origem</th>
                        <th>Data / hora da ocorrência</th>
                        <th>Motivo do contato</th>
                        <th>Situação ocorrência</th>
                        <th>Banco</th>
                        <th>Agência</th>
                        <th>Conta</th>
                        <th>Chave PIX</th>
                        <th>Descricao da Ocorrência </th>
                        <th>Ações</th>
                    </thead>
                    <tbody>
                        {historicoOcorrenciaCliente && historicoOcorrenciaCliente.length > 0 && currentData.map(historico => (
                            <tr>
                                <td>{historico.origem}</td>
                                <td>{historico.dataHoraOcorrencia}</td>
                                <td>{historico.motivoDoContato}</td>
                                <td>{historico.situacaoOcorrencia}</td>
                                <td>{historico.banco}</td>
                                <td>{historico.agencia}</td>
                                <td>{historico.conta}</td>
                                <td>{historico.pix}</td>
                                <td><button style={{ marginLeft: "4rem" }} className='btn btn-success' type="button" onClick={() => AbrirDescricao(historico.descricaoDaOcorrência)}><FaEye color='#fff' size={20} /></button></td>
                                <td style={{ display: 'flex', gap: 20 }}>
                                    <ModalEditarAtendimento
                                        BuscarHistoricoOcorrenciaCliente={this.BuscarHistoricoOcorrenciaCliente}
                                        HistoricoId={historico.id}
                                        cliente={clienteData.cliente}
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
                <Paginacao
                    limit={limit}
                    setLimit={setLimit}
                    offset={offset}
                    total={historicoOcorrenciaCliente.length}
                    setOffset={setOffset}
                />
            </div>
        );
    }
}

export default CrudAtendimento;