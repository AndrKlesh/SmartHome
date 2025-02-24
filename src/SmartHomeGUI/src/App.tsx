import { useState, useEffect } from "react"
import { BrowserRouter as Router, Route, Routes, useNavigate } from "react-router-dom"
import Dashboard from "./components/Dashboard"
import Header from "./components/Header"
import MeasurementHistory from "./components/MeasurementHistory"
import Settings from "./components/Settings"
import "./components/styles.css"

function RedirectToFirstMenuItem() {
	const navigate = useNavigate()

	useEffect(() => {
		navigate("/dashboard/Общие") // Фиксированный путь
	}, [navigate])

	return null // Компонент не рендерит ничего
}

function App() {
	const [isOpen, setIsOpen] = useState(false)

	return (
		<Router>
			<Header isOpen={isOpen} setIsOpen={setIsOpen} />
			<div className={`content ${isOpen ? "shifted" : ""}`}>
				<Routes>
					<Route path="/" element={<RedirectToFirstMenuItem />} />
					<Route path="/dashboard/:name" element={<Dashboard />} />
					<Route path="/history/:topicName" element={<MeasurementHistory />} />
					<Route path="/settings" element={<Settings />} />
				</Routes>
			</div>
		</Router>
	)
}

export default App
