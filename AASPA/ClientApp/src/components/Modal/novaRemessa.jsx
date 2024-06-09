import React, { useEffect, useState } from 'react';
import { Modal, ModalBody, ModalFooter, ModalHeader, Form, FormGroup, Label, Input, Button, Row, Col } from 'reactstrap';
import { api } from '../../api/api';
import { ButtonTooltip } from '../Inputs/ButtonTooltip';
import { FaPlus } from 'react-icons/fa6';

function ModalNovaRemessa({ BuscarRemessas,DownloadRemessa }) {
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

    const anoAtual = new Date().getFullYear();
    const mesAtual = new Date().getMonth() + 1;

    const [anoSelecionado, setAnoSelecionado] = useState(anoAtual);
    const [mesSelecionado, setMesSelecionado] = useState(mesAtual);

    const anos = Array.from({ length: 25 }, (_, i) => anoAtual - i);

    const initState = () => {
        setAnoSelecionado(anoAtual);
        setMesSelecionado(mesAtual);
    }

    const handdleSubmit = () => {
        api.get(`GerarRemessa?mes=${mesSelecionado}&ano=${anoSelecionado}`, async res => {
            initState();
            setShow(false);
            const id = res.data;
            if (await window.confirm('Remessa Gerada Com sucesso!\nDeseja fazer o download do arquivo?')) {
                DownloadRemessa(id)
            }
            BuscarRemessas();
        }, err => {
            alert(err.response.data)
        })
    }

    return (
        <>
            <button className='btn btn-success' onClick={e => setShow(true)}>Gerar Remessa <FaPlus size={20} color='#fff' /></button>
            <Modal isOpen={show}>
                <form onSubmit={e => { e.preventDefault(); handdleSubmit() }}>
                    <ModalHeader>
                        Adicionar Origem
                    </ModalHeader>
                    <ModalBody>
                        <label htmlFor="">Selecione o mês:</label>
                        <select required className='form-control' value={mesSelecionado} onChange={(e) => setMesSelecionado(e.target.value)}>
                            {meses.map((mes) => (
                                <option key={mes.valor} value={mes.valor}>
                                    {mes.nome}
                                </option>
                            ))}
                        </select>
                        <br />
                        <label>Selecione o ano</label>
                        <select required className='form-control' value={anoSelecionado} onChange={(e) => setAnoSelecionado(e.target.value)}>
                            {anos.map((ano) => (
                                <option key={ano} value={ano}>
                                    {ano}
                                </option>
                            ))}
                        </select>
                    </ModalBody>
                    <ModalFooter>
                        <button type='button' onClick={() => { initState(); setShow(false) }} className='btn btn-danger'>Fechar</button>
                        <button type='submit' className='btn btn-success'>Gerar</button>
                    </ModalFooter>
                </form>
            </Modal>
        </>
    );
}

export default ModalNovaRemessa;