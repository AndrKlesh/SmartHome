import {Route, BrowserRouter as Router, Routes} from 'react-router-dom'
import Authpage from './components/Authpage'

function App ()
{
	return (
		<Router>
			<Routes>
				<Route path="/login" element={<Authpage />} />
			</Routes>
		</Router>
	)
}

export default App
