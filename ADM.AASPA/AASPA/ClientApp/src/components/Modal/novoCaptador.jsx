import React from 'react';
import { Modal, ModalBody, ModalFooter, ModalHeader, Form, FormGroup, Label, Input, Button, Row, Col } from 'reactstrap';
import { Mascara } from '../../util/mascara';
import { api } from '../../api/api';
import { Alert } from '../../util/alertas';

class novoCaptador extends React.Component {
    constructor(props) {
        super(props);
        this.state =
        {
            show: false,
            captadorId: 0,
            edit: false,
            captadorNome: '',
            captadorCpfOrCnpj: '',
            captadorDescricao: ''
        }
        this.AbrirModal = (CaptadorId, CaptadorNome, CaptadorDescricao, CaptadorCpfOrCnpj, Edit) => this.setState({
            show: true,
            captadorId: CaptadorId,
            edit: Edit,
            captadorNome: CaptadorNome,
            captadorCpfOrCnpj: CaptadorCpfOrCnpj,
            captadorDescricao: CaptadorDescricao
        })
    }
    render() {
        const { show, edit } = this.state;
        const { BuscarTodosCaptadores } = this.props;
        const setShow = (val) => this.setState({ show: val })
        const InitState = () => {
            this.setState({
                show: false,
                captadorId: 0,
                edit: false,
                captadorNome: '',
                captadorCpfOrCnpj: '',
                captadorDescricao: ''
            });
        }

        const NovoCaptador = () => {
            const data = new FormData();
            data.append('cpfOuCnpj', this.state.captadorCpfOrCnpj);
            data.append('nome', this.state.captadorNome);
            data.append('descricao', this.state.captadorDescricao);

            api.post('NovoCaptador', data, res => {
                Alert(`"Captador cadastrado com sucesso!`);
                InitState();
                BuscarTodosCaptadores();
            }, err => {
                Alert("Houve um erro ao cadastrar o captador", false);
            })
        }

        const EditarCaptador = () => {
            const data = new FormData();

            data.append('CaptadorId', this.state.captadorId);
            data.append('cpfOuCnpj', this.state.captadorCpfOrCnpj);
            data.append('nome', this.state.captadorNome);
            data.append('descricao', this.state.captadorDescricao);

            api.post('EditarCaptador', data, res => {
                Alert(`Captador editado com sucesso!`);
                InitState();
                BuscarTodosCaptadores();
            }, err => {
                Alert("Houve um erro ao editar o captador", false);
            })
        }


        return (
            <Modal isOpen={show}>
                <form onSubmit={e => { e.preventDefault(); edit ? EditarCaptador() : NovoCaptador() }}>
                    <ModalHeader>
                        Descrição
                    </ModalHeader>
                    <ModalBody>
                        <Row>
                            <Col md={12}>
                                <FormGroup>
                                    <Label for="CPF/CNPJ">CPF/CNPJ*</Label>
                                    <Input required maxLength={18} type="text" name='cpfOuCnpj' onChange={e => this.setState({ captadorCpfOrCnpj: e.target.value })} value={Mascara.cpfOrCnpj(this.state.captadorCpfOrCnpj)} />
                                </FormGroup>
                            </Col>
                            <Col md={12}>
                                <FormGroup>
                                    <Label for="nome">Nome*</Label>
                                    <Input required maxLength={255} type="text" name="nome" id="nome" value={this.state.captadorNome} onChange={e => this.setState({ captadorNome: e.target.value.toUpperCase() })} />
                                </FormGroup>
                            </Col>
                        </Row>
                        <Row>
                            <Col md={12}>
                                <FormGroup>
                                    <Label for="cpf">Descrição</Label><br />
                                    <textarea maxLength={1000} name="descricao" id="descricao" className='form-control' onChange={e => this.setState({ captadorDescricao: e.target.value })} value={this.state.captadorDescricao}></textarea>
                                </FormGroup>
                            </Col>
                        </Row>
                    </ModalBody>
                    <ModalFooter>
                        <button onClick={() => { { InitState(); setShow(false) } }} className='btn btn-danger'>Fechar</button>
                        <button type='submit' className='btn btn-success'>Salvar</button>
                    </ModalFooter>
                </form>
            </Modal>
        );
    }
}

export default novoCaptador;