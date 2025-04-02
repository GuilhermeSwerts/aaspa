import { Modal, ModalBody, ModalFooter, ModalHeader } from 'reactstrap';
import React, { Component } from 'react';
import { FaDownload } from 'react-icons/fa6';


class ModalVisualizarAnexo extends Component {
    constructor(props) {
        super(props);
        this.state = {
            anexo: '',
            nome: '',
            tipo: true, // pdf = true,imagem = false,
            show: false
        }
        this.VisualizarAnexo = ({ anexo_nome, anexo_anexo, anexo_tipo }) => {
            this.setState({
                anexo: anexo_anexo,
                nome: anexo_nome,
                tipo: anexo_tipo,
                show: true
            });
        }
    }


    downloadAnexo = () => {
        const { anexo, nome, tipo } = this.state;
        const mimeType = tipo ? 'application/pdf' : 'image/png';
        const byteCharacters = atob(anexo);
        const byteNumbers = new Array(byteCharacters.length);
        for (let i = 0; i < byteCharacters.length; i++) {
            byteNumbers[i] = byteCharacters.charCodeAt(i);
        }
        const byteArray = new Uint8Array(byteNumbers);
        const blob = new Blob([byteArray], { type: mimeType });
        const link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = nome;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }


    render() {
        const { anexo, nome, tipo, show } = this.state;

        return (
            <Modal isOpen={show}>
                <ModalHeader>
                    {nome}
                </ModalHeader>
                <ModalBody>
                    {!tipo && (
                        <img src={`data:application/png;base64,${anexo}`} alt="Imagem" style={{ maxWidth: '100%', height: 'auto' }} />
                    )}
                    {tipo && (
                        <iframe
                            src={`data:application/pdf;base64,${anexo}`}
                            title="PDF"
                            style={{ width: '100%', height: '500px' }}
                            frameBorder="0"
                        />
                    )}
                </ModalBody>
                <ModalFooter>
                    <button onClick={e => this.setState({
                        anexo: '',
                        nome: '',
                        tipo: true,
                        show: false
                    })} className='btn btn-primary'>Fechar</button>
                    <button onClick={this.downloadAnexo} className='btn btn-primary'><FaDownload /></button>
                </ModalFooter>
            </Modal>
        );
    }
}

export default ModalVisualizarAnexo;