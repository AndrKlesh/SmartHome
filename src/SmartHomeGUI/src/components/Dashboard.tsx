import {Box, Card, CardContent, CircularProgress, Divider, Typography} from '@mui/material'
import {useTheme} from '@mui/material/styles'
import {useCallback, useEffect, useState} from 'react'
import {useNavigate, useParams} from 'react-router-dom'
import {API_BASE_URL} from '../Config'
import {DashboardData} from './Types'

const Dashboard = () =>
{
	const {name} = useParams<{name: string}>()
	const [data, setData] = useState<DashboardData[]>([])
	const [loading, setLoading] = useState(true)
	const [error, setError] = useState<string | null>(null)
	const navigate = useNavigate()
	const theme = useTheme()

	const fetchData = useCallback(async () =>
	{
		try
		{
			const response = await fetch(`${API_BASE_URL}/Dashboard/latestPoll/${name}*`)
			if (!response.ok)
			{
				throw new Error(`HTTP error! Status: ${response.status}`)
			}
			const json: DashboardData[] = await response.json()
			setData(json)
		} catch (err)
		{
			setError(err instanceof Error ? err.message : 'Unknown error')
		} finally
		{
			setLoading(false)
		}
	}, [name])

	useEffect(() =>
	{
		fetchData()
	}, [fetchData])

	const handleItemClick = (measurementId: string) =>
	{
		navigate(`/history/${encodeURIComponent(measurementId)}`)
	}

	if (loading)
	{
		return (
			<Box display="flex" justifyContent="center" alignItems="center" height="100vh">
				<CircularProgress />
			</Box>
		)
	}

	if (error)
	{
		return <Typography color={theme.palette.error.main} textAlign="center">Error: {error}</Typography>
	}

	return (
		<Box sx={{p: 3, bgcolor: theme.palette.background.default, color: theme.palette.text.primary}}>
			<Typography variant="h4" sx={{mb: 3, fontWeight: 'bold', color: theme.palette.primary.main}}>
				{name}
			</Typography>
			{data.map((item, index) => (
				<Card
					key={index}
					sx={{
						mb: 3,
						borderRadius: 2,
						bgcolor: theme.palette.background.paper,
						boxShadow: 3,
						transition: 'transform 0.2s ease-in-out',
						'&:hover': {transform: 'scale(1.05)', cursor: 'pointer'},
						width: '100%',
						mx: 'auto',
					}}
					onClick={() => handleItemClick(item.measurementId)}
				>
					<CardContent sx={{p: 3}}>
						<Typography variant="h6" fontWeight={600} color={theme.palette.text.primary}>
							{item.name}
						</Typography>
						<Divider sx={{my: 1, bgcolor: theme.palette.divider}} />
						<Typography variant="body1" color={theme.palette.text.secondary}>
							Значение: {item.value} {item.units}
						</Typography>
						<Typography variant="body1" color={theme.palette.text.secondary}>
							Время: {new Date(item.timestamp).toLocaleString()}
						</Typography>
					</CardContent>
				</Card>
			))}
		</Box>
	)
}

export default Dashboard
