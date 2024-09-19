import React from 'react';
import { NavBar } from '../../components/Layout/layout';
import Logo from '../../assets/logo_original.png';

function Home() {
    return (
        <NavBar>
            <div style={{ display: "flex", justifyContent: 'center', alignItems: "center",flexDirection:'column' }}>
                <h1>Bem Vindo ao ADM AASPA</h1>
                <br />
                <img src={Logo} alt="" />
            </div>
        </NavBar>
    );
}

export default Home;