import React, { useEffect, useRef, useState } from 'react';
import { api } from '../../api/api';
import { NavBar } from '../../components/Layout/layout';
import { Alert, Info } from '../../util/alertas';
import { FaDropbox, FaSearch } from 'react-icons/fa';

function Sindicato() {
    const inputRef = useRef()
    const [progresso, setProgresso] = useState(0);
    const [page, setPage] = useState(1);
    const [dragging, setDragging] = useState(false);
    const [fileName, setFileName] = useState("");
    const [importado, setImportado] = useState(false);

    const [log, setLog] = useState([]);
    const [nb, setNb] = useState("");
    const [data, setData] = useState([]);


    const handleSubmit = () => {
        setImportado(true);
        api.ArquviGrande('Sindicato/Importar', res => { Alert(res.data); setImportado(false); GetLogArquivos() }, err => { Alert(err.response.data, false); setImportado(false); GetLogArquivos() }, setProgresso);
    }

    const handleDragOver = (e) => {
        e.preventDefault();
        setDragging(true);
    };

    const handleDragLeave = () => {
        setDragging(false);
    };

    const handleDrop = (e) => {
        e.preventDefault();
        setDragging(false);

        const file = e.dataTransfer.files[0];
        if (file) {
            setFileName(file.name);
            inputRef.current.files = e.dataTransfer.files;
        }
    };

    const handleFileChange = (e) => {
        const file = e.target.files[0];
        if (file) {
            setFileName(file.name);
        }
    };

    const GetLogArquivos = () => {
        api.get("Sindicato/Buscar/Arquivos", res => {
            setLog(res.data);
        }, err => Alert(err.response.data))
    }

    const BuscarSindicatosNb = () => {
        if (!nb) {
            Info("Informe o Nb.");
            return;
        }

        api.get("Sindicato/Buscar/" + nb, res => {
            setData(res.data);
        }, err => Alert(err.response.data))
    }

    useEffect(() => {
        GetLogArquivos();
    }, [])

    return (
        <NavBar>
            <div style={{ width: '100%', display: 'flex', justifyContent: 'start', alignItems: 'center', gap: 5 }}>
                <div onClick={e => setPage(1)} style={{ transition: '0.5s', cursor: 'pointer', padding: 15, background: page === 1 ? '#fff' : '#ccc', borderRadius: '20px 20px 0 0' }}>
                    Consultar
                </div>
                <div onClick={e => setPage(2)} style={{ transition: '0.5s', cursor: 'pointer', padding: 15, background: page === 2 ? '#fff' : '#ccc', borderRadius: '20px 20px 0 0' }}>
                    Importar
                </div>
            </div>
            <div style={{ width: '100%', minHeight: '80vh', height: 'auto', background: '#fff', padding: 5 }}>
                {page === 1 && <div>
                    <div style={{ width: '100%', display: 'flex', justifyContent: 'center' }} className="input-group">
                        <input value={nb} onChange={e => setNb(e.target.value)} type="text" style={{ border: '1px solid' }} placeholder='Número do benefício' className="form-control-lg" aria-label="Sizing example input" aria-describedby="inputGroup-sizing-sm" />
                        <button onClick={BuscarSindicatosNb} style={{ cursor: 'pointer' }} className="btn btn-primary" id="inputGroup-sizing-sm">
                            <FaSearch />
                        </button>
                    </div>
                    <hr />
                    {data.length == 0 && <h6 style={{ textAlign: "center" }}><FaDropbox /> Nenhum dado encontrado.</h6>}

                    {data.length > 0 && <div className="table-responsive">
                        <table className="table table-striped text-center">
                            <thead >
                                <tr>
                                    {data[0].map((_, i) => {
                                        return <>
                                            <th>Competência</th>
                                            <th>Cod.Ass</th>
                                        </>
                                    })}
                                </tr>
                            </thead>
                            <tbody>
                                {data.map((linha, index) => (
                                    <tr key={index}>
                                        {linha.map((item, idx) => {
                                            return (
                                                <>
                                                    <td key={idx}>{item.competencia}</td>
                                                    <td key={`${idx}_${idx}`}>{item.cdSindicato}</td>
                                                </>
                                            )
                                        })}
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>}
                </div>}

                {page === 2 && <div>
                    <div className="flex flex-col items-center gap-4 p-4">
                        <div className="d-flex flex-column align-items-center gap-3 p-3">
                            <div
                                className={`w-75 p-4 border border-2 rounded text-center ${dragging ? "border-primary bg-light shadow-lg" : "border-secondary bg-white"}`}
                                style={{
                                    height: '30vh',
                                    borderStyle: 'dotted !important',
                                    transition: '0.3s',
                                    textAlign: 'center',
                                    alignItems: 'center',
                                    justifyContent: 'center',
                                    display: 'flex',
                                    width: '40vw !important',
                                }}
                                onDragOver={handleDragOver}
                                onDragLeave={handleDragLeave}
                                onDrop={handleDrop}
                                onClick={() => inputRef.current.click()}
                            >
                                <input
                                    ref={inputRef}
                                    type="file"
                                    className="d-none"
                                    onChange={handleFileChange}
                                />
                                <p className="text-muted mb-0">{fileName || "Arraste e solte um arquivo aqui ou clique para selecionar"}</p>
                            </div>
                            <br />
                            {fileName && <button className="btn btn-primary" onClick={handleSubmit}>
                                Enviar
                            </button>}
                            {importado && <div style={{ display: 'flex', gap: 5 }}>
                                {progresso > 0 && progresso < 99 && <span className="text-secondary">Carregando arquivo: {progresso}%</span>}
                                {progresso >= 99 && <span className="text-secondary">O arquivo está sendo importando, isso pode demorar de 3 a 5 minutos.</span>}
                                <br />
                                <div class="dots-loader">
                                    <div style={{ width: '10px', height: '10px', backgroundColor: 'var(--cor-principal) !important' }}></div>
                                    <div style={{ width: '10px', height: '10px', backgroundColor: 'var(--cor-principal) !important' }}></div>
                                    <div style={{ width: '10px', height: '10px', backgroundColor: 'var(--cor-principal) !important' }}></div>
                                </div>
                            </div>}
                            <hr />
                            <table className='table table-striped'>
                                <thead>
                                    <tr>
                                        <th>Nome Arquivo</th>
                                        <th>Data Cadastro</th>
                                        <th>Data Importacao</th>
                                        <th>Tempo Importacao</th>
                                        <th>Arquivo Importado</th>
                                        <th>Arquivo Com Erro</th>
                                        <th>Erro</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {log.map(arquivo => (<tr>
                                        <td>{arquivo.nomeArquivo}</td>
                                        <td>{arquivo.dataCadastro}</td>
                                        <td>{arquivo.dataImportacao}</td>
                                        <td>{arquivo.arquivoImportado !== "Sim" ? "-" : arquivo.tempoImportacao}</td>
                                        <td>{arquivo.arquivoImportado}</td>
                                        <td>{arquivo.arquivoComErro}</td>
                                        <td>{arquivo.erro}</td>
                                    </tr>))}
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>}
            </div>

        </NavBar>
    );
}

export default Sindicato;