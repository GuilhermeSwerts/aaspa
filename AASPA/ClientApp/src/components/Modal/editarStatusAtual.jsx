import React, { useEffect, useState } from 'react';
import { Modal, ModalBody, ModalFooter, ModalHeader, Form, FormGroup, Label, Input, Button } from 'reactstrap';
import { api } from '../../api/api';
import { FaEdit } from 'react-icons/fa';
import { ButtonTooltip } from '../Inputs/ButtonTooltip';
import { FiMoreHorizontal } from 'react-icons/fi';
import * as Enum from '../../util/enum';
import { Alert, Pergunta } from '../../util/alertas';
import { Size } from '../../util/size';

function ModalEditarStatusAtual({ BuscarTodosClientes, ClienteId, StatusId }) {

    const [show, setShow] = useState(false);
    const [oldStatus, setOldStatus] = useState('');
    const [todosStatus, setTodosStatus] = useState([]);
    const [novoStatus, setNovoStatus] = useState(1);
    const [motivoInativo, setMotivoInativo] = useState('');    

    const BuscarTodosStatus = () => {
        api.get("TodosStatus", res => {
            setTodosStatus(res.data);
        }, err => {
            Alert("Houve um erro ao buscar os status.", false)
        })
    }

    useEffect(() => {
        api.get(`StatusId/${StatusId}`, res => {
            setOldStatus(res.data.status_nome);
            BuscarTodosStatus();
        }, err => {
            Alert("Houve um erro ao buscar status.", false)
        })
    }, [])

    const handleSubmit = async () => {
        if (novoStatus == StatusId) {
            Alert("Escolha um status diferente do atual.", false);
            return;
        }

        if (novoStatus == Enum.EStatus.Inativo) {

            if (!(await Pergunta("Deseja realmente inativar esse cliente?\nVocê não poderá realizar alterações até que seja reativado.\n Deseja continuar?"))) {
                return;
            }
        }

        if (novoStatus == Enum.EStatus.Deletado) {
            if (!(await Pergunta("Deseja realmente deletar esse cliente?"))) {
                return;
            }
        }

        var formData = new FormData();
        if (motivoInativo === '') {
            Alert("Selecione o motivo para inativar o cliente!", false);
            return;
        }
        formData.append("status_id_antigo", StatusId);
        formData.append("status_id_novo", novoStatus);
        formData.append("cliente_id", ClienteId);

        api.post("/AlterarStatusCliente", formData, res => {
            Alert("Status atualizado com sucesso!", true);
            BuscarTodosClientes();
            setShow(false);
        }, err => {
             Alert("Houve um erro ao editar o status.", false);
        })
    }

    const handleCloseModal = () => {
        setShow(false);
        setNovoStatus(1);
        setMotivoInativo('');
    };

    return (
        <form>
            <ButtonTooltip
                onClick={() => setShow(true)}
                className='btn btn-warning button-container-item'
                text={'Editar Status'}
                top={true}
                textButton={<FiMoreHorizontal size={Size.IconeTabela} />}
            />
            <Modal isOpen={show}>
                <ModalHeader>
                    Editar Status Atual
                </ModalHeader>
                <ModalBody>
                    <span>Status Atual: <b>{oldStatus}</b></span>
                    <FormGroup>
                        <Label for="cpf">Selecione o novo status</Label>
                        <br />
                        <select className='form-control' onChange={e => setNovoStatus(e.target.value)} name="novoStatus" id="novoStatus">
                            {todosStatus.map((sts) => (
                                <option value={sts.status_id}>{sts.status_nome}</option>
                            ))}
                        </select>
                    </FormGroup>                    
                </ModalBody>
                <ModalFooter>
                    <button onClick={handleCloseModal} className='btn btn-danger'>Cancelar</button>
                    <button onClick={handleSubmit} className='btn btn-success'>Salvar</button>
                </ModalFooter>
            </Modal>
        </form>
    );
}

export default ModalEditarStatusAtual;