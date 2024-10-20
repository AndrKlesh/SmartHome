import { topicTranslations } from './Types'

export const formatValue = (topic: string, value: string): string =>
{
	const translation = topicTranslations[topic]
	if (translation?.isDoor)
	{
		return value === "1" ? "Открыта" : "Закрыта"
	}
	if (translation?.isBoolean)
	{
		return value === "1" ? "Включено" : "Выключено"
	}
	return value
}
