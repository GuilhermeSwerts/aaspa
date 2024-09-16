import React, { useEffect, useState } from 'react';
import './paginacao.css';

export const Paginacao = ({
    limit, setLimit, total, offset, setOffset, onChange = () => { }, setCurrentPage = () => { }
}) => {
    const MAX_ITEMS = 9;
    const MAX_LEFT = (MAX_ITEMS - 1) / 2;

    const current = offset ? offset / limit + 1 : 1;
    const pages = Math.ceil(total / limit);
    const first = Math.max(current - MAX_LEFT, 1);

    function onPageChange(page) {
        setOffset((page - 1) * limit)
        setCurrentPage(page + 1)
        onChange();
    }

    useEffect(() => {
        onPageChange(1);
    }, [limit])

    return (
        <div>
            <br />
            <ul className='pagination'>
                {current > 5 && <li>
                    <button className='btn btn-success' onClick={() => onPageChange(1)}>...</button>
                </li>}
                {Array.from({ length: Math.min(MAX_ITEMS, pages) })
                    .map((_, i) => i + first)
                    .map((page) => {
                        if (page <= pages)
                            return (
                                <li key={page}>
                                    <button
                                        onClick={() => onPageChange(page)}
                                        className={
                                            page === current
                                                ? 'btn btn-success pagination__item--active'
                                                : 'btn btn-success'
                                        }
                                    >
                                        {page}
                                    </button>
                                </li>
                            )
                    })}
                {current < pages && <li>
                    <button className='btn btn-success' onClick={() => onPageChange(pages)}>...</button>
                </li>}
            </ul>
            <br />
            <div className="pagination">
                <span>Quantidade de registros por página:</span>
                <select value={limit} onChange={e => { setLimit(Number(e.target.value)); onChange() }}>
                    <option value={8}>8</option>
                    <option value={20}>20</option>
                    <option value={50}>50</option>
                    <option value={100}>100</option>
                </select>
            </div>
        </div>
    );
};
