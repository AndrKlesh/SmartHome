import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useUser } from './UserContext';
import './Authpage.css';

interface FormData {
    username: string;
    password: string;
}

const Authpage: React.FC = () => {

    const [formData, setFormData] = useState<FormData>({
        username: "",
        password: ""
    });

    const [errors, setErrors] = useState<string | null>(null);
    const navigate = useNavigate();
    const { setUser } = useUser();

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        setFormData({
            ...formData,
            [e.target.name]: e.target.value
        });
    };


    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();

        /*if (formData.password !== formData.confirmPassword) {
            setErrors("Passwords do not match");
            return;
        }*/

        if (formData.password.length < 6) {
            setErrors("Invalid password");
            return;
        }

        setErrors(null);
        setUser({ username: formData.username });
        navigate("/");
        console.log("Form submitted successfully", formData);
    };

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

                <button type="submit">
                    Login
                </button>

            </form>
        </div>
    );
};
export default Authpage;


