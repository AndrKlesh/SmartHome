import {Box, Card, CardContent, CircularProgress, FormControl, InputLabel, MenuItem, Select, Typography} from '@mui/material'
import {useTheme} from "@mui/material/styles"
import {CategoryScale, Chart as ChartJS, Legend, LineElement, LinearScale, PointElement, Title, Tooltip} from 'chart.js'
import {useCallback, useEffect, useState} from 'react'
import {Line} from 'react-chartjs-2'
import {useParams} from 'react-router-dom'
import {API_BASE_URL} from "../config"
import {Measurement} from './Types'

ChartJS.register(CategoryScale, LinearScale, LineElement, PointElement, Title, Tooltip, Legend)

const periods =
{
	hour: 1,
	'24hours': 24,
	week: 7 * 24,
	month: 30 * 24,
	'3months': 90 * 24,
}

const MeasurementHistory = () =>
{
	const {topicName} = useParams<{topicName: string}>()
	const decodedTopicName = decodeURIComponent(topicName || '')
	const [data, setData] = useState<Measurement[]>([])
	const [loading, setLoading] = useState(true)
	const [error, setError] = useState<string | null>(null)
	const [latestMeasurement, setLatestMeasurement] = useState<Measurement | null>(null)
	const [selectedPeriod, setSelectedPeriod] = useState<'hour' | '24hours' | 'week' | 'month' | '3months'>('hour')

	const theme = useTheme()

	const chartOptions = {
		responsive: true,
		maintainAspectRatio: false,
		scales: {
			x: {
				grid: {color: theme.palette.divider},
				ticks: {color: theme.palette.text.primary},
			},
			y: {
				grid: {color: theme.palette.divider},
				ticks: {color: theme.palette.text.primary},
			},
		},
		elements: {line: {tension: 0.4}},
	}

	const fetchHistory = useCallback(async () =>
	{
		try
		{
			const endDate = new Date().toISOString()
			const startDate = new Date(Date.now() - periods[selectedPeriod] * 60 * 60 * 1000).toISOString()
			const response = await fetch(`${API_BASE_URL}/MeasurementsHistory?measurementId=${encodeURIComponent(decodedTopicName)}&startDate=${startDate}&endDate=${endDate}`)
			if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`)
			const json: Measurement[] = await response.json()
			setData(json)
			setLatestMeasurement(json.length > 0 ? json[json.length - 1] : null)
			setLoading(false)
		} catch (err)
		{
			setError(err instanceof Error ? err.message : 'Unknown error')
			setLoading(false)
		}
	}, [decodedTopicName, selectedPeriod])

	useEffect(() =>
	{
		fetchHistory()
		const intervalId = setInterval(fetchHistory, 1000 * 60)
		return () => clearInterval(intervalId)
	}, [fetchHistory])

	const chartData = {
		labels: data.map(measurement => new Date(measurement.timestamp).toLocaleString()),
		datasets: [
			{
				label: 'Измерения',
				data: data.map(measurement => Number(measurement.value)),
				borderColor: theme.palette.primary.main,
				fill: false,
			},
		],
	}

	if (loading) return <Box sx={{display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh'}}><CircularProgress /></Box>
	if (error) return <Typography color={theme.palette.error.main} variant="h6">{`Ошибка: ${error}`}</Typography>

	return (
		<Box sx={{
			display: 'flex',
			flexDirection: 'column',
			height: '95vh', // Высота экрана
			width: '100%', // Уменьшаем ширину на размер сайдбара
			gap: 2,
			padding: 3,
			maxWidth: '100vw', // Ограничение ширины с учетом сайдбара
			overflow: 'hidden',
		}}>

			{latestMeasurement && (
				<Card elevation={3} sx={{ padding: 2 }}>
					<CardContent>
						<Typography variant="h6">История измерений для: {topicName}</Typography>
						<Typography variant="body1">Последнее значение: {latestMeasurement.value} {latestMeasurement.unit || ''}</Typography>
						<Typography variant="body1">Время последнего обновления: {new Date(latestMeasurement.timestamp).toLocaleString()}</Typography>
					</CardContent>
				</Card>
			)}

			<FormControl fullWidth>
				<InputLabel id="select-period-label">Выберите период</InputLabel>
				<Select labelId="select-period-label" value={selectedPeriod} onChange={(e) => setSelectedPeriod(e.target.value as keyof typeof periods)}>
					<MenuItem value="hour">Последний час</MenuItem>
					<MenuItem value="24hours">Последние 24 часа</MenuItem>
					<MenuItem value="week">Последняя неделя</MenuItem>
					<MenuItem value="month">Последний месяц</MenuItem>
					<MenuItem value="3months">Последние 3 месяца</MenuItem>
				</Select>
			</FormControl>

			<Box sx={{ flex: 1, width: '100%', height: 'calc(100vh - 250px)', overflow: 'auto' }}>
				<Line data={chartData} options={chartOptions} />
			</Box>

		</Box>

	)
}

export default MeasurementHistory
