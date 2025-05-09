/*ROTAS*/
import Cliente from '../views/Cliente';
import Status from '../views/Gerenciamento/Status';
import GBeneficios from '../views/Gerenciamento/Beneficios';
import Beneficios from '../views/Beneficios';
import MotivoContato from '../views/Gerenciamento/MotivoContato/index';
import Teste from '../views/Teste/teste';
import Pagamentos from '../views/Pagamentos';
import HistoricoPagamento from '../views/HistoricoPagamento';
import Origem from '../views/Gerenciamento/Origem';
import HistoricoOcorrenciaCliente from '../views/HistoricoContatoOcorrenciaCliente';
import HistoricoContatoOcorrencia from '../views/HistoricoOcorrencia';
import Remessa from '../views/Gerenciamento/Remessa/Remessa';
import Retorno from '../views/Gerenciamento/Retorno/Retorno'
import RepasseFinanceiro from '../views/Gerenciamento/RepasseFinanceiro/RepasseFinanceiro'
import Relatorio from '../views/Gerenciamento/Relatorio/Relatorio';
import Captador from '../views/Gerenciamento/Captador/index';
import RelatorioRetorno from '../views/Gerenciamento/RelatorioRetorno/RelatorioRetorno';
import RelatorioRepasse from '../views/Gerenciamento/RelatorioRepasse/RelatorioRepasse';
import Cclientes from '../views/CClientes/index';
import Atendimento from '../views/Atendimento/Atendimento';
import SolicitacaoReembolso from '../views/SolicitacaoReembolso/SolicitacaoReembolso';
import Usuarios from '../views/Usuarios/Usuario';
import Login from '../views/Login';
import SituacaoOcorrencia from '../views/Gerenciamento/SituacaoOcorrencia/SituacaoOcorrencia';

/*ICONES*/
import { FaUsersGear } from "react-icons/fa6";
import { FaChartArea, FaChartPie, FaFileDownload, FaFileImport, FaHome, FaUserAltSlash } from "react-icons/fa";
import { FaUsers } from "react-icons/fa";
import { GiReceiveMoney } from "react-icons/gi";
import { FaGears } from "react-icons/fa6";
import { FaFileInvoiceDollar } from "react-icons/fa";
import { BiSupport } from 'react-icons/bi';
import { GiPayMoney } from "react-icons/gi";
import { MdHistory } from "react-icons/md";
import { RiChatHistoryLine } from "react-icons/ri";
import { GiTakeMyMoney } from "react-icons/gi";
import Home from '../views/Home/home';
import Sindicato from '../views/Sindicato';
import NaoAssociados from '../views/NaoAssociado/index';

export const PublicaRotas = [ /*PUBLICA PARA APARECER NO MENU*/
    { Operador: true, Icon: FaHome, nome: 'Home', path: '/', component: Home },

    /*ADMINISTRAÇÃO*/
    { Operador: true, Icon: BiSupport, nome: 'Atendimento', path: '/atendimento', component: Atendimento },
    { Icon: GiTakeMyMoney, nome: 'Reembolso', path: '/reembolso', component: SolicitacaoReembolso },

    { Icon: FaUserAltSlash  , nome: 'Aten. Não Associados', path: '/atendimento-nao-associados', component: NaoAssociados },
    { Icon: FaUsers, nome: 'Clientes', path: '/clientes', component: Cclientes },

    /*GERAÇÃO DE ARQUIVOS*/
    { Icon: FaFileImport, nome: 'Remessa', path: '/rremessa', component: Remessa },
    { Icon: FaFileDownload, nome: 'Retorno', path: '/rretorno', component: Retorno },
    { Icon: FaFileInvoiceDollar, nome: 'Repasse', path: '/rrepassefinanceiro', component: RepasseFinanceiro },

    /*RELATÓRIOS*/
    { Icon: FaChartArea, nome: 'Relatório Retorno', path: '/rrelatorioretorno', component: RelatorioRetorno },
    { Icon: FaChartPie, nome: 'Relatório Repasse', path: '/rrelatoriorepasse', component: RelatorioRepasse },

    /*GERENCIAMENTO*/
    { Icon: FaUsersGear, nome: 'Gerenciar Usuários', path: '/gusuarios', component: Usuarios },
    { Icon: FaGears, nome: 'Gerenciar Captadores', path: '/gcaptador', component: Captador },
    { Icon: FaGears, nome: 'Gerenciar Origem', path: '/gorigem', component: Origem },
    { Icon: FaGears, nome: 'Gerenciar Motivo de Contato', path: '/gmotivocontato', component: MotivoContato },
    { Icon: FaGears, nome: 'Gerenciar Status', path: '/gstatus', component: Status },
    { Icon: FaGears, nome: 'Gerenciar Benefícios', path: '/gbeneficio', component: GBeneficios },
    { Icon: FaGears, nome: 'Gerenciar Situacao Ocorrencia', path: '/gsituacaoocorrencia', component: SituacaoOcorrencia },
    //{ Icon: FaFileImport, nome: 'Importar Arquivo Sindicato', path: '/sindicato', component: Sindicato },
]

export const PrivateRotas = [ /*PUBLICA PARA NÃO APARECER NO MENU*/
    { nome: '', path: '/login', component: Login },
    { nome: '', path: '/cliente', component: Cliente },
    { nome: '', path: '/historicoocorrenciacliente', component: HistoricoOcorrenciaCliente },
    { nome: '', path: '/teste', component: Teste },
    { Icon: GiReceiveMoney, nome: 'Clientes Benefícios', path: '/beneficios', component: Beneficios },
    { Icon: GiPayMoney, nome: 'Clientes Pagamentos', path: '/pagamentos', component: Pagamentos },
    { Icon: MdHistory, nome: 'Clientes Histórico de Pagamentos', path: '/historicopagamento', component: HistoricoPagamento },
    { Icon: RiChatHistoryLine, nome: 'Clientes Histórico de Contato', path: '/hstcocontatoocorrencia', component: HistoricoContatoOcorrencia },
]
