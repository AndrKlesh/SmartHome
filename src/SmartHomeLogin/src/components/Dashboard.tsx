import { useParams } from 'react-router-dom'

const Dashboard: React.FC = () => {
	const { username } = useParams()

	return (
		<div style={{ padding: '50px' }}>
			<h1>Добро пожаловать, {username}</h1>
			<p>Это ваша личная панель управления.</p>
		</div>
	)
}

export default Dashboard
