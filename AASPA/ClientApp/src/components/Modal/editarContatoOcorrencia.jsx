import React, { useEffect, useState } from 'react';
import { Row, Col, Modal, ModalBody, ModalFooter, ModalHeader, Form, FormGroup, Label, Input, Button } from 'reactstrap';
import { api } from '../../api/api';
import { Mascara } from '../../util/mascara';
import { FaPencil } from 'react-icons/fa6';
import { ButtonTooltip } from '../Inputs/ButtonTooltip';
import { Alert } from '../../util/alertas';

function ModalEditarContatoOcorrencia({ ContatoOcorrenciaId, ClienteId, BuscarHistoricoOcorrenciaCliente }) {
    const [show, setShow] = useState(false);
    const [origens, setOrigens] = useState([]);
    const [motivos, setMotivos] = useState([]);

    const [dtOcorrencia, setDtOcorrencia] = useState();
    const [origem, setOrigem] = useState();
    const [motivo, setMotivo] = useState();
    const [situacao, setSituacao] = useState("ATENDIDA");
    const [desc, setDesc] = useState("");

    const initState = () => {
        api.get(`BuscarContatoOcorrenciaById/${ContatoOcorrenciaId}`, res => {
            setDtOcorrencia(res.data.historico_contatos_ocorrencia_dt_ocorrencia);
            setSituacao(res.data.historico_contatos_ocorrencia_situacao_ocorrencia);
            setDesc(res.data.historico_contatos_ocorrencia_descricao);
            
            setMotivo(res.data.historico_contatos_ocorrencia_motivo_contato_id);
            setOrigem(res.data.historico_contatos_ocorrencia_origem_id);
        }, err => {
            Alert('houve um erro ao buscar o Contato/Ocorrencia do id:' + ContatoOcorrenciaId,false)
        })
    }

    const BuscarMotivos = () => {
        api.get("BuscarTodosMotivos", res => {
            setMotivos(res.data);
            setMotivo(res.data[0].motivo_contato_id);
        }, err => {
            Alert('Houve um erro ao buscar os motivos de contato',false)
        })
    }

    const BuscarOrigens = () => {
        api.get("BuscarTodasOrigem", res => {
            setOrigens(res.data);
            setOrigem(res.data[0].origem_id);
        }, err => {
            Alert('Houve um erro ao buscar os motivos de contato',false)
        })
    }

    useEffect(async () => {
        BuscarMotivos();
        BuscarOrigens();
    }, [])

    const handdleSubmit = (e) => {
        const formData = new FormData();

        formData.append("HistoricoContatosOcorrenciaId", ContatoOcorrenciaId)
        formData.append("HistoricoContatosOcorrenciaOrigemId", origem)
        formData.append("HistoricoContatosOcorrenciaClienteId", ClienteId)
        formData.append("HistoricoContatosOcorrenciaMotivoContatoId", motivo)
        formData.append("HistoricoContatosOcorrenciaDtOcorrencia", dtOcorrencia)
        formData.append("HistoricoContatosOcorrenciaDescricao", desc)
        formData.append("HistoricoContatosOcorrenciaSituacaoOcorrencia", situacao)

        api.post("EditarContatoOcorrencia", formData, res => {
            BuscarHistoricoOcorrenciaCliente(ClienteId);
            Alert('Contato/Ocorrência editada com sucesso!',true)
            setShow(false);
        }, err => {
            Alert('Houve um erro ao editar Contato/Ocorrência',false)
        })
    }

    return (
        <>
            <ButtonTooltip
                onClick={() => {setShow(true);initState()}}
                className='btn btn-warning'
                text={'Editar Ocorrencia'}
                top={true}
                textButton={<FaPencil color='#fff' size={20} />}
            />
            <Modal isOpen={show}>
                <form onSubmit={e => { e.preventDefault(); handdleSubmit(e) }}>
                    <ModalHeader>
                        Adicionar Novo Contato/Ocorrencia
                    </ModalHeader>
                    <ModalBody>
                        <Label>Origem</Label>
                        <select name='HistoricoContatosOcorrenciaOrigemId' value={origem} onChange={e => setOrigem(e.target.value)} required className='form-control'>
                            {origens.map(origem => (
                                <option value={origem.origem_id}>{origem.origem_nome}</option>
                            ))}
                        </select>
                        <Label>Data / Hora da ocorrência</Label>
                        <input name='HistoricoContatosOcorrenciaDtOcorrencia' value={dtOcorrencia} onChange={e => setDtOcorrencia(e.target.value)} required type="datetime-local" className='form-control' />
                        <Label>Motivo do contato</Label>
                        <select name='HistoricoContatosOcorrenciaMotivoContatoId' value={motivo} onChange={e => setMotivo(e.target.value)} required className='form-control'>
                            {motivos.map(motivo_contato => (
                                <option value={motivo_contato.motivo_contato_id}>{motivo_contato.motivo_contato_nome}</option>
                            ))}
                        </select>
                        <Label>Situação Da Ocorrência</Label>
                        <select name='HistoricoContatosOcorrenciaSituacaoOcorrencia' value={situacao} onChange={e => setSituacao(e.target.value)} required className='form-control'>
                            <option value="ATENDIDA">ATENDIDA</option>
                            <option value="EM TRATAMENTO">EM TRATAMENTO</option>
                            <option value="CANCELADA">CANCELADA</option>
                            <option value="FINALIZADO">FINALIZADO</option>
                            <option value="REEMBOLSO AGENDADO">REEMBOLSO AGENDADO</option>
                        </select>
                        <Label>Descrição Da Ocorrência</Label>
                        <textarea name='HistoricoContatosOcorrenciaDescricao' value={desc} onChange={e => setDesc(e.target.value)} required maxLength={1000} className='form-control'></textarea>
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

export default ModalEditarContatoOcorrencia;