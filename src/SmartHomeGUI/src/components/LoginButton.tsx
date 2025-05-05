import {Button} from '@mui/material'
import {useTheme} from '@mui/material/styles'
import React from 'react'
import {useNavigate} from 'react-router-dom'

const LoginButton: React.FC = () =>
{
	const navigate = useNavigate()
	const theme = useTheme()

	const handleLogin = (): void =>
	{
		navigate("/login")
	}

	return (
		<Button
			variant="outlined"
			color="primary"
			onClick={handleLogin}
			sx={{
				borderRadius: 2,
				color: theme.palette.primary.main,
				borderColor: theme.palette.primary.main,
				'&:hover': {
					backgroundColor: theme.palette.primary.light,
				}
			}}
		>
			Login
		</Button>
	)
}

export default LoginButton
