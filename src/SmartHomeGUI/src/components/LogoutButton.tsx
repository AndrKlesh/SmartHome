import React from 'react';
import { useNavigate } from 'react-router-dom';
import { useUser } from './UserContext';

const LogoutButton: React.FC = () => {
    const { setUser } = useUser();
    const navigate = useNavigate();

    const handleLogout = () => {
        setUser(null); // Очищаем данные пользователя
        navigate("/login"); // Перенаправляем на страницу входа
    };

    return (
        <div onClick={handleLogout}>
            Logout
        </div>
    );
};

export default LogoutButton;
