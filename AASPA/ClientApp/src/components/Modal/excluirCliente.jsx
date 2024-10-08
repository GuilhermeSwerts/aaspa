import { useState, useEffect, useRef } from 'react';
import { Modal, Button, Form } from 'react-bootstrap';

const ExcluirCliente = ({ show, handleClose, handleConfirm, StatusId }) => {
    const [cancelamento, setCancelamento] = useState('');
    const [motivoCancelamento, setMotivoCancelamento] = useState('');
    const cancelamentoRef = useRef(null);
    const [isProcessing, setIsProcessing] = useState(false);

    const statusCancelamento = [
        { value: '13', label: 'Cancelado a pedido do cliente' },
        { value: '14', label: 'Cancelado não averbado' }
    ];

    const motivoCancelamentoOptions = [
        { value: '', label: 'Selecione o motivo' },
        { value: '008 – Já existe desc. p/ outra entidade', label: '008 – Já existe desc. p/ outra entidade' },
        { value: '012 - Benefício bloqueado para desconto', label: '012 - Benefício bloqueado para desconto' },
        { value: '005 – Benefício não ativo', label: '005 – Benefício não ativo' },
        { value: '002 – Espécie incompatível', label: '002 – Espécie incompatível' },
        { value: '013 – Benefício de Pensão Alimentícia', label: '013 – Benefício de Pensão Alimentícia' },
    ];

    const handleDelete = () => {
        if (cancelamento.trim() === '' || (cancelamento === '14' && motivoCancelamento.trim() === '')) {
            alert('Por favor, informe um motivo para a exclusão.');
            return;
        }

        setIsProcessing(true);

        handleConfirm({ cancelamento, motivoCancelamento, StatusId });

        setCancelamento('');
        setMotivoCancelamento('');
        handleClose();
    };

    useEffect(() => {
        if (show && cancelamentoRef.current) {
            cancelamentoRef.current.focus();
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
                        <Form.Label>Selecione o tipo de cancelamento</Form.Label>
                        <Form.Control
                            as="select"
                            value={cancelamento}
                            onChange={(e) => setCancelamento(e.target.value)}
                            ref={cancelamentoRef}
                        >
                            <option value="">Selecione o tipo</option>
                            {statusCancelamento.map((status) => (
                                <option key={status.value} value={status.value}>
                                    {status.label}
                                </option>
                            ))}
                        </Form.Control>
                    </Form.Group>

                    {cancelamento === '14' && (
                        <Form.Group>
                            <Form.Label>Motivo do Cancelamento</Form.Label>
                            <Form.Control
                                as="select"
                                value={motivoCancelamento}
                                onChange={(e) => setMotivoCancelamento(e.target.value)}
                            >
                                {motivoCancelamentoOptions.map((motivo) => (
                                    <option key={motivo.value} value={motivo.value}>
                                        {motivo.label}
                                    </option>
                                ))}
                            </Form.Control>
                        </Form.Group>
                    )}
                </Form>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={handleClose}>
                    Cancelar
                </Button>
                <Button
                    variant="danger"
                    onClick={handleDelete}
                    disabled={cancelamento.trim() === '' || (cancelamento === '14' && motivoCancelamento.trim() === '')}
                >
                    Confirmar Exclusão
                </Button>
            </Modal.Footer>
        </Modal>
    );
};

export default ExcluirCliente;
