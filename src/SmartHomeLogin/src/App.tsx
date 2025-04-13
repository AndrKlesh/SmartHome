import { Route, BrowserRouter as Router, Routes } from 'react-router-dom'
import Authpage from './components/Authpage'
import Dashboard from './components/Dashboard'
import NoAccess from './components/NoAccess'
import { BASE_NAME } from './config'
function App() {
	return (
			<Router basename={BASE_NAME} >
				<Routes>
					<Route path="/login" element={<Authpage />} />
					<Route path="/dashboard/:username" element={<Dashboard />} />
					<Route path="/dashboard/no-access" element={<NoAccess />} />
				</Routes>
			</Router>
	)
}

export default App
