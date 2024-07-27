import React, { Component } from 'react';
import { Route } from 'react-router';
import { AuthProvider } from '../context/AuthContext';

import Clientes from '../views/Clientes';
import Login from '../views/Login';
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
import RelatorioCarteira from '../views/Gerenciamento/RepasseCarteira/RelatorioCarteira';
import Cclientes from '../views/CClientes/index';

export default _ => {
    return (
        <AuthProvider>
            <Route exact path='/' component={Clientes} />
            <Route exact path='/login' component={Login} />
            <Route exact path='/cliente' component={Cliente} />
            <Route exact path='/ccliente' component={Cclientes} />
            <Route exact path='/beneficios' component={Beneficios} />
            <Route exact path='/pagamentos' component={Pagamentos} />
            <Route exact path='/historicopagamento' component={HistoricoPagamento} />
            <Route exact path='/historicoocorrenciacliente' component={HistoricoOcorrenciaCliente} />
            <Route exact path='/hstcocontatoocorrencia' component={HistoricoContatoOcorrencia} />

            <Route exact path='/teste' component={Teste} />

            <Route exact path='/gcaptador' component={Captador} />
            <Route exact path='/rrelatorio' component={Relatorio} />
            <Route exact path='/rrepassefinanceiro' component={RepasseFinanceiro} />
            <Route exact path='/relatoriocarteira' component={RelatorioCarteira} />
            <Route exact path='/rretorno' component={Retorno} />
            <Route exact path='/rremessa' component={Remessa} />
            <Route exact path='/gorigem' component={Origem} />
            <Route exact path='/gmotivocontato' component={MotivoContato} />
            <Route exact path='/gstatus' component={Status} />
            <Route exact path='/gbeneficio' component={GBeneficios} />
        </AuthProvider>
    );
}
