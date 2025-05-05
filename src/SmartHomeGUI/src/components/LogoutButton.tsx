import {Button} from '@mui/material'
import {useTheme} from '@mui/material/styles'
import React from 'react'
import {useNavigate} from 'react-router-dom'

const LogoutButton: React.FC = () =>
{
	const navigate = useNavigate()
	const theme = useTheme()

	const handleLogout = (): void =>
	{
		// Очистка данных аутентификации
		// localStorage.removeItem('authToken')  // Пример очистки токена
		// sessionStorage.removeItem('userData')  // Пример очистки данных пользователя

		navigate("/login")
	}

	return (
		<Button
			variant="outlined"
			color="primary"
			onClick={handleLogout}
			sx={{
				borderRadius: 2,
				color: theme.palette.primary.main,
				borderColor: theme.palette.primary.main,
				'&:hover': {
					backgroundColor: theme.palette.primary.light,
				},
			}}
		>
			Logout
		</Button>
	)
}

export default LogoutButton
