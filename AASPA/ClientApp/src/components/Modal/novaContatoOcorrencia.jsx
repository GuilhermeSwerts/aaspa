import React, { useEffect, useState } from 'react';
import { Row, Col, Modal, ModalBody, ModalFooter, ModalHeader, Form, FormGroup, Label, Input, Button } from 'reactstrap';
import { api } from '../../api/api';
import { Mascara } from '../../util/mascara';
import { FaPlus } from 'react-icons/fa6';
import { ButtonTooltip } from '../Inputs/ButtonTooltip';

function ModalContatoOcorrencia({ cliente, BuscarHistoricoOcorrenciaCliente = null }) {
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
    const [situacao, setSituacao] = useState("ATENDIDA");
    const [desc, setDesc] = useState("");

    const initState = () => {
        setDtOcorrencia(getDataDeHoje());
        setOrigem(origens[0]);
        setMotivo(motivos[0]);
        setSituacao("ATENDIDA");
        setDesc("");
    }

    const BuscarMotivos = () => {
        api.get("BuscarTodosMotivos", res => {
            setMotivos(res.data);
            setMotivo(res.data[0].motivo_contato_id);
        }, err => {
            alert('Houve um erro ao buscar os motivos de contato')
        })
    }

    const BuscarOrigens = () => {
        api.get("BuscarTodasOrigem", res => {
            setOrigens(res.data);
            setOrigem(res.data[0].origem_id);
        }, err => {
            alert('Houve um erro ao buscar os motivos de contato')
        })
    }

    useEffect(() => {
        BuscarMotivos();
        BuscarOrigens();
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

        api.post("NovoContatoOcorrencia", formData, res => {
            initState();
            if (BuscarHistoricoOcorrenciaCliente) {
                BuscarHistoricoOcorrenciaCliente(cliente.cliente_id);
            }
            alert('Contato/Ocorrência adicionado com sucesso!')
        }, err => {
            alert(err.response.data)
        })
    }

    return (
        <>
            <ButtonTooltip
                onClick={() => setShow(true)}
                className='btn btn-success'
                text={'Historico Contatos/Ocorrências'}
                top={true}
                textButton={<FaPlus size={25} />}
            />
            <Modal isOpen={show}>
                <form onSubmit={e => { e.preventDefault(); handdleSubmit(e) }}>
                    <ModalHeader>
                        Adicionar Novo Contato/Ocorrencia
                    </ModalHeader>
                    <ModalBody>
                        <input type="hidden" value={cliente.cliente_id} />
                        <Label>Origem</Label>
                        <select name='HistoricoContatosOcorrenciaOrigemId' value={origem} onChange={e => setOrigem(e.target.value)} required className='form-control'>
                            {origens.map(origem => (
                                <option value={origem.origem_id}>{origem.origem_nome}</option>
                            ))}
                        </select>
                        <Label>Data / Hora da ocorrência</Label>
                        <input name='HistoricoContatosOcorrenciaDtOcorrencia' value={dtOcorrencia} onChange={e => setDtOcorrencia(e.target.value)} required type="datetime-local" className='form-control' />
                        <Label>Cpf do cliente</Label>
                        <input required type="text" disabled value={Mascara.cpf(cliente.cliente_cpf)} className='form-control' />
                        <Label>Nome do Contato/Usuário</Label>
                        <input required type="text" disabled value={cliente.cliente_nome} className='form-control' />
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

export default ModalContatoOcorrencia;