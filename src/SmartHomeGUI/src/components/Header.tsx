import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { MeasurementLink } from './types';

import './styles.css';

interface HeaderProps {
	isOpen: boolean;
	setIsOpen: (isOpen: boolean) => void;
}

function Header({ isOpen, setIsOpen }: HeaderProps) {
	const [isDarkTheme, setIsDarkTheme] = useState(true);
	const [menu, setData] = useState<MeasurementLink[]>([]);
	const [activeMenuItem, setActiveMenuItem] = useState<string>(''); 

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
		setIsDarkTheme((prevTheme) => !prevTheme);
	};

	const toggleMenu = () => {
		setIsOpen(!isOpen);
	};

	const handleMenuClick = (path: string) => {
		setActiveMenuItem(path); 
	};

	const menuItems = menu
		.filter((item) => item.mode.includes('d'))
		.map((item) => {
			const isActive = activeMenuItem === item.path; 
			return (
				<li
					key={item.path}
					className={isActive ? 'active' : ''}
					onClick={() => handleMenuClick(item.path)} 
				>
					<Link to={`/dashboard/${item.path}`}>
						<button className="theme-toggle-button">
						<span>{item.path}</span>
					</button>
					</Link>
				</li>
			);
		});

	const isSettingsActive = activeMenuItem === 'settings'; 

	return (
		<div className={`sidebar ${isOpen ? 'open' : ''}`} onClick={toggleMenu}>
			<ul>
				{menuItems}
				<li
					className={isSettingsActive ? 'active' : ''}
					onClick={() => handleMenuClick('settings')} 
				>
					<Link to="/settings">
						<button className="theme-toggle-button">
							<span>Настройки</span>
						</button>
					</Link>
				</li>
			</ul>

			<button className="theme-toggle-button" onClick={toggleTheme} aria-label="Toggle theme">
				<div className="icons">{isDarkTheme ? '🌙' : '🔆'}</div>
			</button>
		</div>
	);
}

export default Header;
