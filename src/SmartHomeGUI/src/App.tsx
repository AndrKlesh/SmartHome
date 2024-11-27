import { Route, BrowserRouter as Router, Routes } from 'react-router-dom'
import Dashboard from './components/Dashboard'
import Header from './components/Header'
import Home from './components/Home'
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
