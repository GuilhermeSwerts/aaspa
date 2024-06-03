import 'bootstrap/dist/css/bootstrap.css';
import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter } from 'react-router-dom';
import Router from './router/Router';
import './index.css';
import Loader from './components/Loader/loader';

const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');
const rootElement = document.getElementById('root');

const blockDevTools = () => {
  document.addEventListener("contextmenu", (e) => {
    e.preventDefault();
    alert('Context Menu Blocked in PROD!\nTo unlock it, you need to use a homologation or development version.\n\nMenu de Contexto Bloqueado no PROD!\nPara desbloqueá-lo, você precisa usar uma versão de homologação ou desenvolvimento.')
  })
}

process.env.NODE_ENV === 'production' && blockDevTools();

ReactDOM.render(
  <BrowserRouter basename={baseUrl}>
    <Loader />
    <Router />
  </BrowserRouter>,
  rootElement);
