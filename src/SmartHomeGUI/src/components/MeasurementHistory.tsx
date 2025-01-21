import {useEffect, useState} from 'react'
import {useParams} from 'react-router-dom'
import {CartesianGrid, Line, LineChart, ResponsiveContainer, Tooltip, XAxis, YAxis} from 'recharts'
import './styles.css'
import {Measurement} from './types'



const periods = {
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

	const fetchHistory = async () =>
	{
		try
		{
			const endDate = new Date().toISOString()
			const startDate = new Date(
				Date.now() - periods[selectedPeriod] * 60 * 60 * 1000
			).toISOString()

			const response = await fetch(
				`https://localhost:7098/api/MeasurementsHistory?measurementId=${ encodeURIComponent(
					decodedTopicName
				) }&startDate=${ startDate }&endDate=${ endDate }`
			)

			if (!response.ok)
			{
				throw new Error(`HTTP error! status: ${ response.status }`)
			}

			const json: Measurement[] = await response.json()

			// Фильтруем данные в зависимости от выбранного периода
			const filteredData = filterMeasurementsByMinute(json, selectedPeriod)

			setData(filteredData)
			setLatestMeasurement(
				filteredData.length > 0 ? filteredData[filteredData.length - 1] : null
			)
			setLoading(false)
		} catch (err)
		{
			const error = err as Error
			setError(error.message)
			setLoading(false)
		}
	}

	useEffect(() =>
	{
		fetchHistory() // Initial fetch
		const intervalId = setInterval(fetchHistory, 1000 * 60) // Обновляем данные каждую минуту

		return () => clearInterval(intervalId)
	}, [decodedTopicName, selectedPeriod])

	const filterMeasurementsByMinute = (measurements: Measurement[], period: string): Measurement[] =>
	{
		const intervalMinutes = {
			hour: 1, // оставляем все данные (обновление каждую минуту)
			'24hours': 5, // выбираем данные каждые 5 минут
			week: 60, // выбираем данные каждый час
			month: 240, // выбираем данные каждые 4 часа
			'3months': 1440, // выбираем данные каждый день
		}[period] || 1

		const seenMinutes = new Set<string>()
		return measurements.filter((measurement) =>
		{
			const minute = new Date(measurement.timestamp).toISOString().substring(0, 16)
			const timeKey = `${ Math.floor(new Date(measurement.timestamp).getMinutes() / intervalMinutes) }-${ minute }`

			if (!seenMinutes.has(timeKey))
			{
				seenMinutes.add(timeKey)
				return true
			}
			return false
		})
	}

	const getChartHeight = () =>
	{
		const screenHeight = window.innerHeight
		const screenWidth = window.innerWidth
		return Math.max(400, Math.min(screenHeight * 0.5, screenWidth * 0.4))
	}

	if (loading)
	{
		return <p className="loading">Loading...</p>
	}

	if (error)
	{
		return <p className="error">Error: {error}</p>
	}

	return (
		<div className="measurement-history">
			{latestMeasurement && (
				<div className="sensor-info">
					<p>
						<strong>История измерений для:</strong> {decodedTopicName}
					</p>
					<p>
						<strong>Последнее значение:</strong> {latestMeasurement.value}{' '}
						{latestMeasurement.unit || ''}
					</p>
					<p>
						<strong>Время последнего обновления:</strong>{' '}
						{new Date(latestMeasurement.timestamp).toLocaleString()}
					</p>
					<div className="period-selector">
						<label>Выберите период: </label>
						<select
							value={selectedPeriod}
							onChange={(e) => setSelectedPeriod(e.target.value as keyof typeof periods)}
						>
							<option value="hour">Последний час</option>
							<option value="24hours">Последние 24 часа</option>
							<option value="week">Последняя неделя</option>
							<option value="month">Последний месяц</option>
							<option value="3months">Последние 3 месяца</option>
						</select>
					</div>
				</div>
			)}
			<div className="chart-container">
				<ResponsiveContainer width="100%" height={getChartHeight()}>
					<LineChart
						data={data}
						margin={{top: 20, right: 20, left: 0, bottom: 20}}
					>
						<CartesianGrid stroke="#f5f5f5" />
						<XAxis
							dataKey="timestamp"
							tickFormatter={(tick) =>
								new Date(tick).toLocaleString([], {
									month: '2-digit',
									day: '2-digit',
									hour: '2-digit',
									minute: '2-digit',
								})
							}
							angle={-45}
							textAnchor="end"
							height={80}
						/>
						<YAxis />
						<Tooltip labelFormatter={(label) => new Date(label).toLocaleString()} />
						<Line
							type="monotone"
							dataKey="value"
							stroke="#8884d8"
							dot={false}
							strokeWidth={2}
						/>
					</LineChart>
				</ResponsiveContainer>
			</div>
		</div>
	)
}

export default MeasurementHistory
