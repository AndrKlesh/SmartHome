import { Link } from 'react-router-dom';
import './Header.css';

function Header() {
    return (
        <header>
            <nav>
                <ul>
                    <li>
                        <Link to="/">Главная</Link>
                    </li>
                    <li>
                        <Link to="/dashboard">Dashboard</Link>
                    </li>
                </ul>
            </nav>
        </header>
    );
}

export default Header;
