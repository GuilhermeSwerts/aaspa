import React, { useState } from 'react';
import { Modal, ModalBody, ModalFooter, ModalHeader, Form, FormGroup, Label, Input, Button } from 'reactstrap';
import { api } from '../../api/api';
import { Alert } from '../../util/alertas';
function ModalStatus({ BuscarTodosStatus }) {

    const [show, setShow] = useState(false);
    const [status, setStatus] = useState('');

    const handleSubmit = () => {
        if (status === '') {
            Alert('Preencha o nome do status.', false)
            return;
        }

        var formData = new FormData();
        formData.append("status_nome", status);
        api.post("InserirStatus", formData, res => {
            setStatus('');
            BuscarTodosStatus();
            Alert("Status adicionado com sucesso!")
            setShow(false);
        }, err => {
            Alert(err.response.data, false)
        })
    }

    return (
        <form>
            <button type='button' onClick={() => { setShow(true) }} className='btn btn-primary'>Novo Status</button>
            <Modal isOpen={show}>
                <ModalHeader>
                    Novo Status
                </ModalHeader>
                <ModalBody>
                    <FormGroup>
                        <Label for="cpf">Nome Do Status</Label>
                        <Input required placeholder='Nome Do Status' type="text" name="status" id="status" value={status} onChange={e => setStatus(e.target.value.toUpperCase())} />
                    </FormGroup>
                </ModalBody>
                <ModalFooter>
                    <button onClick={() => { setStatus(''); setShow(false) }} className='btn btn-danger'>Cancelar</button>
                    <button onClick={handleSubmit} className='btn btn-success'>Salvar</button>
                </ModalFooter>
            </Modal>
        </form>
    );
}

export default ModalStatus;