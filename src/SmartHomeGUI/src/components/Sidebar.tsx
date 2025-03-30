import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { API_BASE_URL } from "../config"
import './styles.css'
import { MeasurementLink } from './types'

interface SidebarProps
{
	isOpen: boolean
	setIsOpen: (isOpen: boolean) => void
}

function Sidebar ({ isOpen, setIsOpen }: SidebarProps)
{
	const [isDarkTheme, setIsDarkTheme] = useState(true)
	const [menu, setData] = useState<MeasurementLink[]>([])
	const [activeMenuItem, setActiveMenuItem] = useState<string>('')


	useEffect(() =>
	{
		const getMenu = async () =>
		{
			try
			{
				const response = await fetch(`${ API_BASE_URL }/MeasuresLinks/nextLayer/`)
				if (!response.ok)
				{
					throw new Error(`HTTP error! status: ${ response.status }`)
				}
				const json = await response.json()
				setData(json)
			} catch (error)
			{
				console.error('Ошибка при загрузке меню:', error)
			}
		}

		getMenu()
	}, [])

	useEffect(() =>
	{
		const fetchIcons = async () =>
		{
			const newIcons: { [key: string]: string } = {}
			for (const item of menu)
			{
				try
				{
					const iconUrl = `${ API_BASE_URL }/SvgImages/${ item.path }`
					newIcons[item.path] = iconUrl
				} catch (error)
				{
					console.error(`Ошибка загрузки иконки для ${ item.path }:`, error)
				}
			}
		}

		if (menu.length > 0)
		{
			fetchIcons()
		}
	}, [menu])

	useEffect(() =>
	{
		document.body.classList.toggle('light-theme', !isDarkTheme)
	}, [isDarkTheme])

	const handleMenuClick = (path: string) =>
	{
		setActiveMenuItem(path)
	}

	const toggleTheme = () =>
	{
		setIsDarkTheme((prev) => !prev)
	}

	return (
		<div className={ `sidebar ${ isOpen ? 'open' : '' }` }>
			<ul>
				{ menu.filter((item) => item.mode.includes('d')).map((item) => (
					<li key={ item.path } className={ activeMenuItem === item.path ? 'active' : '' } onClick={ () => handleMenuClick(item.path) }>
						<Link to={ `/dashboard/${ item.path }` }>
							<img src={ `${ API_BASE_URL }/SvgImages/${ item.path }` } className="menu-icon" />
							<span>{ item.path }</span>
						</Link>
					</li>
				)) }
				<li className={ activeMenuItem === 'settings' ? 'active' : '' } onClick={ () => handleMenuClick('settings') }>
					<Link to="/settings">
						<img src={ `${ API_BASE_URL }/SvgImages/Настройки` } className="menu-icon" alt="Настройки" />
						<span>Настройки</span>
					</Link>
				</li>
				<li onClick={ toggleTheme }>
					<button>
						<img src={ `${ API_BASE_URL }/SvgImages/${ isDarkTheme ? 'Светлая тема' : 'Темная тема' }` } alt="Переключить тему" className="menu-icon" />
						<span>{ isDarkTheme ? 'Светлая тема' : 'Темная тема' }</span>
					</button>
				</li>
			</ul>

			{/* Перемещаем кнопку вниз, вне списка ul */ }
			<button className="menu-toggle" onClick={ () => setIsOpen(!isOpen) }>
				{ isOpen ? '❮' : '❯' }
			</button>
		</div>
	)

}

export default Sidebar
