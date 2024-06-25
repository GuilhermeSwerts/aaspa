import React, { useContext, useEffect, useState } from 'react';
import { NavBar } from '../../../components/Layout/layout';
import { AuthContext } from '../../../context/AuthContext';
import { api } from '../../../api/api';
import ModalNovoMotivoDoContato from '../../../components/Modal/novoMotivoContato';
import ModalEditarMotivoDoContato from '../../../components/Modal/editarMotivoContato';
import { Alert } from '../../../util/alertas';

function MotivoContato() {
    const { usuario, handdleUsuarioLogado } = useContext(AuthContext);
    const [motivos, setMotivos] = useState([]);
    const [motivosFiltro, setMotivosFiltro] = useState([]);

    const buscarMotivos = () => {
        api.get("BuscarTodosMotivos", res => {
            setMotivos(res.data);
            setMotivosFiltro(res.data);
        }, err => {
            Alert('Houve um erro ao buscar os motivos de contato', false)
        })
    }
    useEffect(() => {
        handdleUsuarioLogado();
        buscarMotivos();
    }, []);

    const onChangeFiltro = ({ target }) => {
        const { value } = target;
        if (value && value != "")
            setMotivosFiltro(motivos.filter(x => x.motivo_contato_nome.toUpperCase().includes(value.toUpperCase())));
        else
            setMotivosFiltro(motivos);
    }

    return (
        <NavBar pagina_atual={'GERENCIAR MOTIVO CONTATO'} usuario_tipo={usuario && usuario.usuario_tipo} usuario_nome={usuario && usuario.usuario_nome}>
            <div className='row'>
                <div className="col-md-10">
                    <span>Pesquisar pelo Nome Do Motivo Do Contato</span>
                    <input type="text"
                        onChange={onChangeFiltro}
                        className='form-control'
                        placeholder='Nome Do Motivo Do Contato' />
                </div>
                <div style={{ marginTop: '22px' }} className="col-md-2">
                    <ModalNovoMotivoDoContato BuscarMotivos={buscarMotivos} />
                </div>
            </div>
            <hr />
            <table className='table table-striped'>
                <thead>
                    <tr>
                        <th>Motivo Do Contato Id</th>
                        <th>Nome Do Motivo Do Contato</th>
                    </tr>
                </thead>
                <tbody>
                    {motivosFiltro.map(x => (
                        <tr>
                            <td>{x.motivo_contato_id}</td>
                            <td>{x.motivo_contato_nome}</td>
                            <td>
                                <ModalEditarMotivoDoContato BuscarMotivos={buscarMotivos} MotivoId={x.motivo_contato_id} />
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </NavBar>
    );
}

export default MotivoContato;