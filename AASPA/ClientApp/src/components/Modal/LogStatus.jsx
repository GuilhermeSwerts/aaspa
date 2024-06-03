import React, { useEffect, useState } from 'react';
import { Modal, ModalBody, ModalFooter, ModalHeader, Form, FormGroup, Label, Input, Button, Row, Col } from 'reactstrap';
import { api } from '../../api/api';
import { FaEdit } from 'react-icons/fa';
import { MdOutlineHistoryToggleOff } from 'react-icons/md';
import { ButtonTooltip } from '../Inputs/ButtonTooltip';
function ModalLogStatus({ ClienteId, ClienteNome }) {

    const [show, setShow] = useState(false);
    const [status, setStatus] = useState([]);

    function getDataDeHoje() {
        const today = new Date();
        const year = today.getFullYear();
        const month = String(today.getMonth() + 1).padStart(2, '0'); // Janeiro é 0!
        const day = String(today.getDate()).padStart(2, '0');

        return `${year}-${month}-${day}`;
    }

    const [filtroDe, setFiltroDe] = useState(getDataDeHoje());
    const [filtroPara, setFiltroPara] = useState(getDataDeHoje());

    const BuscarLog = () => {
        api.get(`BuscarLogStatusClienteId/${ClienteId}?dtInicio=${filtroDe}&dtFim=${filtroPara}`, res => {
            setStatus(res.data);
        }, err => {
            alert("Houve um erro ao buscar os status.")
        })
    }

    useEffect(() => {
        BuscarLog();
    }, [])

    return (
        <form>
            <ButtonTooltip
                onClick={() => setShow(true)}
                className='btn btn-success'
                text={'Log Status'}
                top={true}
                textButton={<MdOutlineHistoryToggleOff size={25} />}
            />
            <Modal isOpen={show}>
                <ModalHeader>
                    Log Status De: {ClienteNome}
                </ModalHeader>
                <ModalBody>
                    <Row>
                        <Col md={4}>
                            <FormGroup>
                                <Label>De:</Label>
                                <input onChange={e => setFiltroDe(e.target.value)} className='form-control' value={filtroDe} type="date" name="de" id="de" />
                            </FormGroup>
                        </Col>
                        <Col md={4}>
                            <FormGroup>
                                <Label>Para:</Label>
                                <input onChange={e => setFiltroPara(e.target.value)} className='form-control' value={filtroPara} type="date" name="de" id="de" />
                            </FormGroup>
                        </Col>
                        <Col md={4} style={{ marginTop: '2rem' }}>
                            <FormGroup>
                                <Label></Label>
                                <button type="button" onClick={BuscarLog} className='btn btn-success'>Buscar</button>
                            </FormGroup>
                        </Col>
                    </Row>
                    <br />
                    <table className='table table-striped'>
                        <thead>
                            <tr>
                                <td>
                                    Data
                                </td>
                                <td>
                                    De
                                </td>
                                <td>
                                    Para
                                </td>
                            </tr>
                        </thead>
                        <tbody>
                            {status.map((sts, i) => (
                                <tr>
                                    <td>{sts.data}</td>
                                    <td>{sts.de}</td>
                                    <td>{sts.para}</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </ModalBody>
                <ModalFooter>
                    <button onClick={() => { setShow(false) }} className='btn btn-danger'>Fechar</button>
                </ModalFooter>
            </Modal>
        </form>
    );
}

export default ModalLogStatus;