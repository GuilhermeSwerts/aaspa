import React, { useEffect, useContext, useState } from 'react';
import './Relatorio.css';
import { NavBar } from '../../../components/Layout/layout'
import { AuthContext } from '../../../context/AuthContext';
import { ButtonTooltip } from '../../../components/Inputs/ButtonTooltip'
import { FaDownload } from 'react-icons/fa';
import { api } from '../../../api/api';
import moment from 'moment';
import { Mascara } from '../../../util/mascara';

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

    const [detalheProducao, setDetalheProducao] = useState([]);
    const [detalhes, setDetalhes] = useState({
        competencia: "",
        corretora: "",
        remessa: "",
        averbados: "",
        taxaAverbacao: ""
    });
    const [motivosNaoAverbada, setMotivosNaoAverbada] = useState([]);
    const [resumo, SetResumo] = useState({
        "totalRemessa": 3,
        "totalNaoAverbada": 0
    })
    const [taxaNaoAverbado, setTaxaNaoAverbado] = useState(0);

    const BuscarRelatorio = () => {
        const mesCorrente = moment().format('MM');
        const anoCorrente = moment().format('YYYY');
        let ano = anoSelecionado ? anoSelecionado : anoCorrente;
        let mes = mesSelecionado ? mesSelecionado : mesCorrente;
        setMesSelecionado(parseInt(mes));
        setAnoSelecionado(ano);
        api.get(`RelatorioAverbacao?mes=${mes}&ano=${ano}`, response => {
            SetResumo(response.data.resumo);
            setTaxaNaoAverbado(response.data.taxaNaoAverbado);
            setMotivosNaoAverbada(response.data.motivosNaoAverbada)
            setDetalhes(response.data.detalhes);
            setDetalheProducao(response.data.relatorio);
        }, erro => {
            alert('Houve um erro ao buscar o relatório.')
        })
    }

    useEffect(() => {
        handdleUsuarioLogado();
        BuscarRelatorio();
    }, []);

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
                    <button className='btn btn-primary' onClick={BuscarRelatorio}>Buscar</button>
                </div>
            </div>
            <hr />
            <div className="container-relatorio">
                <h1 style={{ textAlign: 'center' }}>EXTRATO DE RETORNO DATA PREV</h1>
                <ResumoProducao
                    motivosNaoAverbada={motivosNaoAverbada}
                    resumoTotal={resumo}
                    taxaNaoAverbado={taxaNaoAverbado}
                    resumo={detalhes}
                    mesSelecionado={mesSelecionado}
                    anoSelecionado={anoSelecionado} />
                <br />
                <DetalheProducao detalhes={detalheProducao} />
            </div>
        </NavBar>
    );
}

function ResumoProducao({ motivosNaoAverbada,
    taxaNaoAverbado, resumoTotal, resumo, mesSelecionado, anoSelecionado }) {
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
                        <li>COMPETENCIA: {mesSelecionado}/{anoSelecionado}</li>
                        <li>CORRETORA: {resumo.corretora}</li>
                        <li>Remessa: {resumo.remessa}</li>
                        <li>Averbados: {resumo.averbados}</li>
                    </ul>
                    <p>Taxa de Averbacao: {resumo.taxaAverbacao}%</p>
                </div>
                <div className="col-md-5 container-relatorio-center">
                    <h4>Motivos não averbados</h4>
                    <ul className='container-motivo-nao-verbados'>
                        {motivosNaoAverbada.map((item, index) => (
                            <li key={index}>{item.descricaoErro}: {item.totalPorCodigoErro} ({item.totalPorcentagem}%)</li>
                        ))}
                    </ul>
                    <p>Total Não averbado: {resumoTotal.totalNaoAverbada} ({taxaNaoAverbado}%)</p>
                </div>
            </div>
        </div>
    );
}

function DetalheProducao({ detalhes }) {

    const [mesSelecionado, setMesSelecionado] = useState(null);
    const [anoSelecionado, setAnoSelecionado] = useState(null);

    const DownloadAverbacao = () => {
        const mesCorrente = moment().format('MM');
        const anoCorrente = moment().format('YYYY');
        let ano = anoSelecionado ? anoSelecionado : anoCorrente;
        let mes = mesSelecionado ? mesSelecionado : mesCorrente;
        setMesSelecionado(parseInt(mes));
        setAnoSelecionado(ano);
        api.get(`DownloadAverbacao?mes=${mes}&ano=${ano}`, res => {
            const { nomeArquivo, base64 } = res.data;

            // Convert base64 to binary string
            let binaryString = atob(base64);

            // Create a buffer and view to store binary data
            let buffer = new ArrayBuffer(binaryString.length);
            let view = new Uint8Array(buffer);

            // Copy binary data to view
            for (let i = 0; i < binaryString.length; i++) {
                view[i] = binaryString.charCodeAt(i);
            }

            // Create a Blob from the binary data
            let blob = new Blob([view], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });

            // Create an object URL for the Blob
            let url = window.URL.createObjectURL(blob);

            // Create a link element
            let a = document.createElement("a");
            a.href = url;
            a.download = nomeArquivo.endsWith(".xlsx") ? nomeArquivo : `${nomeArquivo}.xlsx`;

            // Trigger the download
            a.click();

            // Revoke the object URL
            window.URL.revokeObjectURL(url);
        }, err => {
            alert(err.response.data);
        });
    }

    return (
        <div className="detalhe-producao">
            <div className="resuto-title">
                <h3>Detalhe de Produção</h3>
                <ButtonTooltip
                    onClick={_ => DownloadAverbacao()}
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
                        {detalhes && detalhes.map((detalhe, index) => (
                            <tr key={index}>
                                <td>{detalhe.codExterno}</td>
                                <td>{Mascara.cpf(detalhe.clienteCpf)}</td>
                                <td>{detalhe.clienteNome}</td>
                                <td>{(Mascara.data(detalhe.dataInicioDesconto)).split(' ')[0]}</td>
                                <td>R$ {detalhe.valorDesconto.toFixed(2)}</td>
                                <td>{detalhe.codigoResultado === 1 ? 'AVERBADO' : detalhe.codigoResultado === 2 ? 'NÃO AVERBADO' : 'ERRO'}</td>
                                <td>{detalhe.descricaoErro}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
}

export default Relatorio;
