import React, { useContext, useEffect, useRef, useState } from 'react';
import { NavBar } from '../../../components/Layout/layout';
import { AuthContext } from '../../../context/AuthContext';
import ModalNovoBeneficio from '../../../components/Modal/novoBeneficio';
import { api } from '../../../api/api';
import { Mascara } from '../../../util/mascara';
import DescricaoModal from '../../../components/Modal/descricaoModal';
import { FaEye } from 'react-icons/fa6';
import ModalEditarBeneficio from '../../../components/Modal/editarBeneficio';
import { Alert } from '../../../util/alertas';


function GBeneficios() {
    const { usuario, handdleUsuarioLogado } = useContext(AuthContext);
    const [beneficios, setBeneficios] = useState([]);
    const [beneficiosFiltro, setBeneficiosFiltro] = useState([]);
    const modal = useRef();

    const BuscarTodosBeneficios = () => {
        api.get("BuscarTodosBeneficios", res => {
            setBeneficios(res.data);
            setBeneficiosFiltro(res.data);
        }, er => {
            Alert("Houve um erro ao buscar os beneficios", false);
        })
    }

    useEffect(() => {
        handdleUsuarioLogado();
        BuscarTodosBeneficios();
    }, [])

    const AbrirDescricao = (desc) => {
        modal.current.AbrirModal(desc);
    }

    const onChangeFiltro = ({ target }) => {
        const { value } = target;
        if (value && value != "")
            setBeneficiosFiltro(beneficios.filter(x => x.beneficio_nome_beneficio.toUpperCase().includes(value.toUpperCase())));
        else
            setBeneficiosFiltro(beneficios);
    }

    return (
        <NavBar pagina_atual={'GERENCIAR BENEFICIOS'} usuario_tipo={usuario && usuario.usuario_tipo} usuario_nome={usuario && usuario.usuario_nome}>
            <DescricaoModal ref={modal} />
            <div className='row'>
                <div className="col-md-10">
                    <span>Pesquisar pelo nome do beneficio</span>
                    <input type="text"
                        onChange={onChangeFiltro}
                        className='form-control'
                        placeholder='Nome do beneficio' />
                </div>
                <div style={{ marginTop: '22px' }} className="col-md-2">
                    <ModalNovoBeneficio />
                </div>
            </div>
            <hr />
            <table className='table table-striped'>
                <thead>
                    <tr>
                        <th>Beneficio Id</th>
                        <th>Cod Beneficio</th>
                        <th>Nome do Beneficio</th>
                        <th>Fornecedor Beneficio</th>
                        <th>Data Cadastro Beneficio</th>
                        <th>Valor a pagar ao fornecedor</th>
                        <th>Ver descrição</th>
                        <th>Editar</th>
                    </tr>
                </thead>
                <tbody>
                    {beneficiosFiltro.map(item => (
                        <tr>
                            <td>{item.beneficio_id}</td>
                            <td>{item.beneficio_cod_beneficio}</td>
                            <td>{item.beneficio_nome_beneficio}</td>
                            <td>{item.beneficio_fornecedor_beneficio}</td>
                            <td>{item.beneficio_dt_beneficio}</td>
                            <td>R$ {(item.beneficio_valor_a_pagar_ao_fornecedor + "").replace(".", ",")}</td>
                            <td><button className='btn btn-danger' onClick={() => AbrirDescricao(item.beneficio_descricao_beneficios)}><FaEye size={20} /></button></td>
                            <td><ModalEditarBeneficio BeneficioId={item.beneficio_id} BuscarTodosBeneficios={BuscarTodosBeneficios} /></td>
                        </tr>
                    ))}
                </tbody>
            </table>
            {beneficios.length == 0 && "Nenhum beneficio encontrado..."}
        </NavBar>
    );
}

export default GBeneficios;