import React, { useEffect, useState } from 'react';
import { Modal, ModalBody, ModalFooter, ModalHeader, Form, FormGroup, Label, Input, Button } from 'reactstrap';
import { api } from '../../api/api';
import { FaEdit } from 'react-icons/fa';
import { ButtonTooltip } from '../Inputs/ButtonTooltip';
import { FiMoreHorizontal } from 'react-icons/fi';

function ModalEditarStatusAtual({ BuscarTodosClientes, ClienteId, StatusId }) {

    const [show, setShow] = useState(false);
    const [oldStatus, setOldStatus] = useState('');
    const [todosStatus, setTodosStatus] = useState([]);
    const [novoStatus, setNovoStatus] = useState(1);

    const BuscarTodosStatus = () => {
        api.get("TodosStatus", res => {
            setTodosStatus(res.data);
        }, err => {
            alert("Houve um erro ao buscar os status.")
        })
    }

    useEffect(() => {
        api.get(`StatusId/${StatusId}`, res => {
            setOldStatus(res.data.status_nome);
            BuscarTodosStatus();
        }, err => {
            alert("Houve um erro ao buscar status.")
        })
    }, [])

    const handleSubmit = async () => {
        if (novoStatus === StatusId) {
            alert("Escolha um status diferente do atual.");
            return;
        }

        var formData = new FormData();
        formData.append("status_id_antigo", StatusId);
        formData.append("status_id_novo", novoStatus);
        formData.append("cliente_id", ClienteId);

        api.post("AlterarStatusCliente", formData, async res => {
            await alert("A pagina será recarregada\nPor favor aguarde...");
            BuscarTodosClientes();
            setShow(false);
        }, err => {
            alert("Houve um erro ao editar um status.")
        })
    }

    return (
        <form>
            <ButtonTooltip
                onClick={() => setShow(true)}
                className='btn btn-danger'
                text={'Editar Status Cliente'}
                top={true}
                textButton={<FiMoreHorizontal size={25} />}
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
                    <button onClick={() => { setShow(false) }} className='btn btn-danger'>Cancelar</button>
                    <button onClick={handleSubmit} className='btn btn-success'>Salvar</button>
                </ModalFooter>
            </Modal>
        </form>
    );
}

export default ModalEditarStatusAtual;