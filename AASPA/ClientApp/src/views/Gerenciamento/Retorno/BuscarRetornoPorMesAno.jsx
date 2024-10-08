import React, { useState } from 'react';
import { FaFilter } from 'react-icons/fa6';
import ModalNovaRemessa from '../../../components/Modal/novaRemessa';
import { Collapse } from 'reactstrap';

const BuscarPorMesAno = ({
    mesSelecionado,
    setMesSelecionado,
    anoSelecionado,
    setAnoSelecionado,
    BuscarRemessas, OnClick }) => {
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
    const [isOpen, setIsOpen] = useState(false);

    const anoAtual = new Date().getFullYear();
    const anos = Array.from({ length: 25 }, (_, i) => anoAtual - i);

    return (
        <div>
            <div className="row">
                <div className="col-md-3" style={{ marginTop: '2rem', display: 'flex', gap: 10, justifyContent: 'space-between' }}>
                    <button onClick={e => setIsOpen(!isOpen)} className='btn btn-danger'>{isOpen ? 'Fechar' : 'Exibir'} Filtros<FaFilter /></button>
                </div>
            </div>
            <br />
            <Collapse isOpen={isOpen}>
                <>
                    <small>Filtros:</small>
                    <div className='row'>
                        <div className="col-md-4">
                            <label htmlFor="">Selecione o mês:</label>
                            <select className='form-control' value={mesSelecionado} onChange={(e) => setMesSelecionado(e.target.value)}>
                                {meses.map((mes) => (
                                    <option key={mes.valor} value={mes.valor}>
                                        {mes.nome}
                                    </option>
                                ))}
                            </select>
                        </div>
                        <div className="col-md-4">
                            <label>Selecione o ano</label>
                            <select className='form-control' value={anoSelecionado} onChange={(e) => setAnoSelecionado(e.target.value)}>
                                {anos.map((ano) => (
                                    <option key={ano} value={ano}>
                                        {ano}
                                    </option>
                                ))}
                            </select>
                        </div>
                        <div className="col-md-4" style={{ marginTop: '2rem', display: 'flex', gap: 10, justifyContent: 'space-between' }}>
                            <button className='btn btn-primary' onClick={OnClick}>Buscar Retornos Anteriores</button>
                        </div>
                    </div>
                </>
            </Collapse>
        </div>
    );
};

export default BuscarPorMesAno;
