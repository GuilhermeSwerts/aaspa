import React, { useContext, useEffect, useRef, useState } from 'react';
import { NavBar } from '../../../components/Layout/layout';
import { AuthContext } from '../../../context/AuthContext';
import { api } from '../../../api/api';
import { Mascara } from '../../../util/mascara';
import DescricaoModal from '../../../components/Modal/descricaoModal';
import { FaEye, FaPencil } from 'react-icons/fa6';
import NovoCaptador from '../../../components/Modal/novoCaptador';
import { FaPlus, FaTrash } from 'react-icons/fa';
import { Alert, Pergunta } from '../../../util/alertas';
import { Size } from '../../../util/size';

function Captador() {
    const { usuario, handdleUsuarioLogado } = useContext(AuthContext);
    const [captadores, setCaptadores] = useState([]);
    const [captadoresFiltro, setCaptadoresFiltro] = useState([]);
    const modal = useRef();
    const captadorModal = useRef();

    const BuscarTodosCaptadores = () => {
        api.get("BuscarCaptadores", res => {
            setCaptadores(res.data);
            setCaptadoresFiltro(res.data);
        }, er => {
            Alert("Houve um erro ao buscar os Captadores", false);
        })
    }

    useEffect(() => {
        handdleUsuarioLogado();
        BuscarTodosCaptadores();
    }, [])

    const AbrirDescricao = (desc) => {
        modal.current.AbrirModal(desc);
    }

    const onChangeFiltro = ({ target }) => {
        const { value } = target;
        if (value && value != "")
            setCaptadoresFiltro(captadores.filter(x => x.captador_nome.toUpperCase().includes(value.toUpperCase())));
        else
            setCaptadoresFiltro(captadores);
    }

    const ExcluirCaptador = async (id) => {
        if (await Pergunta("Deseja realmente excluir esse Captador?")) {
            api.delete(`ExcluirCaptador/${id}`, res => {
                BuscarTodosCaptadores();
                Alert(`Captador do id: ${id}" excluido com sucesso!`);
            }, err => {
                Alert("Houve um erro ao excluir o captador do id: " + id, false);
            })
        }
    }

    const AbrirModalCaptador = (CaptadorId, CaptadorNome, CaptadorDescricao, CaptadorCpfOrCnpj, Edit) => {
        captadorModal.current.AbrirModal(CaptadorId, CaptadorNome, CaptadorDescricao, CaptadorCpfOrCnpj, Edit)
    }

    return (
        <NavBar pagina_atual={'GERENCIAR CAPTADOR'} usuario_tipo={usuario && usuario.usuario_tipo} usuario_nome={usuario && usuario.usuario_nome}>
            <DescricaoModal ref={modal} />
            <NovoCaptador ref={captadorModal} BuscarTodosCaptadores={BuscarTodosCaptadores} />
            <div className='row'>
                <div className="col-md-10">
                    <span>Pesquisar pelo nome do Captador</span>
                    <input type="text"
                        onChange={onChangeFiltro}
                        className='form-control'
                        placeholder='Nome do beneficio' />
                </div>
                <div style={{ marginTop: '22px' }} className="col-md-2">
                    <button className='btn btn-primary' onClick={() => AbrirModalCaptador(0, '', '', '', false)}><FaPlus size={Size.IconeTabela}></FaPlus></button>
                </div>
            </div>
            <hr />
            <table className='table table-striped'>
                <thead>
                    <tr>
                        <th>Captador Id</th>
                        <th>CPF/CNPJ Captador</th>
                        <th>Nome do Captador</th>
                        <th>Ver descrição</th>
                        <th>Editar</th>
                        <th>Excluir</th>
                    </tr>
                </thead>
                <tbody>
                    {captadoresFiltro.map(item => (
                        <tr>
                            <td>{item.captador_id}</td>
                            <td>{item.captador_e_cnpj ? Mascara.cnpj(item.captador_cpf_cnpj) : Mascara.cpf(item.captador_cpf_cnpj)}</td>
                            <td>{item.captador_nome}</td>
                            <td><button className='btn btn-success' onClick={() => AbrirDescricao(item.captador_descricao)}><FaEye size={20} /></button></td>
                            <td>
                                <button className='btn btn-warning' onClick={() => AbrirModalCaptador(item.captador_id, item.captador_nome, item.captador_descricao, item.captador_cpf_cnpj, true)}><FaPencil size={20} /></button>
                            </td>
                            <td><button className='btn btn-danger' onClick={() => ExcluirCaptador(item.captador_id)}><FaTrash size={20} /></button></td>
                        </tr>
                    ))}
                </tbody>
            </table>
            {captadores.length == 0 && "Nenhum Captador encontrado..."}
        </NavBar>
    );
}

export default Captador;