import React, { useContext, useEffect, useState } from 'react';
import { NavBar } from '../../../components/Layout/layout';
import { AuthContext } from '../../../context/AuthContext';
import { api } from '../../../api/api';
import ModalNovaOrigem from '../../../components/Modal/novaOrigem';
import ModalEditarOrigem from '../../../components/Modal/editarOrigem';
import { Alert } from '../../../util/alertas';
// import ModalNovoMotivoDoContato from '../../../components/Modal/novoOrigem';
// import ModalEditarMotivoDoContato from '../../../components/Modal/editarOrigem';

function Origem() {
    const { usuario, handdleUsuarioLogado } = useContext(AuthContext);
    const [origem, setOrigem] = useState([]);
    const [origemFiltro, setOrigemFiltro] = useState([]);

    const buscarOrigem = () => {
        api.get("BuscarTodasOrigem", res => {
            setOrigem(res.data);
            setOrigemFiltro(res.data);
        }, err => {
            Alert('Houve um erro ao buscar os motivos de contato', false)
        })
    }

    useEffect(() => {
        handdleUsuarioLogado();
        buscarOrigem();
    }, []);

    const onChangeFiltro = ({ target }) => {
        const { value } = target;
        if (value && value != "")
            setOrigemFiltro(origem.filter(x => x.origem_nome.toUpperCase().includes(value.toUpperCase())));
        else
            setOrigemFiltro(origem);
    }

    return (
        <NavBar pagina_atual={'GERENCIAR ORIGEM'} usuario_tipo={usuario && usuario.usuario_tipo} usuario_nome={usuario && usuario.usuario_nome}>
            <div className='row'>
                <div className="col-md-10">
                    <span>Pesquisar pelo Nome Da Origem</span>
                    <input type="text"
                        onChange={onChangeFiltro}
                        className='form-control'
                        placeholder='Nome Da Origem' />
                </div>
                <div style={{ marginTop: '22px' }} className="col-md-2">
                    <ModalNovaOrigem BuscarOrigem={buscarOrigem} />
                </div>
            </div>
            <hr />
            <table className='table table-striped'>
                <thead>
                    <tr>
                        <th>Origem Id</th>
                        <th>Nome Da Origem</th>
                    </tr>
                </thead>
                <tbody>
                    {origemFiltro.map(x => (
                        <tr>
                            <td>{x.origem_id}</td>
                            <td>{x.origem_nome}</td>
                            <td>
                                <ModalEditarOrigem BuscarOrigem={buscarOrigem} OrigemId={x.origem_id} />
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </NavBar>
    );
}

export default Origem;