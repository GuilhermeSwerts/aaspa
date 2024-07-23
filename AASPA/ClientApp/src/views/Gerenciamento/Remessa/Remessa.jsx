import React, { useContext, useEffect, useState } from 'react';
import { AuthContext } from '../../../context/AuthContext';
import { NavBar } from '../../../components/Layout/layout';
import { FaDownload, FaPlus } from 'react-icons/fa6';
import { Col, Row } from 'reactstrap';
import BuscarPorMesAno from './BuscarPorMesAno';
import { api } from '../../../api/api';
import { Alert } from '../../../util/alertas';
import axios from 'axios';
function Remessa() {
    const { usuario, handdleUsuarioLogado } = useContext(AuthContext)
    const [mesSelecionado, setMesSelecionado] = useState(null);
    const [anoSelecionado, setAnoSelecionado] = useState(null);
    const [remessas, setRemessas] = useState([])
    const BuscarRemessas = () => {
        api.get(`BuscarRemessas?mes=${mesSelecionado}&ano=${anoSelecionado}`, res => {
            setRemessas(res.data);
        }, err => {
            Alert(err.response.data, false)
        })
    }

    useEffect(() => {
        handdleUsuarioLogado();
        BuscarRemessas();
    }, [])

    const DownloadRemessa = async (RemessaId) => {
        try {
            const token = api.access_token;

            const response = await axios.get(`DownloadRemessa/${RemessaId}`, {
                responseType: 'blob',
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            const nomeArquivo = response.headers['x-file-name'];

            const url = window.URL.createObjectURL(new Blob([response.data]));
            const link = document.createElement('a');
            link.href = url;
            link.setAttribute('download', nomeArquivo);
            document.body.appendChild(link);
            link.click();

            link.parentNode.removeChild(link);
        } catch (error) {
            console.error('Erro ao baixar o arquivo:', error);
        }
    }

    return (
        <NavBar pagina_atual={'REMESSA'} usuario_tipo={usuario && usuario.usuario_tipo} usuario_nome={usuario && usuario.usuario_nome}>
            <small>Filtro:</small>
            <BuscarPorMesAno
                mesSelecionado={mesSelecionado}
                setMesSelecionado={setMesSelecionado}
                anoSelecionado={anoSelecionado}
                setAnoSelecionado={setAnoSelecionado}
                BuscarRemessas={BuscarRemessas}
                DownloadRemessa={DownloadRemessa}
                OnClick={BuscarRemessas} />
            <br />
            <br />
            <table className='table table-striped'>
                <thead>
                    <tr>
                        <th>Remessa ID</th>
                        <th>Mês</th>
                        <th>Ano</th>
                        <th>Período</th>
                        <th>Data Criação</th>
                        <th>Status</th>
                        <th>Download</th>
                    </tr>
                </thead>
                <tbody>
                    {remessas.map(remessa => (<tr>
                        <td>{remessa.remessaId}</td>
                        <td>{remessa.mes}</td>
                        <td>{remessa.ano}</td>
                        <td>{remessa.periodo}</td>
                        <td>{remessa.dataCriacao}</td>
                        <td>{remessa.remessa_status === true ? "Ativo" : "Inativo"}</td>
                        <td><button onClick={_ => DownloadRemessa(remessa.remessaId)} className='btn btn-info'><FaDownload size={20} color='#fff' /></button></td>
                    </tr>))}
                </tbody>
            </table>
        </NavBar>
    );
}

export default Remessa;