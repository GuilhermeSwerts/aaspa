import React, { useState } from 'react';

function Dropdown({ options = [], selectedOptions = [], setSelectedOptions = () => { } }) {
    const [isOpen, setIsOpen] = useState(false);

    const toggleDropdown = () => {
        setIsOpen(!isOpen);
    };

    const handleCheckboxChange = (event) => {
        const { value, checked } = event.target;
        setSelectedOptions((prev) =>
            checked ? [...prev, value] : prev.filter((option) => option !== value)
        );
    };

    return (
        <div className="multi-select">
            <div className="form-control" onClick={toggleDropdown}>
                <span style={{
                    display: 'flex',
                    justifyContent: 'space-between',
                    alignItems: 'center'
                }}>Selecione opções
                    <div className={`arrow ${isOpen ? 'down' : ''}`}></div>
                </span>
            </div>

            {isOpen && (
                <ul className="dropdown-list">
                    {options.map(({ value, text }, index) => (
                        <li key={index} className="dropdown-item">
                            <label>
                                <input
                                    checked={selectedOptions.filter(x => x == value).length > 0}
                                    type="checkbox"
                                    value={value}
                                    onChange={handleCheckboxChange}
                                />
                                {text}
                            </label>
                        </li>
                    ))}
                </ul>
            )}
        </div>
    );
}

export default Dropdown;
