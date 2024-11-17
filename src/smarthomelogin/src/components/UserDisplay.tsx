import React from 'react';
import { useUser } from './UserContext';

const UserDisplay: React.FC = () => {
    const { user } = useUser();

    return (
        <div>
            {user ? <a>{user.username}</a> : <a></a>}
        </div>
    );
};

export default UserDisplay;
