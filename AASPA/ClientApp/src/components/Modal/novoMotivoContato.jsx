import React, { useEffect, useState } from 'react';
import { Modal, ModalBody, ModalFooter, ModalHeader, Form, FormGroup, Label, Input, Button, Row, Col } from 'reactstrap';
import { api } from '../../api/api';
import { ButtonTooltip } from '../Inputs/ButtonTooltip';
import { FaPlus } from 'react-icons/fa6';

function ModalNovoMotivoDoContato({ BuscarMotivos }) {
    const [show, setShow] = useState(false);
    const [nomeMotivo, setNomeMotivo] = useState('');

    const initState = () => {
        setNomeMotivo('');
    }

    const handdleSubmit = () => {
        const formData = new FormData();
        formData.append("nomeMotivo", nomeMotivo);

        api.post("NovoMotivo", formData, res => {
            BuscarMotivos();
            initState();
            setShow(false);
            alert('Motivo Do Contato salvo com sucesso!');
        }, er => {
            alert(err.response.data)
        })
    }

    return (
        <>
            <ButtonTooltip
                onClick={() => setShow(true)}
                className='btn btn-success'
                text={'Adicionar Motivo Do Contato'}
                top={true}
                textButton={<FaPlus size={25} />}
            />
            <Modal isOpen={show}>
                <form onSubmit={e => { e.preventDefault(); handdleSubmit() }}>
                    <ModalHeader>
                        Adicionar Motivo Do Contato
                    </ModalHeader>
                    <ModalBody>
                        <label>Nome Do Motivo Do Contato</label>
                        <input required type="text" className='form-control' value={nomeMotivo} onChange={e => setNomeMotivo(e.target.value.toUpperCase())} />
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

export default ModalNovoMotivoDoContato;