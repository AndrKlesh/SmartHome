import { useState, useEffect } from 'react'
import { DashboardData, topicTranslations, formatValue } from './Dashboard' // Импортируем из Dashboard
import './Home.css'

function Home ()
{
	const [data, setData] = useState<DashboardData[]>([])
	const [loading, setLoading] = useState(true)
	const [error, setError] = useState<string | null>(null)

	const fetchData = async () =>
	{
		try
		{
			const response = await fetch('https://localhost:7098/api/Dashboard/latest')
			if (!response.ok)
			{
				throw new Error(`HTTP error! status: ${ response.status }`)
			}
			const json : DashboardData[] = await response.json()
			setData(json.filter(item => item.isFavourite))
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
		fetchData()

		const intervalId = setInterval(() =>
		{
			fetchData()
		}, 1000)

		return () => clearInterval(intervalId)
	}, [])

	if (loading)
	{
		return <p>Loading...</p>
	}

	if (error)
	{
		return <p>Error: { error }</p>
	}

	return (
		<div>
			<div className="dashboard">
				{ data.map((item, index) =>
				{
					const translation = topicTranslations[item.topicName]
					if (!translation) return null

					return (
						<div key={ index } className="dashboard-item">
							<h2>{ translation.name }</h2>
							<p><strong>Значение:</strong> { formatValue(item.topicName, item.value) } { translation.unit }</p>
							<p><strong>Время:</strong> { new Date(item.timestamp).toLocaleString() }</p>
						</div>
					)
				}) }
			</div>
		</div>
	)
}

export default Home