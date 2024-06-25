import React, { useEffect, useState } from 'react';
import { api } from '../../api/api';
import { Row, Col, Modal, ModalBody, ModalFooter, ModalHeader, Form, FormGroup, Label, Input, Button, Container } from 'reactstrap';
import { ButtonTooltip } from '../Inputs/ButtonTooltip';
import { FaGear } from 'react-icons/fa6';
import { Alert } from '../../util/alertas';

function ModalVincularBeneficios({ BuscarTodosClientes, ClienteId }) {
    const [selectedBeneficio, setSelectedBeneficio] = useState([]);

    const [show, setShow] = useState(false);
    const [beneficios, setBeneficios] = useState([]);
    const [clienteData, setClienteData] = useState({
        cliente: { cliente_nome: '' },
    });


    const toggleId = (id) => {
        if (selectedBeneficio.includes(id)) {
            setSelectedBeneficio(selectedBeneficio.filter(beneficioId => beneficioId !== id));
        } else {
            setSelectedBeneficio([...selectedBeneficio, id]);
        }
    };

    const BuscarTodosBeneficios = () => {
        api.get('BuscarTodosBeneficios', res => {
            setBeneficios(res.data);
            BuscarDadosCliente();
        }, err => {
            Alert('Houve um erro ao buscar os beneficios', false)
        })
    }

    const BuscarDadosCliente = () => {
        api.get(`BuscarClienteID/${ClienteId}`, res => {
            setClienteData(res.data);
            const beneficiosAtivos = res.data.beneficios.map(x => x.beneficio_id);
            setSelectedBeneficio(beneficiosAtivos);
        }, err => {
            Alert('Houve um erro ao buscar os dados do cliente', false)
        })
    }

    useEffect(() => {
        BuscarTodosBeneficios();
    }, [])

    const handdleSubmit = () => {
        const formData = new FormData();
        selectedBeneficio.forEach((id, i) => {
            formData.append(`beneficiosIds[${i}]`, id);
        });
        api.post(`VincularBeneficios/${ClienteId}`, formData, res => {
            BuscarTodosBeneficios();
            BuscarTodosClientes();
            Alert('Beneficios Atualizados com sucesso!', true);
            setShow(false);
        }, err => {
            Alert(err.response.data, false)
        })
    }

    return (
        <>
            <ButtonTooltip
                onClick={() => setShow(true)}
                className='btn btn-success'
                text={'Gerenciar Beneficios'}
                top={true}
                textButton={<FaGear color='#fff' size={25} />}
            />
            <Modal isOpen={show}>
                <ModalHeader>
                    Beneficios de: {clienteData.cliente.cliente_nome}
                </ModalHeader>
                <br />
                <Container>
                    <Row>
                        {beneficios.map(beneficio => (
                            <Col sm={4} style={{ display: "flex", justifyContent: 'start', alignItems: 'center', border: '1px solid' }}>
                                <FormGroup check>
                                    <Label check>
                                        <Input
                                            type="checkbox"
                                            checked={selectedBeneficio.includes(beneficio.beneficio_id)}
                                            onChange={() => toggleId(beneficio.beneficio_id)}
                                        />
                                        {beneficio.beneficio_nome_beneficio}
                                    </Label>
                                </FormGroup>
                            </Col>
                        ))}
                        {beneficios.length === 0 && "Nenhum beneficio cadastrado..."}
                    </Row>
                </Container>
                <br />
                <ModalFooter>
                    <button type="button" onClick={e => { BuscarTodosBeneficios(); setShow(false) }} className='btn btn-danger'>Cancelar</button>
                    <button type="button" onClick={handdleSubmit} className='btn btn-success'>Salvar</button>
                </ModalFooter>
            </Modal>
        </>
    );
}

export default ModalVincularBeneficios;