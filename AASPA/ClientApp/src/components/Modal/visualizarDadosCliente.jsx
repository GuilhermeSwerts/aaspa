import React, { useEffect, useState } from 'react';
import { Modal, ModalBody, ModalFooter, ModalHeader, Row, Col } from 'reactstrap';
import { ButtonTooltip } from '../Inputs/ButtonTooltip';
import { FaEye } from 'react-icons/fa';

function ModalVisualizarCliente({ Cliente }) {
    const [show, setShow] = useState(false);

    const {
        cliente_id,
        cliente_cpf,
        cliente_nome,
        cliente_cep,
        cliente_logradouro,
        cliente_bairro,
        cliente_localidade,
        cliente_uf,
        cliente_numero,
        cliente_complemento,
        cliente_dataNasc,
        cliente_dataCadastro,
        cliente_nrDocto,
        cliente_empregador,
        cliente_matriculaBeneficio,
        cliente_nomeMae,
        cliente_nomePai,
        cliente_telefoneFixo,
        cliente_telefoneCelular,
        cliente_possuiWhatsapp,
        cliente_funcaoAASPA,
        cliente_email,
        cliente_situacao,
        cliente_sexo,
        cliente_estado_civil,
        cliente_remessa_id,
        cliente_StatusIntegral
    } = Cliente;

    return (
        <>
            <ButtonTooltip
                onClick={() => setShow(true)}
                className='btn btn-success button-container-item'
                text={'Visualizar Dados Cliente'}
                top={true}
                textButton={<FaEye size={10} />}
            />
            <Modal isOpen={show} modalClassName='custom-modal'>
                <ModalHeader>
                    Dados Do Cliente
                </ModalHeader>
                <ModalBody>
                    <h3>DADOS INFORMATIVOS:</h3>
                    <Row>
                        <Col md={2}>
                            <label>ID:</label>
                            <input className='form-control' type="text" disabled value={cliente_id} />
                        </Col>
                        <Col md={4}>
                            <label>Data de Cadastro:</label>
                            <input className='form-control' type="text" disabled value={new Date(cliente_dataCadastro).toLocaleDateString()} />
                        </Col>
                        <Col md={2}>
                            <label>Remessa ID:</label>
                            <input className='form-control' type="text" disabled value={cliente_remessa_id} />
                        </Col>
                    </Row>
                    <hr />
                    <h3>DADOS PESSOAIS:</h3>
                    <Row>
                        <Col md={2}>
                            <label>CPF:</label>
                            <input className='form-control' type="text" disabled value={cliente_cpf} />
                        </Col>
                        <Col md={4}>
                            <label>Nome:</label>
                            <input className='form-control' type="text" disabled value={cliente_nome} />
                        </Col>
                        <Col md={2}>
                            <label>Data de Nascimento:</label>
                            <input className='form-control' type="text" disabled value={new Date(cliente_dataNasc).toLocaleDateString()} />
                        </Col>
                        <Col md={2}>
                            <label>Sexo:</label>
                            <input className='form-control' type="text" disabled value={cliente_sexo === 1 ? 'Masculino' : cliente_sexo === 2 ? 'Feminino' : 'Outros'} />
                        </Col>
                        <Col md={2}>
                            <label>Estado Civil:</label>
                            <select disabled name="estadoCivil" value={cliente_estado_civil} className='form-control'>
                                <option value="1">Solteiro</option>
                                <option value="2">Casado</option>
                                <option value="3">Viúvo</option>
                                <option value="4">Separado judiscialmente</option>
                                <option value="5">União estável</option>
                                <option value="6">Outros</option>
                            </select>
                        </Col>
                    </Row>
                    <hr />
                    <h3>DADOS ENDEREÇO:</h3>
                    <Row>
                        <Col md={2}>
                            <label>CEP:</label>
                            <input className='form-control' type="text" disabled value={cliente_cep} />
                        </Col>
                        <Col md={6}>
                            <label>Logradouro:</label>
                            <input className='form-control' type="text" disabled value={cliente_logradouro} />
                        </Col>
                        <Col md={4}>
                            <label>Bairro:</label>
                            <input className='form-control' type="text" disabled value={cliente_bairro} />
                        </Col>
                    </Row>
                    <Row>
                        <Col md={2}>
                            <label>Número:</label>
                            <input className='form-control' type="text" disabled value={cliente_numero} />
                        </Col>
                        <Col md={4}>
                            <label>Localidade:</label>
                            <input className='form-control' type="text" disabled value={cliente_localidade} />
                        </Col>
                        <Col md={2}>
                            <label>UF:</label>
                            <input className='form-control' type="text" disabled value={cliente_uf} />
                        </Col>
                        <Col md={4}>
                            <label>Complemento:</label>
                            <input className='form-control' type="text" disabled value={cliente_complemento} />
                        </Col>
                    </Row>
                    <hr />
                    <h3>DADOS GERAIS:</h3>
                    <Row>
                        <Col md={2}>
                            <label>Número do Documento:</label>
                            <input className='form-control' type="text" disabled value={cliente_nrDocto} />
                        </Col>
                        <Col md={2}>
                            <label>Empregador:</label>
                            <input className='form-control' type="text" disabled value={cliente_empregador} />
                        </Col>
                        <Col md={4}>
                            <label>Matrícula Benefício:</label>
                            <input className='form-control' type="text" disabled value={cliente_matriculaBeneficio} />
                        </Col>
                    </Row>
                    <Row>
                        <Col md={6}>
                            <label>Nome da Mãe:</label>
                            <input className='form-control' type="text" disabled value={cliente_nomeMae} />
                        </Col>
                        <Col md={6}>
                            <label>Nome da Pai:</label>
                            <input className='form-control' type="text" disabled value={cliente_nomePai} />
                        </Col>
                    </Row>
                    <Row>
                        <Col md={3}>
                            <label>Telefone Fixo:</label>
                            <input className='form-control' type="text" disabled value={cliente_telefoneFixo} />
                        </Col>
                        <Col md={3}>
                            <label>Telefone Celular:</label>
                            <input className='form-control' type="text" disabled value={cliente_telefoneCelular} />
                        </Col>
                        <Col md={2}>
                            <label>Possui WhatsApp:</label>
                            <input className='form-control' type="text" disabled value={cliente_possuiWhatsapp ? 'Sim' : 'Não'} />
                        </Col>
                        <Col md={4}>
                            <label>Função AASPA:</label>
                            <input className='form-control' type="text" disabled value={cliente_funcaoAASPA} />
                        </Col>
                    </Row>
                    <Row>
                        <Col md={6}>
                            <label>Email:</label>
                            <input className='form-control' type="text" disabled value={cliente_email} />
                        </Col>
                        <Col md={2}>
                            <label>Status Integraall</label>
                            <input className='form-control' type="text" disabled value={cliente_StatusIntegral} />
                        </Col>
                    </Row>
                </ModalBody>
                <ModalFooter>
                    <button type='button' onClick={() => { setShow(false) }} className='btn btn-danger'>Voltar</button>
                </ModalFooter>
            </Modal>
        </>
    );
}

export default ModalVisualizarCliente;