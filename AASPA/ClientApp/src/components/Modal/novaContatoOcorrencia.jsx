import React, { useEffect, useState } from 'react';
import { Row, Col, Modal, ModalBody, ModalFooter, ModalHeader, Form, FormGroup, Label, Input, Button } from 'reactstrap';
import { api } from '../../api/api';
import { Mascara } from '../../util/mascara';
import { FaPlus } from 'react-icons/fa6';
import { ButtonTooltip } from '../Inputs/ButtonTooltip';
import { Alert } from '../../util/alertas';

function ModalContatoOcorrencia({ cliente, BuscarHistoricoOcorrenciaCliente = null, isEdit = false }) {
    const [show, setShow] = useState(false);
    const [origens, setOrigens] = useState([]);
    const [motivos, setMotivos] = useState([]);

    function getDataDeHoje() {
        const today = new Date();
        const year = today.getFullYear();
        const month = String(today.getMonth() + 1).padStart(2, '0'); // Janeiro é 0!
        const day = String(today.getDate()).padStart(2, '0');

        return `${year}-${month}-${day} ${today.getHours()}:${today.getMinutes()}:00`;
    }

    const [dtOcorrencia, setDtOcorrencia] = useState(getDataDeHoje());
    const [origem, setOrigem] = useState();
    const [motivo, setMotivo] = useState();
    const [situacao, setSituacao] = useState("EM TRATAMENTO");
    const [desc, setDesc] = useState("");
    const [banco, setBanco] = useState("");
    const [agencia, setAgencia] = useState("");
    const [conta, setConta] = useState("");
    const [digito, setDigito] = useState("");
    const [pix, setPIX] = useState("");

    const initState = () => {
        setDtOcorrencia(getDataDeHoje());
        if (origens && origens.length > 0) {
            setOrigem(origens[0].origem_id);
        } else {
            setOrigem(origens[0]);
        }
        if (motivo && motivo.length > 0) {
            setOrigem(motivos[0].motivo_contato_id);
        } else {
            setOrigem(motivos[0]);
        }
        setSituacao("EM TRATAMENTO");
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

    useEffect(() => {
        BuscarMotivos();
        BuscarOrigens();
        initState();
    }, [])

    const handdleSubmit = (e) => {
        const formData = new FormData();

        formData.append("HistoricoContatosOcorrenciaId", 0)
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

        api.post("NovoContatoOcorrencia", formData, res => {
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

    return (
        <>
            <ButtonTooltip
                onClick={() => setShow(true)}
                className='btn btn-success'
                text={'Novo Atendimento'}
                top={false}
                Icon={<FaPlus size={17} />}
                textButton={'Novo Atendimento'}
            />
            <Modal isOpen={show} style={{ maxWidth: '80%', margin: '0 auto' }}>
                <form onSubmit={e => { e.preventDefault(); handdleSubmit(e) }}>
                    <ModalHeader>
                        Novo Atendimento
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

export default ModalContatoOcorrencia;