import React, { useEffect, useState } from 'react';
import { Modal, ModalBody, ModalFooter, ModalHeader, Form, FormGroup, Label, Input, Button, Row, Col } from 'reactstrap';
import { api } from '../../api/api';
import { FaEdit } from 'react-icons/fa';
import { MdOutlineHistoryToggleOff } from 'react-icons/md';
import { ButtonTooltip } from '../Inputs/ButtonTooltip';
import { Alert } from '../../util/alertas';
import { Size } from '../../util/size';
function ModalLogStatus({ ClienteId }) {

    const [show, setShow] = useState(false);
    const [status, setStatus] = useState([]);

    const BuscarLog = () => {
        api.get(`BuscarLogStatusClienteId/${ClienteId}`, res => {
            setShow(true);
            setStatus(res.data);
        }, err => {
            Alert("Houve um erro ao buscar os status.", false)
        })
    }

    return (
        <form>
            <ButtonTooltip
                backgroundColor={'#008000'}
                onClick={() => BuscarLog()}
                className='btn btn-success button-container-item'
                text={'Log Status'}
                top={true}
                textButton={<MdOutlineHistoryToggleOff size={Size.IconeTabela} />}
            />
            <Modal isOpen={show}>
                <ModalBody>
                    <table className='table table-striped'>
                        <thead>
                            <tr>
                                <td>Id</td>
                                <td>Nome Status</td>
                                <td>Data Atualização</td>
                            </tr>
                        </thead>
                        <tbody>
                            {status.map((sts, i) => (
                                <tr>
                                    <td>{sts.id}</td>
                                    <td>{sts.status} {i === 0 && "(ATUAL)"}</td>
                                    <td>{sts.data}</td>
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