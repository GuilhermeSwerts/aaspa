import React, { useEffect, useState } from 'react';
import { FaKey, FaPlus, FaUserPen } from 'react-icons/fa6';
import { api } from '../../api/api';
import { FaUserTimes } from 'react-icons/fa';
import { Alert, Pergunta } from '../../util/alertas';
import { NavBar } from '../../components/Layout/layout';

function Usuarios() {
    const [usuarios, setUsuarios] = useState([]);
    const [usuariosFiltro, setUsuariosFiltro] = useState([]);
    const [filtroNome, setFiltroNome] = useState();
    const [editingIndex, setEditingIndex] = useState(null);
    const [novoUsuario, setNovoUsuario] = useState(false);

    const BuscarTodosUsuarios = () => {
        api.get("BuscarTodosUsuarios", res => {
            setUsuarios(res.data);
            setUsuariosFiltro(res.data);
        }, err => {
            Alert("Houve um erro ao buscar os usuários", false)
        })
    }

    useEffect(() => {
        //if (!usuarios.IsMaster)
        //    window.location.href = "/";

        BuscarTodosUsuarios();
        //BuscarTiposUsuario();
    }, [])

    const FiltroNome = e => {
        setFiltroNome(e.target.value);
        setUsuariosFiltro(usuarios.filter(usuario => usuario.nome.toUpperCase().includes(e.target.value.toUpperCase())))
    }

    const handleEditClick = (index) => {
        setEditingIndex(index);
    };

    const handleSaveClick = async (index) => {

        const nome = document.getElementById('nome' + index);
        if (!nome || nome.value === "") {
            Alert("Digite um nome", false, true);
            return;
        }
        const tipo = document.getElementById('tipo' + index);
        if (!tipo || tipo.value === "" || tipo.value === 0) {
            Alert("Selecione um tipo de usuario", false, true);
            return;
        }
        const usu = document.getElementById('usuario' + index);
        if (!usu || usu.value === "") {
            Alert("Digite um nome de usuário", false, true);
            return;
        }

        const form = {
            nome: nome.value,
            tipo: tipo.value,
            usuario: usu.value,
            id: usuarios[index].usuarioId,
        }

        if (await Pergunta("Deseja realmente salvar as informações?")) {
            if (novoUsuario) {
                api.post("Usuario", form, res => {
                    setEditingIndex(null);
                    setNovoUsuario(false);
                    BuscarTodosUsuarios();
                    Alert("Usuário cadastrado com sucesso!")
                }, err => {
                    Alert(err.data && err.data.response ? err.data.response : "Houve um ao cadastrar um novo usuário.", false)
                })
            } else {
                api.put("Usuario/AtualizarUsuario", form, res => {
                    setEditingIndex(null);
                    setNovoUsuario(false);
                    BuscarTodosUsuarios();
                    Alert("Usuário atualizado com sucesso!")
                }, err => {
                    Alert("Houve um erro ao atualizar o usuário.", false)
                })
            }
        }
    };

    const AlterarStatusCliente = async (usuarioId, nome) => {
        if (await Pergunta(`Deseja realmente excluir o usuário de ${nome}?`)) {
            api.put(`ExcluirUsuario/${usuarioId}`, {}, res => {
                BuscarTodosUsuarios();
                Alert(`Usuário de ${nome} foi excluido com sucesso!`)
            }, err => {
                Alert("Houve um erro ao atualizar o status do usuário.", false)
            })
        }
    }

    const ResetarSenhaUsuario = async (usuarioId, usuarioNome) => {
        if (await Pergunta("Deseja realmente resetar a senha de " + usuarioNome)) {
            api.put(`Usuario/ResetaSenha/${usuarioId}`, {}, res => {
                Alert(`Senha de ${usuarioNome} resetada para P@drao123`);
            }, erro => {
                Alert("Houve um erro ao resetar a senha do usuário.", false)
            })
        }
    }

    const NovoUsuario = async () => {
        let dataNow = ''
        let dataHoje = new Date()
        let mes = `${(dataHoje.getMonth() + 1)}`;
        let dia = `${dataHoje.getDay()}`
        dataNow = `${dataHoje.getFullYear()}-${mes.padStart(2, "0")}-${dia.padStart(2, "0")}T${dataHoje.getHours()}:${dataHoje.getMinutes()}:${dataHoje.getSeconds()}`
        const form = {
            usuarioId: 0,
            nome: '',
            tipoUsuario: 1,
            usuario: '',
            dataCadastro: dataNow
        }

        let copy = [...usuariosFiltro];
        copy.push(form);
        copy.reverse();
        setUsuariosFiltro(copy);
        setEditingIndex(0);
        setNovoUsuario(true);
    }

    return (
        <NavBar>
            <div className="container-table">
                <div style={{
                    padding: '1rem',
                    background: 'var(--cor-principal)',
                    color: '#fff',
                    width: '100%',
                    margin: '0'
                }}
                    className='row'
                >
                    <div style={{ display: 'flex', justifyContent: 'end', marginRight: 20 }}>
                        <button
                            className="btn btn-success"
                            onClick={NovoUsuario}
                        >
                            Novo do usuário <FaPlus size={20} color='#fff' />
                        </button>
                    </div>
                    <div class="col-md-12" style={{ marginBottom: '10px' }}>
                        <label for="validationTooltipUsername">Filtros:</label>
                        {<div class="input-group">
                            <input
                                type="text"
                                placeholder="Nome do produto"
                                value={filtroNome}
                                className='form-control'
                                onChange={FiltroNome}
                            />
                        </div>}
                    </div>
                </div>
                <br />
                <div className='row'>
                    <div className="col-md-12">
                        <table className="table table-striped">
                            <thead>
                                <tr>
                                    <th>#</th>
                                    <th>Nome</th>
                                    <th>Nome Usuário</th>
                                    <th>Tipo Usuário</th>
                                    <th>Data Cadastro</th>
                                    <th>Editar</th>
                                    <th>Resetar Senha</th>
                                    <th>Excluir</th>
                                </tr>
                            </thead>
                            <tbody>
                                {usuariosFiltro.map((usuario, i) => {
                                    const date = new Date(usuario.dataCadastro);
                                    const day = String(date.getDate()).padStart(2, '0');
                                    const month = String(date.getMonth() + 1).padStart(2, '0');
                                    const year = date.getFullYear();
                                    const hours = String(date.getHours()).padStart(2, '0');
                                    const minutes = String(date.getMinutes()).padStart(2, '0');
                                    const seconds = String(date.getSeconds()).padStart(2, '0');
                                    const formattedDateTime = `${day}/${month}/${year} ${hours}:${minutes}:${seconds}`;

                                    const isEditing = editingIndex === i;
                                    return (
                                        <tr className="selecao" key={usuario.usuarioId}>
                                            <td >{usuario.usuarioId}</td>
                                            <td >
                                                {isEditing ? (
                                                    <>
                                                        <label>Nome:</label>
                                                        <input required type="text" id={'nome' + i} className='form-control' defaultValue={usuario.nome} />
                                                    </>
                                                ) : (
                                                    usuario.nome
                                                )}
                                            </td>
                                            <td >
                                                {isEditing ? (
                                                    <>
                                                        <label>Nome Usuário:</label>
                                                        <input required type="text" id={'usuario' + i} className='form-control' defaultValue={usuario.usuario} />
                                                    </>
                                                ) : (
                                                    usuario.usuario
                                                )}
                                            </td>
                                            <td >
                                                {isEditing ? (
                                                    <>
                                                        <label>Tipo Usuário:</label>
                                                        <select id={'tipo' + i} required className="form-control" defaultValue={usuario.tipoUsuario}>
                                                            <option value={1}>MASTER</option>
                                                            <option value={2}>OPERADOR</option>
                                                        </select>
                                                    </>
                                                ) : (
                                                    usuario.tipoUsuario === 1 ? "MASTER" : "OPERADOR"
                                                )}
                                            </td>
                                            <td data-label="Data Cadastro">
                                                {formattedDateTime}
                                            </td>
                                            <td data-label="Editar">
                                                {isEditing ? (
                                                    <div style={{ display: 'flex', gap: 5 }}>
                                                        <button className='btn btn-primary' onClick={() => handleSaveClick(i)}>Salvar</button>
                                                        <button className='btn btn-danger' onClick={() => { setEditingIndex(null); setUsuariosFiltro(usuariosFiltro.filter(x => x.usuarioId !== 0)); setNovoUsuario(false) }}>Cancelar</button>
                                                    </div>
                                                ) : (
                                                    <button className='btn btn-warning' onClick={() => handleEditClick(i)}><FaUserPen color='#fff' /></button>
                                                )}
                                            </td>
                                            {!isEditing && <td data-label="Resetar Senha"><button className='btn btn-success' onClick={() => ResetarSenhaUsuario(usuario.usuarioId, usuario.nome)}><FaKey /></button></td>}
                                            {!isEditing && <td data-label="Excluir"><button className='btn btn-danger' onClick={() => AlterarStatusCliente(usuario.usuarioId, usuario.nome)}><FaUserTimes /></button></td>}
                                            {isEditing && <td colSpan={2}></td>}
                                        </tr>
                                    );
                                })}
                                {usuariosFiltro.length === 0 && (
                                    <tr>
                                        <td colSpan={7}><span>Não foi encontrado nenhum usuário...</span></td>
                                    </tr>
                                )}
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </NavBar>
    );
}

export default Usuarios;