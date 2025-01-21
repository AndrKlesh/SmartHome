import React from 'react'
import {useNavigate} from 'react-router-dom'


const LogoutButton: React.FC = () =>
{
	const navigate = useNavigate()

	const handleLogout = () =>
	{
		navigate("/login") // Перенаправляем на страницу входа
	}

	return (
		<div onClick={handleLogout}>
			Logout
		</div>
	)
}

export default LogoutButton
