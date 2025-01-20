import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
//import LogoutButton from './LogoutButton'
import './styles.css'

function Header() {
    const [isDarkTheme, setIsDarkTheme] = useState(true)
    const [menu, setData] = useState<string[]>([])

    useEffect(() => {
        const getMenu = async () => {
            const response = await fetch('https://localhost:7098/api/MeasuresLinks/nextLayer/')
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`)
            }
            const json = await response.json()
            setData(json)
        }
        getMenu()
    }, [])

    useEffect(() => {
        // Сразу устанавливаем тему при загрузке страницы
        if (isDarkTheme) {
            document.body.classList.remove('light-theme')
        } else {
            document.body.classList.add('light-theme')
        }
    }, [isDarkTheme]) // Этот эффект сработает только при изменении темы

    const toggleTheme = () => {
        setIsDarkTheme(prevTheme => !prevTheme) // Переключаем тему
    }

    return (
        <header>
            <nav>
                <div className="nav-links">
                    <ul>
                        {
                            menu.map((item, index) => (
                                <li>
                                    <Link key={index} to={{ pathname: `/dashboard/${item}` }}>{item}</Link>
                                </li>
                            ))
                        }
                        <li>
                            <Link to="/settings">Settings</Link>
                        </li>
                    </ul>
                </div>
                <button
                    className="theme-toggle-button"
                    onClick={toggleTheme}
                    aria-label="Toggle theme"
                >
                    {isDarkTheme ? '🌙' : '☀️'}
                </button>
            </nav>
        </header>
    )
}

export default Header
