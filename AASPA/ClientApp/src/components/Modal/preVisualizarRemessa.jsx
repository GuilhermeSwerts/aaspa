import React, { Component } from 'react';
import { api } from '../../api/api';
import { Mascara } from '../../util/mascara';
import { TbZoomMoney } from 'react-icons/tb';
import { RiChatHistoryLine } from 'react-icons/ri';
import { FaDownload, FaUserEdit } from 'react-icons/fa';
import { Alert } from '../../util/alertas';
import { Paginacao } from '../Paginacao/Paginacao';


class PreVisualizarRemessa extends Component {
    constructor(props) {
        super(props);
        this.state = {
            show: false,
            ano: '',
            mes: '',
            clientes: [],
            qtdClientes: 0,
            currentPage: 1,
            clientesPerPage: 10,
            dateInit: '',
            dateFim: '',
            anoSelecionado: '',
            mesSelecionado: '',
            offset: 0,
            limit: 10,
        };
        this.Show = (dateInit, dateFim, anoSelecionado, mesSelecionado, paginaAtual = 1) => {
            api.get(`PreVisualizar?DateInit=${dateInit}&DateEnd=${dateFim}&PaginaAtual=${paginaAtual}&QtdPorPagina=${this.state.limit}`, res => {
                this.setState({ clientes: res.data.clientes, qtdClientes: res.data.totalClientes, dateInit, dateFim, anoSelecionado, mesSelecionado });
            }, err => {
                Alert("Houve um erro ao buscar clientes.", false);
            });
            this.setState({ show: true, ano: anoSelecionado, mes: mesSelecionado, dateInit, dateFim });
        };
    }

    closeModal = () => {
        this.setState({ show: false });
    };

    handlePageChange = (newPage) => {
        this.setState({ currentPage: newPage });
    };

    handleDownload = () => {
        window.open(`${api.ambiente}/DownloadClienteFiltro?dateInitAverbacao=${this.state.dateInit}&dateEndAverbacao=${this.state.dateFim}`)
    }

    render() {
        const { ano, mes, clientes, qtdClientes, limit, offset, totalClientes, dateInit, dateFim, anoSelecionado, mesSelecionado } = this.state;

        const setOffset = (value) => { this.setState({ offset: value }) }
        const setLimit = (value) => { this.setState({ limit: value }) }

        const BuscarTodosClientes = (pagina) => this.Show(dateInit, dateFim, anoSelecionado, mesSelecionado, pagina);

        return (
            <div style={{ display: this.state.show ? 'block' : 'none' }} className='previsualizar-container'>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                    <h1>Remessa: D.SUB.GER.176.{ano}{String(mes).padStart(2, '0')}</h1>
                    <h1 onClick={this.closeModal} className='previsualizar-close-button' style={{ textAlign: 'end' }}>X</h1>
                </div>
                <br />
                <span>Total Clientes: {qtdClientes}</span>
                <button onClick={this.handleDownload} className='btn btn-primary'><FaDownload /></button>
                <table className='table table-striped'>
                    <thead>
                        <tr>
                            <th>#</th>
                            <th>CPF</th>
                            <th>N° Benefício</th>
                            <th>Nome</th>
                            <th>Telefone(Celular)</th>
                            <th>Data Averbação</th>
                            <th>Data Cadastro</th>
                        </tr>
                    </thead>
                    <tbody>
                        {clientes.map((cliente) => (
                            <tr className='selecao' key={cliente.cliente_id}>
                                <td>{cliente.cliente_id}</td>
                                <td>{Mascara.cpf(cliente.cliente_cpf)}</td>
                                <td>{cliente.cliente_matriculaBeneficio}</td>
                                <td>{cliente.cliente_nome}</td>
                                <td>{Mascara.telefone(cliente.cliente_telefoneCelular)}</td>
                                <td>{Mascara.data(cliente.cliente_DataAverbacao)}</td>
                                <td>{Mascara.data(cliente.cliente_dataCadastro)}</td>
                            </tr>
                        ))}
                        {clientes.length === 0 && <tr><td colSpan="8"><span>Nenhum cliente foi encontrado...</span></td></tr>}
                    </tbody>
                </table>
                {this.state.show && <Paginacao
                    limit={limit}
                    setLimit={setLimit}
                    offset={offset}
                    total={qtdClientes}
                    setOffset={setOffset}
                    setCurrentPage={value => BuscarTodosClientes(value)}
                />}

            </div>
        );
    }
}

export default PreVisualizarRemessa;
