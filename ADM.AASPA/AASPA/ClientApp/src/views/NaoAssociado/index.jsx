import React, { useContext, useEffect, useRef, useState } from 'react';
import { AuthContext } from '../../context/AuthContext';
import { api } from '../../api/api';
import { Mascara } from '../../util/mascara';
import { NavBar } from '../../components/Layout/layout';
import CrudAtendimentoNaoAssociado from './CrudAtendimentoNaoAssociado';
import TableNaoAssociado from './TableNaoAssociado';
import { FaPlus } from 'react-icons/fa';
import { Alert, Pergunta } from '../../util/alertas';
import HistoricoAtendimentoNaoAssociado from './HistoricoAtendimentoNaoAssociado';
import ModalAtendimentoNaoAssociado from './ModalAtendimentoNaoAssociado';

function NaoAssociado() {
    const { usuario } = useContext(AuthContext);
    const refModal = useRef();
    const [form, setForm] = useState({
        id: 0,
        nome: '',
        cpf: '',
        origem: '',
        dataHora: '',
        motivo: '',
        situacao: '',
        telefone: '',
        descricao: '',
    });
    const [errors, setErrors] = useState({});
    const [id, setId] = useState(null);
    const [nome, setNome] = useState('');
    const [motivos, setMotivos] = useState([]);
    const [situacaoOcorrencias, setSituacaoOcorrencias] = useState([]);
    const [origens, setOrigens] = useState([]);
    const [historico, setHistorico] = useState([]);

    const [show, setShow] = useState(false);

    const [naoAssociados, setNaoAssociados] = useState([]);

    //**paginação**
    const [totalClientes, setTotalClientes] = useState(0);
    const [limit, setLimit] = useState(8);
    const [offset, setOffset] = useState(0);
    const [paginaAtual, SetPaginaAtual] = useState(1);

    const BuscarTodosNaoAssociados = (pPagina) => {

        if (!pPagina) pPagina = paginaAtual;

        api.get(`api/NaoAssociados?page=${pPagina}&pageSize=${limit}`, ({ data }) => {
            setNaoAssociados(data.data)
            setTotalClientes(data.totalItems);
        })
    }

    const BuscarMotivos = () => {
        api.get("BuscarTodosMotivos", res => {
            setMotivos(res.data);
        }, err => {
            Alert('Houve um erro ao buscar os motivos de contato', false)
        })
    }

    const BuscarSituacaoOcorrencias = () => {
        api.get("api/SituacaoOcorrencia", res => {
            setSituacaoOcorrencias(res.data);
        }, err => {
            Alert('Houve um erro ao buscar os Situação Ocorrencias', false)
        })
    }

    const BuscarOrigens = () => {
        api.get("BuscarTodasOrigem", res => {
            setOrigens(res.data);
        }, err => {
            Alert('Houve um erro ao buscar os motivos de contato', false)
        })
    }

    useEffect(() => {
        BuscarMotivos();
        BuscarSituacaoOcorrencias();
        BuscarOrigens();
        BuscarTodosNaoAssociados();
    }, [])

    const handleChange = (e) => {
        const { name, value } = e.target;
        setForm((prev) => ({ ...prev, [name]: value }));
    };

    const validate = () => {
        const newErrors = {};
        if (!form.nome) newErrors.nome = 'Nome é obrigatório';
        if (!form.cpf) newErrors.cpf = 'CPF é obrigatório';
        if (!form.origem) newErrors.origem = 'Origem é obrigatória';
        if (!form.dataHora) newErrors.dataHora = 'Data/Hora é obrigatória';
        if (!form.motivo) newErrors.motivo = 'Motivo é obrigatório';
        if (!form.situacao) newErrors.situacao = 'Situação é obrigatória';
        if (!form.telefone) newErrors.telefone = 'Telefone é obrigatório';
        if (!form.descricao) newErrors.descricao = 'Descrição é obrigatória';
        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        if (validate()) {
            const data = new FormData();

            data.append("nome", form.nome);
            data.append("cpf",form.cpf);
            data.append("origem",form.origem);
            data.append("dataHora",form.dataHora);
            data.append("motivo",form.motivo);
            data.append("situacao",form.situacao);
            data.append("telefone",form.telefone);
            data.append("descricao",form.descricao);

            api.post("api/NaoAssociados", data, res => {
                BuscarTodosNaoAssociados();
                onClose();
                Alert("Atendimento registrado com sucesso!");
            }, err => {

            })
        }
    };

    const onClose = () => {
        setForm({
            id: 0,
            nome: '',
            cpf: '',
            origem: '',
            dataHora: '',
            motivo: '',
            situacao: '',
            telefone: '',
            descricao: '',
        })
        setId(null)
        setErrors({})
        setShow(false)
        setHistorico([])
        setNome('')
    }

    const selecionaNaoAssociado = (cpf, nome) => {
        api.get(`api/NaoAssociados/history/${cpf}`, ({ data }) => {
            setHistorico(data)
            setNome(nome)
            setShow(true)
        })
    }

    const deleteAtendimento = async (id) => {
        if ((await Pergunta("Deseja realmente excluir esse atendimento?"))) {
            api.delete("api/NaoAssociados/" + id, res => {
                Alert("Atendimento excluido com sucesso!");
                BuscarTodosNaoAssociados();
                onClose();
            }, err => {
                Alert("ops...tivemos um problema e o atendimento não foi excluido!");
            })
        }
    }

    const editarAtendimento = async (id) => {
        refModal.current.Open(id)
    }

    return (
        <NavBar usuario_tipo={usuario?.usuario_tipo} usuario_nome={usuario?.usuario_nome}>
            <ModalAtendimentoNaoAssociado
                ref={refModal}
                situacaoOcorrencias={situacaoOcorrencias}
                origens={origens}
                motivos={motivos}

                onClose={onClose}
                id={id}
                historico={historico}
            />
            {historico.length == 0 && <>
                {show && <CrudAtendimentoNaoAssociado
                    errors={errors}
                    form={form}
                    handleChange={handleChange}
                    handleSubmit={handleSubmit}
                    origens={origens}
                    motivos={motivos}
                    onClose={onClose}
                    id={id}
                    historico={historico}
                    situacaoOcorrencias={situacaoOcorrencias}
                />}
                {!show && <button onClick={e => setShow(true)} className='btn btn-primary'><FaPlus /> Novo Atendimento</button>}
                {!show && <TableNaoAssociado
                    selecionaNaoAssociado={selecionaNaoAssociado}
                    naoAssociados={naoAssociados}
                    BuscarTodosNaoAssociados={BuscarTodosNaoAssociados}
                    SetPaginaAtual={SetPaginaAtual}
                    limit={limit}
                    offset={offset}
                    setLimit={setLimit}
                    setOffset={setOffset}
                    totalClientes={totalClientes}
                />}
            </>}
            {historico.length > 0 && <HistoricoAtendimentoNaoAssociado
                historico={historico}
                onClose={onClose}
                nome={nome}
                setId={setId}
                deleteAtendimento={deleteAtendimento}
                editarAtendimento={editarAtendimento}
            />}
        </NavBar>
    );
}

export default NaoAssociado;