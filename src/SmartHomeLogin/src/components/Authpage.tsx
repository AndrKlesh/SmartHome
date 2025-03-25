import {useState} from 'react'
import {useNavigate} from 'react-router-dom'
import { useUser } from './UserContext'
import './Authpage.css'
import { API_BASE_URL } from '../config'

interface FormData
{
	username: string
	password: string
}

const Authpage: React.FC = () =>
{

	const [formData, setFormData] = useState<FormData>({
		username: '',
		password: ''
	})

	const [errors, setErrors] = useState<string | null>(null)
	const navigate = useNavigate()
	const {setUser} = useUser()

	const handleChange = (e: React.ChangeEvent<HTMLInputElement>) =>
	{
		setFormData({
			...formData,
			[e.target.name]: e.target.value
		})
	}

	// Отправка данных на сервер
	const handleSubmit = async (e: React.FormEvent) => { 
		e.preventDefault();
		setErrors(null);

		try {

			const response = await fetch(API_BASE_URL + '/auth/login', {
				method: 'POST',
				headers: { 'Content-Type': 'application/json' },
				body: JSON.stringify({
					username: formData.username,
					password: formData.password,
				}),
			});

			console.log('Response status:', response.status); // Логируем статус ответа
			/*const text = await response.text(); // Сначала читаем ответ как текст
			console.log('Response text:', text); // Логируем текст ответа */

			

			if (response.ok) {
				// TODO
			}
			else {
				const errorData = await response.json();
				setErrors(errorData.message || 'Ошибка входа');
			}
		}
		catch (error) {
			console.error('Ошибка при входе:', error);
			setErrors('Произошла ошибка при входе');
		}
	}

	return (
		<div className="registration-form">
			<h2>Login</h2>
			<form onSubmit={handleSubmit}>
				<div>
					<label htmlFor="username">Username</label>
					<input
						type="text"
						id="username"
						name="username"
						value={formData.username}
						onChange={handleChange}
						required
					/>
				</div>

				<div>
					<label htmlFor="password">Password</label>
					<input
						type="password"
						id="password"
						name="password"
						value={formData.password}
						onChange={handleChange}
						required
					/>
				</div>

				{errors && <p style={{ color: 'red' }}>{errors}</p>}

				<button type="submit">Login</button>
			</form>
		</div>
	);
};

export default Authpage;
