import React, { Component } from 'react';
import { Modal, ModalBody, ModalFooter, ModalHeader, Form, FormGroup, Label, Input, Button, Row, Col } from 'reactstrap';
import { api } from '../../api/api';
import { Alert } from '../../util/alertas';


class ModalSituacaoOcorrencia extends Component {
    constructor(props) {
        super(props);
        this.state = this.initialState
        this.NovaSituacaoOcorrencia = () => {
            this.setState({ ...this.state, show: true });
        }
        this.EditaSituacaoOcorrencia = (id, nome) => {
            this.setState({ ...this.state, show: true, id, nome, isEdit: true });
        }
    }

    initialState = {
        show: false,
        nome: '',
        isEdit: false,
        id: 0
    };

    closeModal = () => {
        this.setState(this.initialState);
    }

    render() {
        const { show, isEdit, nome, id } = this.state;
        const { BuscarSituacaoOcorrencias } = this.props;
        const handdleSubmit = () => {
            const data = {
                id, 
                nome
            }
            api.post(`api/SituacaoOcorrencia${isEdit ? "/Editar/" + id : ''}`, {nome}, result => {
                Alert(`Situacao Ocorrencia ${isEdit ? 'editada' : 'adicionada'} com sucesso!`);
                BuscarSituacaoOcorrencias();
                this.closeModal();
            }, err => {
                Alert('Houve um erro na solicitação, por favor tenta novamente mais tarde.', false);
            })
        }

        return (
            <>
                <Modal isOpen={show}>
                    <form onSubmit={e => { e.preventDefault(); handdleSubmit() }}>
                        <ModalHeader>
                            {isEdit ? 'Editar' : 'Adicionar'} Situação Ocorrencia
                        </ModalHeader>
                        <ModalBody>
                            <label>Nome Da Situação Ocorrencia</label>
                            <input required type="text" className='form-control' value={nome} onChange={e => this.setState({ nome: e.target.value.toUpperCase() })} />
                        </ModalBody>
                        <ModalFooter>
                            <button type='button' onClick={this.closeModal} className='btn btn-danger'>Fechar</button>
                            <button type='submit' className='btn btn-primary'>Salvar</button>
                        </ModalFooter>
                    </form>
                </Modal>
            </>
        );
    }
}

export default ModalSituacaoOcorrencia;