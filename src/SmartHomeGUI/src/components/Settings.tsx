import React, { useState, useEffect } from 'react';
import './Settings.css';

interface TopicData {
    key: string;
    name: string;
    unit: string;
    mqttTopic: string;
    converterName: string;
}

const SubscribeToMqttTopics: React.FC = () => {
    const [data, setData] = useState<TopicData[]>([]);
    const [editingKey, setEditingKey] = useState<string | null>(null);
    const [formValues, setFormValues] = useState<Partial<TopicData>>({});
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    // Загружаем данные из localStorage при монтировании компонента
    useEffect(() => {
        const savedData = localStorage.getItem('mqttTopics');
        if (savedData) {
            setData(JSON.parse(savedData));
        }
    }, []);

    // Сохраняем данные в localStorage при изменении состояния data
    useEffect(() => {
        if (data.length > 0) {
            localStorage.setItem('mqttTopics', JSON.stringify(data));
        }
    }, [data]);

    const isEditing = (record: TopicData): boolean => record.key === editingKey;

    const handleEdit = (record: TopicData): void => {
        setEditingKey(record.key);
        setFormValues(record);
    };

    const handleCancel = (): void => {
        setEditingKey(null);
        setFormValues({});
    };

    const handleSave = async (key: string): Promise<void> => {
        if (!formValues.name || !formValues.mqttTopic) {
            alert("Name and MQTT Topic are required");
            return;
        }

        try {
            const response = await fetch('https://localhost:7098/api/Subscriptions/addSubscription', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    measurementId: formValues.key,
                    measurementName: formValues.name,
                    unit: formValues.unit,
                    mqttTopic: formValues.mqttTopic,
                }),
            });

            if (!response.ok) {
                throw new Error('Failed to add topic');
            }



        } catch {
            setError('Failed to add topic. Please try again.');
        } finally {
            setLoading(false);
        }

        setData((prevData) =>
            prevData.map((item) =>
                item.key === key ? { ...item, ...formValues } : item
            )
        );
        setEditingKey(null);
        setFormValues({});
    };

    const handleDelete = (key: string): void => {
        setData((prevData) => {
            const updatedData = prevData.filter((item) => item.key !== key);
            return updatedData;
        });
    };

    const handleAdd = (): void => {
        //setLoading(true);
        setError(null);
        const newKey = `new-${Date.now()}`;
        const newRecord: TopicData = {
            key: formValues.key || '',
            name: formValues.name || '',
            unit: formValues.unit || '',
            mqttTopic: formValues.mqttTopic || '',
            converterName: 'DefaultConverter',
        };
        setData((prevData) => [...prevData, newRecord]);

        setEditingKey(newKey);
        setFormValues({});

    };

    const handleInputChange = (
        e: React.ChangeEvent<HTMLInputElement>,
        field: keyof TopicData
    ) => {
        setFormValues((prevValues) => ({
            ...prevValues,
            [field]: e.target.value,
        }));
    };

    return (
        <div className="dark-table-container">
            <h2>Subscribe to MQTT Topics</h2>
            {error && <div className="error-message">{error}</div>}
            <table>
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>Name</th>
                        <th>Units</th>
                        <th>MQTT Topic</th>
                        <th>Converter</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {data.map((record) => (
                        <tr key={record.key}>
                            {isEditing(record) ? (
                                <>
                                    <td>
                                        <input
                                            type="text"
                                            value={formValues.key || ''}
                                            onChange={(e) => handleInputChange(e, 'key')}
                                        />
                                    </td>
                                    <td>
                                        <input
                                            type="text"
                                            value={formValues.name || ''}
                                            onChange={(e) => handleInputChange(e, 'name')}
                                        />
                                    </td>
                                    <td>
                                        <input
                                            type="text"
                                            value={formValues.unit || ''}
                                            onChange={(e) => handleInputChange(e, 'unit')}
                                        />
                                    </td>
                                    <td>
                                        <input
                                            type="text"
                                            value={formValues.mqttTopic || ''}
                                            onChange={(e) => handleInputChange(e, 'mqttTopic')}
                                        />
                                    </td>
                                    <td>
                                        <input
                                            type="text"
                                            value={formValues.converterName || 'DefaultConverter'}
                                            onChange={(e) => handleInputChange(e, 'converterName')}
                                        />
                                    </td>
                                    <td>
                                        <button onClick={() => handleSave(record.key)}>Save</button>
                                        <button onClick={handleCancel}>Cancel</button>
                                    </td>
                                </>
                            ) : (
                                <>
                                    <td>{record.key}</td>
                                    <td>{record.name}</td>
                                    <td>{record.unit}</td>
                                    <td>{record.mqttTopic}</td>
                                    <td>{record.converterName}</td>
                                    <td>
                                        <button onClick={() => handleEdit(record)}>Edit</button>
                                        <button onClick={() => handleDelete(record.key)}>
                                            Delete
                                        </button>
                                    </td>
                                </>
                            )}
                        </tr>
                    ))}
                </tbody>
            </table>
            <button onClick={handleAdd} className="add-button" disabled={loading}>
                {loading ? 'Adding...' : 'Add Topic'}
            </button>
        </div>
    );
};

export default SubscribeToMqttTopics;
