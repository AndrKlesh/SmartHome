export interface DashboardData
{
	topicName: string
	value: string
	timestamp: string
	isFavourite: boolean
	unit: string
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