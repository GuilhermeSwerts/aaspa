import { useEffect, useState } from "react";
import { Mascara } from "../../util/mascara";
import { FaBackspace } from "react-icons/fa";
import { AiOutlineRollback } from "react-icons/ai";

export default ({ id, errors, handleChange, handleSubmit, form, origens, motivos, situacaoOcorrencias, onClose, historico }) => {

    return (<form onSubmit={handleSubmit}>
        <button type="button" onClick={onClose} className="btn btn-primary"><AiOutlineRollback size={25} />Voltar</button><br />
        <small>Dados Gerais:</small>
        <div className="row">
            <div className="col-md-8">
                <label>Nome do não associado</label>
                <input
                    type="text"
                    name="nome"
                    className='form-control'
                    placeholder='Nome Completo...'
                    value={form.nome}
                    onChange={handleChange}
                />
                {errors.nome && <small className="text-danger">{errors.nome}</small>}
            </div>
            <div className="col-md-4">
                <label>Cpf do não associado</label>
                <input
                    type="text"
                    name="cpf"
                    className='form-control'
                    placeholder='xxx.xxx.xxx-xx'
                    value={Mascara.cpf(form.cpf)}
                    onChange={handleChange}
                />
                {errors.cpf && <small className="text-danger">{errors.cpf}</small>}
            </div>
        </div>
        <hr />
        <small>Dados Atendimento:</small>
        <div className="row">
            <div className="col-md-3">
                <label>Origem</label>
                <select name='origem' value={form.origem} onChange={handleChange} required className='form-control'>
                    <option value={''}>Selecione</option>
                    {origens.map(origem => (
                        <option value={origem.origem_id}>{origem.origem_nome}</option>
                    ))}
                </select>
                {errors.origem && <small className="text-danger">{errors.origem}</small>}
            </div>
            <div className="col-md-3">
                <label>Data / Hora da ocorrência</label>
                <input
                    type="datetime-local"
                    name="dataHora"
                    className='form-control'
                    value={form.dataHora}
                    onChange={handleChange}
                />
                {errors.dataHora && <small className="text-danger">{errors.dataHora}</small>}
            </div>
            <div className="col-md-3">
                <label>Motivo do contato</label>
                <select name='motivo' value={form.motivo} onChange={handleChange} required className='form-control'>
                    <option value={''}>Selecione</option>
                    {motivos.map(motivo_contato => (
                        <option value={motivo_contato.motivo_contato_id}>{motivo_contato.motivo_contato_nome}</option>
                    ))}
                </select>
                {errors.motivo && <small className="text-danger">{errors.motivo}</small>}
            </div>
            <div className="col-md-3">
                <label>Situação da Ocorrência</label>
                <select name='situacao' value={form.situacao} onChange={handleChange} required className='form-control'>
                    <option value={''}>Selecione</option>
                    {situacaoOcorrencias.map(item => (
                        <option value={item.id}>{item.nome}</option>
                    ))}
                </select>
                {errors.situacao && <small className="text-danger">{errors.situacao}</small>}
            </div>
        </div>
        <hr />
        <small>Dados Extras:</small>
        <div className="row">
            <div className="col-md-3">
                <label>Telefone de contato</label>
                <input
                    type="text"
                    name="telefone"
                    className='form-control'
                    placeholder='(xx) xxxxx-xxxx'
                    value={Mascara.telefone(form.telefone)}
                    onChange={handleChange}
                />
                {errors.telefone && <small className="text-danger">{errors.telefone}</small>}
            </div>
        </div>

        <hr />

        <div className='row'>
            <div className="col-md-12">
                <label>Descrição da Ocorrência</label>
                <textarea
                    name='descricao'
                    required
                    maxLength={1000}
                    className='form-control'
                    value={form.descricao}
                    onChange={handleChange}
                ></textarea>
                <small>{1000 - form.descricao.length}/1000</small>
                {errors.descricao && <small className="text-danger">{errors.descricao}</small>}
            </div>
        </div>
        <hr />
        <button type="submit" style={{ width: '100%' }} className="btn btn-primary">Adicionar Novo Contato</button>
    </form>)
}