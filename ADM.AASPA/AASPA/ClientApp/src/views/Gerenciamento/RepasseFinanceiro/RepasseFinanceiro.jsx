import React, { useContext, useEffect, useState } from 'react';
import { AuthContext } from '../../../context/AuthContext';
import { NavBar } from '../../../components/Layout/layout';
import { Col, Row, Button, Input, FormGroup, Label, Container } from 'reactstrap';
import { api } from '../../../api/api';
import { Collapse } from 'reactstrap';
import { Alert, Info } from '../../../util/alertas';
import { useDropzone } from 'react-dropzone';
import { FaChevronUp, FaChevronDown } from "react-icons/fa";
import Paginacao from '../../../components/Paginacao/PaginacaoSimplificada';

function RepasseFinanceiro() {
    const { usuario, handdleUsuarioLogado } = useContext(AuthContext);
    const meses = [
        'Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho',
        'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'
    ];
    const [isOpen, setIsOpen] = useState(false);
    const [file, setFile] = useState(null);
    const [fileName, setFileName] = useState('');
    const [importar, setImportar] = useState(false);
    const [dataFile, setDataFile] = useState({
        tipo: '',
        mes: '',
        ano: ''
    })
    const [repasses, setRepasses] = useState([]);
    const [repassesData, setRepassesData] = useState([]);
    const { getRootProps, getInputProps, isDragActive } = useDropzone({
        onDrop: (acceptedFiles) => {
            const selectedFile = acceptedFiles[0];
            if (selectedFile) {
                var name = selectedFile.name;
                if (name.includes('D.SUB.GER.177.REP')) {
                    const anoMes = name.slice(-6);
                    const ano = anoMes.slice(0, 4);
                    const mes = anoMes.slice(4);
                    setDataFile({
                        tipo: 'Repasse',
                        mes: meses[parseInt(mes, 10) - 2],
                        ano: ano
                    })
                    setFile(selectedFile);
                    setFileName(name);
                    setImportar(true);
                } else {
                    Info('Nome do arquivo incorreto.\nNome Base: D.SUB.GER.177.REP.[ANOMES]')
                }
            }
        },
    });
    //**paginação**
    const [limit, setLimit] = useState(10);
    const [currentPage, setCurrentPage] = useState(1);

    const handleSubmit = () => {
        if (file) {
            EnviarRetorno(file);
            HabilitarImportacao();
        } else {
            Alert('Por favor, selecione um arquivo antes de enviar.', false);
        }
    };

    const EnviarRetorno = (file) => {
        const formData = new FormData();
        formData.append('file', file);
        api.post("LerRetornoRemessa", formData, res => {
            Alert("Arquivo do importado com sucesso!");
            BuscarRepasses();
        }, err => {
            Alert(err.response.data, false)
        });
    };

    const HabilitarImportacao = () => {
        setImportar(false);
        setDataFile({
            tipo: '',
            mes: '',
            ano: ''
        })
        setFile(null);
    };

    const BuscarRepasses = () => {
        api.get(`BuscarRepasses`, response => {
            setRepasses(response.data);
            setLimit(5);
        }, error => {
            console.error('Erro ao buscar repasses:', error); // Log de erro no console para debug
            Alert('Erro ao buscar repasses: ' + error.response.data, false);
        })
    };

    useEffect(() => {
        handdleUsuarioLogado();
        BuscarRepasses();
        HabilitarImportacao(false);
    }, []);

    return (
        <NavBar pagina_atual={'REPASSE'} usuario_tipo={usuario && usuario.usuario_tipo} usuario_nome={usuario && usuario.usuario_nome}>
            <div className="row">
                <div className="col-md-3" style={{ marginTop: '2rem', display: 'flex', gap: 10, justifyContent: 'space-between' }}>
                    <button onClick={e => setIsOpen(!isOpen)} className='btn btn-danger'>Adicionar Novo Repasse {isOpen ? <FaChevronDown size={10} /> : <FaChevronUp size={10} />}</button>
                </div>
            </div>
            <br />
            <Collapse isOpen={isOpen}>
                <Container>
                    {!importar && <Row className="w-100">
                        <Col {...getRootProps()} className="d-flex flex-column justify-content-center align-items-center border border-primary p-5" style={{ borderRadius: '8px' }}>
                            <input {...getInputProps()} />
                            {isDragActive ? (
                                <p className="text-primary">Solte os arquivos aqui...</p>
                            ) : (
                                <p className="text-secondary">Arraste e solte um arquivo aqui ou clique para selecionar</p>
                            )}
                        </Col>
                    </Row>}
                    {importar && <Row className="w-100">
                        <Col
                            className="d-flex flex-column justify-content-center align-items-center border border-primary p-5"
                            style={{ borderRadius: '8px', borderStyle: 'dotted !important' }}
                        >
                            <ul>
                                <li><span><strong>Arquivo:</strong> {fileName}</span></li>
                                <li><span><strong>Tipo:</strong> {dataFile.tipo}</span> </li>
                                <li><span><strong>Mês Competente:</strong> {dataFile.mes}</span> </li>
                                <li><span><strong>Ano Competente:</strong> {dataFile.ano}</span> </li>
                            </ul>
                            <button className='btn btn-primary' onClick={handleSubmit}>Enviar Arquivo</button>
                        </Col>
                    </Row>}
                </Container>
            </Collapse>
            <br />
            <table className='table table-striped'>
                <thead>
                    <tr>
                        <th>Repasse Id</th>
                        <th>Nome Remessa Competente</th>
                        <th>Nome Retorno Competente</th>
                        <th>Nome Repasse Competente</th>
                        <th>Data Importação</th>
                    </tr>
                </thead>
                <tbody>
                    {repassesData.map(repasse => (
                        <tr>
                            <td>{repasse.repasseId}</td>
                            <td>{repasse.nomeRemessaCompetente}</td>
                            <td>{repasse.nomeRetornoCompetente}</td>
                            <td>{repasse.nomeRepasseCompetente}</td>
                            <td>{repasse.dataImportacao}</td>
                        </tr>
                    ))}
                    {repasses.length === 0 && <tr><td colSpan={5}>Nenhum Repasse encontrado...</td></tr>}
                </tbody>
            </table>
            <Paginacao
                paginaAtual={currentPage} 
                setPaginaAtual={setCurrentPage}
                qtdPorPagina={limit} 
                setQtdPorPagina={setLimit}
                setData={setRepassesData} 
                data={repasses} 
            />
        </NavBar>
    );
}

export default RepasseFinanceiro;
