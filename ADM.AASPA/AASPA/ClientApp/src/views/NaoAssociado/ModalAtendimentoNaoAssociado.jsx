import React, { Component } from 'react';
import { Modal } from 'react-bootstrap';
import { Mascara } from '../../util/mascara';
import { api } from '../../api/api';
import { Alert } from '../../util/alertas';

class ModalAtendimentoNaoAssociado extends Component {
    constructor(props) {
        super(props);
        this.state = this.initialState;
        this.Open = (id) => {
            api.get(`/api/NaoAssociados/${id}`, ({ data }) => {
                this.setState({
                    form: {
                        nome: data.nome_nao_associados,
                        cpf: data.cpf_nao_associados,
                        origem: data.origem_id,
                        dataHora: data.data_ocorrencia,
                        motivo: data.motivo_contato_id,
                        situacao: data.situacao_ocorrencia_id,
                        telefone: data.telefone,
                        descricao: data.descricao_nao_associado
                    },
                    show: true,
                    id: id
                })
            })
        }
    }

    initialState = {
        id: 0,
        show: false,
        form: {
            nome: '',
            cpf: '',
            origem: '',
            dataHora: '',
            motivo: '',
            situacao: '',
            telefone: '',
            descricao: ''
        },
        errors: {}
    }

    handleChange = (e) => {
        const { name, value } = e.target;
        this.setState(prevState => ({
            form: {
                ...prevState.form,
                [name]: value
            }
        }));
    };

    validate = () => {
        const { form } = this.state;
        const newErrors = {};
        if (!form.nome) newErrors.nome = 'Nome é obrigatório';
        if (!form.cpf) newErrors.cpf = 'CPF é obrigatório';
        if (!form.origem) newErrors.origem = 'Origem é obrigatória';
        if (!form.dataHora) newErrors.dataHora = 'Data/Hora é obrigatória';
        if (!form.motivo) newErrors.motivo = 'Motivo é obrigatório';
        if (!form.situacao) newErrors.situacao = 'Situação é obrigatória';
        if (!form.telefone) newErrors.telefone = 'Telefone é obrigatório';
        if (!form.descricao) newErrors.descricao = 'Descrição é obrigatória';
        this.setState({ errors: newErrors });
        return Object.keys(newErrors).length === 0;
    };

    handleSubmit = (e) => {
        e.preventDefault();
        if (this.validate()) {
            api.put("api/NaoAssociados/" + this.state.id, this.state.form, res => {
                this.props.BuscarTodosNaoAssociados?.();
                this.props.onClose?.();
                Alert("Atendimento atualizado com sucesso!");
            }, err => {
                console.error(err);
                Alert("Erro ao registrar atendimento.", false);
            });
        }
    };

    onClose = () => {
        this.setState(this.initialState);
    }

    render() {
        const { form, errors, show } = this.state;
        const { origens = [], motivos = [], situacaoOcorrencias = [] } = this.props;

        return (
            <Modal show={show}>
                <Modal.Header closeButton>
                    <Modal.Title>Editar Atendimento</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <small>Dados Gerais:</small>
                    <label>Nome do não associado</label>
                    <input
                        type="text"
                        name="nome"
                        className='form-control'
                        placeholder='Nome Completo...'
                        value={form.nome}
                        onChange={this.handleChange}
                    />
                    {errors.nome && <small className="text-danger">{errors.nome}</small>}

                    <label>Cpf do não associado</label>
                    <input
                        type="text"
                        name="cpf"
                        className='form-control'
                        placeholder='xxx.xxx.xxx-xx'
                        value={Mascara.cpf(form.cpf)}
                        onChange={this.handleChange}
                    />
                    {errors.cpf && <small className="text-danger">{errors.cpf}</small>}

                    <label>Origem</label>
                    <select name='origem' value={form.origem} onChange={this.handleChange} className='form-control'>
                        <option value=''>Selecione</option>
                        {origens.map(origem => (
                            <option key={origem.origem_id} value={origem.origem_id}>
                                {origem.origem_nome}
                            </option>
                        ))}
                    </select>
                    {errors.origem && <small className="text-danger">{errors.origem}</small>}

                    <label>Data / Hora da ocorrência</label>
                    <input
                        type="datetime-local"
                        name="dataHora"
                        className='form-control'
                        value={form.dataHora}
                        onChange={this.handleChange}
                    />
                    {errors.dataHora && <small className="text-danger">{errors.dataHora}</small>}

                    <label>Motivo do contato</label>
                    <select name='motivo' value={form.motivo} onChange={this.handleChange} className='form-control'>
                        <option value=''>Selecione</option>
                        {motivos.map(motivo => (
                            <option key={motivo.motivo_contato_id} value={motivo.motivo_contato_id}>
                                {motivo.motivo_contato_nome}
                            </option>
                        ))}
                    </select>
                    {errors.motivo && <small className="text-danger">{errors.motivo}</small>}

                    <label>Situação da Ocorrência</label>
                    <select name='situacao' value={form.situacao} onChange={this.handleChange} className='form-control'>
                        <option value=''>Selecione</option>
                        {situacaoOcorrencias.map(item => (
                            <option key={item.id} value={item.id}>
                                {item.nome}
                            </option>
                        ))}
                    </select>
                    {errors.situacao && <small className="text-danger">{errors.situacao}</small>}

                    <label>Telefone de contato</label>
                    <input
                        type="text"
                        name="telefone"
                        className='form-control'
                        placeholder='(xx) xxxxx-xxxx'
                        value={Mascara.telefone(form.telefone)}
                        onChange={this.handleChange}
                    />
                    {errors.telefone && <small className="text-danger">{errors.telefone}</small>}

                    <label>Descrição da Ocorrência</label>
                    <textarea
                        name='descricao'
                        maxLength={1000}
                        className='form-control'
                        value={form.descricao}
                        onChange={this.handleChange}
                    ></textarea>
                    <small>{1000 - form.descricao.length}/1000</small>
                    {errors.descricao && <small className="text-danger">{errors.descricao}</small>}
                </Modal.Body>
                <Modal.Footer>
                    <button onClick={this.onClose} className='btn btn-info'>Cancelar</button>
                    <button onClick={e=> {this.handleSubmit();this.onClose()}} className='btn btn-primary'>Salvar</button>
                </Modal.Footer>
            </Modal>
        );
    }
}

export default ModalAtendimentoNaoAssociado;
