import React, { useContext, useEffect, useRef, useState } from 'react';
import { NavBar } from '../../../components/Layout/layout';
import { AuthContext } from '../../../context/AuthContext';
import { api } from '../../../api/api';
import { Alert } from '../../../util/alertas';
import ModalSituacaoOcorrencia from '../../../components/Modal/SituacaoOcorrencia';
import { ButtonTooltip } from '../../../components/Inputs/ButtonTooltip';
import { FaPlus } from 'react-icons/fa';
import { Size } from '../../../util/size';
import { FaPencil } from 'react-icons/fa6';

function SituacaoOcorrencia() {
    const { usuario, handdleUsuarioLogado } = useContext(AuthContext);
    const modalSituacaoOcorrencia = useRef();
    const [situacaoOcorrencias, setSituacaoOcorrencias] = useState([]);
    const [busca, setBusca] = useState('');

    const BuscarSituacaoOcorrencias = () => {
        api.get("api/SituacaoOcorrencia", res => {
            setSituacaoOcorrencias(res.data);
        }, err => {
            Alert('Houve um erro ao buscar os motivos de contato', false)
        })
    }
    useEffect(() => {
        handdleUsuarioLogado();
        BuscarSituacaoOcorrencias();
    }, []);

    return (
        <NavBar pagina_atual={'GERENCIAR SITUACAO OCORRENCIA'} usuario_tipo={usuario && usuario.usuario_tipo} usuario_nome={usuario && usuario.usuario_nome}>
            <ModalSituacaoOcorrencia
                ref={modalSituacaoOcorrencia}
                BuscarSituacaoOcorrencias={BuscarSituacaoOcorrencias}
            />
            <div className='row'>
                <div className="col-md-10">
                    <span>Pesquisar pelo Nome Do Situacao Ocorrencia</span>
                    <input type="text"
                        onChange={e => setBusca(e.target.value)}
                        className='form-control'
                        placeholder='Nome Do Situacao Ocorrencia' />
                </div>
                <div style={{ marginTop: '22px' }} className="col-md-2">
                    <ButtonTooltip
                        onClick={() => modalSituacaoOcorrencia.current.NovaSituacaoOcorrencia()}
                        className='btn btn-success'
                        text={'Adicionar Situacao Ocorrencia'}
                        top={true}
                        textButton={<FaPlus size={Size.IconeTabela} />}
                    />
                </div>
            </div>
            <hr />
            <table className='table table-striped'>
                <thead>
                    <tr>
                        <th>Situacao Ocorrencia Id</th>
                        <th>Nome Do Situacao Ocorrencia</th>
                        <th>Editar</th>
                    </tr>
                </thead>
                <tbody>
                    {situacaoOcorrencias.filter(x => x.nome.toUpperCase().includes(busca.toUpperCase())).map(x => (
                        <tr>
                            <td>{x.id}</td>
                            <td>{x.nome}</td>
                            <td>
                                <ButtonTooltip
                                    onClick={() => modalSituacaoOcorrencia.current.EditaSituacaoOcorrencia(x.id, x.nome)}
                                    className='btn btn-success'
                                    text={'Adicionar Situacao Ocorrencia'}
                                    top={true}
                                    textButton={<FaPencil size={Size.IconeTabela} />}
                                />
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </NavBar>
    );
}

export default SituacaoOcorrencia;