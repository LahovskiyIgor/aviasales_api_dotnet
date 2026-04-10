import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import './AdminHeader.css';

const AdminHeader = ({ activePage }) => {
    const navigate = useNavigate();
    const { user, logout } = useAuth();

    const handleLogout = () => {
        logout();
        navigate('/login');
    };

    return (
        <header className="admin-header">
            <div className="header-content">
                <h1 className="logo" onClick={() => navigate('/admin/flights')}>
                    ✈️ AirlineAPI - Админ панель
                </h1>
                <nav className="header-nav">
                    <button 
                        className={`nav-btn ${activePage === 'flights' ? 'active' : ''}`}
                        onClick={() => navigate('/admin/flights')}
                    >
                        Рейсы
                    </button>
                    <button 
                        className={`nav-btn ${activePage === 'airplanes' ? 'active' : ''}`}
                        onClick={() => navigate('/admin/airplanes')}
                    >
                        Самолёты
                    </button>
                    <button 
                        className={`nav-btn ${activePage === 'airports' ? 'active' : ''}`}
                        onClick={() => navigate('/admin/airports')}
                    >
                        Аэропорты
                    </button>
                </nav>
                <div className="user-info">
                    <span className="user-name">{user?.username} (Admin)</span>
                    <button className="logout-btn" onClick={handleLogout}>
                        Выйти
                    </button>
                </div>
            </div>
        </header>
    );
};

export default AdminHeader;
