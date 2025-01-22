export interface DashboardData
{
	measurementId: string
	name: string
	value: string
	units: string
	timestamp: string
}

export interface TopicData
{
	measurementId: string
	measurementName: string
	unit: string
	mqttTopic: string
	converterName: string
}

export interface Measurement
{
	value: string
	timestamp: string
	unit: string
}

export interface MeasurementLink
{
	path: string
	mode: string
}
