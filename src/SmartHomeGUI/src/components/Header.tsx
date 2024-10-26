import { Link } from 'react-router-dom'
import UserDisplay  from './UserDisplay';
import LogoutButton  from './LogoutButton';
import './Header.css'

function Header ()
{
	return (
		<header>
			<nav>
				<ul>
					<UserDisplay />
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
				</ul>
			</nav>
		</header>
	)
}

export default Header
