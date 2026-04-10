import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import flightService from '../services/flightService';
import ticketService from '../services/ticketService';
import './FlightDetailPage.css';

const FlightDetailPage = () => {
    const { id } = useParams();
    const navigate = useNavigate();

    const [flight, setFlight] = useState(null);
    const [seats, setSeats] = useState([]);
    const [occupiedSeatIds, setOccupiedSeatIds] = useState(new Set());
    const [selectedSeatId, setSelectedSeatId] = useState(null);
    const [myTickets, setMyTickets] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [notification, setNotification] = useState(null);

    useEffect(() => {
        loadFlightDetails();
        loadMyTickets();
    }, [id]);

    const loadFlightDetails = async () => {
        try {
            setLoading(true);
            const flightData = await flightService.getDetails(id);
            setFlight(flightData);

            // Загружаем все места самолета
            const allSeats = flightData.airplane?.seats || [];
            setSeats(allSeats);

            // Загружаем занятые места
            const occupiedSeats = await ticketService.getOccupiedSeats(id);
            const occupiedIds = new Set(occupiedSeats.map(s => s.id));
            setOccupiedSeatIds(occupiedIds);
            
            // Подсчитываем проданные и забронированные билеты на основе данных о билетах
            const tickets = flightData.tickets || [];
            const soldCount = tickets.filter(t => t.bookingStatus === 'Оплачен').length;
            const reservedCount = tickets.filter(t => t.bookingStatus === 'Зарезервирован').length;
            setFlight(prev => ({
                ...prev,
                soldTickets: soldCount,
                reservedTickets: reservedCount
            }));
        } catch (err) {
            setError('Не удалось загрузить информацию о рейсе');
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    const loadMyTickets = async () => {
        try {
            const tickets = await ticketService.getMyTickets();
            // Фильтруем билеты только для этого рейса
            const flightTickets = tickets.filter(t => t.flightId === parseInt(id));
            setMyTickets(flightTickets);
        } catch (err) {
            console.error('Не удалось загрузить билеты:', err);
        }
    };

    const handleSeatClick = (seat) => {
        if (occupiedSeatIds.has(seat.id)) {
            return; // Место занято
        }

        // Если место уже выбрано в текущей сессии - снимаем выделение
        if (selectedSeatId === seat.id) {
            setSelectedSeatId(null);
        } else {
            setSelectedSeatId(seat.id);
        }
    };

    const handleReserve = async () => {
        if (!selectedSeatId) {
            showNotification('Выберите место для бронирования', 'error');
            return;
        }

        try {
            await ticketService.reserveSeat(parseInt(id), selectedSeatId);
            showNotification('Место успешно забронировано!', 'success');
            setSelectedSeatId(null);
            await loadFlightDetails();
            await loadMyTickets();
        } catch (err) {
            const message = err.response?.data?.message || 'Не удалось забронировать место';
            showNotification(message, 'error');
        }
    };

    const handlePay = async (ticketId) => {
        try {
            await ticketService.payTicket(ticketId);
            showNotification('Билет успешно оплачен!', 'success');
            await loadMyTickets();
        } catch (err) {
            const message = err.response?.data?.message || 'Не удалось оплатить билет';
            showNotification(message, 'error');
        }
    };

    const handleCancel = async (ticketId) => {
        if (!window.confirm('Вы уверены, что хотите отменить билет?')) {
            return;
        }

        try {
            await ticketService.cancelTicket(ticketId);
            showNotification('Билет успешно отменен', 'success');
            await loadMyTickets();
            await loadFlightDetails();
        } catch (err) {
            const message = err.response?.data?.message || 'Не удалось отменить билет';
            showNotification(message, 'error');
        }
    };

    const showNotification = (message, type) => {
        setNotification({ message, type });
        setTimeout(() => setNotification(null), 3000);
    };

    const isSeatOccupied = (seatId) => {
        return occupiedSeatIds.has(seatId);
    };

    const getTicketForSeat = (seatId) => {
        return myTickets.find(t => t.seatId === seatId && t.bookingStatus !== 'Отменен');
    };

    if (loading) {
        return <div className="loading">Загрузка информации о рейсе...</div>;
    }

    if (error || !flight) {
        return <div className="error">{error || 'Рейс не найден'}</div>;
    }

    const availableSeatsCount = seats.length - occupiedSeatIds.size;

    return (
        <div className="flight-detail-page">
            <button className="back-button" onClick={() => navigate('/flights')}>
                ← Назад к списку рейсов
            </button>

            {notification && (
                <div className={`notification ${notification.type}`}>
                    {notification.message}
                </div>
            )}

            {/* Информация о рейсе */}
            <div className="flight-info-card">
                <div className="flight-info-header">
                    <span className="flight-number-large">{flight.flightNumber}</span>
                    <span className="airplane-info">{flight.airplane?.model}</span>
                </div>

                <div className="flight-route-detail">
                    <div className="airport-detail departure">
                        <span className="airport-name">{flight.departureAirport?.name}</span>
                        <span className="airport-location">{flight.departureAirport?.location}</span>
                    </div>
                    <span className="route-arrow-large">→</span>
                    <div className="airport-detail arrival">
                        <span className="airport-name">{flight.arrivalAirport?.name}</span>
                        <span className="airport-location">{flight.arrivalAirport?.location}</span>
                    </div>
                </div>

                <div className="flight-times">
                    <div className="time-block">
                        <span className="time-label">Вылет</span>
                        <span className="time-value-large">
                            {new Date(flight.departureTime).toLocaleTimeString('ru-RU', {
                                hour: '2-digit',
                                minute: '2-digit'
                            })}
                        </span>
                        <span className="date-value">
                            {new Date(flight.departureTime).toLocaleDateString('ru-RU', {
                                day: '2-digit',
                                month: '2-digit',
                                year: 'numeric'
                            })}
                        </span>
                    </div>
                    <div className="time-block">
                        <span className="time-label">Прилёт</span>
                        <span className="time-value-large">
                            {new Date(flight.arrivalTime).toLocaleTimeString('ru-RU', {
                                hour: '2-digit',
                                minute: '2-digit'
                            })}
                        </span>
                        <span className="date-value">
                            {new Date(flight.arrivalTime).toLocaleDateString('ru-RU', {
                                day: '2-digit',
                                month: '2-digit',
                                year: 'numeric'
                            })}
                        </span>
                    </div>
                </div>

                <div className="flight-status">
                    <div className="status-item">
                        <span className="status-label">Всего мест</span>
                        <span className="status-value">{seats.length}</span>
                    </div>
                    <div className="status-item">
                        <span className="status-label">Занято</span>
                        <span className="status-value">{occupiedSeatIds.size}</span>
                    </div>
                    <div className="status-item">
                        <span className="status-label">Доступно</span>
                        <span className="status-value available">{availableSeatsCount}</span>
                    </div>
                    <div className="status-item">
                        <span className="status-label">Продано</span>
                        <span className="status-value">{flight.soldTickets}</span>
                    </div>
                    <div className="status-item">
                        <span className="status-label">Забронировано</span>
                        <span className="status-value">{flight.reservedTickets}</span>
                    </div>
                </div>
            </div>

            {/* Сетка мест */}
            <div className="seats-section">
                <h2>Выберите место</h2>

                <div className="seats-legend">
                    <div className="legend-item">
                        <div className="legend-seat available">A</div>
                        <span>Свободно</span>
                    </div>
                    <div className="legend-item">
                        <div className="legend-seat occupied">X</div>
                        <span>Занято</span>
                    </div>
                    <div className="legend-item">
                        <div className="legend-seat selected">✓</div>
                        <span>Выбрано</span>
                    </div>
                </div>

                <div className="seats-grid-container">
                    <div className="seats-grid">
                        {seats.map((seat, index) => {
                            const isOccupied = isSeatOccupied(seat.id);
                            const isSelected = selectedSeatId === seat.id;

                            let className = 'seat';
                            if (isOccupied) {
                                className += ' occupied';
                            } else if (isSelected) {
                                className += ' selected';
                            } else {
                                className += ' available';
                            }

                            // Разделяем места на группы по 6 (для прохода)
                            if (index > 0 && index % 6 === 0 && index < seats.length - 1) {
                                return (
                                    <>
                                        <div key={`aisle-${index}`} className="aisle"></div>
                                        <div
                                            key={seat.id}
                                            className={className}
                                            onClick={() => handleSeatClick(seat)}
                                        >
                                            <span className="seat-number">{seat.seatNumber}</span>
                                            <span className="seat-sector">{seat.sector.substring(0, 3)}</span>
                                        </div>
                                    </>
                                );
                            }

                            return (
                                <div
                                    key={seat.id}
                                    className={className}
                                    onClick={() => handleSeatClick(seat)}
                                >
                                    <span className="seat-number">{seat.seatNumber}</span>
                                    <span className="seat-sector">{seat.sector.substring(0, 3)}</span>
                                </div>
                            );
                        })}
                    </div>
                </div>

                <div className="seat-actions">
                    <button
                        className="reserve-btn"
                        onClick={handleReserve}
                        disabled={!selectedSeatId}
                    >
                        Забронировать место
                    </button>
                </div>
            </div>

            {/* Мои билеты на этот рейс */}
            {myTickets.length > 0 && (
                <div className="my-tickets-section">
                    <h2>Мои билеты на этот рейс</h2>
                    <div className="tickets-list">
                        {myTickets.map(ticket => (
                            <div key={ticket.id} className="ticket-card">
                                <div className="ticket-info">
                                    <div className="ticket-seat">
                                        Место: {ticket.seat?.seatNumber || `#${ticket.seatId}`} ({ticket.seat?.sector || 'N/A'})
                                    </div>
                                    <span className={`ticket-status ${ticket.bookingStatus.toLowerCase()}`}>
                                        {ticket.bookingStatus}
                                    </span>
                                </div>
                                <div className="ticket-actions">
                                    {ticket.bookingStatus === 'Зарезервирован' && (
                                        <>
                                            <button
                                                className="pay-btn"
                                                onClick={() => handlePay(ticket.id)}
                                            >
                                                Оплатить
                                            </button>
                                            <button
                                                className="cancel-btn"
                                                onClick={() => handleCancel(ticket.id)}
                                            >
                                                Отменить
                                            </button>
                                        </>
                                    )}
                                    {ticket.bookingStatus === 'Оплачен' && (
                                        <button
                                            className="cancel-btn"
                                            onClick={() => handleCancel(ticket.id)}
                                        >
                                            Отменить
                                        </button>
                                    )}
                                </div>
                            </div>
                        ))}
                    </div>
                </div>
            )}
        </div>
    );
};

export default FlightDetailPage;