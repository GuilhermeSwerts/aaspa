import React, { useEffect, useState } from 'react';
import { Modal, ModalBody, ModalFooter, ModalHeader, Form, FormGroup, Label, Input, Button, Row, Col } from 'reactstrap';
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
        cliente_remessa_id
    } = Cliente;

    return (
        <>
            <ButtonTooltip
                onClick={() => setShow(true)}
                className='btn btn-success'
                text={'Visualizar Dados Cliente'}
                top={true}
                textButton={<FaEye size={25} />}
            />
            <Modal isOpen={show}>
                <ModalHeader>
                    Dados Do Cliente
                </ModalHeader>
                <ModalBody>
                    <p><strong>ID:</strong> {cliente_id}</p>
                    <p><strong>CPF:</strong> {cliente_cpf}</p>
                    <p><strong>Nome:</strong> {cliente_nome}</p>
                    <p><strong>CEP:</strong> {cliente_cep}</p>
                    <p><strong>Logradouro:</strong> {cliente_logradouro}</p>
                    <p><strong>Bairro:</strong> {cliente_bairro}</p>
                    <p><strong>Localidade:</strong> {cliente_localidade}</p>
                    <p><strong>UF:</strong> {cliente_uf}</p>
                    <p><strong>Número:</strong> {cliente_numero}</p>
                    <p><strong>Complemento:</strong> {cliente_complemento}</p>
                    <p><strong>Data de Nascimento:</strong> {new Date(cliente_dataNasc).toLocaleDateString()}</p>
                    <p><strong>Data de Cadastro:</strong> {new Date(cliente_dataCadastro).toLocaleDateString()}</p>
                    <p><strong>Número do Documento:</strong> {cliente_nrDocto}</p>
                    <p><strong>Empregador:</strong> {cliente_empregador}</p>
                    <p><strong>Matrícula Benefício:</strong> {cliente_matriculaBeneficio}</p>
                    <p><strong>Nome da Mãe:</strong> {cliente_nomeMae}</p>
                    <p><strong>Nome do Pai:</strong> {cliente_nomePai}</p>
                    <p><strong>Telefone Fixo:</strong> {cliente_telefoneFixo}</p>
                    <p><strong>Telefone Celular:</strong> {cliente_telefoneCelular}</p>
                    <p><strong>Possui WhatsApp:</strong> {cliente_possuiWhatsapp ? 'Sim' : 'Não'}</p>
                    <p><strong>Função AASPA:</strong> {cliente_funcaoAASPA}</p>
                    <p><strong>Email:</strong> {cliente_email}</p>
                    <p><strong>Situação:</strong> {cliente_situacao ? 'Ativo' : 'Inativo'}</p>
                    <p><strong>Sexo:</strong> {cliente_sexo === 0 ? 'Masculino' : 'Feminino'}</p>
                    <p><strong>Estado Civil:</strong> {cliente_estado_civil}</p>
                    <p><strong>Remessa ID:</strong> {cliente_remessa_id}</p>
                </ModalBody>
                <ModalFooter>
                    <button type='button' onClick={() => { setShow(false) }} className='btn btn-danger'>Voltar</button>
                </ModalFooter>
            </Modal>
        </>
    );
}

export default ModalVisualizarCliente;