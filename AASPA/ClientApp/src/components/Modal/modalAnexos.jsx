import { Modal, ModalBody, ModalFooter, ModalHeader } from 'reactstrap';
import React, { Component, createRef } from 'react';
import { FaDownload } from 'react-icons/fa6';
import { api } from '../../api/api';
import ModalVisualizarAnexo from './modalVisualizarAnexo';

class ModalAnexos extends Component {
    constructor(props) {
        super(props);
        this.state = {
            anexos: [],
            show: false
        }
        this.VisualizarAnexos = (id) => {
            api.get(`BuscarContatoOcorrenciaById/${id}`, res => {
                let arquivos = [];

                res.data.anexos.forEach(anexo => {
                    arquivos.push({ file: anexo.anexo_anexo, fileName: anexo.anexo_nome });
                });

                this.setState({ anexos: arquivos, show: true })
            }, err => {
                Alert('houve um erro ao buscar o Contato/Ocorrencia do id:' + HistoricoId, false)
            })
        }
        this.modal = createRef();
    }

    render() {
        const { anexos, show } = this.state;

        const VisualizarAnexo = (index) => {
            var anexo = anexos[index];
            let obj = {
                anexo_nome: anexo.fileName,
                anexo_anexo: anexo.file,
                anexo_tipo: anexo.fileName.includes('.pdf')
            }
            this.modal.current.VisualizarAnexo(obj);
        }

        return (
            <Modal isOpen={show}>
                <ModalVisualizarAnexo ref={this.modal} />
                <ModalHeader>
                    Anexos
                </ModalHeader>
                <ModalBody>
                    <div className="container">
                        <ul>
                            {anexos.map((anexo, i) => (
                                <li onClick={e => VisualizarAnexo(i)}><button className='btn btn-link'>{anexo.fileName}</button></li>
                            ))}
                        </ul>
                    </div>
                </ModalBody>
                <ModalFooter>
                    <button onClick={e => this.setState({ show: false, anenxos: [] })} className='btn btn-primary'>Fechar</button>
                </ModalFooter>
            </Modal>
        );
    }
}

export default ModalAnexos;