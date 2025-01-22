import {Route, BrowserRouter as Router, Routes} from 'react-router-dom'
import Dashboard from './components/Dashboard'
import Header from './components/Header'
import MeasurementHistory from './components/MeasurementHistory'
import Settings from './components/Settings'

function App ()
{
	return (
		<Router>
			<Header />
			<Routes>
				<Route path="/dashboard/:name" element={<Dashboard />} />
				<Route path="/history/:topicName" element={<MeasurementHistory />} />
				<Route path="/settings" element={<Settings />} />
			</Routes>
		</Router>
	)
}

export default App
