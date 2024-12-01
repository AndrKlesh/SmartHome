import React, { useState } from 'react';
import './Settings.css';

interface TopicData {
    key: string;
    name: string;
    units: string;
    mqttTopic: string;
    converterName: string;
}

const SubscribeToMqttTopics: React.FC = () => {
    const [data, setData] = useState<TopicData[]>([]);
    const [editingKey, setEditingKey] = useState<string | null>(null);
    const [formValues, setFormValues] = useState<Partial<TopicData>>({});

    const isEditing = (record: TopicData): boolean => record.key === editingKey;

    const handleEdit = (record: TopicData): void => {
        setEditingKey(record.key);
        setFormValues(record);
    };

    const handleCancel = (): void => {
        setEditingKey(null);
        setFormValues({});
    };

    const handleSave = (key: string): void => {
        setData((prevData) =>
            prevData.map((item) =>
                item.key === key ? { ...item, ...formValues } : item
            )
        );
        setEditingKey(null);
        setFormValues({});
    };

    const handleDelete = (key: string): void => {
        setData((prevData) => prevData.filter((item) => item.key !== key));
    };

    const handleAdd = (): void => {
        const newKey = `new-${data.length + 1}`;
        const newRecord: TopicData = {
            key: newKey,
            name: '',
            units: '',
            mqttTopic: '',
            converterName: 'DefaultConverter',
        };
        setData((prevData) => [...prevData, newRecord]);
        handleEdit(newRecord);
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
            <table>
                <thead>
                    <tr>
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
                                            value={formValues.name || ''}
                                            onChange={(e) => handleInputChange(e, 'name')}
                                        />
                                    </td>
                                    <td>
                                        <input
                                            type="text"
                                            value={formValues.units || ''}
                                            onChange={(e) => handleInputChange(e, 'units')}
                                        />
                                    </td>
                                    <td>
                                        <input
                                            type="text"
                                            value={formValues.mqttTopic || ''}
                                            onChange={(e) => handleInputChange(e, 'mqttTopic')}
                                        />
                                    </td>
                                    <td>{record.converterName}</td>
                                    <td>
                                        <button onClick={() => handleSave(record.key)}>Save</button>
                                        <button onClick={handleCancel}>Cancel</button>
                                    </td>
                                </>
                            ) : (
                                <>
                                    <td>{record.name}</td>
                                    <td>{record.units}</td>
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
            <button onClick={handleAdd} className="add-button">
                Add Topic
            </button>
        </div>
    );
};

export default SubscribeToMqttTopics;
