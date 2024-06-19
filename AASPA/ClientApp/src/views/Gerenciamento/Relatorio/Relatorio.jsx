import React, { useEffect, useContext, useState } from 'react';
import './Relatorio.css';
import { NavBar } from '../../../components/Layout/layout'
import { AuthContext } from '../../../context/AuthContext';
import { ButtonTooltip } from '../../../components/Inputs/ButtonTooltip'
import { FaDownload } from 'react-icons/fa';
import { api } from '../../../api/api';
import moment from 'moment'; 

function Relatorio() {
    const { usuario, handdleUsuarioLogado } = useContext(AuthContext);
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
    const [mesSelecionado, setMesSelecionado] = useState(null);
    const [anoSelecionado, setAnoSelecionado] = useState(null);
    const anoAtual = new Date().getFullYear();
    const anos = Array.from({ length: 25 }, (_, i) => anoAtual - i);

    const BuscarRelatorio = () => {
        const mesCorrente = moment().format('MM');
        const anoCorrente = moment().format('YYYY');
        let ano = anoSelecionado ? anoSelecionado : anoCorrente;
        let mes = mesSelecionado ? mesSelecionado : mesCorrente;
        setMesSelecionado(parseInt(mes));
        setAnoSelecionado(ano);
        api.get(`RelatorioAverbacao?mes=${mes}&ano=${ano}`,response=> {
            debugger
        },erro=> {
            alert('Houve um erro ao buscar o relatório.')
        })
    }

    useEffect(() => {
        handdleUsuarioLogado();
        BuscarRelatorio();
    }, []);


    const resumoProducao = {
        competencia: 'mar/24',
        corretora: 'Confia',
        remessa: 2209,
        averbados: 1715,
        taxaAverbacao: '78%',
        motivosNaoAverbados: [
            { motivo: '002 - Espécie incompatível', total: 0, porcentagem: '0%' },
            { motivo: '004 - NB inexistente no cadastro', total: 0, porcentagem: '0%' },
            { motivo: '005 - Benefício não ativo', total: 31, porcentagem: '1%' },
            { motivo: '008 - MR ultrapassada MR do titular', total: 2, porcentagem: '0%' },
            { motivo: '008 - Já existe desc. p/ outra entidade', total: 371, porcentagem: '17%' },
            { motivo: '012 - Benefício bloqueado para desconto', total: 90, porcentagem: '4%' },
        ],
        totalNaoAverbado: { total: 494, porcentagem: '22%' },
    };

    const detalheProducao = [
        { codExterno: '6244450691', cpf: '348.301.558-25', nome: 'JOSEFINA PINHEIRO', dataAdesao: '03/04/2024', taxaAssociativa: 'R$ 45,00', status: 'Não Averbado', motivo: '008 - Já existe desc. p/ outra entidade' },
        { codExterno: '1562713490', cpf: '142.894.186-00', nome: 'WANDERSON LUIZ DA COSTA GENOVEZ', dataAdesao: '01/03/2024', taxaAssociativa: 'R$ 45,00', status: 'Averbado', motivo: '' },
    ];

    return (
        <NavBar usuario_tipo={usuario && usuario.usuario_tipo} usuario_nome={usuario && usuario.usuario_nome}>
            <small>FILTRO:</small>
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
                    <button className='btn btn-primary' onClick={() => { }}>Buscar</button>
                </div>
            </div>
            <hr />
            <div className="container-relatorio">
                <h1 style={{ textAlign: 'center' }}>EXTRATO DE RETORNO DATA PREV</h1>
                <ResumoProducao resumo={resumoProducao} />
                <br />
                <DetalheProducao detalhes={detalheProducao} />
            </div>
        </NavBar>
    );
}

function ResumoProducao({ resumo }) {
    return (
        <div className="container-main-resumo">
            <div className="resuto-title">
                <h3>Resumo de Produção</h3>
            </div>
            <br />
            <div className="row" style={{ width: '100%', gap: 20, justifyContent: 'center' }}>
                <div className="col-md-5 container-relatorio-center">
                    <h4>Detalhes</h4>
                    <ul className='container-motivo-nao-verbados'>
                        <li>COMPETENCIA: {resumo.competencia}</li>
                        <li>CORRETORA: {resumo.corretora}</li>
                        <li>Remessa: {resumo.remessa}</li>
                        <li>Averbados: {resumo.averbados}</li>
                    </ul>
                    <p>Taxa de Averbacao: {resumo.taxaAverbacao}</p>
                </div>
                <div className="col-md-5 container-relatorio-center">
                    <h4>Motivos não averbados</h4>
                    <ul className='container-motivo-nao-verbados'>
                        {resumo.motivosNaoAverbados.map((item, index) => (
                            <li key={index}>{item.motivo}: {item.total} ({item.porcentagem})</li>
                        ))}
                    </ul>
                    <p>Total Não averbado: {resumo.totalNaoAverbado.total} ({resumo.totalNaoAverbado.porcentagem})</p>
                </div>
            </div>
        </div>
    );
}

function DetalheProducao({ detalhes }) {
    return (
        <div className="detalhe-producao">
            <div className="resuto-title">
                <h3>Detalhe de Produção</h3>
                <ButtonTooltip
                    onClick={() => { }}
                    className='btn btn-primary'
                    text={'Extrair Para Excel(.csv)'}
                    top={false}
                    textButton={<FaDownload size={25} />}
                />
            </div>
            <br />
            <div className="container-relatorio-center">

                <table className='table table-striped'>
                    <thead>
                        <tr>
                            <th>Cod Externo</th>
                            <th>CPF</th>
                            <th>Nome</th>
                            <th>Data Adesão</th>
                            <th>Taxa Associativa</th>
                            <th>Status</th>
                            <th>Motivo</th>
                        </tr>
                    </thead>
                    <tbody>
                        {detalhes.map((detalhe, index) => (
                            <tr key={index}>
                                <td>{detalhe.codExterno}</td>
                                <td>{detalhe.cpf}</td>
                                <td>{detalhe.nome}</td>
                                <td>{detalhe.dataAdesao}</td>
                                <td>{detalhe.taxaAssociativa}</td>
                                <td>{detalhe.status}</td>
                                <td>{detalhe.motivo}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
}

export default Relatorio;
