import React, { useEffect, useState } from 'react';
import { Modal, ModalBody, ModalFooter, ModalHeader, Form, FormGroup, Label, Input, Button, Row, Col } from 'reactstrap';
import { api } from '../../api/api';
import { ButtonTooltip } from '../Inputs/ButtonTooltip';
import { FaPencil, FaPlus } from 'react-icons/fa6';
import { Alert } from '../../util/alertas';
import { Size } from '../../util/size';

function ModalEditarOrigem({ BuscarOrigem, OrigemId }) {
    const [show, setShow] = useState(false);
    const [nomeOrigem, setNomeOrigem] = useState('');

    const initState = () => {
        api.get(`BuscarOrigemId/${OrigemId}`, res => {
            setNomeOrigem(res.data.origem_nome);
        })
    }

    const handdleSubmit = () => {
        const formData = new FormData();
        formData.append("OrigemId", OrigemId);
        formData.append("NomeOrigem", nomeOrigem);

        api.post("EditarOrigem", formData, res => {
            BuscarOrigem();
            initState();
            setShow(false);
            Alert('Origem editado com sucesso!', true);
        }, er => {
            Alert('Houve um erro ao editar a Origem!', false)
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
                text={'Editar Origem'}
                top={true}
                textButton={<FaPencil color='#fff' size={Size.IconeTabela} />}
            />
            <Modal isOpen={show}>
                <form onSubmit={e => { e.preventDefault(); handdleSubmit() }}>
                    <ModalHeader>
                        Editar Origem
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

export default ModalEditarOrigem;