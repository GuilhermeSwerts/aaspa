import React, { useEffect } from 'react';

function Paginacao({ paginaAtual, qtdPorPagina, setData, data, setPaginaAtual, setQtdPorPagina }) {
    
    const indexOfLastClient = paginaAtual * qtdPorPagina;
    const indexOfFirstClient = indexOfLastClient - qtdPorPagina;
    const currentClients = data.slice(indexOfFirstClient, indexOfLastClient);
    const totalPages = Math.ceil(data.length / qtdPorPagina);

    useEffect(()=> {
        setData(currentClients);
    },[paginaAtual])

    useEffect(() => {
        setData(currentClients);
    }, [currentClients]);

    useEffect(()=> {
        setData(currentClients);
    },[qtdPorPagina])


    const handlePageChange = (newPage) => {
        setPaginaAtual(newPage);
    };

    return (
        <div className="pagination" style={{ display: 'flex', justifyContent: 'center', alignItems: "center", gap: 5 }}>
            <button
                className='btn btn-primary'
                onClick={() => handlePageChange(paginaAtual - 1)}
                disabled={paginaAtual === 1}
            >
                Anterior
            </button>
            <span>{paginaAtual} de {totalPages > 0 ? totalPages : 1}</span>
            <button
                className='btn btn-primary'
                onClick={() => handlePageChange(paginaAtual + 1)}
                disabled={paginaAtual === totalPages}
            >
                Próxima
            </button>
            <select value={qtdPorPagina} onChange={e => setQtdPorPagina(e.target.value)}>
                <option value={5}>5</option>
                <option value={10}>10</option>
                <option value={20}>20</option>
                <option value={50}>50</option>
                <option value={100}>100</option>
            </select>
        </div>
    );
}

export default Paginacao;