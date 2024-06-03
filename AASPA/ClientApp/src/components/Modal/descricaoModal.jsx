import React from 'react';
import { Modal, ModalBody, ModalFooter, ModalHeader, Form, FormGroup, Label, Input, Button } from 'reactstrap';

class DescricaoModal extends React.Component {
    constructor(props) {
        super(props);
        this.state =
        {
            show: false,
            msg: ''
        }
        this.AbrirModal = (desc) => this.setState({ show: true, msg: desc })
    }

    render() {
        const { show } = this.state;
        const setShow = (val) => this.setState({ show: val })

        return (
            <Modal isOpen={show}>
                <ModalHeader>
                    Descrição
                </ModalHeader>
                <ModalBody>
                    {this.state.msg}
                </ModalBody>
                <ModalFooter>
                    <button onClick={() => { setShow(false) }} className='btn btn-danger'>Fechar</button>
                </ModalFooter>
            </Modal>
        );
    }
}

export default DescricaoModal;