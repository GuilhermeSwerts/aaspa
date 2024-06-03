import React from 'react';
import { Row, Col, FormGroup, Label, Input } from 'reactstrap';

function ClienteForm({
    getLogadouro,
    handleChangeCaptador,
    handleChange,
    captador,
    cliente,
    Mascara,
    onSubmit }) {
    return (
        <form onSubmit={e => { e.preventDefault(); onSubmit() }} style={{ padding: 20 }}>
            <button type='button' className='btn btn-link' onClick={() => window.location.href = '/'}>Voltar</button>
            <h4>Dados do Cliente:</h4>
            <Row>
                <Col md={4}>
                    <FormGroup>
                        <Label for="cpf">CPF</Label>
                        <Input required type="text" maxLength={14} name="cpf" id="cpf" value={Mascara.cpf(cliente.cpf)} onChange={handleChange} />
                    </FormGroup>
                </Col>
                <Col md={4}>
                    <FormGroup>
                        <Label for="nome">Nome</Label>
                        <Input required type="text" name="nome" id="nome" value={cliente.nome} onChange={handleChange} />
                    </FormGroup>
                </Col>
                <Col md={4}>
                    <FormGroup>
                        <Label for="email">E-mail</Label>
                        <Input required type="email" name="email" id="email" value={cliente.email} onChange={handleChange} />
                    </FormGroup>
                </Col>
            </Row>
            <Row>
                <Col md={2}>
                    <FormGroup>
                        <Label for="nome">CEP</Label>
                        <Input required maxLength={9} type="text" name="cep" id="cep" value={Mascara.cep(cliente.cep)} onChange={e => { handleChange(e); getLogadouro(e) }} />
                    </FormGroup>
                </Col>
                <Col md={6}>
                    <FormGroup>
                        <Label for="endereco">Logradouro</Label>
                        <Input required type="text" name="logradouro" id="logradouro" value={cliente.logradouro} onChange={handleChange} />
                    </FormGroup>
                </Col>
                <Col md={3}>
                    <FormGroup>
                        <Label for="nome">Complemento</Label>
                        <Input required type="text" name="complemento" id="complemento" value={cliente.complemento} onChange={handleChange} />
                    </FormGroup>
                </Col>
            </Row>
            <Row>
                <Col md={5}>
                    <FormGroup>
                        <Label for="nome">localidade</Label>
                        <Input required type="text" name="localidade" id="localidade" value={cliente.localidade} onChange={handleChange} />
                    </FormGroup>
                </Col>
                <Col md={2}>
                    <FormGroup>
                        <Label for="endereco">Bairro</Label>
                        <Input required type="text" name="bairro" id="bairro" value={cliente.bairro} onChange={handleChange} />
                    </FormGroup>
                </Col>
                <Col md={2}>
                    <FormGroup>
                        <Label for="endereco">Uf</Label>
                        <Input required type="text" name="uf" id="uf" value={cliente.uf} onChange={handleChange} />
                    </FormGroup>
                </Col>
                <Col md={3}>
                    <FormGroup>
                        <Label for="endereco">Numero</Label>
                        <Input required maxLength={5} type="text" name="numero" id="numero" value={cliente.numero} onChange={handleChange} />
                    </FormGroup>
                </Col>
            </Row>
            <Row>
                <Col md={3}>
                    <FormGroup>
                        <Label for="dataNascimento">Data de Nascimento</Label>
                        <Input type="date" name="dataNasc" id="dataNasc" value={cliente.dataNasc} onChange={handleChange} />
                    </FormGroup>
                </Col>
                <Col md={3}>
                    <FormGroup>
                        <Label for="nrDocumento">Nr. Documento (RG/CNH etc)</Label>
                        <Input type="text" maxLength={10} name="nrDocto" id="nrDocto" value={cliente.nrDocto} onChange={handleChange} />
                    </FormGroup>
                </Col>
                <Col md={3}>
                    <FormGroup>
                        <Label for="empregador">Empregador</Label>
                        <Input type="text" name="empregador" id="empregador" value={cliente.empregador} onChange={handleChange} />
                    </FormGroup>
                </Col>
                <Col md={3}>
                    <FormGroup>
                        <Label for="matriculaBeneficio">Matrícula/Benefício</Label>
                        <Input type="text" name="matriculaBeneficio" id="matriculaBeneficio" value={cliente.matriculaBeneficio} onChange={handleChange} />
                    </FormGroup>
                </Col>
            </Row>
            <Row>
                <Col md={4}>
                    <FormGroup>
                        <Label for="telefoneFixo">Telefone Fixo</Label>
                        <Input type="text" maxLength={14} name="telefoneFixo" id="telefoneFixo" value={Mascara.telefone(cliente.telefoneFixo)} onChange={handleChange} />
                    </FormGroup>
                </Col>
                <Col md={4}>
                    <FormGroup>
                        <Label for="telefoneCelular">Telefone Celular</Label>
                        <Input type="text" name="telefoneCelular" maxLength={15} id="telefoneCelular" value={Mascara.telefone(cliente.telefoneCelular)} onChange={handleChange} />
                    </FormGroup>
                </Col>
                <Col md={4}>
                    <FormGroup check style={{ marginTop: '2.5rem' }}>
                        <Label check>
                            <Input type="checkbox" name="possuiWhatsapp" id="possuiWhatsapp" checked={cliente.possuiWhatsapp} onChange={handleChange} />
                            Possui WhatsApp
                        </Label>
                    </FormGroup>
                </Col>
            </Row>
            <Row>
                <Col md={6}>
                    <FormGroup>
                        <Label for="nomeMae">Nome da Mãe</Label>
                        <Input type="text" name="nomeMae" id="nomeMae" value={cliente.nomeMae} onChange={handleChange} />
                    </FormGroup>
                </Col>
                <Col md={6}>
                    <FormGroup>
                        <Label for="nomePai">Nome do Pai</Label>
                        <Input type="text" name="nomePai" id="nomePai" value={cliente.nomePai} onChange={handleChange} />
                    </FormGroup>
                </Col>
            </Row>
            <Row>
                <Col md={4}>
                    <FormGroup>
                        <Label for="funcaoAASPA">Função AASPA</Label>
                        <Input type="text" name="funcaoAASPA" id="funcaoAASPA" value={cliente.funcaoAASPA} onChange={handleChange} />
                    </FormGroup>
                </Col>
            </Row>
            <hr />
            <h4>Dados do Captador</h4>
            <Row>
                <Col md={4}>
                    <FormGroup>
                        <Label for="CPF/CNPJ">CPF/CNPJ</Label>
                        <Input maxLength={18} type="text" name='cpfOuCnpj' onChange={handleChangeCaptador} value={captador.cpfOuCnpj.length > 14 ? Mascara.cnpj(captador.cpfOuCnpj) : Mascara.cpf(captador.cpfOuCnpj)} />
                    </FormGroup>
                </Col>
                <Col md={4}>
                    <FormGroup>
                        <Label for="nome">Nome</Label>
                        <Input maxLength={255} type="text" name="nome" id="nome" value={captador.nome} onChange={handleChangeCaptador} />
                    </FormGroup>
                </Col>
            </Row>
            <Row>
                <Col md={12}>
                    <FormGroup>
                        <Label for="cpf">Descrição</Label><br />
                        <textarea maxLength={1000} name="descricao" id="descricao" className='form-control' onChange={handleChangeCaptador} value={captador.descricao}></textarea>
                    </FormGroup>
                </Col>
            </Row>
            <hr />
            <Row style={{ display: 'flex', justifyContent: 'end', gap: 20, padding: 20 }}>
                <button type="button" className='btn btn-danger' onClick={() => window.history.back()}>Cancelar</button>
                <button type="submit" className='btn btn-success'>Salvar</button>
            </Row>
        </form>
    );
}

export default ClienteForm;