export interface DashboardData
{
	measurementId: string
	measurementTag: string
	name: string
	value: string
	units: string
	timestamp: string
	isFavourite: boolean
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