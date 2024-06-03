import React, { useEffect, useState } from 'react';
import { Modal, ModalBody, ModalFooter, ModalHeader, Form, FormGroup, Label, Input, Button, Row, Col } from 'reactstrap';
import { api } from '../../api/api';
import { ButtonTooltip } from '../Inputs/ButtonTooltip';
import { Mascara } from '../../util/mascara';
import { FaPencil } from 'react-icons/fa6';

function ModalEditarPagamento({ ClienteId, ClienteNome, PagamentoId, BuscarPagamentos }) {
    const [show, setShow] = useState(false);
    const [dataPgto, setDataPgto] = useState();
    const [valorPgto, setValorPgto] = useState('');

    const handdleSubmit = () => {
        const formData = new FormData();

        formData.append("PagamentoId", PagamentoId);
        formData.append("ClienteId", ClienteId);
        formData.append("ValorPago", valorPgto.replace('R$ ', '').replace(',', '.').replace(' ', ''));
        formData.append("DataPagamento", dataPgto);

        api.post("EditarPagamento", formData, res => {
            BuscarPagamentos(ClienteId)
            setShow(false);
            alert('Pagamento editado com sucesso!');
        }, er => {
            alert('Houve um erro ao salvar um novo pagamento!')
        })
    }

    const BuscarPagamento = () => {
        api.get(`BuscarPagamentoId/${PagamentoId}`, res => {
            setDataPgto(res.data.dtPagamento);
            setValorPgto(Mascara.moeda(("" + res.data.valorPago).replace(".", ",")))
        }, err => {
            alert('Houve um erro ao buscar dados do pagamento')
        })
    }

    useEffect(() => {
        BuscarPagamento();
    }, [])

    return (
        <>
            <ButtonTooltip
                onClick={() => setShow(true)}
                className='btn btn-warning'
                text={'Editar Pagamento'}
                top={true}
                textButton={<FaPencil color='#fff' size={20} />}
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
                        <button type='button' onClick={() => { BuscarPagamento(); setShow(false) }} className='btn btn-danger'>Fechar</button>
                        <button type='submit' className='btn btn-primary'>Salvar</button>
                    </ModalFooter>
                </form>
            </Modal>
        </>
    );
}

export default ModalEditarPagamento;