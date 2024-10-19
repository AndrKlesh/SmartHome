import { BrowserRouter as Router, Route, Routes } from 'react-router-dom'
import Home from './components/Home'
import Dashboard from './components/Dashboard'
import Header from './components/Header'

function App ()
{
	return (
		<Router>
			<Header />
			<Routes>
				<Route path="/" element={ <Home /> } />
				<Route path="/dashboard" element={ <Dashboard /> } />
			</Routes>
		</Router>
	)
}

export default App
