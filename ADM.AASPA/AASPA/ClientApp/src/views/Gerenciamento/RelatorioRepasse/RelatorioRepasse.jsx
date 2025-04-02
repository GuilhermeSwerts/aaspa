import React, { useEffect, useContext, useState } from 'react';
import './Relatorio.css';
import { NavBar } from '../../../components/Layout/layout'
import { AuthContext } from '../../../context/AuthContext';
import { ButtonTooltip } from '../../../components/Inputs/ButtonTooltip'
import { FaDownload } from 'react-icons/fa';
import { api } from '../../../api/api';
import moment from 'moment';
import { Mascara } from '../../../util/mascara';
import { Alert, Info } from '../../../util/alertas';
import { Size } from '../../../util/size';

function RelatorioCarteira() {
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
    const [mesSelecionado, setMesSelecionado] = useState('');
    const [anoSelecionado, setAnoSelecionado] = useState('');
    const anoAtual = new Date().getFullYear();
    const anos = Array.from({ length: 25 }, (_, i) => anoAtual - i);

    const [relatorio, setRelatorio] = useState([]);
    const [totalDescontos, setTotalDescontos] = useState(0);
    const [valorTotalRepasse, setValorTotalRepasse] = useState(0);
    const [mesRepasse, setMesRepasse] = useState('');
    const [competencia, setCompetencia] = useState('');

    const [show, setshow] = useState(false);

    const BuscarRelatorio = () => {
        setshow(false);
        api.get(`RelatorioRepasse?mes=${mesSelecionado}&ano=${anoSelecionado}`, response => {
            setRelatorio(response.data.relatorio);
            setTotalDescontos(response.data.cabecalho.totalDescontos)
            setValorTotalRepasse(response.data.cabecalho.valorTotalRepasse);
            setMesRepasse(response.data.cabecalho.mesRepasse)
            setCompetencia(response.data.cabecalho.competencia)
            setshow(true)
        }, erro => {
            Alert(erro.response ? erro.response.data : 'Houve um erro ao buscar o relatório.', false)
        })
    }

    useEffect(() => {
        handdleUsuarioLogado();
        const mesCorrente = moment().format('MM');
        const anoCorrente = moment().format('YYYY');
        let ano = anoSelecionado ? anoSelecionado : anoCorrente;
        let mes = mesSelecionado ? mesSelecionado : mesCorrente;
        setMesSelecionado(parseInt(mes));
        setAnoSelecionado(ano);
    }, []);

    const DownloadRelatorio = () => {
        api.get(`DownloadRelatorio?ano=${anoSelecionado}&mes=${mesSelecionado}&tiporel=0`, res => {
            const { nomeArquivo, base64 } = res.data;
            let binaryString = atob(base64);
            let buffer = new ArrayBuffer(binaryString.length);
            let view = new Uint8Array(buffer);
            for (let i = 0; i < binaryString.length; i++) {
                view[i] = binaryString.charCodeAt(i);
            }
            let blob = new Blob([view], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
            let url = window.URL.createObjectURL(blob);
            let a = document.createElement("a");
            a.href = url;
            a.download = `RelatorioRepasse_${anoSelecionado}${mesSelecionado}.xlsx`;
            a.click();
            window.URL.revokeObjectURL(url);
        }, erro => {
            Alert(erro.response ? erro.response.data : 'Houve um erro ao fazer o download do relatório.', false)
        })
    }

    const Filtro = () => {
        return (
            <div className='row'>
                <div className="col-md-2">
                    <label htmlFor="">Selecione o mês:</label>
                    <select className='form-control' value={mesSelecionado} onChange={(e) => setMesSelecionado(e.target.value)}>
                        {meses.map((mes) => (
                            <option key={mes.valor} value={mes.valor}>
                                {mes.nome}
                            </option>
                        ))}
                    </select>
                </div>
                <div className="col-md-2">
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
        );
    }

    function Resumo() {
        return (
            relatorio.length > 0 && <div className="container-main-resumo">
                <div className="resuto-title">
                    <h3>Resumo de Produção</h3>
                </div>
                <br />
                <div style={{ width: '100%', gap: 10, display: 'flex', justifyContent: 'center' }}>
                    <div className="col-md-4 container-relatorio-center">
                        <h4>Detalhes Sintético</h4>
                        <ul className='container-motivo-nao-verbados'>
                            <li>Competencia: {competencia}</li>
                            <li>Mês Repasse: {mesRepasse}</li>
                            <li>Total Descontos: {totalDescontos}</li>
                        </ul>
                        <p>Valor Total Repasse: {valorTotalRepasse}</p>
                    </div>
                </div>
            </div>
        );
    }

    function Relatorio() {
        const [paginaAtual, setPaginaAtual] = useState(1);
        const itensPorPagina = 10;

        const totalPaginas = Math.ceil(relatorio.length / itensPorPagina);

        const indexInicial = (paginaAtual - 1) * itensPorPagina;
        const indexFinal = indexInicial + itensPorPagina;
        const itensAtuais = relatorio.slice(indexInicial, indexFinal);

        const handlePageChange = (numeroPagina) => {
            setPaginaAtual(numeroPagina);
        };

        const renderizarPaginas = () => {
            const paginas = [];
            const range = 2;

            let start = Math.max(1, paginaAtual - range);
            let end = Math.min(totalPaginas, paginaAtual + range);

            if (start > 1) {
                paginas.push(
                    <button key={1} onClick={() => handlePageChange(1)}>1</button>
                );
                if (start > 2) {
                    paginas.push(<span key="ellipsis-start">...</span>);
                }
            }

            for (let i = start; i <= end; i++) {
                paginas.push(
                    <button
                        key={i}
                        onClick={() => handlePageChange(i)}
                        disabled={i === paginaAtual}
                    >
                        {i}
                    </button>
                );
            }

            if (end < totalPaginas) {
                if (end < totalPaginas - 1) {
                    paginas.push(<span key="ellipsis-end">...</span>);
                }
                paginas.push(
                    <button key={totalPaginas} onClick={() => handlePageChange(totalPaginas)}>
                        {totalPaginas}
                    </button>
                );
            }

            return paginas;
        };

        return (
            relatorio.length > 0 && <div className="detalhe-producao">
                <div className="resuto-title">
                    <h3>Detalhe de Produção</h3>
                    <ButtonTooltip
                        onClick={_ => DownloadRelatorio()}
                        className='btn btn-primary'
                        text={'Extrair Para Excel(.csv)'}
                        top={false}
                        textButton={<FaDownload size={Size.IconeTabela} />}
                    />
                </div>
                <br />
                <div className="container-relatorio-center">

                    <table className='table table-striped'>
                        <thead>
                            <tr>
                                <th>NR.BENEFICIO</th>
                                <th>CPF</th>
                                <th>Nome</th>
                                <th>Data Adesão</th>
                                <th>Taxa Associativa</th>
                                <th>Parcela</th>
                            </tr>
                        </thead>
                        <tbody>
                            {itensAtuais && itensAtuais.map((detalhe, index) => (
                                <tr key={index}>
                                    <td>{detalhe.codExterno}</td>
                                    <td>{Mascara.cpf(detalhe.cpf)}</td>
                                    <td>{detalhe.nome}</td>
                                    <td>{detalhe.dataInicioDesconto ? (Mascara.data(detalhe.dataInicioDesconto)).split(' ')[0] : '-'}</td>
                                    <td>{detalhe.taxaAssociativa}</td>
                                    <td>{detalhe.parcela}</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                    <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', gap: 10 }}>
                        {paginaAtual > 1 && (
                            <button className='btn btn-primary' onClick={() => handlePageChange(paginaAtual - 1)}>Anterior</button>
                        )}
                        {renderizarPaginas()}
                        {paginaAtual < totalPaginas && (
                            <button className='btn btn-primary' onClick={() => handlePageChange(paginaAtual + 1)}>Próximo</button>
                        )}
                    </div>
                </div>
            </div>
        );
    }

    return (
        <NavBar pagina_atual={'CARTEIRA'} usuario_tipo={usuario && usuario.usuario_tipo} usuario_nome={usuario && usuario.usuario_nome}>
            <h1>Extrato de repasse INSS</h1>
            <small>FILTRO:</small>
            {Filtro()}
            <hr />
            {Resumo()}
            <br />
            {Relatorio()}
        </NavBar>
    );
}
export default RelatorioCarteira;
