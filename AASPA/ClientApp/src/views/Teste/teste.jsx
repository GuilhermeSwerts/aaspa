import React, { useContext, useEffect, useState } from 'react';
import { AuthContext } from '../../context/AuthContext';
import { NavBar } from '../../components/Layout/layout';
import { FaDownload, FaPlus } from 'react-icons/fa6';
import { Col, Row } from 'reactstrap';
import BuscarPorMesAno from './BuscarPorMesAno';
import { api } from '../../api/api';

function Teste() {
    const { usuario, handdleUsuarioLogado } = useContext(AuthContext)
    const meses = [
        { valor: 1, nome: 'Janeiro' },
        { valor: 2, nome: 'Fevereiro' },
        { valor: 3, nome: 'Março' },
        { valor: 4, nome: 'Abril' },
        { valor: 5, nome: 'Maio' },
        { valor: 6, nome: 'Junho' },
        { valor: 7, nome: 'Julho' },
        { valor: 8, nome: 'Agosto' },
        { valor: 9, nome: 'Setembro' },
        { valor: 10, nome: 'Outubro' },
        { valor: 11, nome: 'Novembro' },
        { valor: 12, nome: 'Dezembro' },
    ];
    const [remessas, setRemessas] = useState([])
    const BuscarRemessas = () => {
        api.get("BuscarRemessas", res => {
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
        alert(RemessaId)
    }

    return (
        <NavBar usuario_tipo={usuario && usuario.usuario_tipo} usuario_nome={usuario && usuario.usuario_nome}>
            <small>Filtro:</small>
            <BuscarPorMesAno BuscarRemessas={BuscarRemessas} DownloadRemessa={DownloadRemessa} OnClick={() => { }} />
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
                        <td>{meses.filter(x => x.valor == remessa.mês)[0].nome}</td>
                        <td>{remessa.ano}</td>
                        <td>{remessa.dataCriacao}</td>
                        <td><button onClick={_ => DownloadRemessa(remessa.remessaId)} className='btn btn-info'><FaDownload size={20} color='#fff' /></button></td>
                    </tr>))}
                </tbody>
            </table>
        </NavBar>
    );
}

export default Teste;