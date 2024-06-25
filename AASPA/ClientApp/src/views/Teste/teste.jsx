import React from 'react';
import { Alert, Pergunta } from '../../util/alertas';

function Teste() {

    const teste = async () => {
        var valor = await Pergunta('TESTE');
        alert(valor);
    }

    return (
        <div>
            <button onClick={() => Alert('TESTE', true)}>Teste Ok</button>
            <br />
            <button onClick={() => Alert('TESTE', false)}>Teste erro</button>
            <br />
            <button onClick={teste}>Teste Pergunta</button>
        </div>
    );
}

export default Teste;