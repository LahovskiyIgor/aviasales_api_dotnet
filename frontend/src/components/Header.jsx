import { useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import './Header.css';

const Header = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { user, logout } = useAuth();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const isActive = (path) => location.pathname === path;

  return (
    <header className="header">
      <div className="header-container">
        <div className="header-logo" onClick={() => navigate('/flights')}>
          <span className="logo-icon">✈️</span>
          <span className="logo-text">AirlineAPI</span>
        </div>

        <nav className="header-nav">
          {user && (
            <>
              <button
                className={`nav-link ${isActive('/flights') ? 'active' : ''}`}
                onClick={() => navigate('/flights')}
              >
                Рейсы
              </button>
            </>
          )}
        </nav>

        <div className="header-user">
          {user && (
            <>
              <span className="user-info">
                <span className="user-name">{user.username}</span>
                <span className="user-role">{user.role === 'Admin' ? 'Администратор' : 'Пассажир'}</span>
              </span>
              <button className="logout-btn" onClick={handleLogout}>
                Выйти
              </button>
            </>
          )}
        </div>
      </div>
    </header>
  );
};

export default Header;
