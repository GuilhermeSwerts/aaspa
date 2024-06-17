import React, { useContext, useEffect, useState } from 'react';
import { AuthContext } from '../../../context/AuthContext';
import { NavBar } from '../../../components/Layout/layout';
import { Col, Row, Button, Input, FormGroup, Label } from 'reactstrap';
import { api } from '../../../api/api';
import BuscarPorMesAno from './BuscarRepassePorMesAno';
import moment from 'moment';
import axios from 'axios';
import { format } from 'date-fns';
import { ptBR, tr } from 'date-fns/locale';
import ReactPaginate from 'react-paginate';

function RepasseFinanceiro() {
    const { usuario, handdleUsuarioLogado } = useContext(AuthContext);
    const [mesSelecionado, setMesSelecionado] = useState(null);
    const [anoSelecionado, setAnoSelecionado] = useState(null);
    const [retorno, setRetorno] = useState([]);
    const [file, setFile] = useState(null);
    const [fileName, setFileName] = useState('');
    const [importar, setImportar] = useState(false);
    const [currentPage, setCurrentPage] = useState(0);

    const itemsPerPage = 10;

    const handlePageClick = ({ selected }) => {
        setCurrentPage(selected);
    };

    const handleFileChange = (event) => {
        const selectedFile = event.target.files[0];
        setFile(selectedFile);
        setFileName(selectedFile ? selectedFile.name : '');
    };

    const handleSubmit = () => {
        if (file) {
            EnviarRetorno(file);
            setImportar(false);
        } else {
            alert('Por favor, selecione um arquivo antes de enviar.');
        }
    };

    const getMonthName = (monthNumber) => {
        const monthNames = [
            'Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho',
            'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'
        ];
        return monthNames[monthNumber - 1];
    };

    const EnviarRetorno = (file) => {
        const formData = new FormData();
        formData.append('file', file);
        api.post("LerRetornoRemessa", formData, res => {
            const anoMes = String(res.data);
            let mes = getMonthName(parseInt(anoMes.slice(4, 6)));
            let ano = anoMes.slice(0, 4);
            alert("Arquivo do mês " + mes + " de " + ano + " importado com sucesso!");
            BuscarRetorno();
        }, err => {
            alert(err.response.data)
        });
    };

    const HabilitarImportacao = () => {
        setImportar(true);
    };

    const BuscarRetorno = () => {
        const mesCorrente = moment().format('MM');
        const anoCorrente = moment().format('YYYY');
        let ano = anoSelecionado ? anoSelecionado : anoCorrente;
        let mes = mesSelecionado ? mesSelecionado : mesCorrente;
        setMesSelecionado(parseInt(mes));
        setAnoSelecionado(ano);
        axios.post(`BuscarRetorno?mes=${mes}&ano=${ano}`)
            .then(response => {
                console.log('Resposta da API:', response.data);
                if (response.status === 204) {
                    setRetorno({ retornos: [] });
                } else {
                    setRetorno(response.data);
                }
                console.log('Valor retorno idretorno', retorno);
            })
            .catch(error => {
                console.error('Erro ao buscar retorno:', error); // Log de erro no console para debug
                alert('Erro ao buscar retorno: ' + error.response.data);
            });
    };

    useEffect(() => {
        handdleUsuarioLogado();
        BuscarRetorno();
        setImportar(false);
    }, []);

    // Calculando os dados da página atual
    const offset = currentPage * itemsPerPage;
    const currentItems = retorno.retornos ? retorno.retornos.slice(offset, offset + itemsPerPage) : [];

    return (
        <NavBar usuario_tipo={usuario && usuario.usuario_tipo} usuario_nome={usuario && usuario.usuario_nome}>
            <Row>
                <Col md="6">
                    <BuscarPorMesAno
                        mesSelecionado={mesSelecionado}
                        setMesSelecionado={setMesSelecionado}
                        anoSelecionado={anoSelecionado}
                        setAnoSelecionado={setAnoSelecionado}
                        BuscarRemessas={BuscarRetorno}
                        OnClick={BuscarRetorno}
                    />
                </Col>
                <Col md="12" style={{ marginTop: '2rem' }}>
                    {!importar && (
                        <Button color="primary" onClick={HabilitarImportacao}>Importar Arquivo</Button>
                    )}
                    {importar === true && (
                        <Col md="12">
                            <Row>
                                <Col md="6">
                                    <Label for="fileID" className="btn btn-secondary" style={{ marginTop: '8px', marginRight: '10px' }}>
                                        Selecionar Arquivo
                                    </Label>
                                    <Button color="primary" onClick={handleSubmit}>
                                        Enviar Retorno
                                    </Button>
                                </Col>
                                <Col md="6">
                                    <Input
                                        onChange={handleFileChange}
                                        type="file"
                                        id="fileID"
                                        style={{ display: 'none' }}
                                    />
                                    {fileName && <p>Arquivo selecionado: {fileName}</p>}
                                </Col>
                            </Row>
                        </Col>
                    )}
                </Col>
            </Row>
            <br />
            <br />
            {retorno && (
                <>
                    <div className="p-3 border rounded">
                        <Row>
                            <Col md='4'>
                                <h5>Dados Retorno:</h5>
                                <h6>Id do Retorno: {retorno.idRetorno}</h6>
                                <h6>Nome Arquivo Retorno: {retorno.nomeArquivoRetorno}</h6>
                                <h6>Data de Importação: {retorno.dataImportacao ? format(new Date(retorno.dataImportacao), "dd-MM-yyyy hh:mm:ss", { locale: ptBR }) : ''}</h6>
                            </Col>
                            <Col md='4'>
                                <h5>Dados Remessa:</h5>
                                <h6>Id Remessa: {retorno.idRemessa}</h6>
                                <h6>Nome Arquivo Remessa: {retorno.nomeArquivoRemessa}</h6>
                                <h6>Data Geração Remessa: {retorno.dataHoraGeracaoRemessa ? format(new Date(retorno.dataHoraGeracaoRemessa), "dd-MM-yyyy hh:mm:ss", { locale: ptBR }) : ''}</h6>
                            </Col>
                            <Col md='4'>
                                <h5>Dados Repasse:</h5>
                                <h6>Id Repasse: {retorno.idRemessa}</h6>
                                <h6>Nome Arquivo Remessa: {retorno.nomeArquivoRemessa}</h6>
                                <h6>Data Geração Remessa: {retorno.dataHoraGeracaoRemessa ? format(new Date(retorno.dataHoraGeracaoRemessa), "dd-MM-yyyy hh:mm:ss", { locale: ptBR }) : ''}</h6>
                            </Col>
                        </Row>
                    </div>
                    <table className='table table-striped'>
                        <thead>
                            <tr>
                                <th>Repasse/Financeiro ID</th>
                                <th>Número Benefício</th>
                                <th>Competencia Desconto</th>
                                <th>Espécie</th>
                                <th>Uf</th>
                                <th>Desconto</th>
                            </tr>
                        </thead>
                        <tbody>
                            {[1, 2, 3, 4, 5, 6, 8].map((x, i) => (
                                <tr>
                                    <td>{i + 1}</td>
                                    <td>{12342243523}</td>
                                    <td>Teste</td>
                                    <td>Dinheiro</td>
                                    <td>MG</td>
                                    <td>R$ {`${123.12.toFixed(2)}`}</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </>
            )}
        </NavBar>
    );
}

export default RepasseFinanceiro;
