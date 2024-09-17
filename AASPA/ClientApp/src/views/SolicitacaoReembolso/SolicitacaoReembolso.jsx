import React, { useContext, useEffect, useState } from 'react';
import { GiPayMoney } from "react-icons/gi";
import { NavBar } from '../../components/Layout/layout';
import { AuthContext } from '../../context/AuthContext';
import Paginacao from '../../components/Paginacao/PaginacaoSimplificada';
import { api } from '../../api/api';
import { Alert, Pergunta } from '../../util/alertas';
import { OverlayTrigger, Tooltip } from 'react-bootstrap';
import { Mascara } from '../../util/mascara';
import { CiBank } from "react-icons/ci";
import { Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';
import { FaDownload } from 'react-icons/fa6';
import { Size } from '../../util/size';

function SolicitacaoReembolso() {
    const { usuario, handdleUsuarioLogado } = useContext(AuthContext)
    const [solicitacoes, setSolicitacoes] = useState([]);
    const [show, setShow] = useState(false);
    const [dtInicio, setDtInicio] = useState('');
    const [dtFim, setDtFim] = useState('');
    const initStateDadosBancarios = {
        chavePix: '',
        banco: '',
        agencia: '',
        conta: '',
        cpf: ''
    };
    const [dadosBancarios, setDadosBancarios] = useState(initStateDadosBancarios);

    //***PAGINAÇÃO
    const [dataPaginacao, setDataPaginacao] = useState([]);
    const [qtdPorPagina, setQtdPorPagina] = useState(5);
    const [paginaAtual, setPaginaAtual] = useState(1);

    const BuscarSolicitacoes = () => {
        api.get(`SolicitacaoReembolso?dtInicio=${dtInicio}&dtFim=${dtFim}`, res => {
            setSolicitacoes(res.data);
        }, err => {
            Alert('Houve um erro ao buscar os dados', false)
        })
    }

    const InformarPagamento = async (id, name) => {
        if (await Pergunta('Deseja realmente informar o pagamento do(a) ' + name))
            api.post(`SolicitacaoReembolso/${id}`, {}, res => {
                BuscarSolicitacoes();
                Alert('Pagamento informado com sucesso!', true);
            }, err => {
                console.clear();
                console.log(err);
                Alert('Houve um erro ao informar o pagamento', false)
            })
    }

    useEffect(() => {
        handdleUsuarioLogado();
        BuscarSolicitacoes();
    }, [])

    const AbrirModalDadosBancarios = (chavePix, banco, agencia, conta, cpf) => {
        setShow(true);
        setDadosBancarios({
            chavePix: chavePix,
            banco: banco,
            agencia: agencia,
            conta: conta,
            cpf: cpf
        })
    }

    const Download = () => {
        window.open(`${api.ambiente}/DownloadSolicitacaoReembolso?${dtInicio}&dtFim=${dtFim}`)
    }

    const DownloadFile = (byteArray, fileName) => {
        const blob = new Blob([new Uint8Array(byteArray)], { type: "application/octet-stream" });
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement("a");
        a.href = url;
        a.download = fileName; // Nome do arquivo
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
    }

    return (
        <NavBar pagina_atual='SOLICITAÇÃO REEMBOLSO' usuario_tipo={usuario && usuario.usuario_tipo} usuario_nome={usuario && usuario.usuario_nome}>
            <Modal isOpen={show}>
                <ModalHeader>
                    Dados bancário
                </ModalHeader>
                <ModalBody>
                    {dadosBancarios.chavePix && dadosBancarios.chavePix !== '' && <>
                        <label>Chave PIX:</label>
                        <textarea className='form-control' value={dadosBancarios.chavePix} disabled></textarea>
                    </>}
                    {!dadosBancarios.chavePix || dadosBancarios.chavePix === '' && <>
                        <label>Cpf:</label>
                        <input type="text" className='form-control' value={dadosBancarios.cpf} disabled />
                        <br />
                        <label>Banco:</label>
                        <input type="text" className='form-control' value={dadosBancarios.banco} disabled />
                        <br />
                        <label>Agência:</label>
                        <input type="text" className='form-control' value={dadosBancarios.agencia} disabled />
                        <br />
                        <label>Conta:</label>
                        <input type="text" className='form-control' value={dadosBancarios.conta} disabled />
                        <br />
                        <label>Banco:</label>
                        <input type="text" className='form-control' value={dadosBancarios.banco} disabled />
                    </>}
                </ModalBody>
                <ModalFooter>
                    <button onClick={e => { setDadosBancarios(initStateDadosBancarios); setShow(false) }} className="btn btn-danger">Fechar</button>
                </ModalFooter>
            </Modal>
            <div className="row">
                <div className="col-md-3">
                    <label>Data Solicitação de:</label>
                    <input value={dtInicio} onChange={e => setDtInicio(e.target.value)} type="date" className='form-control' />
                </div>
                <div className="col-md-3">
                    <label htmlFor="">até:</label>
                    <input value={dtFim} onChange={e => setDtFim(e.target.value)} type="date" className='form-control' />
                </div>
                <div className="col-md-3" style={{ marginTop: '2rem' }}>
                    <button className='btn btn-primary'>Buscar</button>
                </div>
                <div className="col-md-3" style={{ marginTop: '2rem' }}>
                    <button className='btn btn-primary' onClick={Download}>Download <FaDownload /></button>
                </div>
            </div>
            <br />
            <table className='table table-striped'>
                <thead >
                    <tr>
                        <th>#</th>
                        <th>Nome</th>
                        <th>Cpf</th>
                        <th>Nb</th>
                        <th>Telefone</th>
                        {/* <th>ChavePix</th> */}
                        {/* <th>Banco</th> */}
                        {/* <th>Agencia</th> */}
                        {/* <th>Conta</th> */}
                        <th>Protocolo</th>
                        <th>Situacao</th>
                        <th>Dt Solicitacao</th>
                        <th>Dt Pagamento</th>
                        <th>Dados Bancários</th>
                        <th>Informar Pagamento</th>
                    </tr>
                </thead>
                <tbody>
                    {solicitacoes.map((x, rowIndex) => (
                        <tr className="selecao">
                            <td>{x.idSolicitacao}</td>
                            <td>
                                <OverlayTrigger
                                    placement="top"
                                    overlay={<Tooltip id={`tooltip-${rowIndex}`}>{x.nome}</Tooltip>}
                                >
                                    <span>
                                        {x.nome.length > 20 ? `${x.nome.substring(0, 20)}...` : x.nome}
                                    </span>
                                </OverlayTrigger>
                            </td>
                            <td>{Mascara.cpf(x.cpf)}</td>
                            <td>{x.nb}</td>
                            <td>{x.telefone}</td>
                            {/* <td>{ x.chavePix}</td> */}
                            {/* <td>{x.banco}</td> */}
                            {/* <td>{x.agencia}</td> */}
                            {/* <td>{x.conta}</td> */}
                            <td>{x.protocolo}</td>
                            <td>{x.situacao}</td>
                            <td>{x.dtSolicitacao}</td>
                            <td>{x.dtPagamento ? x.dtPagamento : "-"}</td>
                            <td><button onClick={e => AbrirModalDadosBancarios(x.chavePix, x.banco, x.agencia, x.conta, x.cpf)} className='btn btn-success'><CiBank size={Size.IconeTabela} /></button></td>
                            <td><button onClick={e => InformarPagamento(x.idSolicitacao, x.nome)} className='btn btn-success'><GiPayMoney size={Size.IconeTabela} /></button></td>
                        </tr>
                    ))}
                    {solicitacoes.length === 0 && <tr><td colSpan={14}>Nenhum dado encontrado....</td></tr>}
                </tbody>
            </table>
            <br />
            <Paginacao
                data={dataPaginacao}
                paginaAtual={paginaAtual}
                qtdPorPagina={qtdPorPagina}
                setPaginaAtual={setPaginaAtual}
                setQtdPorPagina={setQtdPorPagina}
                setData={setDataPaginacao}
            />
        </NavBar>
    );
}

export default SolicitacaoReembolso;