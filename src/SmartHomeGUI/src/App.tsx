import { BrowserRouter as Router, Route, Routes } from 'react-router-dom'
import Home from './components/Home'
import Dashboard from './components/Dashboard'
import Header from './components/Header'
import MeasurementHistory from './components/MeasurementHistory'

function App ()
{
	return (
		<Router>
			<Header />
			<Routes>
				<Route path="/" element={ <Home /> } />
				<Route path="/dashboard" element={ <Dashboard /> } />
				<Route path="/history/:topicName" element={ <MeasurementHistory /> } />
			</Routes>
		</Router>
	)
}

export default App
