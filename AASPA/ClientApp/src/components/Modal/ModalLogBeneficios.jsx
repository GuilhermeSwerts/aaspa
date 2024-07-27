import React, { useEffect, useState } from 'react';
import { Modal, ModalFooter, ModalHeader, Form, FormGroup, Label, Input, Button, Row, Col } from 'reactstrap';
import { api } from '../../api/api';
import { MdOutlineWorkHistory } from 'react-icons/md';
import { ButtonTooltip } from '../../components/Inputs/ButtonTooltip';
import { Alert } from '../../util/alertas';

function ModalLogBeneficios({ ClienteId, ClienteNome }) {
    const [show, setShow] = useState(false);

    function getDataDeHoje() {
        const today = new Date();
        const year = today.getFullYear();
        const month = String(today.getMonth() + 1).padStart(2, '0'); // Janeiro é 0!
        const day = String(today.getDate()).padStart(2, '0');

        return `${year}-${month}-${day}`;
    }

    const [filtroDe, setFiltroDe] = useState(getDataDeHoje());
    const [filtroPara, setFiltroPara] = useState(getDataDeHoje());
    const [beneficios, setBeneficios] = useState([]);

    const BuscarLogBeneficios = () => {
        api.get(`BuscarLogBeneficios/${ClienteId}?dtInicio=${filtroDe}&dtFim=${filtroPara}`, res => {
            setBeneficios(res.data);
        }, err => {
            Alert("Houve um erro ao buscar os status.", false)
        })
    }

    useEffect(() => {
        BuscarLogBeneficios();
    }, [])

    return (
        <form>
            <ButtonTooltip
                backgroundColor={'#009900'}
                onClick={() => setShow(true)}
                className='btn btn-success button-container-item'
                text={'Log Beneficio'}
                top={true}
                textButton={<MdOutlineWorkHistory size={10} />}
            />
            <Modal isOpen={show}>
                <ModalHeader>
                    Log Beneficio De: {ClienteNome}
                </ModalHeader>
                <small style={{ textAlign: 'center' }}>Filtro: Data cadastro do log.</small>
                <Row>
                    <Col md={1} />
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
                    <Col md={3} style={{ marginTop: '2rem' }}>
                        <FormGroup>
                            <Label></Label>
                            <button type="button" onClick={BuscarLogBeneficios} className='btn btn-success'>Buscar</button>
                        </FormGroup>
                    </Col>
                </Row>
                <br />
                <table className='table table-striped'>
                    <thead>
                        <tr>
                            <td>Nome</td>
                            <td>Ação</td>
                            <td>Data do vinculo</td>
                            <td>Data da remoção do vinculo</td>
                        </tr>
                    </thead>
                    <tbody>
                        {beneficios.map((beneficio, i) => (
                            <tr>
                                <td>{beneficio.nome}</td>
                                <td>{beneficio.acao}</td>
                                <td>{beneficio.dataVinculo}</td>
                                <td>{beneficio.dataRemocaoVinculo}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
                <ModalFooter>
                    <button onClick={() => { setShow(false) }} className='btn btn-danger'>Fechar</button>
                </ModalFooter>
            </Modal>
        </form>
    );
}

export default ModalLogBeneficios;