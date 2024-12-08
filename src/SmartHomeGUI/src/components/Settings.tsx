import React, { useState, useEffect } from 'react'
import { TopicData } from './types'
import './Settings.css'



const SubscribeToMqttTopics: React.FC = () =>
{
	const [data, setData] = useState<TopicData[]>([])
	const [editingKey, setEditingKey] = useState<string | null>(null)
	const [formValues, setFormValues] = useState<Partial<TopicData>>({})
	const [loading, setLoading] = useState(false)
	const [error, setError] = useState<string | null>(null)

	const generateUniqueKey = () => crypto.randomUUID();

	const isEditing = (record: TopicData): boolean => record.measurementId === editingKey

	// Загрузка данных при загрузке компонента
	useEffect(() =>
	{
		const fetchData = async () =>
		{
			setLoading(true)
			try
			{
				const response = await fetch('https://localhost:7098/api/Subscriptions/getAllSubscriptions')
				if (!response.ok)
				{
					throw new Error('Failed to fetch topics')
				}
				const result = await response.json()
				setData(result)
			} catch
			{
				setError('Failed to load topics.')
			} finally
			{
				setLoading(false)
			}
		}

		fetchData()
	}, [])

	const handleAdd = async (): Promise<void> =>
	{
		setError(null)
		const newKey = generateUniqueKey()
		const newRecord: TopicData = {
			measurementId: newKey,
			measurementName: formValues.measurementName || '',
			unit: formValues.unit || '',
			mqttTopic: formValues.mqttTopic || '',
			converterName: formValues.converterName || '',
		}

		try
		{
			const response = await fetch('https://localhost:7098/api/Subscriptions/addSubscription', {
				method: 'POST',
				headers: { 'Content-Type': 'application/json' },
				body: JSON.stringify({
					measurementId: newRecord.measurementId,
					measurementName: newRecord.measurementName,
					unit: newRecord.unit,
					mqttTopic: newRecord.mqttTopic,
				}),
			})

			if (!response.ok)
			{
				throw new Error('Failed to add topic')
			}

			// Обновляем состояние данных в Dashboard
			setData((prevData) => [...prevData, newRecord])
		} catch
		{
			setError('Failed to add topic. Please try again.')
		}
	}


	const handleDelete = async (measurementId: string): Promise<void> =>
	{
		setError(null)

		try
		{
			const response = await fetch(`https://localhost:7098/api/Subscriptions/deleteSubscription/${ measurementId }`, {
				method: 'DELETE',
			})

			if (!response.ok)
			{
				throw new Error('Failed to delete topic')
			}

			setData((prevData) => prevData.filter((item) => item.measurementId !== measurementId))
		} catch
		{
			setError('Failed to delete topic. Please try again.')
		}
	}

	const handleEdit = (record: TopicData): void =>
	{
		setEditingKey(record.measurementId)
		setFormValues(record)
	}

	const handleCancel = (): void =>
	{
		setEditingKey(null)
		setFormValues({})
	}

	const handleSave = async (measurementId: string): Promise<void> =>
	{
		if (!formValues.measurementName || !formValues.mqttTopic)
		{
			alert('Name and MQTT Topic are required')
			return
		}

		try
		{
			const response = await fetch(`https://localhost:7098/api/Subscriptions/updateSubscription/${ measurementId }`, {
				method: 'PUT',
				headers: { 'Content-Type': 'application/json' },
				body: JSON.stringify({
					measurementId: formValues.measurementId,
					measurementName: formValues.measurementName,
					unit: formValues.unit,
					mqttTopic: formValues.mqttTopic,
				}),
			})

			if (!response.ok)
			{
				throw new Error('Failed to update topic')
			}

			setData((prevData) =>
				prevData.map((item) =>
					item.measurementId === measurementId ? { ...item, ...formValues } : item
				)
			)
		} catch
		{
			setError('Failed to update topic. Please try again.')
		} finally
		{
			setEditingKey(null)
			setFormValues({})
		}
	}

	const handleInputChange = (
		e: React.ChangeEvent<HTMLInputElement>,
		field: keyof TopicData
	) =>
	{
		setFormValues((prevValues) => ({
			...prevValues,
			[field]: e.target.value,
		}))
	}

	return (
		<div className="dark-table-container">
			<h2>Subscribe to MQTT Topics</h2>
			{ error && <div className="error-message">{ error }</div> }
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
					{ data.map((record) => (
						<tr key={ record.measurementId }>
							{ isEditing(record) ? (
								<>
									<td>{ record.measurementId }</td>
									<td>
										<input
											type="text"
											value={ formValues.measurementName || '' }
											onChange={ (e) => handleInputChange(e, 'measurementName') }
										/>
									</td>
									<td>
										<input
											type="text"
											value={ formValues.unit || '' }
											onChange={ (e) => handleInputChange(e, 'unit') }
										/>
									</td>
									<td>
										<input
											type="text"
											value={ formValues.mqttTopic || '' }
											onChange={ (e) => handleInputChange(e, 'mqttTopic') }
										/>
									</td>
									<td>
										<input
											type="text"
											value={ formValues.converterName || '' }
											onChange={ (e) => handleInputChange(e, 'converterName') }
										/>
									</td>
									<td>
										<button onClick={ () => handleSave(record.measurementId) }>Save</button>
										<button onClick={ handleCancel }>Cancel</button>
									</td>
								</>
							) : (
								<>
									<td>{ record.measurementId }</td>
									<td>{ record.measurementName }</td>
									<td>{ record.unit }</td>
									<td>{ record.mqttTopic }</td>
									<td>{ record.converterName }</td>
									<td>
										<button onClick={ () => handleEdit(record) }>Edit</button>
										<button onClick={ () => handleDelete(record.measurementId) }>Delete</button>
									</td>
								</>
							) }
						</tr>
					)) }
				</tbody>
			</table>
			<button onClick={ handleAdd } className="add-button" disabled={ loading }>
				{ loading ? 'Adding...' : 'Add Topic' }
			</button>
		</div>
	)
}

export default SubscribeToMqttTopics
