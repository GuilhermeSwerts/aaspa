import React from 'react';
import './loader.css';
import { FaGear } from 'react-icons/fa6';
import Logo from '../../assets/Logo.png'

class Loader extends React.Component {
    constructor(props) {
        super(props)
        this.state = {
            show: false
        }
        this.showLoader = _ => {
            this.setState({ show: true })
        }
        this.hideLoader = _ => {
            this.setState({ show: false })
        }
    }

    render() {
        return (
            <div className='pl_container' id='loadingpanel'>
                <img src={Logo} />
                {/* <FaGear className='loader' size={150} color='#fff'></FaGear> */}
                {/* <span style={{ color: '#fff',fontSize:'30px' }}>Aguarde...</span> */}
                <div class="dots-loader">
                    <div></div>
                    <div></div>
                    <div></div>
                </div>
            </div>
        );
    }
}

export default Loader;