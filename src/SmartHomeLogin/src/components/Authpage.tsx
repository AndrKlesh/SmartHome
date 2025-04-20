import { useState, useRef, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { API_BASE_URL } from '../config'
import './style.css'

interface FormData {
	username: string
	password: string
}
interface RegisterData extends FormData { }

const Authpage: React.FC = () => {

	const navigate = useNavigate()

	// Форма входа
	const [loginData, setLoginData] = useState<FormData>({
		username: '',
		password: ''
	})

	// Форма регистрации
	const [registerData, setRegisterData] = useState<RegisterData>({
		username: '',
		password: ''
	})

	// Состояния ошибок 
	const [loginError, setLoginErrors] = useState<string | null>(null)
	const [registerError, setRegisterError] = useState<string | null>(null)
	const [isRegistering, setIsRegistering] = useState(false)

	const containerRef = useRef<HTMLDivElement>(null)

	useEffect(() => {
		if (containerRef.current) {
			if (isRegistering) {
				containerRef.current.classList.add('active')
			} else {
				containerRef.current.classList.remove('active')
			}
		}
	}, [isRegistering])

	// Обработчик изменения для формы входа
	const handleChangeLogin = (e: React.ChangeEvent<HTMLInputElement>) => {
		setLoginData({
			...loginData,
			[e.target.name]: e.target.value
		})
	}

	// Обработчик изменения для формы регистрации
	const handleChangeRegister = (e: React.ChangeEvent<HTMLInputElement>) => {
		setRegisterData({
			...registerData,
			[e.target.name]: e.target.value
		})
	}


	// Отправка данных входа на сервер 
	const handleLoginSubmit = async (e: React.FormEvent) => {
		e.preventDefault();
		setLoginErrors(null);

		try {

			const response = await fetch(API_BASE_URL + '/auth/login', {
				method: 'POST',
				headers: { 'Content-Type': 'application/json' },
				body: JSON.stringify({
					username: loginData.username,
					password: loginData.password,
				}),
			});

			console.log('Response status:', response.status);

			if (response.ok) {
				// TODO
				// const data = await response.json()
				setLoginData({ username: '', password: '' }) // очищаем форму
				// редирект на dashboard
				navigate(`/dashboard/${loginData.username}`) 
			}
			else {
				const errorData = await response.json();
				setLoginErrors(errorData.message || 'Ошибка входа');

				// редирект на страницу "нет доступа"
				navigate('/dashboard/no-access')
			}
		}
		catch (error) {
			console.error('Ошибка при входе:', error);
			setLoginErrors('Произошла ошибка при входе');
		}
	}

	// Отправка данных регистрации пользователя на сервер 
	const handleRegSubmit = async (e: React.FormEvent) => {
		e.preventDefault();
		setRegisterError(null);

		// TODO
		// Провека токена и пароля пользователя

		try {

			const response = await fetch(API_BASE_URL + '/auth/register', {
				method: 'POST',
				headers: { 'Content-Type': 'application/json' },
				body: JSON.stringify({
					username: registerData.username,
					password: registerData.password,
				}),
			});
			console.log('Response status:', response.status);

			if (response.ok) {
				// TODO
				setRegisterData({ username: '', password: '' })
				setIsRegistering(false);
			}
			else {
				const errorData = await response.json();
				setRegisterError(errorData.message || 'Ошибка регистрации');
			}
		}
		catch (error) {
			console.error('Ошибка регистрации:', error);
			setRegisterError('Произошла ошибка регистрации');
		}
	}

	return (
		<div className="container" ref={containerRef}>
			<div className="form-container sign-up">
				<form onSubmit={handleRegSubmit}>
					<h1>Создать аккаунт</h1>
					<div className="social-icons">
						<a href="#" className="icons">
							<i className="fa-brands fa-vk"></i>
						</a>
						<a href="#" className="icons">
							<i className="fa-solid fa-at"></i>
						</a>
					</div>
					<span>придумайте логин и пароль для регистрации аккаунта</span>
					<input type="text"
						id="register-username"
						name="username"
						value={registerData.username}
						onChange={handleChangeRegister}
						required
						placeholder="username" />


					<input type="password"
						id="register-password"
						name="password"
						value={registerData.password}
						onChange={handleChangeRegister}
						required
						placeholder="password" />
					<button type="submit">Создать</button>

					{registerError && <p style={{ color: 'red' }}>{registerError}</p>}

				</form>
			</div>

			<div className="form-container sign-in">
				<form onSubmit={handleLoginSubmit}>
					<h1>Вход в аккаунт</h1>
					<div className="social-icons">
						<a href="#" className="icons">
							<i className="fa-brands fa-vk"></i>
						</a>
						<a href="#" className="icons">
							<i className="fa-solid fa-at"></i>
						</a>
					</div>
					<span>используйте логин и пароль для входа в аккаунт</span>

					<input type="text"
						id="login-username"
						name="username"
						value={loginData.username}
						onChange={handleChangeLogin}
						required
						placeholder="username" />


					<input type="password"
						id="login-password"
						name="password"
						value={loginData.password}
						onChange={handleChangeLogin}
						required
						placeholder="password" />
					<a href="#">Забыли пароль?</a>
					<button type="submit">Вход</button>
					{loginError && <p style={{ color: 'red' }}>{loginError}</p>}
				</form>
			</div>

			<div className="toggle-container">
				<div className="toggle">
					<div className="toggle-panel toggle-left">
						<h1>С возвращением!</h1>
						<p>Войдите в аккаунт</p>
						<button className="hidden" onClick={() => setIsRegistering(false)}>
							Вход
						</button>
					</div>

					<div className="toggle-panel toggle-right">
						<h1>Добро пожаловать!</h1>
						<p>Создайте аккаунт</p>
						<button className="hidden" onClick={() => setIsRegistering(true)}>
							Создать
						</button>
					</div>
				</div>

			</div>
		</div>

	);
};

export default Authpage;
