import React, { useEffect, useState } from 'react';
import { Modal, ModalBody, ModalFooter, ModalHeader, Form, FormGroup, Label, Input, Button, Row, Col } from 'reactstrap';
import { api } from '../../api/api';
import { MdOutlineAttachMoney } from 'react-icons/md';
import { ButtonTooltip } from '../Inputs/ButtonTooltip';
import { Mascara } from '../../util/mascara';

function ModalNovoPagamento({ ClienteId, ClienteNome, BuscarPagamentos = null }) {
    const [show, setShow] = useState(false);

    const initState = () => {
        setDataPgto(getDataDeHoje());
        setValorPgto('');
    }

    function getDataDeHoje() {
        const today = new Date();
        const year = today.getFullYear();
        const month = String(today.getMonth() + 1).padStart(2, '0'); // Janeiro é 0!
        const day = String(today.getDate()).padStart(2, '0');

        return `${year}-${month}-${day}`;
    }

    const [dataPgto, setDataPgto] = useState(getDataDeHoje());
    const [valorPgto, setValorPgto] = useState('');

    const handdleSubmit = () => {
        const formData = new FormData();
        formData.append("ClienteId", ClienteId);
        formData.append("ValorPago", valorPgto.replace('R$ ', '').replace(',', '.').replace(' ', ''));
        formData.append("DataPagamento", dataPgto);

        api.post("NovoPagamento", formData, res => {
            initState();
            if (BuscarPagamentos) {
                BuscarPagamentos(ClienteId)
            }
            alert('Pagamento salvo com sucesso!');
            setShow(false);
        }, err => {
            alert(err.response.data)
        })
    }

    return (
        <>
            <ButtonTooltip
                onClick={() => setShow(true)}
                className='btn btn-success'
                text={'Vincular Novo Pagamento'}
                top={true}
                textButton={<MdOutlineAttachMoney size={25} />}
            />
            <Modal isOpen={show}>
                <form onSubmit={e => { e.preventDefault(); handdleSubmit() }}>
                    <ModalHeader>
                        Pagamento De: {ClienteNome}
                    </ModalHeader>
                    <ModalBody>
                        <label>Data Do Pagamento</label>
                        <input required type="date" className='form-control' value={dataPgto} onChange={e => setDataPgto(e.target.value)} />
                        <br />
                        <label>Valor Pago</label>
                        <input required type="text" className='form-control' value={valorPgto} onChange={e => setValorPgto(Mascara.moeda(e.target.value))} />
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

export default ModalNovoPagamento;