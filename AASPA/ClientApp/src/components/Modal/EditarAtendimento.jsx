import React, { useEffect, useRef, useState } from 'react';
import { Row, Col, Modal, ModalBody, ModalFooter, ModalHeader, Form, FormGroup, Label, Input, Button } from 'reactstrap';
import { api } from '../../api/api';
import { Mascara } from '../../util/mascara';
import { FaPencil, FaEye, FaPlus } from 'react-icons/fa6';
import { ButtonTooltip } from '../Inputs/ButtonTooltip';
import { Alert } from '../../util/alertas';
import { Size } from '../../util/size';
import { FaTrash } from 'react-icons/fa6';
import { FiPaperclip } from 'react-icons/fi';
import ModalVisualizarAnexo from './modalVisualizarAnexo';

function ModalEditarAtendimento({ cliente, BuscarHistoricoOcorrenciaCliente = null, HistoricoId }) {
    const modalAnexo = useRef();
    const [show, setShow] = useState(false);
    const [origens, setOrigens] = useState([]);
    const [motivos, setMotivos] = useState([]);
    const [telefone, setTelefone] = useState("");
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
    const [tipoChavePix, setTipoChavePix] = useState("CPF");
    const [tipoPagamento, setTipoPagamento] = useState(true);

    const [anexos, setAnexos] = useState([]);

    const onChangeTipoPagamento = e => setTipoPagamento(e.target.value === "0")
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
        setPIX("");
        setTipoChavePix("CPF");
        setTipoPagamento(true);
        setTelefone("");
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

    function base64ToFile(base64String, fileName, mimeType) {
        const byteString = window.atob(base64String);
        const ab = new ArrayBuffer(byteString.length);
        const ia = new Uint8Array(ab);

        for (let i = 0; i < byteString.length; i++) {
            ia[i] = byteString.charCodeAt(i);
        }

        const blob = new Blob([ab], { type: mimeType });

        return new File([blob], fileName, { type: mimeType });
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
        formData.append("HistoricoContatosOcorrenciaTipoChavePix", tipoChavePix)
        formData.append("HistoricoContatosOcorrenciaTelefone", telefone)

        for (const file of anexos) {
            if (file.existente) {
                let blob = base64ToFile(file.file, file.fileName, file.fileName.includes(".pdf") ? 'application/pdf' : 'image/png');
                formData.append('HistoricoContatosOcorrenciaAnexos', blob, file.fileName);
            } else {
                formData.append('HistoricoContatosOcorrenciaAnexos', file.file, file.fileName)
            }
        }

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
            setTipoChavePix(res.data.historico_contatos_ocorrencia_tipo_chave_pix);
            setTelefone(res.data.historico_contatos_ocorrencia_telefone);

            let arquivos = [];
            res.data.anexos.forEach(anexo => {
                arquivos.push({ file: anexo.anexo_anexo, fileName: anexo.anexo_nome, existente: true });
            });

            setAnexos(arquivos);

            setShow(true);
        }, err => {
            Alert('houve um erro ao buscar o Contato/Ocorrencia do id:' + HistoricoId, false)
        })
    }

    const RemoverAnexo = (index) => {
        var arry = [...anexos];
        arry.splice(index, 1);
        setAnexos(arry);
    }

    const handleAnexoChange = (event) => {
        const file = event.target.files[0];
        const allowedTypes = ['application/pdf', 'image/jpeg', 'image/png'];
        if (file && allowedTypes.includes(file.type)) {
            const novoAnexo = { file: file, fileName: file.name };
            let arry = [...anexos];
            arry.push(novoAnexo);
            setAnexos(arry);
        } else {
            alert('Somente arquivos PDF ou imagens (JPG, PNG) são permitidos.');
        }
    };

    const VisualizarAnexo = (index) => {
        var anexo = anexos[index];
        let obj = {
            anexo_nome: anexo.fileName,
            anexo_anexo: anexo.file,
            anexo_tipo: anexo.fileName.includes('.pdf')
        }
        modalAnexo.current.VisualizarAnexo(obj);
    }

    return (
        <>
            <ButtonTooltip
                onClick={handleOpenModal}
                className='btn btn-warning'
                text={'Editar Atendimento'}
                top={true}
                textButton={<FaPencil size={Size.IconeTabela} />}
            />
            <Modal isOpen={show} style={{ maxWidth: '80%', margin: '0 auto' }}>
                <ModalVisualizarAnexo ref={modalAnexo} />
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
                                    <option value="REEMBOLSO AGENDADO">REEMBOLSO AGENDADO</option>
                                    <option value="DADOS INVALIDOS">DADOS INVALIDOS</option>
                                </select>
                            </div>
                        </div>
                        <hr />
                        <small><b>Dados Bancários:</b></small>
                        <div className='row'>
                            <div className="col-md-3">
                                <label>Tipo de depósito:</label>
                                <select onChange={onChangeTipoPagamento} className='form-control'>
                                    <option value="0">PIX</option>
                                    <option value="1">Dados bancários</option>
                                </select>
                            </div>
                            {!tipoPagamento && <>
                                <div className="col-md-3">
                                    <Label>Banco</Label>
                                    <input type="text" value={banco} onChange={e => setBanco(e.target.value)} className='form-control' />
                                </div>
                                <div className="col-md-2">
                                    <Label>Agência</Label>
                                    <input maxLength={6} type="text" value={agencia} onChange={e => setAgencia(e.target.value)} className='form-control' />
                                </div>
                                <div className="col-md-3">
                                    <Label>Conta</Label>
                                    <input maxLength={15} type="text" value={conta} onChange={e => setConta(e.target.value)} placeholder='Conta sem dígito' className='form-control' />
                                </div>
                                <div className="col-md-1">
                                    <Label>Dígito</Label>
                                    <input maxLength={15} type="text" value={digito} onChange={e => setDigito(e.target.value)} placeholder='Dígito' className='form-control' />
                                </div>
                            </>}
                            {tipoPagamento && <>
                                <div className="col-md-3">
                                    <Label>Tipo Chave PIX</Label>
                                    <select value={tipoChavePix} onChange={e => setTipoChavePix(e.target.value)} className='form-control'>
                                        <option value="CPF">CPF</option>
                                        <option value="CNPJ">CNPJ</option>
                                        <option value="telefone">telefone</option>
                                        <option value="e-mail">E-mail</option>
                                        <option value="chave aleatória">Chave aleatória</option>
                                    </select>
                                </div>
                                <div className="col-md-4">
                                    <Label>Chave PIX</Label>
                                    <input type="text" value={pix} onChange={e => setPIX(e.target.value)} placeholder='Chave PIX' className='form-control' />
                                </div>
                            </>}
                        </div>
                        <hr />
                        <small><b>Dados Extras:</b></small>
                        <div className="row">
                            <div className="col-md-3">
                                <Label>Telefone De Contato</Label>
                                <input className='form-control' maxLength={15} value={(Mascara.telefone(telefone))} placeholder='(__) _____-____' onChange={e => setTelefone(e.target.value)} type="text" name="" id="" />
                            </div>
                        </div>
                        <hr />
                        <small>Anexos:</small>
                        <div className="row">
                            <div className="col-md-4">
                                <button type='button' onClick={() => document.getElementById('anexo').click()} className='btn btn-primary'>Novo Anexo <FiPaperclip size={Size.IconeTabela} /></button>
                                <input onChange={handleAnexoChange} type="file" style={{ display: 'none' }} id='anexo' />
                                <br />
                                {anexos.map((anexo, i) => (
                                    <div style={{ width: '100%', display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginTop: 5 }}>
                                        {anexo.fileName}
                                        <div style={{ gap: 5, display: 'flex', justifyContent: 'center', alignItems: 'center', }}>
                                            {anexo.existente && <button type='button' onClick={e => VisualizarAnexo(i)} className='btn btn-primary'><FaEye size={Size.IconePequeno} /></button>}
                                            <button type='button' onClick={e => RemoverAnexo(i)} className='btn btn-primary'><FaTrash size={Size.IconePequeno} /></button>
                                        </div>
                                    </div>
                                ))}
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
                        <button type='submit' className='btn btn-primary'>Salvar Alterações</button>
                    </ModalFooter>
                </form>
            </Modal>
        </>
    );
}

export default ModalEditarAtendimento;