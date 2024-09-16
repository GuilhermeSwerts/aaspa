import React, { Component } from 'react';
import { Route } from 'react-router';
import { AuthProvider } from '../context/AuthContext';
import { PublicaRotas, PrivateRotas } from './Rotas';

export default _ => {
    return (
        <AuthProvider>
            {PublicaRotas.map(rota => (
                <Route key={rota} exact path={rota.path} component={rota.component} />
            ))}
            {PrivateRotas.map(rota => (
                <Route key={rota} exact path={rota.path} component={rota.component} />
            ))}
        </AuthProvider>
    );
}
