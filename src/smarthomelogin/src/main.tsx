import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import App from './App.tsx'
import { UserProvider } from './components/UserContext'


createRoot(document.getElementById('root')!).render(
	<StrictMode>
		<UserProvider>
			<App />
		</UserProvider>
	</StrictMode>,
)

