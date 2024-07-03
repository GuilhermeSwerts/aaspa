import React, { useEffect, useState } from 'react';
import { Modal, ModalBody, ModalFooter, ModalHeader, Form, FormGroup, Label, Input, Button, Row, Col } from 'reactstrap';
import { api } from '../../api/api';
import { MdOutlineAttachMoney } from 'react-icons/md';
import { ButtonTooltip } from '../Inputs/ButtonTooltip';
import { Mascara } from '../../util/mascara';
import { Alert } from '../../util/alertas';
import { LuImport } from "react-icons/lu";
import moment from 'moment';

function ImportarCLientesIntegral() {
    const [show, setShow] = useState(false);

    const initState = () => {
        setDataCadastroSelecionada(getDataDeHoje());
    }

    function getDataDeHoje() {
        const today = new Date();
        const year = today.getFullYear();
        const month = String(today.getMonth() + 1).padStart(2, '0'); // Janeiro é 0!
        const day = String(today.getDate()).padStart(2, '0');

        return `${year}-${month}-${day}`;
    }
    const [dataCadastroSelecionada, setDataCadastroSelecionada] = useState(getDataDeHoje());
    const [dataCadastroFimSelecionada, setDataCadastroFimSelecionada] = useState(getDataDeHoje());

    const handdleSubmit = () => {
        api.get(`GetClientesIntegraall?DataCadastroInicio=${dataCadastroSelecionada}&DataCadastroFim=${dataCadastroFimSelecionada}`, res => {
            initState();
            Alert('Clientes Cadastrados com sucesso!');
            setShow(false);
        }, err => {
            Alert(err.response.data, false)
        })
    }

    return (
        <>
            <button
                style={{ width: '100%' }}
                type='button'
                onClick={() => setShow(true)}
                className='btn btn-primary'>Importar Clientes Integrall <LuImport size={17} /></button>
            <Modal isOpen={show}>
                <form onSubmit={e => { e.preventDefault(); handdleSubmit() }}>
                    <ModalHeader>
                        Importar Clientes Integral
                    </ModalHeader>
                    <ModalBody>
                        <div className='row'>
                            <div className="col-md-4">
                                <label htmlFor="">De:</label>
                                <input
                                    required
                                    className='form-control'
                                    type="date"
                                    name="date"
                                    id="date"
                                    value={dataCadastroSelecionada}
                                    onChange={e => setDataCadastroSelecionada(e.target.value)}
                                />
                            </div>
                            <div className="col-md-4">
                                <label htmlFor="">Até:</label>
                                <input
                                    required
                                    className='form-control'
                                    type="date"
                                    name="dateFim"
                                    id="dateFim"
                                    value={dataCadastroFimSelecionada}
                                    onChange={e => setDataCadastroFimSelecionada(e.target.value)}
                                />
                            </div>
                        </div>
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

export default ImportarCLientesIntegral;