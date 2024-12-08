import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import './styles.css';
import { DashboardData } from './types';
import { formatValue } from './utils';

const Dashboard = () => {
    const [data, setData] = useState<DashboardData[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchData = async () => {
            try {
                const response = await fetch('https://localhost:7098/api/Dashboard/latest');
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                const json: DashboardData[] = await response.json();
                setData(json);
                setLoading(false);
            } catch (err) {
                const error = err as Error;
                setError(error.message);
                setLoading(false);
            }
        };

        fetchData(); // Initial fetch
        const intervalId = setInterval(fetchData, 1000);

        return () => clearInterval(intervalId);
    }, []);

    const handleItemClick = (topicName: string) => {
        const encodedTopicName = encodeURIComponent(topicName);
        navigate(`/history/${encodedTopicName}`);
    };

    const toggleFavourite = async (topicName: string, currentFavouriteState: boolean) => {
        try {
            const response = await fetch('https://localhost:7098/api/Dashboard/toggleFavourite', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ topicName, isFavourite: !currentFavouriteState }),
            });

            if (response.ok) {
                setData((prevData) =>
                    prevData.map((item) =>
                        item.topicName === topicName ? { ...item, isFavourite: !currentFavouriteState } : item
                    )
                );
            } else {
                console.error(`Ошибка при обновлении состояния: ${response.status}`);
            }
        } catch (error) {
            console.error(`Произошла ошибка: ${error}`);
        }
    };

    if (loading) {
        return <p>Loading...</p>;
    }

    if (error) {
        return <p>Error: {error}</p>;
    }

    return (
        <div className="container">
            {data.map((item, index) => (
                <div
                    key={index}
                    className="box"
                    onClick={() => handleItemClick(item.topicName)}
                >
                    <h2>{item.topicName}</h2> {/* Используем topicName как имя измерения */}
                    <p>
                        Значение: {formatValue(item.value, item.unit)} {/* Используем только value и unit */}
                    </p>
                    <p>Время: {new Date(item.timestamp).toLocaleString()}</p>

                    <div
                        className={`favourite-star ${item.isFavourite ? 'active' : ''}`}
                        onClick={(e) => {
                            e.stopPropagation();
                            toggleFavourite(item.topicName, item.isFavourite);
                        }}
                    >
                        ★
                    </div>
                </div>
            ))}
        </div>
    );
};

export default Dashboard;
