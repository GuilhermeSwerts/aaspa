import React, { useEffect, useState } from 'react';
import { Modal, ModalBody, ModalFooter, ModalHeader, Form, FormGroup, Label, Input, Button, Row, Col } from 'reactstrap';
import { api } from '../../api/api';
import { ButtonTooltip } from '../Inputs/ButtonTooltip';
import { FaPencil, FaPlus } from 'react-icons/fa6';

function ModalEditarMotivoDoContato({ BuscarMotivos, MotivoId }) {
    const [show, setShow] = useState(false);
    const [nomeMotivo, setNomeMotivo] = useState('');

    const initState = () => {
        api.get(`BuscarMotivosId/${MotivoId}`, res => {
            setNomeMotivo(res.data.motivo_contato_nome);
        })
    }

    const handdleSubmit = () => {
        const formData = new FormData();
        formData.append("MotivoId", MotivoId);
        formData.append("NomeMotivo", nomeMotivo);

        api.post("EditarMotivo", formData, res => {
            BuscarMotivos();
            initState();
            setShow(false);
            alert('Motivo Do Contato editado com sucesso!');
        }, er => {
            alert('Houve um erro ao editar o Motivo Do Contato!')
        })
    }

    useEffect(() => {
        initState();
    }, [])

    return (
        <>
            <ButtonTooltip
                onClick={() => setShow(true)}
                className='btn btn-warning'
                text={'Editar Motivo Do Contato'}
                top={true}
                textButton={<FaPencil color='#fff' size={25} />}
            />
            <Modal isOpen={show}>
                <form onSubmit={e => { e.preventDefault(); handdleSubmit() }}>
                    <ModalHeader>
                        Editar Motivo Do Contato
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

export default ModalEditarMotivoDoContato;