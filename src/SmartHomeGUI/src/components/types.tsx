export interface DashboardData
{
	measurementId: string
	value: string
	timestamp: string
	//isFavourite: boolean
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