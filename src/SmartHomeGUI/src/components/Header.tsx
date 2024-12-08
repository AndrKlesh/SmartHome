import { Link } from 'react-router-dom'
import LogoutButton from './LogoutButton'
import { useState, useEffect } from 'react'
import './styles.css'

function Header ()
{
    const [isDarkTheme, setIsDarkTheme] = useState(true)

    useEffect(() =>
    {
        // Сразу устанавливаем тему при загрузке страницы
        if (isDarkTheme)
        {
            document.body.classList.remove('light-theme')
        } else
        {
            document.body.classList.add('light-theme')
        }
    }, [isDarkTheme]) // Этот эффект сработает только при изменении темы

    const toggleTheme = () =>
    {
        setIsDarkTheme(prevTheme => !prevTheme) // Переключаем тему
    }

    return (
        <header>
            <nav>
                <div className="nav-links">
                    <ul>
                        <li>
                            <Link to="/">Home</Link>
                        </li>
                        <li>
                            <Link to="/dashboard">Dashboard</Link>
                        </li>
                        <li>
                            <Link to="/login">Login</Link>
                        </li>
                        <li>
                            <Link to="/login"><LogoutButton /></Link>
                        </li>
                        <li>
                            <Link to="/settings">Settings</Link>
                        </li>
                    </ul>
                </div>
                <button
                    className="theme-toggle-button"
                    onClick={ toggleTheme }
                    aria-label="Toggle theme"
                >
                    { isDarkTheme ? '🌙' : '☀️' }
                </button>
            </nav>
        </header>
    )
}

export default Header
