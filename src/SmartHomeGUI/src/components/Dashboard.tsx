import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useParams } from 'react-router-dom'
import './styles.css'
import { DashboardData } from './types'
import { formatValue } from './utils'

const Dashboard = () => {
	const { name } = useParams<{ name: string }>()
	const [data, setData] = useState<DashboardData[]>([])
	const [loading, setLoading] = useState(true)
	const [error, setError] = useState<string | null>(null)
	const navigate = useNavigate()

	useEffect(() => {
		let repeat = true
		const fetchData = async () => {
			while (repeat) {
				try {
					const response = await fetch(`https://localhost:7098/api/Dashboard/latestPoll/${name}*`)
					if (!response.ok) {
						throw new Error(`HTTP error! status: ${response.status}`)
					}
					const json: DashboardData[] = await response.json()
					setData(json)
					setLoading(false)
				} catch (err) {
					const error = err as Error
					setError(error.message)
					setLoading(false)
				}
			}
		}

		fetchData()

		return () =>
		{
			repeat = false
			setData([])
		}
	}, [name]);
	const handleItemClick = (measurementId: string) => {
		const encodedMeasurementId = encodeURIComponent(measurementId)
		navigate(`/history/${encodedMeasurementId}`)
	}

	if (loading) {
		return <p>Loading...</p>
	}

	if (error) {
		return <p>Error: {error}</p>
	}

	return (
		<div className="container">
			<p>{name}</p>
			{data.map((item, index) => (
				<div
					key={index}
					className="box"
					onClick={() => handleItemClick(item.measurementId)}
				>
					<h2>{item.name}</h2> {/* Используем topicName как имя измерения */}
					<p>
						Значение: {formatValue(item.value, item.units)} {/* Используем только value и unit */}
					</p>
					<p>Время: {new Date(item.timestamp).toLocaleString()}</p>
				</div>
			))}
		</div>
	)
}

export default Dashboard
