import { useState, useEffect } from "react"
import { BrowserRouter as Router, Route, Routes, Navigate } from "react-router-dom"
import Dashboard from "./components/Dashboard"
import Header from "./components/Header"
import MeasurementHistory from "./components/MeasurementHistory"
import Settings from "./components/Settings"
import "./components/styles.css"

function App ()
{
	const [isOpen, setIsOpen] = useState(false)
	const [firstMenuItem, setFirstMenuItem] = useState<string | null>(null)

	useEffect(() =>
	{
		const fetchMenu = async () =>
		{
			try
			{
				const response = await fetch("https://localhost:7098/api/MeasuresLinks/nextLayer/")
				if (!response.ok)
				{
					throw new Error(`Ошибка загрузки меню: ${ response.status }`)
				}
				const menu = await response.json()
				if (menu.length > 0)
				{
					setFirstMenuItem(menu[0].path) // Берём первый доступный путь
				}
			} catch (error)
			{
				console.error("Ошибка при загрузке меню:", error)
			}
		}

		fetchMenu()
	}, [])

	return (
		<Router>
			<Header isOpen={ isOpen } setIsOpen={ setIsOpen } />
			<div className={ `content ${ isOpen ? "shifted" : "" }` }>
				<Routes>
					{/* Динамическое перенаправление, когда меню загружено */ }
					<Route
						path="/"
						element={ firstMenuItem ? <Navigate to={ `/dashboard/${ firstMenuItem }` } /> : null }
					/>
					<Route path="/dashboard/:name" element={ <Dashboard /> } />
					<Route path="/history/:topicName" element={ <MeasurementHistory /> } />
					<Route path="/settings" element={ <Settings /> } />
				</Routes>
			</div>
		</Router>
	)
}

export default App
