import React, { useState } from 'react';
import { Modal, Button } from 'react-bootstrap';
import { api } from '../../api/api';
import { Alert } from '../../util/alertas';

function ModalAlterarSenha({ show, setShow }) {
    const [senhaAntiga, setsenhaAntiga] = useState('');
    const [senhaNova, setsenhaNova] = useState('');
    const [senhaConfirmaNova, setConfirmaNova] = useState('');
    const [verSenhas, setVerSenhas] = useState(false);

    const init = () => {
        setsenhaAntiga('');
        setsenhaNova('');
        setsenhaAntiga('');
    }

    const handleTrocaSenha = () => {
        if (senhaNova !== senhaConfirmaNova) {
            Alert('As senhas não coincidem.', false, true);
            setsenhaNova('');
            setsenhaAntiga('');
            return;
        }

        api.post(`TrocaSenha?senhaAtual=${senhaAntiga}&senhaNova=${senhaNova}`, {}, res => {
            Alert('Senha alterada com sucesso!');
            init();
            setShow(false);
        }, err => {
            Alert('Houve um erro ao trocar a senha: ' + err.response.data, false)
        })

    }

    return (
        <Modal show={show}>
            <form onSubmit={e => { e.preventDefault(); handleTrocaSenha() }}>
                <Modal.Header>
                    <Modal.Title>Troca Senha</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <label>Senha Atual:</label>
                    <input required type={verSenhas ? 'text' : 'password'} className='form-control' value={senhaAntiga} onChange={e => setsenhaAntiga(e.target.value)} /><br />
                    <label>Senha Nova:</label>
                    <input required type={verSenhas ? 'text' : 'password'} className='form-control' value={senhaNova} onChange={e => setsenhaNova(e.target.value)} /><br />
                    <label>Confirma Nova Senha:</label>
                    <input required type={verSenhas ? 'text' : 'password'} className='form-control' value={senhaConfirmaNova} onChange={e => setConfirmaNova(e.target.value)} /><br />
                    <div>
                        <input type="checkbox" onChange={e => setVerSenhas(!verSenhas)} />
                        <label htmlFor="">Ver senhas</label>
                    </div>
                </Modal.Body>
                <Modal.Footer>
                    <Button type='button' variant="secondary" onClick={e => { setShow(false); init() }}>Fechar</Button>
                    <Button variant="primary" type='submit'>Alterar senha</Button>
                </Modal.Footer>
            </form>
        </Modal>
    );
}

export default ModalAlterarSenha;
