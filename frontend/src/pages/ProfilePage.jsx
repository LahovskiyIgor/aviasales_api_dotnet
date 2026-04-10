import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import profileService from '../services/profileService';
import ticketService from '../services/ticketService';
import './ProfilePage.css';

const ProfilePage = () => {
    const navigate = useNavigate();
    const { user, logout } = useAuth();
    
    // Состояние профиля
    const [passenger, setPassenger] = useState(null);
    const [isEditing, setIsEditing] = useState(false);
    const [editForm, setEditForm] = useState({
        firstName: '',
        lastName: '',
        middleName: '',
        email: '',
        phone: ''
    });
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [saveError, setSaveError] = useState(null);
    const [saveSuccess, setSaveSuccess] = useState(false);

    // Состояние билетов
    const [tickets, setTickets] = useState([]);
    const [activeTab, setActiveTab] = useState('upcoming'); // 'upcoming' или 'past'
    const [showTicketModal, setShowTicketModal] = useState(false);
    const [selectedTicket, setSelectedTicket] = useState(null);
    const [cancelLoading, setCancelLoading] = useState(null);

    useEffect(() => {
        loadProfile();
        loadTickets();
    }, []);

    const loadProfile = async () => {
        try {
            setLoading(true);
            const data = await profileService.getMyProfile();
            setPassenger(data);
            setEditForm({
                firstName: data.firstName || '',
                lastName: data.lastName || '',
                middleName: data.middleName || '',
                email: data.email || '',
                phone: data.phone || ''
            });
        } catch (err) {
            setError('Не удалось загрузить данные профиля');
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    const loadTickets = async () => {
        try {
            const data = await profileService.getMyTickets();
            setTickets(data);
        } catch (err) {
            console.error('Failed to load tickets:', err);
        }
    };

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setEditForm(prev => ({
            ...prev,
            [name]: value
        }));
    };

    const handleSaveProfile = async () => {
        try {
            setSaveError(null);
            setSaveSuccess(false);
            await profileService.updateProfile(editForm);
            setSaveSuccess(true);
            setIsEditing(false);
            // Обновляем данные профиля
            const updatedData = await profileService.getMyProfile();
            setPassenger(updatedData);
            
            setTimeout(() => setSaveSuccess(false), 3000);
        } catch (err) {
            setSaveError('Не удалось сохранить изменения. Проверьте корректность данных.');
            console.error(err);
        }
    };

    const handleCancelEdit = () => {
        if (passenger) {
            setEditForm({
                firstName: passenger.firstName || '',
                lastName: passenger.lastName || '',
                middleName: passenger.middleName || '',
                email: passenger.email || '',
                phone: passenger.phone || ''
            });
        }
        setIsEditing(false);
        setSaveError(null);
        setSaveSuccess(false);
    };

    const handleShowTicket = (ticket) => {
        setSelectedTicket(ticket);
        setShowTicketModal(true);
    };

    const handleCloseTicketModal = () => {
        setShowTicketModal(false);
        setSelectedTicket(null);
    };

    const handleCancelTicket = async (ticketId) => {
        if (!window.confirm('Вы уверены, что хотите отменить этот билет?')) {
            return;
        }

        try {
            setCancelLoading(ticketId);
            await ticketService.cancelTicket(ticketId);
            // Обновляем список билетов
            await loadTickets();
        } catch (err) {
            alert('Не удалось отменить билет. Пожалуйста, попробуйте позже.');
            console.error(err);
        } finally {
            setCancelLoading(null);
        }
    };

    const handleLogout = () => {
        logout();
        navigate('/login');
    };

    // Фильтрация билетов
    const filteredTickets = tickets.filter(ticket => {
        const flightDate = new Date(ticket.flight?.departureTime);
        const now = new Date();
        const isUpcoming = flightDate >= now;
        const isActiveStatus = ticket.bookingStatus === 'Оплачен' || ticket.bookingStatus === 'Зарезервирован';
        
        if (activeTab === 'upcoming') {
            return isUpcoming && isActiveStatus;
        } else {
            return !isUpcoming || ticket.bookingStatus === 'Отменен';
        }
    });

    if (loading) {
        return <div className="loading">Загрузка профиля...</div>;
    }

    const calculatedPrice = selectedTicket 
        ? (selectedTicket.calculatedPrice || (selectedTicket.flight?.basePrice * selectedTicket.seat?.priceMultiplier) || 0)
        : 0;

    const qrValue = selectedTicket ? JSON.stringify({
        ticketId: selectedTicket.id,
        flightNumber: selectedTicket.flight?.flightNumber,
        passenger: `${selectedTicket.passenger?.firstName} ${selectedTicket.passenger?.lastName}`,
        seat: selectedTicket.seat?.seatNumber,
        departure: selectedTicket.flight?.departureTime,
        status: selectedTicket.bookingStatus
    }) : '';

    return (
        <div className="profile-page">
            {/* Header */}
            <header className="profile-header">
                <div className="header-content">
                    <h1 className="logo" onClick={() => navigate('/flights')}>
                        ✈️ AirlineAPI
                    </h1>
                    <nav className="header-nav">
                        <button className="nav-btn" onClick={() => navigate('/flights')}>
                            Рейсы
                        </button>
                        <button className="nav-btn active" onClick={() => navigate('/profile')}>
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

            <div className="profile-content">
                {/* Личные данные */}
                <section className="profile-section personal-data">
                    <div className="section-header">
                        <h2>Личные данные</h2>
                        {!isEditing && (
                            <button className="edit-btn" onClick={() => setIsEditing(true)}>
                                Редактировать
                            </button>
                        )}
                    </div>

                    {isEditing ? (
                        <div className="edit-form">
                            <div className="form-row">
                                <div className="form-group">
                                    <label htmlFor="lastName">Фамилия</label>
                                    <input
                                        type="text"
                                        id="lastName"
                                        name="lastName"
                                        value={editForm.lastName}
                                        onChange={handleInputChange}
                                    />
                                </div>
                                <div className="form-group">
                                    <label htmlFor="firstName">Имя</label>
                                    <input
                                        type="text"
                                        id="firstName"
                                        name="firstName"
                                        value={editForm.firstName}
                                        onChange={handleInputChange}
                                    />
                                </div>
                                <div className="form-group">
                                    <label htmlFor="middleName">Отчество</label>
                                    <input
                                        type="text"
                                        id="middleName"
                                        name="middleName"
                                        value={editForm.middleName}
                                        onChange={handleInputChange}
                                    />
                                </div>
                            </div>
                            <div className="form-row">
                                <div className="form-group">
                                    <label htmlFor="email">Email</label>
                                    <input
                                        type="email"
                                        id="email"
                                        name="email"
                                        value={editForm.email}
                                        onChange={handleInputChange}
                                    />
                                </div>
                                <div className="form-group">
                                    <label htmlFor="phone">Телефон</label>
                                    <input
                                        type="tel"
                                        id="phone"
                                        name="phone"
                                        value={editForm.phone}
                                        onChange={handleInputChange}
                                    />
                                </div>
                            </div>
                            
                            {saveError && <div className="error-message">{saveError}</div>}
                            {saveSuccess && <div className="success-message">Данные успешно сохранены!</div>}
                            
                            <div className="form-actions">
                                <button className="save-btn" onClick={handleSaveProfile}>
                                    Сохранить
                                </button>
                                <button className="cancel-btn" onClick={handleCancelEdit}>
                                    Отмена
                                </button>
                            </div>
                        </div>
                    ) : (
                        <div className="personal-info">
                            <div className="info-row">
                                <span className="info-label">ФИО:</span>
                                <span className="info-value">
                                    {passenger?.lastName} {passenger?.firstName} {passenger?.middleName}
                                </span>
                            </div>
                            <div className="info-row">
                                <span className="info-label">Email:</span>
                                <span className="info-value">{passenger?.email}</span>
                            </div>
                            <div className="info-row">
                                <span className="info-label">Телефон:</span>
                                <span className="info-value">{passenger?.phone}</span>
                            </div>
                        </div>
                    )}
                </section>

                {/* Мои бронирования */}
                <section className="profile-section bookings-section">
                    <div className="section-header">
                        <h2>Мои бронирования</h2>
                    </div>

                    <div className="tabs">
                        <button 
                            className={`tab-btn ${activeTab === 'upcoming' ? 'active' : ''}`}
                            onClick={() => setActiveTab('upcoming')}
                        >
                            Предстоящие
                        </button>
                        <button 
                            className={`tab-btn ${activeTab === 'past' ? 'active' : ''}`}
                            onClick={() => setActiveTab('past')}
                        >
                            Прошедшие
                        </button>
                    </div>

                    <div className="tickets-list">
                        {filteredTickets.length === 0 ? (
                            <p className="no-tickets">
                                {activeTab === 'upcoming' 
                                    ? 'У вас нет предстоящих рейсов' 
                                    : 'Архив поездок пуст'}
                            </p>
                        ) : (
                            filteredTickets.map(ticket => (
                                <div key={ticket.id} className="ticket-card">
                                    <div className="ticket-header">
                                        <span className="ticket-number">Билет № {ticket.id}</span>
                                        <span className={`ticket-status ${ticket.bookingStatus.toLowerCase()}`}>
                                            {ticket.bookingStatus}
                                        </span>
                                    </div>
                                    
                                    <div className="ticket-route">
                                        <div className="route-point">
                                            <span className="airport-code">{ticket.flight?.departureAirport?.name}</span>
                                            <span className="airport-city">{ticket.flight?.departureAirport?.location}</span>
                                        </div>
                                        <span className="route-arrow">→</span>
                                        <div className="route-point">
                                            <span className="airport-code">{ticket.flight?.arrivalAirport?.name}</span>
                                            <span className="airport-city">{ticket.flight?.arrivalAirport?.location}</span>
                                        </div>
                                    </div>

                                    <div className="ticket-info">
                                        <div className="info-item">
                                            <span className="label">Рейс:</span>
                                            <span className="value">{ticket.flight?.flightNumber}</span>
                                        </div>
                                        <div className="info-item">
                                            <span className="label">Дата вылета:</span>
                                            <span className="value">
                                                {new Date(ticket.flight?.departureTime).toLocaleDateString('ru-RU', {
                                                    day: '2-digit',
                                                    month: '2-digit',
                                                    year: 'numeric'
                                                })}
                                                {' '}
                                                {new Date(ticket.flight?.departureTime).toLocaleTimeString('ru-RU', {
                                                    hour: '2-digit',
                                                    minute: '2-digit'
                                                })}
                                            </span>
                                        </div>
                                        <div className="info-item">
                                            <span className="label">Место:</span>
                                            <span className="value">{ticket.seat?.seatNumber}</span>
                                        </div>
                                        <div className="info-item">
                                            <span className="label">Пассажир:</span>
                                            <span className="value">
                                                {ticket.passenger?.lastName} {ticket.passenger?.firstName}
                                            </span>
                                        </div>
                                    </div>

                                    <div className="ticket-actions">
                                        {(ticket.bookingStatus === 'Оплачен' || ticket.bookingStatus === 'Зарезервирован') && (
                                            <>
                                                <button 
                                                    className="show-ticket-btn"
                                                    onClick={() => handleShowTicket(ticket)}
                                                >
                                                    Показать билет
                                                </button>
                                                <button 
                                                    className="cancel-btn-small"
                                                    onClick={() => handleCancelTicket(ticket.id)}
                                                    disabled={cancelLoading === ticket.id}
                                                >
                                                    {cancelLoading === ticket.id ? 'Отмена...' : 'Отмена брони'}
                                                </button>
                                            </>
                                        )}
                                    </div>
                                </div>
                            ))
                        )}
                    </div>
                </section>
            </div>

            {/* Modal для просмотра билета */}
            {showTicketModal && selectedTicket && (
                <div className="modal-overlay" onClick={handleCloseTicketModal}>
                    <div className="modal-content" onClick={(e) => e.stopPropagation()}>
                        <button className="modal-close" onClick={handleCloseTicketModal}>×</button>
                        
                        <div className="ticket-view">
                            <div className="ticket-view-header">
                                <div className="airline-logo">✈️ AirlineAPI</div>
                                <div className={`ticket-status-badge ${selectedTicket.bookingStatus.toLowerCase()}`}>
                                    {selectedTicket.bookingStatus}
                                </div>
                            </div>

                            <div className="ticket-view-body">
                                <div className="ticket-row">
                                    <div className="ticket-info-block">
                                        <span className="info-label">Рейс</span>
                                        <span className="info-value flight-number">
                                            {selectedTicket.flight?.flightNumber || `#${selectedTicket.flightId}`}
                                        </span>
                                    </div>
                                    <div className="ticket-info-block">
                                        <span className="info-label">Пассажир</span>
                                        <span className="info-value">
                                            {selectedTicket.passenger?.firstName} {selectedTicket.passenger?.lastName}
                                        </span>
                                    </div>
                                </div>

                                <div className="ticket-route-view">
                                    <div className="route-point departure">
                                        <span className="airport-name">{selectedTicket.flight?.departureAirport?.name}</span>
                                        <span className="airport-location">{selectedTicket.flight?.departureAirport?.location}</span>
                                        <span className="departure-time">
                                            {new Date(selectedTicket.flight?.departureTime).toLocaleTimeString('ru-RU', {
                                                hour: '2-digit',
                                                minute: '2-digit'
                                            })}
                                        </span>
                                        <span className="departure-date">
                                            {new Date(selectedTicket.flight?.departureTime).toLocaleDateString('ru-RU', {
                                                day: '2-digit',
                                                month: 'short'
                                            })}
                                        </span>
                                    </div>
                                    
                                    <div className="route-arrow">→</div>
                                    
                                    <div className="route-point arrival">
                                        <span className="airport-name">{selectedTicket.flight?.arrivalAirport?.name}</span>
                                        <span className="airport-location">{selectedTicket.flight?.arrivalAirport?.location}</span>
                                        <span className="arrival-time">
                                            {new Date(selectedTicket.flight?.arrivalTime).toLocaleTimeString('ru-RU', {
                                                hour: '2-digit',
                                                minute: '2-digit'
                                            })}
                                        </span>
                                        <span className="arrival-date">
                                            {new Date(selectedTicket.flight?.arrivalTime).toLocaleDateString('ru-RU', {
                                                day: '2-digit',
                                                month: 'short'
                                            })}
                                        </span>
                                    </div>
                                </div>

                                <div className="ticket-details-row">
                                    <div className="ticket-info-block">
                                        <span className="info-label">Место</span>
                                        <span className="info-value seat">{selectedTicket.seat?.seatNumber || `#${selectedTicket.seatId}`}</span>
                                        <span className="seat-class">{selectedTicket.seat?.sector}</span>
                                    </div>
                                    <div className="ticket-info-block">
                                        <span className="info-label">Самолёт</span>
                                        <span className="info-value">{selectedTicket.flight?.airplane?.model || 'N/A'}</span>
                                    </div>
                                    <div className="ticket-info-block price-block">
                                        <span className="info-label">Цена</span>
                                        <span className="info-value price">{calculatedPrice} ₽</span>
                                    </div>
                                </div>

                                <div className="ticket-qr-view">
                                    {selectedTicket.bookingStatus === 'Оплачен' ? (
                                        <>
                                            <img 
                                                src={`https://api.qrserver.com/v1/create-qr-code/?size=150x150&data=${encodeURIComponent(qrValue)}`}
                                                alt="QR Code"
                                                className="qr-code-img"
                                            />
                                            <span className="qr-label">Отсканируйте для проверки</span>
                                        </>
                                    ) : (
                                        <div className="qr-placeholder">
                                            <span>QR-код доступен только для оплаченных билетов</span>
                                        </div>
                                    )}
                                </div>
                            </div>

                            <div className="ticket-view-footer">
                                <span className="ticket-id">Билет № {selectedTicket.id}</span>
                                <span className="booking-date">
                                    Дата покупки: {new Date().toLocaleDateString('ru-RU')}
                                </span>
                            </div>
                        </div>

                        <div className="modal-actions">
                            <button className="print-btn-modal" onClick={() => window.print()}>
                                📄 Распечатать
                            </button>
                            <button className="close-modal-btn" onClick={handleCloseTicketModal}>
                                Закрыть
                            </button>
                        </div>
                    </div>
                </div>
            )}

            {/* Footer */}
            <footer className="profile-footer">
                <div className="footer-content">
                    <div className="footer-links">
                        <a href="#" className="footer-link">О нас</a>
                        <a href="#" className="footer-link">Политика конфиденциальности</a>
                        <a href="#" className="footer-link">Условия использования</a>
                        <a href="#" className="footer-link">Контакты</a>
                    </div>
                    <div className="footer-copyright">
                        © 2024 AirlineAPI. Все права защищены.
                    </div>
                </div>
            </footer>
        </div>
    );
};

export default ProfilePage;
