import React, { useEffect, useRef, useState } from 'react';
import { Modal, ModalBody, ModalFooter, ModalHeader, Form, FormGroup, Label, Input, Button, Row, Col } from 'reactstrap';
import { api } from '../../api/api';
import { ButtonTooltip } from '../Inputs/ButtonTooltip';
import { FaPlus } from 'react-icons/fa6';
import { Alert, Pergunta } from '../../util/alertas';
import PreVisualizarRemessa from './preVisualizarRemessa';
import { Mascara } from '../../util/mascara';

function ModalNovaRemessa({ BuscarRemessas, DownloadRemessa }) {
    const [show, setShow] = useState(false);

    const meses = [
        { valor: 1, nome: 'Janeiro' },
        { valor: 2, nome: 'Fevereiro' },
        { valor: 3, nome: 'Março' },
        { valor: 4, nome: 'Abril' },
        { valor: 5, nome: 'Maio' },
        { valor: 6, nome: 'Junho' },
        { valor: 7, nome: 'Julho' },
        { valor: 8, nome: 'Agosto' },
        { valor: 9, nome: 'Setembro' },
        { valor: 10, nome: 'Outubro' },
        { valor: 11, nome: 'Novembro' },
        { valor: 12, nome: 'Dezembro' },
    ];

    const getData = (isFim) => {
        const today = new Date();
        const year = today.getFullYear();
        let month = String(today.getDate() > 15 ? (new Date().getMonth() + 1) : new Date().getMonth()).padStart(2, '0');
        const day = isFim ? new Date(year, today.getMonth() + (today.getDate() > 15 ? 1 : 0), 0).getDate() : 1;

        return `${year}-${month}-${String(day).padStart(2, '0')}`;
    };


    const anoAtual = new Date().getFullYear();
    const mesAtual = new Date().getDate() > 15 ? (new Date().getMonth() + 1) : new Date().getMonth();

    const [editarCompetencia, setEditarCompetencia] = useState(false);

    const [anoSelecionado, setAnoSelecionado] = useState(anoAtual);
    const [mesSelecionado, setMesSelecionado] = useState(mesAtual);

    const [dateInit, setdateInit] = useState(getData(false));
    const [dateFim, setDateFim] = useState(getData(true));

    const anos = Array.from({ length: 25 }, (_, i) => anoAtual - i);

    const visualizarRef = useRef()

    const initState = () => {
        setAnoSelecionado(anoAtual);
        setMesSelecionado(mesAtual);
    }

    const handdleSubmit = async () => {
        const nomeMes = meses.filter(x => x.valor === parseFloat(mesSelecionado))[0];
        const diaInicio = dateInit.split('-')[2];
        const diaFim = dateFim.split('-')[2];
        const mesInicio = meses.filter(x => x.valor === parseFloat(dateInit.split('-')[1]))[0].nome;
        const mesFim = meses.filter(x => x.valor === parseFloat(dateFim.split('-')[1]))[0].nome;

        if (await Pergunta(`Deseja realmente gerar a remessa do mês competente ${nomeMes.nome} do período do dia ${diaInicio}/${mesInicio} há ${diaFim}/${mesFim}`))
            api.get(`GerarRemessa?mes=${mesSelecionado}&ano=${anoSelecionado}&dateInit=${dateInit}&dateEnd=${dateFim}`, async res => {
                initState();
                setShow(false);
                const id = res.data;
                if (await Pergunta('Remessa Gerada Com sucesso!\nDeseja fazer o download do arquivo?')) {
                    DownloadRemessa(id)
                }
                BuscarRemessas();
            }, err => {
                Alert(err.response.data, false)
            })
    }

    const handdlePreVisualizar = () => {
        visualizarRef.current.Show(dateInit, dateFim, anoSelecionado, mesSelecionado);
    }

    return (
        <>
            <button className='btn btn-success' onClick={e => setShow(true)}>Gerar Remessa <FaPlus size={20} color='#fff' /></button>
            <PreVisualizarRemessa ref={visualizarRef} />
            <Modal isOpen={show}>
                <form onSubmit={e => { e.preventDefault(); handdleSubmit() }}>
                    <ModalHeader>
                        Gerar Remessa
                    </ModalHeader>
                    <ModalBody>
                        <span>Mês/Ano competente: {meses.filter(x => x.valor === parseFloat(mesSelecionado))[0].nome}/{anoSelecionado}</span>
                        <br /><br />
                        <div>
                            <input onChange={_ => setEditarCompetencia(!editarCompetencia)} value={editarCompetencia} type="checkbox" name="" id="" />
                            <label>Editar competência</label>
                        </div>

                        {editarCompetencia && <>
                            <br />
                            <h5>Período de busca os clientes:</h5>
                            <Row>
                                <Col md={6}>
                                    <label htmlFor="">De:</label>
                                    <input onChange={e => setdateInit(e.target.value)} className='form-control' type="date" value={dateInit} name="dateInit" id="dateInit" />
                                </Col>
                                <Col md={6}>
                                    <label>Até</label>
                                    <input onChange={e => setDateFim(e.target.value)} value={dateFim} className='form-control' type="date" name="dateFim" id="dateFim" />
                                </Col>
                            </Row>

                            <hr />

                            <h5>Mês/Ano competente do arquivo:</h5>
                            <Row>
                                <Col md={6}>
                                    <label htmlFor="">Selecione o mês:</label>
                                    <select required className='form-control' value={mesSelecionado} onChange={(e) => setMesSelecionado(e.target.value)}>
                                        {meses.map((mes) => (
                                            <option key={mes.valor} value={mes.valor}>
                                                {mes.nome}
                                            </option>
                                        ))}
                                    </select>
                                </Col>
                                <Col md={6}>
                                    <label>Selecione o ano</label>
                                    <select required className='form-control' value={anoSelecionado} onChange={(e) => setAnoSelecionado(e.target.value)}>
                                        {anos.map((ano) => (
                                            <option key={ano} value={ano}>
                                                {ano}
                                            </option>
                                        ))}
                                    </select>
                                </Col>
                            </Row>
                        </>}
                    </ModalBody>
                    <ModalFooter>
                        <button type='button' onClick={() => { initState(); setShow(false) }} className='btn btn-danger'>Fechar</button>
                        <button type='button' onClick={handdlePreVisualizar} className='btn btn-info'>Pré-Visualizar</button>
                        <button type='submit' className='btn btn-success'>Gerar</button>
                    </ModalFooter>
                </form>
            </Modal>
        </>
    );
}

export default ModalNovaRemessa;