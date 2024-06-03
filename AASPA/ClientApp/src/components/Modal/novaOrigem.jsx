import React, { useEffect, useState } from 'react';
import { Modal, ModalBody, ModalFooter, ModalHeader, Form, FormGroup, Label, Input, Button, Row, Col } from 'reactstrap';
import { api } from '../../api/api';
import { ButtonTooltip } from '../Inputs/ButtonTooltip';
import { FaPlus } from 'react-icons/fa6';

function ModalNovaOrigem({ BuscarOrigem }) {
    const [show, setShow] = useState(false);
    const [nomeOrigem, setNomeOrigem] = useState('');

    const initState = () => {
        setNomeOrigem('');
    }

    const handdleSubmit = () => {
        const formData = new FormData();
        formData.append("nomeOrigem", nomeOrigem);

        api.post("NovaOrigem", formData, res => {
            BuscarOrigem();
            initState();
            setShow(false);
            alert('Origem salvo com sucesso!');
        }, er => {
            alert(err.response.data)
        })
    }

    return (
        <>
            <ButtonTooltip
                onClick={() => setShow(true)}
                className='btn btn-success'
                text={'Adicionar Origem'}
                top={true}
                textButton={<FaPlus size={25} />}
            />
            <Modal isOpen={show}>
                <form onSubmit={e => { e.preventDefault(); handdleSubmit() }}>
                    <ModalHeader>
                        Adicionar Origem
                    </ModalHeader>
                    <ModalBody>
                        <label>Nome Do Origem</label>
                        <input required type="text" className='form-control' value={nomeOrigem} onChange={e => setNomeOrigem(e.target.value.toUpperCase())} />
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

export default ModalNovaOrigem;