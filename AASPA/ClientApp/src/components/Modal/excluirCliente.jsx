import { useState } from 'react';
import { Modal, Button, Form } from 'react-bootstrap';
import { useEffect, useRef } from 'react';


const ExcluirCliente = ({ show, handleClose, handleConfirm }) => {
    const [deleteReason, setDeleteReason] = useState('');
    const deleteReasonRef = useRef(null);

    const handleDelete = () => {
        if (deleteReason.trim() === '') {
            alert('Por favor, informe um motivo para a exclusão.');
            return;
        }

        handleConfirm(deleteReason);

        setDeleteReason('');
        handleClose();
    };

    useEffect(() => {
        if (show && deleteReasonRef.current) {
            deleteReasonRef.current.focus();
        }
    }, [show]);


    return (
        <Modal show={show} onHide={handleClose}>
            <Modal.Header closeButton>
                <Modal.Title>Confirmar Exclusão</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Form>
                    <Form.Group>
                        <Form.Label>Motivo da Exclusão</Form.Label>
                        <Form.Control
                            as="textarea"
                            rows={3}
                            value={deleteReason}
                            ref={deleteReasonRef}
                            onChange={(e) => setDeleteReason(e.target.value)}
                            placeholder="Informe o motivo da exclusão"
                        />
                    </Form.Group>
                </Form>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={handleClose}>
                    Cancelar
                </Button>
                <Button 
                    variant="danger" 
                    onClick={handleDelete} 
                    disabled={deleteReason.trim() === ''}
                >
                    Confirmar Exclusão
                </Button>
            </Modal.Footer>
        </Modal>
    );
};

export default ExcluirCliente;
