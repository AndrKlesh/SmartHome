export interface DashboardData
{
	measurementId: string
	value: string
	timestamp: string
	//isFavourite: boolean
}

export const topicTranslations: { [key: string]: { name: string; unit: string; isBoolean?: boolean; isDoor?: boolean } } = {
	"home/door": { name: "Состояние двери", unit: "", isDoor: true },
	"home/outside/temperature": { name: "Температура на улице", unit: "°C" },
	"home/living_room/temperature": { name: "Температура в гостиной", unit: "°C" },
	"home/living_room/lighting": { name: "Свет в гостиной", unit: "", isBoolean: true },
	"home/bathroom/cold_water_temp": { name: "Температура холодной воды", unit: "°C" },
	"home/bathroom/hot_water_temp": { name: "Температура горячей воды", unit: "°C" },
	"home/bathroom/air_humidity": { name: "Влажность воздуха", unit: "%" },
	"home/bathroom/venting": { name: "Состояние вентиляции", unit: "", isBoolean: true }
}
