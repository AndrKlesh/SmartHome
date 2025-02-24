import { JSX, useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { MeasurementLink } from './types';
import './Sidebar.css';
import './styles.css';

interface HeaderProps {
	isOpen: boolean; // Состояние меню (открыто/закрыто)
	setIsOpen: (isOpen: boolean) => void; // Функция для изменения состояния
}

function Header({ isOpen, setIsOpen }: HeaderProps) {
	const [isDarkTheme, setIsDarkTheme] = useState(true);
	const [menu, setData] = useState<MeasurementLink[]>([]);


	useEffect(() => {
		const getMenu = async () => {
			try {
				const response = await fetch('https://localhost:7098/api/MeasuresLinks/nextLayer/');
				if (!response.ok) {
					throw new Error(`HTTP error! status: ${response.status}`);
				}
				const json = await response.json();
				setData(json);
			} catch (error) {
				console.error('Ошибка при загрузке меню:', error);
			}
		};

		getMenu();
	}, []);

	useEffect(() => {
		document.body.classList.toggle('light-theme', !isDarkTheme);
	}, [isDarkTheme]);

	const toggleTheme = () => {
		setIsDarkTheme((prevTheme) => !prevTheme); // Переключаем тему
	};

	const toggleMenu = () => {
		setIsOpen(!isOpen); // Переключаем состояние меню
	};

	// Отображение пунктов меню
	const menuItems = menu
		.filter((item) => item.mode.includes('d'))
		.map((item) => (
			<li key={item.path}>
				<Link to={{ pathname: `/dashboard/${item.path}` }}>
					<span>{item.path}</span> {/* Название пункта */}
				</Link>
			</li>
		));

	return (
		<header>
			<div className={`sidebar ${isOpen ? 'open' : ''}`} onClick={toggleMenu}>
				<ul>
					{menuItems}
					<li>
						<Link to="/settings">
							<span>Настройки</span>
						</Link>
					</li>
				</ul>

				<button
					className="theme-toggle-button"
					onClick={toggleTheme}
					aria-label="Toggle theme"
				>
					<div className="icons">
						{isDarkTheme ? '🌙' : '🔆'}
					</div>
				</button>
			</div>
		</header>
	);
}

export default Header;
