import { Route, BrowserRouter as Router, Routes } from 'react-router-dom'
import Authpage from './components/Authpage'
import { UserProvider } from './components/UserContext'
import { BASE_NAME } from './config'
function App() {
	return (
		<UserProvider>
			<Router basename={BASE_NAME} >
				<Routes>
					<Route path="/login" element={<Authpage />} />
				</Routes>
			</Router>
		</UserProvider>
	)
}

export default App
