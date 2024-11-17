import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { DashboardData, topicTranslations } from './Types'
import { formatValue } from './Utils'
import './CommonContainersStyles.css'

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
            const response = await fetch('https://localhost:7098/api/Dashboard/latest')
            if (!response.ok)
            {
                throw new Error(`HTTP error! status: ${ response.status }`)
            }
            const json: DashboardData[] = await response.json()
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

    const handleItemClick = (topicName: string) =>
    {
        const encodedTopicName = encodeURIComponent(topicName)
        navigate(`/history/${ encodedTopicName }`)
    }

    const toggleFavourite = async (topicName: string, currentFavouriteState: boolean) =>
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
                    prevData.filter(item =>
                        item.topicName !== topicName || !currentFavouriteState
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
        <div className="dashboard">
            { data.map((item, index) =>
            {
                const translation = topicTranslations[item.topicName]
                if (!translation) return null

                return (
                    <div
                        key={ index }
                        className="dashboard-item"
                        onClick={ () => handleItemClick(item.topicName) }
                    >
                        <h2>{ translation.name }</h2>
                        <p>Значение: { formatValue(item.topicName, item.value) } { translation.unit }</p>
                        <p>Время: { new Date(item.timestamp).toLocaleString() }</p>

                        <div
                            className={ `favourite-star ${ item.isFavourite ? 'active' : '' }` }
                            onClick={ (e) =>
                            {
                                e.stopPropagation() // Предотвращаем переход при клике на звезду
                                toggleFavourite(item.topicName, item.isFavourite)
                            } }
                        >
                            ★
                        </div>
                    </div>
                )
            }) }
        </div>
    )
}

export default Home
