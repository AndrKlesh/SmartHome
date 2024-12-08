import { Link } from 'react-router-dom'
import LogoutButton from './LogoutButton'
import './styles.css'

function Header ()
{
	return (
		<header>
			<nav>
				<ul>
					<li>
						<Link to="/">Home</Link>
					</li>
					<li>
						<Link to="/dashboard">Dashboard</Link>
					</li>

					<li>
						<Link to="/login">Login</Link>
					</li>

					<li>
						<Link to="/login"><LogoutButton /></Link>
					</li>

					<li>
						<Link to="/settings">Settings</Link>
					</li>
				</ul>
			</nav>
		</header>
	)
}

export default Header
