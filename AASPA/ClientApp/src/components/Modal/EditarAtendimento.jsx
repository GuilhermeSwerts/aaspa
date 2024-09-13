import React, { useEffect, useState } from 'react';
import { Row, Col, Modal, ModalBody, ModalFooter, ModalHeader, Form, FormGroup, Label, Input, Button } from 'reactstrap';
import { api } from '../../api/api';
import { Mascara } from '../../util/mascara';
import { FaPencil, FaPlus } from 'react-icons/fa6';
import { ButtonTooltip } from '../Inputs/ButtonTooltip';
import { Alert } from '../../util/alertas';

function ModalEditarAtendimento({ cliente, BuscarHistoricoOcorrenciaCliente = null, HistoricoId }) {
    const [show, setShow] = useState(false);
    const [origens, setOrigens] = useState([]);
    const [motivos, setMotivos] = useState([]);

    const [dtOcorrencia, setDtOcorrencia] = useState('');
    const [origem, setOrigem] = useState();
    const [motivo, setMotivo] = useState();
    const [situacao, setSituacao] = useState("ATENDIDA");
    const [desc, setDesc] = useState("");
    const [banco, setBanco] = useState("");
    const [agencia, setAgencia] = useState("");
    const [conta, setConta] = useState("");
    const [digito, setDigito] = useState("");
    const [pix, setPIX] = useState("");

    const initState = () => {
        setDtOcorrencia('');
        setOrigem(origens[0]);
        setMotivo(motivos[0]);
        setSituacao("ATENDIDA");
        setDesc("");
        setBanco("");
        setAgencia("");
        setConta("");
        setDigito("");
        setPIX("")
    }

    const BuscarMotivos = () => {
        api.get("BuscarTodosMotivos", res => {
            setMotivos(res.data);
            setMotivo(res.data[0].motivo_contato_id);
        }, err => {
            Alert('Houve um erro ao buscar os motivos de contato', false)
        })
    }

    const BuscarOrigens = () => {
        api.get("BuscarTodasOrigem", res => {
            setOrigens(res.data);
            setOrigem(res.data[0].origem_id);
        }, err => {
            Alert('Houve um erro ao buscar os motivos de contato', false)
        })
    }

    const handdleSubmit = (e) => {
        const formData = new FormData();

        formData.append("HistoricoContatosOcorrenciaId", HistoricoId)
        formData.append("HistoricoContatosOcorrenciaOrigemId", origem)
        formData.append("HistoricoContatosOcorrenciaClienteId", cliente.cliente_id)
        formData.append("HistoricoContatosOcorrenciaMotivoContatoId", motivo)
        formData.append("HistoricoContatosOcorrenciaDtOcorrencia", dtOcorrencia)
        formData.append("HistoricoContatosOcorrenciaDescricao", desc)
        formData.append("HistoricoContatosOcorrenciaSituacaoOcorrencia", situacao)
        formData.append("HistoricoContatosOcorrenciaBanco", banco)
        formData.append("HistoricoContatosOcorrenciaAgencia", agencia)
        formData.append("HistoricoContatosOcorrenciaConta", conta)
        formData.append("HistoricoContatosOcorrenciaDigito", digito)
        formData.append("HistoricoContatosOcorrenciaPix", pix)

        api.post("EditarContatoOcorrencia", formData, res => {
            initState();
            if (BuscarHistoricoOcorrenciaCliente) {
                BuscarHistoricoOcorrenciaCliente(cliente.cliente_id);
            }
            Alert('Contato/Ocorrência adicionado com sucesso!', true)
            setShow(false);
        }, err => {
            Alert(err.response.data, false)
        })
    }

    const handleOpenModal = () => {
        BuscarMotivos();
        BuscarOrigens();
        api.get(`BuscarContatoOcorrenciaById/${HistoricoId}`, res => {
            setDtOcorrencia(res.data.historico_contatos_ocorrencia_dt_ocorrencia);
            setSituacao(res.data.historico_contatos_ocorrencia_situacao_ocorrencia);
            setDesc(res.data.historico_contatos_ocorrencia_descricao);
            setMotivo(res.data.historico_contatos_ocorrencia_motivo_contato_id);
            setOrigem(res.data.historico_contatos_ocorrencia_origem_id);
            setBanco(res.data.historico_contatos_ocorrencia_banco);
            setAgencia(res.data.historico_contatos_ocorrencia_agencia);
            setConta(res.data.historico_contatos_ocorrencia_conta);
            setDigito(res.data.historico_contatos_ocorrencia_digito);
            setPIX(res.data.historico_contatos_ocorrencia_chave_pix);
            setShow(true)
        }, err => {
            Alert('houve um erro ao buscar o Contato/Ocorrencia do id:' + HistoricoId, false)
        })
    }

    return (
        <>
            <ButtonTooltip
                onClick={handleOpenModal}
                className='btn btn-warning'
                text={'Editar Atendimento'}
                top={false}
                textButton={<FaPencil size={17} />}
            />
            <Modal isOpen={show} style={{ maxWidth: '80%', margin: '0 auto' }}>
                <form onSubmit={e => { e.preventDefault(); handdleSubmit(e) }}>
                    <ModalHeader>
                        Editar Atendimento
                    </ModalHeader>
                    <ModalBody style={{ display: 'flex', flexDirection: 'column', gap: '10px' }}>
                        <input type="hidden" value={cliente.cliente_id} />
                        <small><b>Dados Gerais:</b></small>
                        <div className="row">
                            <div className="col-md-8">
                                <Label>Nome do Contato/Usuário</Label>
                                <input required type="text" disabled value={cliente.cliente_nome} className='form-control' />
                            </div>
                            <div className="col-md-4">
                                <Label>Cpf do cliente</Label>
                                <input required type="text" disabled value={Mascara.cpf(cliente.cliente_cpf)} className='form-control' />
                            </div>
                        </div>
                        <hr />
                        <small><b>Dados Atendimento:</b></small>
                        <div className="row">
                            <div className="col-md-3">
                                <Label>Origem</Label>
                                <select name='HistoricoContatosOcorrenciaOrigemId' value={origem} onChange={e => setOrigem(e.target.value)} required className='form-control'>
                                    {origens.map(origem => (
                                        <option value={origem.origem_id}>{origem.origem_nome}</option>
                                    ))}
                                </select>
                            </div>
                            <div className="col-md-3">
                                <Label>Data / Hora da ocorrência</Label>
                                <input name='HistoricoContatosOcorrenciaDtOcorrencia' value={dtOcorrencia} onChange={e => setDtOcorrencia(e.target.value)} required type="datetime-local" className='form-control' />
                            </div>
                            <div className="col-md-3">
                                <Label>Motivo do contato</Label>
                                <select name='HistoricoContatosOcorrenciaMotivoContatoId' value={motivo} onChange={e => setMotivo(e.target.value)} required className='form-control'>
                                    {motivos.map(motivo_contato => (
                                        <option value={motivo_contato.motivo_contato_id}>{motivo_contato.motivo_contato_nome}</option>
                                    ))}
                                </select>
                            </div>
                            <div className="col-md-3">
                                <Label>Situação Da Ocorrência</Label>
                                <select name='HistoricoContatosOcorrenciaSituacaoOcorrencia' value={situacao} onChange={e => setSituacao(e.target.value)} required className='form-control'>
                                    <option value="ATENDIDA">ATENDIDA</option>
                                    <option value="EM TRATAMENTO">EM TRATAMENTO</option>
                                    <option value="CANCELADA">CANCELADA</option>
                                    <option value="FINALIZADO">FINALIZADO</option>
                                </select>
                            </div>
                        </div>
                        <hr />
                        <small><b>Dados Bancários:</b></small>
                        <div className='row'>
                            <div className="col-md-3">
                                <Label>Banco</Label>
                                <input type="text" value={banco} onChange={e => setBanco(e.target.value)} className='form-control' />
                            </div>
                            <div className="col-md-2">
                                <Label>Agência</Label>
                                <input maxLength={4} type="text" value={agencia} onChange={e => setAgencia(e.target.value)} className='form-control' />
                            </div>
                            <div className="col-md-3">
                                <Label>Conta</Label>
                                <input maxLength={15} type="text" value={conta} onChange={e => setConta(e.target.value)} placeholder='Conta sem dígito' className='form-control' />
                            </div>
                            <div className="col-md-1">
                                <Label>Dígito</Label>
                                <input maxLength={15} type="text" value={digito} onChange={e => setDigito(e.target.value)} placeholder='Dígito' className='form-control' />
                            </div>
                            <div className="col-md-3">
                                <Label>Chave PIX</Label>
                                <input type="text" value={pix} onChange={e => setPIX(e.target.value)} placeholder='Chave PIX' className='form-control' />
                            </div>
                        </div>
                        <hr />
                        <div className='row'>
                            <div className="col-md-12">
                                <Label>Descrição Da Ocorrência</Label>
                                <textarea name='HistoricoContatosOcorrenciaDescricao' value={desc} onChange={e => setDesc(e.target.value)} required maxLength={1000} className='form-control'></textarea>
                            </div>
                        </div>
                    </ModalBody>
                    <ModalFooter>
                        <button type='button' onClick={() => { initState(); setShow(false) }} className='btn btn-danger'>Fechar</button>
                        <button type='submit' className='btn btn-primary'>Salvar</button>
                    </ModalFooter>
                </form>
            </Modal>
        </>
    );
}

export default ModalEditarAtendimento;