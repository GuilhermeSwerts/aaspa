import React, { Component } from 'react';
import { FaArrowRight, FaHistory, FaUser } from 'react-icons/fa';
import { IoIosCloseCircle } from "react-icons/io";
import { api } from '../../api/api';
import { Modal, ModalBody, ModalHeader } from 'reactstrap';
import { Mascara } from '../../util/mascara';
import { MdOutlinePendingActions } from "react-icons/md";
import { MdDateRange } from "react-icons/md";
import { Alert } from '../../util/alertas';
class ModalLogAtendimento extends Component {
    constructor(props) {
        super(props);
        this.state = {
            logs: [],
            selecionado: {},
            show: false,
            Id: 0
        }
        this.open = (hstId) => {
            api.get(`Log/Alteracao?tabelaFk=${hstId}&ETipoLog=Atendimento`, res => {
                this.setState({ logs: res.data, selecionado: res.data[0], show: true, Id: hstId })
            }, err => {
                Alert("Houve um erro ao buscar log do atendimento " + hstId)
            })
        }
    }

    render() {

        const { logs, selecionado, show, Id } = this.state;

        const selecionarLog = (log) => {
            this.setState({ selecionado: log })
        }

        return (
            <Modal isOpen={show} modalClassName='custom-modal'>
                <ModalBody>
                    <div style={{ width: '95%', display: 'flex', justifyContent: 'space-between', alignItems: 'center', margin: '2rem' }}>
                        <strong>ID: {Id}</strong>
                        <button className='btn btn-primary' onClick={e => this.setState({
                            logs: [],
                            selecionado: {},
                            show: false,
                            Id: 0
                        })}><IoIosCloseCircle /></button>
                    </div>
                    <div className="container">
                        <div className="row timeline">
                            <div className="col-md-3">
                                <div style={{ width: 40, color: '#000', fontWeight: 'bold' }}>Logs:</div>
                                {logs.map(log => (
                                    <div className="timeline-item">
                                        <div className="avatar" style={{ width: 60 }}><FaHistory /></div>
                                        <div className="content" style={{ cursor: 'pointer', width: 250 }} onClick={e => selecionarLog(log)}>
                                            <small>
                                                <strong>{log.usuario} - {log.titulo}</strong>
                                            </small>

                                            <br />
                                            <div className="detalhes-timeline">
                                                <span className="time">{Mascara.data(log.dtCadastro)}</span>
                                                <FaArrowRight color='#fff' />
                                            </div>
                                        </div>
                                    </div>
                                ))}
                            </div>
                            <div className="col-md-9">
                                {selecionado && <div className="card-details">
                                    <div style={{ display: 'flex', justifyContent: 'space-between', flexDirection: 'column', alignItems: 'start' }}>
                                        <strong><FaUser />: {selecionado.usuario}</strong>
                                        <strong><MdOutlinePendingActions />: {selecionado.titulo}</strong>
                                    </div>
                                    <br />
                                    <p style={{ marginTop: 10, whiteSpace: 'pre-line' }}>{selecionado.log}</p>
                                    <span className="time" style={{ display: 'flex', alignItems: 'center' }}><MdDateRange />{Mascara.data(selecionado.dtCadastro)}</span>
                                </div>}
                            </div>
                        </div>
                    </div>
                </ModalBody>
            </Modal>
        );
    }
}

export default ModalLogAtendimento;

// <div className="timeline-item">
//                             <div className="avatar" style={{width:40}}><FaHistory /></div>
//                             <div className="content">
//                                 <small>
//                                     <strong>Guilherme - Adicionado(s) Novo(s) Anexo(s)</strong>
//                                 </small>
//                                 <br />
//                                 {/* <p style={{ marginTop: 10 }}>Arquivos Adicionados: Captura de tela 2024-09-25 100359.png,Captura de tela 2024-09-26 014827.png,Captura de tela 2024-09-26 014521.png</p> */}
//                                 <span className="time">22/10/2200 22:33:22</span>
//                             </div>
//                         </div>