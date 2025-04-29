import {Box, Button, CircularProgress, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, TextField, Typography, useTheme} from '@mui/material'
import React, {useCallback, useEffect, useState} from 'react'
import {API_BASE_URL} from "../config"
import ErrorMessage from './ErrorMessage'
import {TopicData} from './Types'

const Settings: React.FC = () =>
{
	const [data, setData] = useState<TopicData[]>([])
	const [editingKey, setEditingKey] = useState<string | null>(null)
	const [formValues, setFormValues] = useState<Partial<TopicData>>({})
	const [loading, setLoading] = useState(false)
	const [error, setError] = useState<string | null>(null)

	const theme = useTheme()

	const generateUniqueKey = () => crypto.randomUUID()

	const isEditing = (record: TopicData): boolean =>
		record.measurementId === editingKey

	const fetchData = useCallback(async () =>
	{
		setLoading(true)
		try
		{
			const response = await fetch(API_BASE_URL + '/Subscriptions/getAllSubscriptions')
			if (!response.ok)
			{
				throw new Error('Failed to fetch topics')
			}
			const result = await response.json()
			setData(result)
		} catch (err)
		{
			setError('Failed to load topics.')
		} finally
		{
			setLoading(false)
		}
	}, [])

	useEffect(() =>
	{
		fetchData()
	}, [fetchData])

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
			const response = await fetch(API_BASE_URL + '/Subscriptions/addSubscription', {
				method: 'POST',
				headers: {'Content-Type': 'application/json'},
				body: JSON.stringify(newRecord),
			})

			if (!response.ok)
			{
				throw new Error('Failed to add topic')
			}

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
			const response = await fetch(API_BASE_URL + `/deleteSubscription`, {
				method: 'DELETE',
				headers: {'Content-Type': 'application/json'},
				body: JSON.stringify({measurementId}),
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
			setError('Name and MQTT Topic are required')
			return
		}

		try
		{
			const response = await fetch(API_BASE_URL + `/Subscriptions/updateSubscription/${measurementId}`, {
				method: 'PUT',
				headers: {'Content-Type': 'application/json'},
				body: JSON.stringify(formValues),
			})

			if (!response.ok)
			{
				throw new Error('Failed to update topic')
			}

			setData((prevData) =>
				prevData.map((item) =>
					item.measurementId === measurementId ? {...item, ...formValues} : item
				)
			)
		} catch
		{
			setError('Failed to update topic')
		} finally
		{
			setEditingKey(null)
			setFormValues({})
		}
	}

	const handleInputChange = (
		e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
		field: keyof TopicData
	) =>
	{
		setFormValues((prevValues) => ({
			...prevValues,
			[field]: e.target.value,
		}))
	}

	return (
		<Box sx={{width: '100%', border: `none`, padding: 3, backgroundColor: theme.palette.background.default, color: theme.palette.text.primary}}>
			<Typography variant="h4" sx={{marginBottom: 3, fontWeight: 'bold', color: theme.palette.primary.main, textAlign: 'center'}}>
				Настройки
			</Typography>
			<TableContainer sx={{border: `1px solid ${theme.palette.divider}`}}>
				<Table aria-label="topics table" sx={{borderCollapse: 'separate'}}>
					<TableHead>
						<TableRow>
							<TableCell sx={{width: '30%', border: `1px solid ${theme.palette.divider}`}}>Id</TableCell>
							<TableCell sx={{width: '15%', border: `1px solid ${theme.palette.divider}`}}>Name</TableCell>
							<TableCell sx={{width: '10%', border: `1px solid ${theme.palette.divider}`}}>Units</TableCell>
							<TableCell sx={{width: '25%', border: `1px solid ${theme.palette.divider}`}}>MQTT Topic</TableCell>
							<TableCell sx={{width: '10%', border: `1px solid ${theme.palette.divider}`}}>Converter</TableCell>
							<TableCell sx={{width: '10%', border: `1px solid ${theme.palette.divider}`}}>Actions</TableCell>
						</TableRow>
					</TableHead>
					<TableBody>
						{data.map((record) => (
							<TableRow
								key={record.measurementId}
								sx={{
									'&:hover': {backgroundColor: theme.palette.action.hover},
									borderBottom: `1px solid ${theme.palette.divider}`,
								}}
							>
								{isEditing(record) ? (
									<>
										<TableCell sx={{border: `1px solid ${theme.palette.divider}`}}>
											<Typography variant="body2">{record.measurementId}</Typography>
										</TableCell>
										<TableCell sx={{border: `1px solid ${theme.palette.divider}`}}>
											<TextField
												value={formValues.measurementName || ''}
												onChange={(e) => handleInputChange(e, 'measurementName')}
												size="small"
												sx={{width: '100%'}}
											/>
										</TableCell>
										<TableCell sx={{border: `1px solid ${theme.palette.divider}`}}>
											<TextField
												value={formValues.unit || ''}
												onChange={(e) => handleInputChange(e, 'unit')}
												size="small"
												sx={{width: '100%'}}
											/>
										</TableCell>
										<TableCell sx={{border: `1px solid ${theme.palette.divider}`}}>
											<TextField
												value={formValues.mqttTopic || ''}
												onChange={(e) => handleInputChange(e, 'mqttTopic')}
												size="small"
												sx={{width: '100%'}}
											/>
										</TableCell>
										<TableCell sx={{border: `1px solid ${theme.palette.divider}`}}>
											<TextField
												value={formValues.converterName || ''}
												onChange={(e) => handleInputChange(e, 'converterName')}
												size="small"
												sx={{width: '100%'}}
											/>
										</TableCell>
										<TableCell>
											<Box sx={{display: 'flex', flexDirection: 'column', gap: 1}}>
												<Button
													onClick={() => handleSave(record.measurementId)}
													variant="contained"
													color="primary"
													fullWidth
												>
													Save
												</Button>
												<Button onClick={handleCancel} variant="outlined" color="error" fullWidth>
													Cancel
												</Button>
											</Box>
										</TableCell>
									</>
								) : (
									<>
										<TableCell sx={{border: `1px solid ${theme.palette.divider}`}}>
											<Typography variant="body2">{record.measurementId}</Typography>
										</TableCell>
										<TableCell sx={{border: `1px solid ${theme.palette.divider}`}}>
											<Typography variant="body2">{record.measurementName}</Typography>
										</TableCell>
										<TableCell sx={{border: `1px solid ${theme.palette.divider}`}}>
											<Typography variant="body2">{record.unit}</Typography>
										</TableCell>
										<TableCell sx={{border: `1px solid ${theme.palette.divider}`}}>
											<Typography variant="body2">{record.mqttTopic}</Typography>
										</TableCell>
										<TableCell sx={{border: `1px solid ${theme.palette.divider}`}}>
											<Typography variant="body2">{record.converterName}</Typography>
										</TableCell>
										<TableCell sx={{border: `1px solid ${theme.palette.divider}`}}>
											<Box sx={{display: 'flex', flexDirection: 'column', gap: 1}}>
												<Button onClick={() => handleEdit(record)} variant="contained" color="primary" fullWidth>
													Edit
												</Button>
												<Button onClick={() => handleDelete(record.measurementId)} variant="outlined" color="error" fullWidth>
													Delete
												</Button>
											</Box>
										</TableCell>
									</>
								)}
							</TableRow>
						))}
					</TableBody>
				</Table>
			</TableContainer>
			<Box sx={{mt: 2}}>
				<Button onClick={handleAdd} variant="contained" color="primary" disabled={loading} fullWidth>
					{loading ? <CircularProgress size={24} /> : 'Add Topic'}
				</Button>
			</Box>
			{error && <ErrorMessage message={error} />}
		</Box>
	)
}

export default Settings
