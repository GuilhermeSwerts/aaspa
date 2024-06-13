import React, { useContext, useEffect, useState } from 'react';
import { api } from '../../../api/api';
import ModalEditarStatus from '../../../components/Modal/editarStatus';
import { NavBar } from '../../../components/Layout/layout';
import { AuthContext } from '../../../context/AuthContext';
import ModalStatus from '../../../components/Modal/novoStatus';

function Status() {
    const { usuario, handdleUsuarioLogado } = useContext(AuthContext);
    const [status, setStatus] = useState([]);
    const [statusFiltro, setStatusFiltro] = useState([]);
    const BuscarTodosStatus = () => {
        api.get("TodosStatus", res => {
            setStatus(res.data);
            setStatusFiltro(res.data);
        }, err => {
            alert("Houve um erro ao buscar os status.")
        })
    }

    useEffect(() => {
        handdleUsuarioLogado();
        BuscarTodosStatus();
    }, [])


    const onChangeFiltro = ({ target }) => {
        const { value } = target;
        if (value && value != "")
            setStatusFiltro(status.filter(x => x.status_nome.toUpperCase().includes(value.toUpperCase())));
        else
            setStatusFiltro(status);
    }

    return (
        <NavBar usuario_tipo={usuario && usuario.usuario_tipo} usuario_nome={usuario && usuario.usuario_nome}>
            <div className='row'>
                <div className="col-md-10">
                    <span>Pesquisar pelo nome do status</span>
                    <input type="text"
                        onChange={onChangeFiltro}
                        className='form-control'
                        placeholder='Nome do status' />
                </div>
                <div style={{ marginTop: '22px' }} className="col-md-2">
                    <ModalStatus BuscarTodosStatus={BuscarTodosStatus} />
                </div>
            </div>
            <hr />
            <table className='table table-striped'>
                <thead>
                    <tr>
                        <th>Status Id</th>
                        <th>Nome do Status</th>
                        <th>Editar</th>
                    </tr>
                </thead>
                <tbody>
                    {statusFiltro.map(item => (
                        <tr>
                            <td>{item.status_id}</td>
                            <td>{item.status_nome}</td>
                            <td>{item.status_id != 1 && item.status_id != 2 && item.status_id != 3 && <ModalEditarStatus BuscarTodosStatus={BuscarTodosStatus} StatusId={item.status_id} />}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
            {status.length == 0 && "Nenhum status encontrado..."}
        </NavBar>
    );
}

export default Status;