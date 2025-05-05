import {Alert, AlertTitle} from '@mui/material'
import {useTheme} from '@mui/material/styles'
import React from 'react'

interface ErrorMessageProps
{
	message: string
}

const ErrorMessage: React.FC<ErrorMessageProps> = ({message}: ErrorMessageProps) =>
{
	const theme = useTheme()

	return (
		<Alert
			severity="error"
			sx={{
				borderRadius: 2,
				boxShadow: 2,
				backgroundColor: theme.palette.error.light,
				color: theme.palette.error.contrastText,
				'& .MuiAlert-icon': {
					color: theme.palette.error.main,
				},
				'& .MuiAlert-message': {
					fontWeight: 500,
				},
			}}
		>
			<AlertTitle sx={{fontWeight: 600}}>
				Ошибка
			</AlertTitle>
			{message}
		</Alert>
	)
}

export default ErrorMessage
