import React from 'react';
import { Paginacao } from '../../components/Paginacao/Paginacao';
import { Mascara } from '../../util/mascara';

export default ({ naoAssociados, BuscarTodosNaoAssociados, totalClientes, limit, offset, setOffset, setLimit, SetPaginaAtual,selecionaNaoAssociado }) => {
    return (
        <>
            <table className='table table-striped'>
                <thead>
                    <tr>
                        <th>Nome</th>
                        <th>CPF</th>
                        <th>Telefone(Celular)</th>
                        <th>Quantidade de atendimentos</th>
                    </tr>
                </thead>
                <tbody>
                    {naoAssociados.map(cliente => {
                        return (
                            <tr className='selecao' onClick={e => selecionaNaoAssociado(cliente.cpf,cliente.nome)}>
                                <td>{cliente.nome}</td>
                                <td>{Mascara.cpf(cliente.cpf)}</td>
                                <td>{Mascara.telefone(cliente.telefoneCelular)}</td>
                                <td>{cliente.qtd}</td>
                            </tr>
                        )
                    })}
                    {naoAssociados.length == 0 && <span>Nenhum não associado foi encontrado...</span>}
                </tbody>
            </table>
            <Paginacao
                limit={limit}
                setLimit={setLimit}
                offset={offset}
                total={totalClientes}
                setOffset={setOffset}
                onChange={BuscarTodosNaoAssociados}
                setCurrentPage={SetPaginaAtual}
            />
        </>
    )
}