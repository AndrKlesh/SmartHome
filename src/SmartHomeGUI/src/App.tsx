import {Box, CircularProgress, Container, CssBaseline} from "@mui/material"
import {ThemeProvider, createTheme} from "@mui/material/styles"
import {useCallback, useEffect, useState} from "react"
import {Navigate, Route, BrowserRouter as Router, Routes} from "react-router-dom"
import Dashboard from "./components/Dashboard"
import MeasurementHistory from "./components/MeasurementHistory"
import Settings from "./components/Settings"
import Sidebar from "./components/Sidebar"
import {API_BASE_URL, BASE_NAME} from "./config"

interface MeasurementLink
{
	path: string
	mode: string
}

const App = () =>
{
	const [menu, setMenu] = useState<MeasurementLink[]>([])
	const [firstMenuItem, setFirstMenuItem] = useState<string | null>(null)
	const [isDarkTheme, setDarkTheme] = useState(true)

	const toggleTheme = useCallback(() => setDarkTheme((prev) => !prev), [])

	const fetchMenu = useCallback(async () =>
	{
		try
		{
			const response = await fetch(`${API_BASE_URL}/MeasuresLinks/nextLayer/`)
			if (!response.ok)
			{
				throw new Error(`Ошибка загрузки меню: ${response.status}`)
			}
			const data = await response.json()
			setMenu(data)
			if (data.length > 0)
			{
				setFirstMenuItem(data[0].path)
			}
		} catch (error)
		{
			console.error("Ошибка при загрузке меню:", error)
		}
	}, [])

	useEffect(() =>
	{
		fetchMenu()
	}, [fetchMenu])

	const theme = createTheme({
		palette: {
			mode: isDarkTheme ? "dark" : "light",
		},
		typography: {
			fontFamily: '"Roboto", "Arial", sans-serif',
		},
		components: {
			MuiButton: {
				defaultProps: {
					variant: "contained",
				},
			},
			MuiAppBar: {
				styleOverrides: {
					root: {
						borderRadius: "8px",
						boxShadow: "none",
					},
				},
			},
		},
	})

	if (!firstMenuItem)
	{
		return (
			<ThemeProvider theme={theme}>
				<CssBaseline />
				<Container sx={{mt: 2}}>
					<Box display="flex" justifyContent="center">
						<CircularProgress />
					</Box>
				</Container>
			</ThemeProvider>
		)
	}

	return (
		<ThemeProvider theme={theme}>
			<CssBaseline />
			<Router basename={BASE_NAME}>
				<Box sx={{ display: "flex", height: "100vh", overflow: "hidden" }}>
					<Sidebar menu={menu} isDarkTheme={isDarkTheme} toggleTheme={toggleTheme} />
					<Box sx={{ display: "flex", flexDirection: "column", flexGrow: 1, minWidth: 0 }}>
						<Container sx={{ flexGrow: 1, minWidth: 0, maxWidth: "100vw", overflow: "auto", p: 2 }}>
							<Routes>
								<Route path="/" element={<Navigate to={`/dashboard/${firstMenuItem}`} />} />
								<Route path="/dashboard/:name" element={<Dashboard />} />
								<Route path="/history/:topicName" element={<MeasurementHistory />} />
								<Route path="/settings" element={<Settings />} />
							</Routes>
						</Container>
					</Box>
				</Box>
			</Router>
		</ThemeProvider>
	);

}

export default App
