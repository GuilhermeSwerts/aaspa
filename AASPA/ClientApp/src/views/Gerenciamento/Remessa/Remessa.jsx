import React, { useContext, useEffect, useState } from 'react';
import { AuthContext } from '../../../context/AuthContext';
import { NavBar } from '../../../components/Layout/layout';
import { FaDownload, FaPlus } from 'react-icons/fa6';
import { Col, Row } from 'reactstrap';
import BuscarPorMesAno from './BuscarPorMesAno';
import { api } from '../../../api/api';

function Remessa() {
    const { usuario, handdleUsuarioLogado } = useContext(AuthContext)
    const [mesSelecionado, setMesSelecionado] = useState(null);
    const [anoSelecionado, setAnoSelecionado] = useState(null);
    const [remessas, setRemessas] = useState([])
    const BuscarRemessas = () => {
        api.get(`BuscarRemessas?mes=${mesSelecionado}&ano=${anoSelecionado}`, res => {
            setRemessas(res.data);
        }, err => {
            alert(err.response.data)
        })
    }

    useEffect(() => {
        handdleUsuarioLogado();
        BuscarRemessas();
    }, [])

    const DownloadRemessa = (RemessaId) => {
        api.get(`DownloadRemessa/${RemessaId}`, res => {
            const { nomeArquivo, base64 } = res.data;
            let csvContent = atob(base64);
            var blob = new Blob([csvContent], { type: "data:application/octet-stream;base64" });
            var url = window.URL.createObjectURL(blob);
            var a = document.createElement("a");
            a.href = url
            a.download = nomeArquivo+'.txt';
            a.click();
        }, err => {
            alert(err.response.data)
        })
    }

    return (
        <NavBar usuario_tipo={usuario && usuario.usuario_tipo} usuario_nome={usuario && usuario.usuario_nome}>
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
                        <th>Data Criação</th>
                        <th>Download</th>
                    </tr>
                </thead>
                <tbody>
                    {remessas.map(remessa => (<tr>
                        <td>{remessa.remessaId}</td>
                        <td>{remessa.mes}</td>
                        <td>{remessa.ano}</td>
                        <td>{remessa.dataCriacao}</td>
                        <td><button onClick={_ => DownloadRemessa(remessa.remessaId)} className='btn btn-info'><FaDownload size={20} color='#fff' /></button></td>
                    </tr>))}
                </tbody>
            </table>
        </NavBar>
    );
}

export default Remessa;