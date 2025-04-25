import React from 'react';
import { AiOutlineRollback } from 'react-icons/ai';
import { Mascara } from '../../util/mascara';
import { FaPencil } from 'react-icons/fa6';
import { FaTrash } from 'react-icons/fa';

export default ({ historico, onClose, nome, editarAtendimento, deleteAtendimento }) => (<>
    <button type="button" onClick={onClose} className="btn btn-primary"><AiOutlineRollback size={25} />Voltar</button><br />
    <h1 style={{ textAlign: 'center' }}>Histórico de {nome}</h1>
    <table className='table table-striped'>
        <thead>
            <tr>
                <th>Origem</th>
                <th>Data / Hora da ocorrência</th>
                <th>Motivo do contato</th>
                <th>Situação da Ocorrência</th>
                <th>Telefone de contato</th>
                <th>Descrição da Ocorrência</th>
                <th>Editar</th>
                <th>Excluir</th>
            </tr>
        </thead>
        <tbody>
            {historico.map(hist => {
                return (
                    <tr className='selecao'>
                        <td>{hist.origem}</td>
                        <td>{Mascara.data(hist.data)}</td>
                        <td>{hist.motivo}</td>
                        <td>{hist.ocorrencia}</td>
                        <td>{Mascara.telefone(hist.telefone)}</td>
                        <td>{hist.descricao}</td>
                        <td><button onClick={e => editarAtendimento(hist.id)} className='btn btn-success'><FaPencil /></button></td>
                        <td><button onClick={e => deleteAtendimento(hist.id)} className='btn btn-danger'><FaTrash /></button></td>
                    </tr>
                )
            })}
        </tbody>
    </table>
</>)