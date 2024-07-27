import React, { Component } from 'react';
import { api } from '../../api/api';
import { Mascara } from '../../util/mascara';
import { TbZoomMoney } from 'react-icons/tb';
import { RiChatHistoryLine } from 'react-icons/ri';
import { FaDownload, FaUserEdit } from 'react-icons/fa';

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
            dateFim: ''
        };
        this.Show = (dateInit, dateFim, anoSelecionado, mesSelecionado) => {
            api.get(`BuscarTodosClientes?dateInitAverbacao=${dateInit}&dateEndAverbacao=${dateFim}`, res => {
                this.setState({ clientes: res.data.clientes, qtdClientes: res.data.totalClientes });
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
        const { ano, mes, clientes, currentPage, clientesPerPage, qtdClientes } = this.state;

        // Calcular índice dos itens a serem exibidos na página atual
        const indexOfLastClient = currentPage * clientesPerPage;
        const indexOfFirstClient = indexOfLastClient - clientesPerPage;
        const currentClients = clientes.slice(indexOfFirstClient, indexOfLastClient);

        // Calcular o número total de páginas
        const totalPages = Math.ceil(clientes.length / clientesPerPage);

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
                            <th>Status Atual</th>
                        </tr>
                    </thead>
                    <tbody>
                        {currentClients.map((cliente) => (
                            <tr className='selecao' key={cliente.cliente.cliente_id}>
                                <td>{cliente.cliente.cliente_id}</td>
                                <td>{Mascara.cpf(cliente.cliente.cliente_cpf)}</td>
                                <td>{cliente.cliente.cliente_matriculaBeneficio}</td>
                                <td>{cliente.cliente.cliente_nome}</td>
                                <td>{Mascara.telefone(cliente.cliente.cliente_telefoneCelular)}</td>
                                <td>{Mascara.data(cliente.cliente.cliente_DataAverbacao)}</td>
                                <td>{Mascara.data(cliente.cliente.cliente_dataCadastro)}</td>
                                <td>{cliente.statusAtual.status_nome}</td>
                            </tr>
                        ))}
                        {clientes.length === 0 && <tr><td colSpan="8"><span>Nenhum cliente foi encontrado...</span></td></tr>}
                    </tbody>
                </table>
                <div className="pagination" style={{ display: 'flex', justifyContent: 'center', alignItems: "center", gap: 5 }}>
                    <button
                        className='btn btn-primary'
                        onClick={() => this.handlePageChange(currentPage - 1)}
                        disabled={currentPage === 1}
                    >
                        Anterior
                    </button>
                    <span>{currentPage} de {totalPages > 0 ? totalPages : 1}</span>
                    <button
                        className='btn btn-primary'
                        onClick={() => this.handlePageChange(currentPage + 1)}
                        disabled={currentPage === totalPages}
                    >
                        Próxima
                    </button>
                </div>

            </div>
        );
    }
}

export default PreVisualizarRemessa;
