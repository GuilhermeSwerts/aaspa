import React, { useContext, useEffect, useState } from 'react';
import { AuthContext } from '../../../context/AuthContext';
import { NavBar } from '../../../components/Layout/layout';
import { Col, Row, Button, Input, FormGroup, Label } from 'reactstrap';
import { api } from '../../../api/api';
import BuscarPorMesAno from './BuscarRetornoPorMesAno';
import moment from 'moment';
import axios from 'axios';
import { format } from 'date-fns';
import { ptBR } from 'date-fns/locale';
import ReactPaginate from 'react-paginate';
import { Alert } from '../../../util/alertas';

function Retorno() {
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
            Alert('Por favor, selecione um arquivo antes de enviar.', false);
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
            Alert("Arquivo do mês " + mes + " de " + ano + " importado com sucesso!");
            BuscarRetorno();
        }, err => {
            Alert(err.response.data, false)
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
                Alert('Erro ao buscar retorno: ' + error.response.data, false);
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
        <NavBar pagina_atual={'RETORNO'} usuario_tipo={usuario && usuario.usuario_tipo} usuario_nome={usuario && usuario.usuario_nome}>
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
                            <Col md='6'>
                                <h5>Dados Retorno:</h5>
                                <h6>Id do Retorno: {retorno.idRetorno}</h6>
                                <h6>Nome Arquivo Retorno: {retorno.nomeArquivoRetorno}</h6>
                                <h6>Data de Importação: {retorno.dataImportacao ? format(new Date(retorno.dataImportacao), "dd-MM-yyyy hh:mm:ss", { locale: ptBR }) : ''}</h6>
                            </Col>
                            <Col md='6'>
                                <h5>Dados Remessa:</h5>
                                <h6>Id Remessa: {retorno.idRemessa}</h6>
                                <h6>Nome Arquivo Remessa: {retorno.nomeArquivoRemessa}</h6>
                                <h6>Data Geração Remessa: {retorno.dataHoraGeracaoRemessa ? format(new Date(retorno.dataHoraGeracaoRemessa), "dd-MM-yyyy hh:mm:ss", { locale: ptBR }) : ''}</h6>
                            </Col>
                        </Row>
                    </div>
                    <table className='table table-striped'>
                        <thead>
                            <tr>
                                <th>Retorno ID</th>
                                <th>Número Benefício</th>
                                <th>Código Operação</th>
                                <th>Código Resultado</th>
                                <th>Motivo Rejeição</th>
                                <th>Perc. Desconto</th>
                                <th>Data Início Desconto</th>
                                <th>Código Espécie Benefício</th>
                            </tr>
                        </thead>
                        <tbody>
                            {currentItems.map(remessa => (
                                <tr key={remessa.id}>
                                    <td>{remessa.id}</td>
                                    <td>{remessa.numero_Beneficio}</td>
                                    <td>{remessa.codigo_Operacao}</td>
                                    <td>{remessa.codigo_Resultado}</td>
                                    <td>{remessa.motivo_Rejeicao}</td>
                                    <td>{(remessa.valor_Desconto / 100).toFixed(2)}</td>
                                    <td>{remessa.data_Inicio_Desconto ? format(new Date(remessa.data_Inicio_Desconto), "dd-MM-yyyy", { locale: ptBR }) : ''}</td>
                                    <td>{remessa.codigo_Especie_Beneficio}</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                    <ReactPaginate
                        previousLabel={'anterior'}
                        nextLabel={'próximo'}
                        breakLabel={'...'}
                        breakClassName={'break-me'}
                        pageCount={Math.ceil((retorno.retornos ? retorno.retornos.length : 0) / itemsPerPage)}
                        marginPagesDisplayed={2}
                        pageRangeDisplayed={5}
                        onPageChange={handlePageClick}
                        containerClassName={'pagination justify-content-center'}
                        activeClassName={'active'}
                        previousClassName={'page-item'}
                        nextClassName={'page-item'}
                        previousLinkClassName={'page-link'}
                        nextLinkClassName={'page-link'}
                        breakClassName={'page-item'}
                        breakLinkClassName={'page-link'}
                    />
                </>
            )}
        </NavBar>
    );
}

export default Retorno;
