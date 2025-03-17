import { Route, BrowserRouter as Router, Routes } from 'react-router-dom'
import Authpage from './components/Authpage'
import { UserProvider } from './components/UserContext'

function App() {
	return (
		<UserProvider>
			<Router>
				<Routes>
					<Route path="/login" element={<Authpage />} />
				</Routes>
			</Router>
		</UserProvider>
	)
}

export default App
