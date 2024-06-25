import React, { useEffect, useState } from 'react';
import { Modal, ModalBody, ModalFooter, ModalHeader, Form, FormGroup, Label, Input, Button } from 'reactstrap';
import { api } from '../../api/api';
import { FaEdit } from 'react-icons/fa';
import { Alert } from '../../util/alertas';
function ModalEditarStatus({ BuscarTodosStatus, StatusId }) {

    const [show, setShow] = useState(false);
    const [oldStatus, setOldStatus] = useState('');
    const [status, setStatus] = useState('');

    useEffect(() => {
        api.get(`StatusId/${StatusId}`, res => {
            setStatus(res.data.status_nome);
            setOldStatus(res.data.status_nome);
        }, err => {
            Alert("Houve um erro ao buscar status.", false)
        })
    }, [])

    const handleSubmit = () => {
        if (status === '' && oldStatus === status) {
            Alert('Preencha o nome do status.', false)
            return;
        }

        var formData = new FormData();
        formData.append("status_id", StatusId);
        formData.append("status_nome", status);
        api.post("EditarStatus", formData, res => {
            BuscarTodosStatus();
            Alert("Status Editado com sucesso!", true)
            setShow(false);
        }, err => {
            Alert("Houve um erro ao editar um status.", false)
        })
    }

    return (
        <form>
            <button type='button' onClick={() => { setShow(true) }} className='btn btn-warning'><FaEdit color='#fff' size={20} /></button>
            <Modal isOpen={show}>
                <ModalHeader>
                    Editar Status
                </ModalHeader>
                <ModalBody>
                    <FormGroup>
                        <Label for="cpf">Nome Do Status</Label>
                        <Input required placeholder='Nome Do Status' type="text" name="status" id="status" value={status} onChange={e => setStatus(e.target.value.toUpperCase())} />
                    </FormGroup>
                </ModalBody>
                <ModalFooter>
                    <button onClick={() => { setStatus(oldStatus); setShow(false) }} className='btn btn-danger'>Cancelar</button>
                    <button onClick={handleSubmit} className='btn btn-success'>Salvar</button>
                </ModalFooter>
            </Modal>
        </form>
    );
}

export default ModalEditarStatus;