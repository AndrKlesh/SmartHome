import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { MeasurementLink } from './types'
import { API_BASE_URL } from "../config"
import './styles.css'

interface HeaderProps
{
	isOpen: boolean
	setIsOpen: (isOpen: boolean) => void
}

function Header ({ isOpen, setIsOpen }: HeaderProps)
{
	const [isDarkTheme, setIsDarkTheme] = useState(true)
	const [menu, setData] = useState<MeasurementLink[]>([])
	const [activeMenuItem, setActiveMenuItem] = useState<string>('')
	const [icons, setIcons] = useState<{ [key: string]: string }>({})

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
					console.log(iconUrl)
				} catch (error)
				{
					console.error(`Ошибка загрузки иконки для ${ item.path }:`, error)
				}
			}
			setIcons(newIcons)
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

	const setDarkTheme = () =>
	{
		setIsDarkTheme(true)
	}

	const setLightTheme = () =>
	{
		setIsDarkTheme(false)
	}

	const toggleMenu = () =>
	{
		setIsOpen(!isOpen)
	}

	const handleMenuClick = (path: string) =>
	{
		setActiveMenuItem(path)
	}

	const menuItems = menu
		.filter((item) => item.mode.includes('d'))
		.map((item) =>
		{
			const isActive = activeMenuItem === item.path
			return (
				<li
					key={ item.path }
					className={ isActive ? 'active' : '' }
					onClick={ () => handleMenuClick(item.path) }
				>
					<Link to={ `/dashboard/${ item.path }` }>
						<button className="theme-toggle-button">
							<img src={ icons[item.path] } alt={ item.path } className="menu-icon" />
							<span>{ item.path }</span>
						</button>
					</Link>
				</li>
			)
		})

	const isSettingsActive = activeMenuItem === 'settings'

	return (
		<div className={ `sidebar ${ isOpen ? 'open' : '' }` } onClick={ toggleMenu }>
			<ul>
				{ menuItems }
				<li
					className={ isSettingsActive ? 'active' : '' }
					onClick={ () => handleMenuClick('settings') }
				>
					<Link to="/settings">
						<button className="theme-toggle-button">
							<img src={ `${ API_BASE_URL }/SvgImages/Настройки` } className="menu-icon" />
							<span>Настройки</span>
						</button>
					</Link>
				</li>
			</ul>

				<button className="theme-toggle-button" onClick={ setDarkTheme } aria-label="Темная тема">
					<img src={ `${ API_BASE_URL }/SvgImages/Темная тема` } alt={ 'Темная тема' } className="menu-icon" />
					<span>Темная тема</span>
				</button>
				<button className="theme-toggle-button" onClick={ setLightTheme } aria-label="Светлая тема">
					<img src={ `${ API_BASE_URL }/SvgImages/Светлая тема` } alt={ 'Светлая тема' } className="menu-icon" />
					<span>Светлая тема</span>
				</button>
		</div>
	)
}

export default Header
