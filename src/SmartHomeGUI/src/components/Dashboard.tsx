import { useState, useEffect } from 'react';
import { DashboardData, topicTranslations } from './types';
import { formatValue } from './utils';
import './Dashboard.css';

const Dashboard = () => {
    const [data, setData] = useState<DashboardData[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

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
        <div>
            <div className="dashboard">
                {data.map((item, index) => {
                    const translation = topicTranslations[item.topicName];
                    if (!translation) return null;

                    return (
                        <div key={index} className="dashboard-item">
                            <h2>{translation.name}</h2>
                            <p>Значение: {formatValue(item.topicName, item.value)} {translation.unit}</p>
                            <p>Время: {new Date(item.timestamp).toLocaleString()}</p>

                            <div
                                className={`favourite-star ${item.isFavourite ? 'active' : ''}`}
                                onClick={() => toggleFavourite(item.topicName, item.isFavourite)}
                            >
                                ★
                            </div>
                        </div>
                    );
                })}
            </div>
        </div>
    );
};

export default Dashboard;
