import React, { useContext, useEffect, useState } from 'react';
import { NavBar } from '../../components/Layout/layout';
import { AuthContext } from '../../context/AuthContext';
import { GetParametro } from '../../util/parametro';
import { api } from '../../api/api';
import { Mascara } from '../../util/mascara';
import { ButtonTooltip } from '../../components/Inputs/ButtonTooltip';
import { FaPencil, FaTrash } from 'react-icons/fa6';
import NovoPagamento from '../../components/Modal/NovoPagamento';
import EditarPagamento from '../../components/Modal/editarPagamento';

function HistoricoPagamento() {
    const { usuario, handdleUsuarioLogado } = useContext(AuthContext);
    const [ClienteId, setClienteId] = useState(0);
    const [clienteData, setClienteData] = useState({
        cliente: { cliente_nome: '', cliente_cpf: '', cliente_id: 0 }
    });
    const [pagamentos, setPagamentos] = useState([]);

    const BuscarPagamentos = (id) => {
        api.get(`BuscarHistoricoPagamento/${id}`, res => {
            setPagamentos(res.data);
        }, erro => {
            alert('Houve um erro ao buscar o historico de pagamento.')
        })
    }

    useEffect(() => {
        handdleUsuarioLogado();
        const id = GetParametro("clienteId");
        if (!id) {
            window.location.href = '/pagamentos'
        }
        setClienteId(id);
        api.get(`BuscarClienteID/${id}`, res => {
            setClienteData(res.data);
            BuscarPagamentos(id);
        }, erro => {
            alert('Houve um erro ao buscar o cliente.')
        })

    }, [])

    const ExcluirPagamento = async id => {
        const res = await window.confirm("Deseja realmente excluir esse pagamento?");
        if (res) {
            api.delete(`ExcluirPagamento/${id}`, res => {
                BuscarPagamentos(clienteData.cliente.cliente_id);
                alert('Pagamento excluido com sucesso!');
            }, err => {
                alert('Houve um erro ao excluido o pagamento.')
            })
        }
    }

    return (
        <NavBar usuario_tipo={usuario && usuario.usuario_tipo} usuario_nome={usuario && usuario.usuario_nome}>
            <button className='btn btn-link' onClick={() => window.history.back()}>Voltar</button>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                <div>
                    <h4>Cliente: {clienteData.cliente.cliente_nome}</h4>
                    <h4>CPF: {Mascara.cpf(clienteData.cliente.cliente_cpf)}</h4>
                </div>
                <NovoPagamento BuscarPagamentos={BuscarPagamentos} ClienteId={clienteData.cliente.cliente_id} ClienteNome={clienteData.cliente.cliente_nome} />
            </div>
            <table className='table table-striped'>
                <thead>
                    <tr>
                        <th>Valor Pago</th>
                        <th>Data Do Pagamento</th>
                        <th>Data Cadastro</th>
                        <th>Ações</th>
                    </tr>
                </thead>
                <tbody>
                    {pagamentos.map(pagamento => (
                        <tr>
                            <td>R$ {("" + pagamento.valorPago).replace(".", ",")}</td>
                            <td>{pagamento.dtPagamento}</td>
                            <td>{pagamento.dt_Cadastro}</td>
                            <td style={{ display: 'flex', gap: 5 }}>
                                <EditarPagamento
                                    BuscarPagamentos={BuscarPagamentos}
                                    ClienteId={clienteData.cliente.cliente_id}
                                    ClienteNome={clienteData.cliente.cliente_nome}
                                    PagamentoId={pagamento.pagamentoId}
                                />
                                <ButtonTooltip
                                    onClick={() => ExcluirPagamento(pagamento.pagamentoId)}
                                    className='btn btn-danger'
                                    text={'Excluir Pagamentos'}
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

export default HistoricoPagamento;