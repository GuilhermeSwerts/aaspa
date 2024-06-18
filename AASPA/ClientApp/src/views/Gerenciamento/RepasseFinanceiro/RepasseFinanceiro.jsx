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

function RepasseFinanceiro() {
    const { usuario, handdleUsuarioLogado } = useContext(AuthContext);
    const [mesSelecionado, setMesSelecionado] = useState(null);
    const [anoSelecionado, setAnoSelecionado] = useState(null);
    const [file, setFile] = useState(null);
    const [fileName, setFileName] = useState('');
    const [importar, setImportar] = useState(false);

    const [dadosRepasse, setDadosRepasse] = useState([]);

    const [remessa, setRemessa] = useState({
        remessa_id: "",
        remessa_ano_mes: "",
        nome_arquivo_remessa: "",
        remessa_data_criacao: "",
        remessa_periodo_de: "",
        remessa_periodo_ate: ""
    });

    const [retorno, setRetorno] = useState({
        retorno_Id: "",
        anoMes: "",
        nome_Arquivo_Retorno: "",
        remessa_Id: "",
        data_Importacao: ""
    });

    const [repasse, setRepasse] = useState({
        retorno_financeiro_id: "",
        repasse: "",
        competencia_Repasse: "",
        remessa_id: "",
        retorno_id: "",
        ano_mes: "",
        data_importacao: "",
        nome_arquivo: "",
    });

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
            alert("Arquivo do importado com sucesso!");
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
        api.get(`BuscarRepasse?mes=${mes}&ano=${ano}`, response => {
            setRemessa(response.data.remessa);
            setRetorno(response.data.retorno);
            setRepasse(response.data.repasses);
            setDadosRepasse(response.data.dadosRepasse);
        }, error => {
            console.error('Erro ao buscar retorno:', error); // Log de erro no console para debug
            alert('Erro ao buscar retorno: ' + error.response.data);
        })
    };

    useEffect(() => {
        handdleUsuarioLogado();
        BuscarRetorno();
        setImportar(false);
    }, []);

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
                                        Enviar Repasse
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
                                <h5>Dados Remessa:</h5>
                                <h6>Id Remessa: {remessa.remessa_id}</h6>
                                <h6>Nome Arquivo Remessa: {remessa.nome_arquivo_remessa}</h6>
                                <h6>Data Geração Remessa: {remessa.remessa_data_criacao ? format(new Date(remessa.remessa_data_criacao), "dd-MM-yyyy hh:mm:ss", { locale: ptBR }) : ''}</h6>
                            </Col>
                            <Col md='4'>
                                <h5>Dados Retorno:</h5>
                                <h6>Id do Retorno: {retorno.retorno_Id}</h6>
                                <h6>Nome Arquivo Retorno: {retorno.nome_Arquivo_Retorno}</h6>
                                <h6>Data de Importação: {retorno.data_Importacao ? format(new Date(retorno.data_Importacao), "dd-MM-yyyy hh:mm:ss", { locale: ptBR }) : ''}</h6>
                            </Col>
                            <Col md='4'>
                                <h5>Dados Repasse:</h5>
                                <h6>Id Repasse: {repasse.retorno_financeiro_id}</h6>
                                <h6>Nome Arquivo Remessa: {repasse.nome_arquivo}</h6>
                                <h6>Data Geração Remessa: {repasse.data_importacao ? format(new Date(repasse.data_importacao), "dd-MM-yyyy hh:mm:ss", { locale: ptBR }) : ''}</h6>
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
                            {dadosRepasse.map((data, i) => (
                                <tr>
                                    <td>{data.id}</td>
                                    <td>{data.numero_beneficio}</td>
                                    <td>{data.competencia_desconto}</td>
                                    <td>{data.especie}</td>
                                    <td>{data.uf}</td>
                                    <td>R$ {`${data.desconto.toFixed(2)}`}</td>
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
