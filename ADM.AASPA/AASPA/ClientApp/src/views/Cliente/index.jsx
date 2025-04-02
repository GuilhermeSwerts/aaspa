import React, { useState, useContext, useEffect } from 'react';
import { NavBar } from '../../components/Layout/layout';
import { AuthContext } from '../../context/AuthContext';
import { Mascara } from '../../util/mascara';
import ClienteForm from "./clienteForm";
import { GetParametro } from '../../util/parametro';
import { api } from '../../api/api';
import axios from 'axios';
import { Alert } from '../../util/alertas';

function Cliente() {
    const { usuario, handdleUsuarioLogado } = useContext(AuthContext);
    const [clienteId, setClienteId] = useState(0);
    const initState = {
        cpf: '',
        nome: '',
        cep: '',
        logradouro: '',
        bairro: '',
        localidade: '',
        uf: '',
        numero: '',
        complemento: '',
        dataNasc: '',
        nrDocto: '',
        empregador: '',
        matriculaBeneficio: '',
        nomeMae: '',
        nomePai: '',
        telefoneFixo: '',
        telefoneCelular: '',
        possuiWhatsapp: false,
        funcaoAASPA: 'Associado',
        email: '',
        sexo: 0,
        estadoCivil: 1,
        dataCad: '',
        remessaId: ''
    };
    const initStateCaptador = {
        cpfOuCnpj: '',
        nome: '',
        descricao: ''
    }
    const [cliente, setCliente] = useState(initState);
    const [captador, setCaptador] = useState(initStateCaptador);

    const [captadores, setCaptadores] = useState([]);
    const [captadorSelecionado, setcaptadorSelecionado] = useState(0);
    const BuscarTodosCaptadores = () => {
        api.get("BuscarCaptadores", res => {
            setCaptadores(res.data);
        }, er => {
            Alert("Houve um erro ao buscar os captadores", false);
        })
    }

    const BuscarClienteId = (id) => {
        api.get(`BuscarClienteID/${id}`, res => {
            const clt = res.data.cliente;
            const cpt = res.data.captador;

            const dtNasc = clt.cliente_dataNasc.replace("T00:00:00", "");
            setCliente({
                cpf: clt.cliente_cpf,
                nome: clt.cliente_nome,
                cep: clt.cliente_cep,
                logradouro: clt.cliente_logradouro,
                bairro: clt.cliente_bairro,
                localidade: clt.cliente_localidade,
                uf: clt.cliente_uf,
                numero: clt.cliente_numero,
                complemento: clt.cliente_complemento,
                dataNasc: dtNasc,
                nrDocto: clt.cliente_nrDocto,
                empregador: clt.cliente_empregador,
                matriculaBeneficio: clt.cliente_matriculaBeneficio,
                nomeMae: clt.cliente_nomeMae,
                nomePai: clt.cliente_nomePai,
                telefoneFixo: clt.cliente_telefoneFixo,
                telefoneCelular: clt.cliente_telefoneCelular,
                possuiWhatsapp: clt.cliente_possuiWhatsapp,
                funcaoAASPA: clt.cliente_funcaoAASPA,
                email: clt.cliente_email,
                estadoCivil: clt.cliente_estado_civil,
                sexo: clt.cliente_sexo,
                dataCad: clt.cliente_dataCadastro.split('T')[0],
                remessaId: clt.cliente_remessa_id && clt.cliente_remessa_id > 0 ? clt.cliente_remessa_id : ''
            })
            setCaptador({
                cpfOuCnpj: cpt.captador_cpf_cnpj,
                nome: cpt.captador_nome,
                descricao: cpt.captador_descricao
            })
            setcaptadorSelecionado(cpt.captador_id);
        }, erro => {
            Alert('Houve um erro ao buscar o cliente.', false)
        })
    }

    useEffect(() => {
        handdleUsuarioLogado();
        BuscarTodosCaptadores();

        var id = GetParametro("clienteId");
        if (id) {
            setClienteId(id);
            BuscarClienteId(id);
        }
    }, [])

    const handleChange = (e) => {
        const { name, value, type, checked } = e.target;
        const newValue = type === 'checkbox' ? checked : value;
        setCliente({ ...cliente, [name]: newValue });
    };

    const handleChangeCaptador = (e) => {
        const { name, value, type, checked } = e.target;
        const newValue = type === 'checkbox' ? checked : value;
        setCaptador({ ...captador, [name]: newValue });
    };

    const getLogadouro = (event) => {
        let cep = event.target.value.replace('.', '').replace('.', '').replace('-', '');
        if (cep.length < 8) return;

        const url = `https://viacep.com.br/ws/${cep}/json/`;
        fetch(url)
            .then(response => {
                return response.json();
            })
            .then(data => {
                if (data.erro) {
                    alert("CEP invalido!");
                }

                setCliente({
                    ...cliente,
                    cep: cep,
                    logradouro: data.logradouro,
                    bairro: data.bairro,
                    localidade: data.localidade,
                    uf: data.uf
                })
            }).catch(erro => {
                console.log(erro)
            })
    }

    const handdleEnviarFormulario = () => {
        if (captadorSelecionado == 0) {
            Alert("Selecione um captador", false);
            return;
        }

        var formData = new FormData();
        addCampos(formData);
        let edicao = clienteId && clienteId > 0;
        api.post(edicao ? "/EditarCliente" : "/NovoCliente", formData, res => {
            Alert(`Cliente ${edicao ? 'Editado' : 'Cadastrado'} Com Sucesso!`);
            setCliente(initState);
            setCaptador(initStateCaptador);
            if (edicao) {
                BuscarClienteId(clienteId)
            }
        }, err => {
            Alert(err.response.data, false);
        });
    }

    const addCampos = (formData) => {
        // Cliente
        formData.append('Cliente[Id]', clienteId);
        formData.append('Cliente[Cpf]', cliente.cpf);
        formData.append('Cliente[Nome]', cliente.nome);
        formData.append('Cliente[Cep]', cliente.cep);
        formData.append('Cliente[Logradouro]', cliente.logradouro);
        formData.append('Cliente[Bairro]', cliente.bairro);
        formData.append('Cliente[Localidade]', cliente.localidade);
        formData.append('Cliente[Uf]', cliente.uf);
        formData.append('Cliente[Numero]', cliente.numero);
        formData.append('Cliente[Complemento]', cliente.complemento);
        formData.append('Cliente[DataNasc]', cliente.dataNasc);
        formData.append('Cliente[NrDocto]', cliente.nrDocto);
        formData.append('Cliente[Empregador]', cliente.empregador);
        formData.append('Cliente[MatriculaBeneficio]', cliente.matriculaBeneficio);
        formData.append('Cliente[NomeMae]', cliente.nomeMae);
        formData.append('Cliente[NomePai]', cliente.nomePai);
        formData.append('Cliente[TelefoneFixo]', cliente.telefoneFixo);
        formData.append('Cliente[TelefoneCelular]', cliente.telefoneCelular);
        formData.append('Cliente[PossuiWhatsapp]', cliente.possuiWhatsapp);
        formData.append('Cliente[FuncaoAASPA]', cliente.funcaoAASPA);
        formData.append('Cliente[Email]', cliente.email);
        formData.append('Cliente[EstadoCivil]', cliente.estadoCivil);
        formData.append('Cliente[Sexo]', cliente.sexo);
        formData.append('Cliente[DataCad]', cliente.dataCad);

        // Captador
        formData.append('Captador[cpfOuCnpj]', captador.cpfOuCnpj);
        formData.append('Captador[nome]', captador.nome);
        formData.append('Captador[descricao]', captador.descricao);
    }

    const onChangeCaptador = (e) => {
        const { value } = e.target;
        let cSelecionado = captadores.filter(x => x.captador_id == value)[0];
        if (cSelecionado) {
            setCaptador({
                cpfOuCnpj: cSelecionado.captador_cpf_cnpj,
                nome: cSelecionado.captador_nome,
                descricao: cSelecionado.captador_descricao
            });
            setcaptadorSelecionado(value);
        }
    }

    return (
        <NavBar usuario_tipo={usuario && usuario.usuario_tipo} usuario_nome={usuario && usuario.usuario_nome}>
            <ClienteForm
                Mascara={Mascara}
                captador={captador}
                cliente={cliente}
                getLogadouro={getLogadouro}
                handleChange={handleChange}
                handleChangeCaptador={handleChangeCaptador}
                onSubmit={handdleEnviarFormulario}
                isEdit={clienteId > 0}
                captadores={captadores}
                captadorSelecionado={captadorSelecionado}
                setcaptadorSelecionado={onChangeCaptador}
            />
        </NavBar>
    );
}

export default Cliente;