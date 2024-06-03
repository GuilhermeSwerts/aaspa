import React, { useState } from 'react';
import { Row, Col, Modal, ModalBody, ModalFooter, ModalHeader, Form, FormGroup, Label, Input, Button } from 'reactstrap';
import { api } from '../../api/api';
import { Mascara } from '../../util/mascara';


function ModalNovoBeneficio() {
    const [show, setShow] = useState(false);

    const [cod_beneficio, setCodBeneficio] = useState('');
    const [nome_beneficio, setNome_beneficio] = useState('');
    const [fornecedor_beneficio, setFornecedor_beneficio] = useState('');
    const [valor_a_pagar_ao_fornecedor, setValor_a_pagar_ao_fornecedor] = useState('');
    const [descricao_beneficios, setDescricao_beneficios] = useState('');

    const setInit = () => {
        setCodBeneficio(''); setNome_beneficio(''); setFornecedor_beneficio(''); setValor_a_pagar_ao_fornecedor(''); setDescricao_beneficios('');
    }

    const handleSubmit = () => {
        var formData = new FormData();
        var cod = parseInt(cod_beneficio);
        formData.append("CodBeneficio", cod);
        formData.append("NomeBeneficio", nome_beneficio);
        formData.append("FornecedorBeneficio", fornecedor_beneficio);
        formData.append("ValorAPagarAoFornecedor", valor_a_pagar_ao_fornecedor.replace('R$ ', '').replace(',', '.').replace(' ', ''));
        formData.append("DescricaoBeneficios", descricao_beneficios);

        api.post("NovoBeneficio", formData, res => {
            setInit();
            alert("Beneficio adicionado com sucesso!")
            setShow(false);
        }, err => {
            alert(err.response.data)
        })
    }

    return (
        <>
            <button type='button' onClick={() => { setShow(true) }} className='btn btn-primary'>Novo Beneficio</button>
            <Modal isOpen={show}>
                <form onSubmit={e => { e.preventDefault(); handleSubmit(); }}>
                    <ModalHeader>
                        Novo Status
                    </ModalHeader>
                    <ModalBody>
                        <Row>
                            <Col md={6}>
                                <FormGroup>
                                    <Label for="cod_beneficio">Cod.Beneficio</Label>
                                    <Input maxLength={10} required placeholder='Cod.Beneficio' type="text" name="cod_beneficio" id="cod_beneficio" value={cod_beneficio} onChange={e => setCodBeneficio(e.target.value)} />
                                </FormGroup>
                            </Col>
                            <Col md={6}>
                                <FormGroup>
                                    <Label for="nome_beneficio">Nome Beneficio</Label>
                                    <Input maxLength={255} required placeholder='Nome Beneficio' type="text" name="nome_beneficio" id="nome_beneficio" value={nome_beneficio} onChange={e => setNome_beneficio(e.target.value.toUpperCase())} />
                                </FormGroup>
                            </Col>
                        </Row>
                        <Row>
                            <Col md={6}>
                                <FormGroup>
                                    <Label for="fornecedor_beneficio">Fornecedor do Beneficio</Label>
                                    <Input maxLength={255} required placeholder='Fornecedor do Beneficio' type="text" name="fornecedor_beneficio" id="fornecedor_beneficio" value={fornecedor_beneficio} onChange={e => setFornecedor_beneficio(e.target.value)} />
                                </FormGroup>
                            </Col>
                            <Col md={6}>
                                <FormGroup>
                                    <Label for="valor_a_pagar_ao_fornecedor">Valor a pagar ao Fornecedor</Label>
                                    <Input required placeholder='Valor a pagar ao Fornecedor' type="text" name="valor_a_pagar_ao_fornecedor" id="valor_a_pagar_ao_fornecedor" value={valor_a_pagar_ao_fornecedor} onChange={e => setValor_a_pagar_ao_fornecedor(Mascara.moeda(e.target.value))} />
                                </FormGroup>
                            </Col>
                        </Row>
                        <Row>
                            <Col md={12}>
                                <FormGroup>
                                    <Label for="descricao_beneficios">Descrição dos beneficios</Label><br />
                                    <textarea className='form-control' maxLength={1000} required placeholder='Descrição dos beneficios' type="text" name="descricao_beneficios" id="descricao_beneficios" value={descricao_beneficios} onChange={e => setDescricao_beneficios(e.target.value)} />
                                </FormGroup>
                            </Col>
                        </Row>
                    </ModalBody>
                    <ModalFooter>
                        <button onClick={() => { setInit(); setShow(false) }} className='btn btn-danger'>Cancelar</button>
                        <button type='submit' className='btn btn-success'>Salvar</button>
                    </ModalFooter>
                </form>
            </Modal >
        </>
    );
}

export default ModalNovoBeneficio;