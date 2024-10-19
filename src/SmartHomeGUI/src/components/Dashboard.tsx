import { useState, useEffect } from 'react'
import './Dashboard.css'

export interface DashboardData 
{
	topicName : string
	value : string
	timestamp : string
	isFavourite : boolean
}

export const topicTranslations : { [key : string] : { name : string, unit : string, isBoolean ?: boolean, isDoor ?: boolean } } =
{
	"home/door": { name: "Состояние двери", unit: "", isDoor: true },
	"home/outside/temperature": { name: "Температура на улице", unit: "°C" },
	"home/living_room/temperature": { name: "Температура в гостиной", unit: "°C" },
	"home/living_room/lighting": { name: "Свет в гостиной", unit: "", isBoolean: true },
	"home/bathroom/cold_water_temp": { name: "Температура холодной воды", unit: "°C" },
	"home/bathroom/hot_water_temp": { name: "Температура горячей воды", unit: "°C" },
	"home/bathroom/air_humidity": { name: "Влажность воздуха", unit: "%" },
	"home/bathroom/venting": { name: "Состояние вентиляции", unit: "", isBoolean: true }
}

export const formatValue = (topic : string, value : string) : string =>
{
	const translation = topicTranslations[topic]
	if (translation?.isDoor)
	{
		return value === "1" ? "Открыта" : "Закрыта"
	}
	if (translation?.isBoolean)
	{
		return value === "1" ? "Включено" : "Выключено"
	}
	return value
}

function Dashboard ()
{
	const [data, setData] = useState<DashboardData[]>([])
	const [loading, setLoading] = useState(true)
	const [error, setError] = useState<string | null>(null)

	useEffect(() =>
	{
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
				setData(json)
				setLoading(false)
			} catch (err)
			{
				const error = err as Error
				setError(error.message)
				setLoading(false)
			}
		}

		fetchData() // Initial fetch
		const intervalId = setInterval(fetchData, 1000)

		return () => clearInterval(intervalId)
	}, [])

	const toggleFavourite = async (topicName : string, currentFavouriteState : boolean) =>
	{
		try
		{
			const response = await fetch('https://localhost:7098/api/Dashboard/toggleFavourite', {
				method: 'POST',
				headers: {
					'Content-Type': 'application/json',
				},
				body: JSON.stringify({ topicName, isFavourite: !currentFavouriteState }),
			})

			if (response.ok)
			{
				setData((prevData) =>
					prevData.map((item) =>
						item.topicName === topicName ? { ...item, isFavourite: !currentFavouriteState } : item
					)
				)
			} else
			{
				console.error(`Ошибка при обновлении состояния: ${ response.status }`)
			}
		} catch (error)
		{
			console.error(`Произошла ошибка: ${ error }`)
		}
	}

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

							<div
								className={ `favourite-star ${ item.isFavourite ? 'active' : '' }` }
								onClick={ () => toggleFavourite(item.topicName, item.isFavourite) }
							>
								★
							</div>
						</div>
					)
				}) }
			</div>
		</div>
	)
}

export default Dashboard
