import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import './Header.css';

const Header = ({ activePage }) => {
    const navigate = useNavigate();
    const { user, logout } = useAuth();

    const handleLogout = () => {
        logout();
        navigate('/login');
    };

    return (
        <header className="main-header">
            <div className="header-content">
                <h1 className="logo" onClick={() => navigate('/flights')}>
                    ✈️ AirlineAPI
                </h1>
                <nav className="header-nav">
                    <button 
                        className={`nav-btn ${activePage === 'flights' ? 'active' : ''}`}
                        onClick={() => navigate('/flights')}
                    >
                        Рейсы
                    </button>
                    <button 
                        className={`nav-btn ${activePage === 'profile' ? 'active' : ''}`}
                        onClick={() => navigate('/profile')}
                    >
                        Профиль
                    </button>
                </nav>
                <div className="user-info">
                    <span className="user-name">{user?.username}</span>
                    <button className="logout-btn" onClick={handleLogout}>
                        Выйти
                    </button>
                </div>
            </div>
        </header>
    );
};

export default Header;
