import React, { useState } from 'react';
import { api } from '../../api/api';

function Paginacao({ children, parametros, qtdPaginas, setQtdPaginas }) {
    const [paginaAtual, setPaginaAtual] = useState(1);

    const BuscarTodosClientes = (pPagina) => {
        if (!pPagina) {
            pPagina = paginaAtual;
        }

        api.get(`BuscarTodosClientes?${parametros}&paginaAtual=${pPagina}`, res => {
            setClientes([]);
            setClientesFiltro([]);

            setClientes(res.data.clientes);
            setClientesFiltro(res.data.clientes);
            setQtdPaginas(res.data.qtdPaginas);

            if (res.data.qtdPaginas < paginaAtual)
                setPaginaAtual(1);

            setTotalClientes(res.data.totalClientes);
        }, err => {
            Alert("Houve um erro ao buscar clientes.", false)
        })
    }

    const AlterarPagina = async (pagina, isProxima) => {
        let buscar = false;

        if (isProxima && (paginaAtual + pagina) > qtdPaginas) {
            if (await Pergunta("Numero da página digitada maior que quantidade de paginas\nDeseja buscar pelo numero maximo?")) {
                setPaginaAtual(qtdPaginas);
                buscar = true;
                BuscarTodosClientes(qtdPaginas)
            } else {
                return;
            }
        } else if (!isProxima && (paginaAtual - pagina) <= 0) {
            if (await Pergunta("Numero da página digitada menor que 1\nDeseja buscar pelo numero minimo?")) {
                setPaginaAtual(1);
                buscar = true;
                BuscarTodosClientes(1)
            } else {
                return;
            }
        } else {
            if (isProxima) {
                setPaginaAtual(paginaAtual + pagina);
                BuscarTodosClientes(paginaAtual + pagina)
            } else {
                setPaginaAtual(paginaAtual - pagina);
                BuscarTodosClientes(paginaAtual - pagina)
            }
        }
    }

    return (
        <>
            {children}
            <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', gap: 20, color: '#000' }}>
                <button onClick={() => { AlterarPagina(10, false) }} disabled={paginaAtual === 1} className='btn btn-primary'>-10</button>
                <button onClick={() => { AlterarPagina(5, false) }} disabled={paginaAtual === 1} className='btn btn-primary'>-5</button>
                <button onClick={() => { AlterarPagina(1, false) }} disabled={paginaAtual === 1} className='btn btn-primary'>Anterior</button>
                <span>{paginaAtual} de {qtdPaginas}</span>
                <button onClick={() => { AlterarPagina(1, true) }} disabled={paginaAtual >= qtdPaginas} className='btn btn-primary'>Próxima</button>
                <button onClick={() => { AlterarPagina(5, true) }} disabled={paginaAtual >= qtdPaginas} className='btn btn-primary'>+5</button>
                <button onClick={() => { AlterarPagina(10, true) }} disabled={paginaAtual >= qtdPaginas} className='btn btn-primary'>+10</button>
            </div>
        </>
    );
}

export default Paginacao;