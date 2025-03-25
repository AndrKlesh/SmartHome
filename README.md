# Проект "Умный дом"
Данный проект разрабатывается в период учебной практики в рамках ИТ-акселератора ОмГУ.

# Запуск
Для запуска решения нужно создать папку `.vscode` в корне каждого JavaScript.Sdk проекта. В папке `.vscode` нужно создать файл `launch.json` с таким содержимым:

```json
{
	"version": "0.2.0",
	"configurations":
	[
		{
			"name": "localhost (Chrome)",
			"request": "launch",
			"type": "chrome",
			"url": "http://localhost:5173",
			"webRoot": "${workspaceFolder}"
		}
	]
}
```
