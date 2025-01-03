import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import './styles.css'
import { DashboardData } from './types'
import { formatValue } from './utils'

const Home = () =>
{
	const [data, setData] = useState<DashboardData[]>([])
	const [loading, setLoading] = useState(true)
	const [error, setError] = useState<string | null>(null)
	const navigate = useNavigate()

	const fetchData = async () =>
	{
		try
		{
			const response = await fetch('https://localhost:7098/api/Favourites/latest')
			if (!response.ok)
			{
				throw new Error(`HTTP error! status: ${ response.status }`)
			}
			const json: DashboardData[] = await response.json()
			setData(json)
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

	const handleItemClick = (topicName: string) =>
	{
		const encodedTopicName = encodeURIComponent(topicName)
		navigate(`/history/${ encodedTopicName }`)
	}

	const toggleFavourite = async (measurementId: string, currentFavouriteState: boolean) =>
	{
		try
		{
			const response = await fetch('https://localhost:7098/api/Favourites/toggleFavourite', {
				method: 'POST',
				headers: {
					'Content-Type': 'application/json',
				},
				body: JSON.stringify({ measurementId, isFavourite: !currentFavouriteState }),
			})

			if (response.ok)
			{
				setData((prevData) =>
					prevData.map((item) =>
						item.measurementId === measurementId ? { ...item, isFavourite: !currentFavouriteState } : item
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
		<div className="container">
			{ data.map((item, index) => (
				<div
					key={ index }
					className="box"
					onClick={() => handleItemClick(item.measurementId)}
				>
					<h2>{item.name}</h2> {/* Используем пользовательское имя */}
					<p>
						Значение: { formatValue(item.value, item.units) } {/* Форматируем значение и единицу */ }
					</p>
					<p>Время: { new Date(item.timestamp).toLocaleString() }</p>

					<div
						className={ `favourite-star ${ item.isFavourite ? 'active' : '' }` }
						onClick={ (e) =>
						{
							e.stopPropagation() // Предотвращаем переход при клике на звезду
							toggleFavourite(item.measurementId, true)
						} }
					>
						★
					</div>
				</div>
			)) }
		</div>
	)
}

export default Home
