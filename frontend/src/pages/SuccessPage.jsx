import { useState, useEffect } from 'react';
import { useParams, useNavigate, Link, useLocation } from 'react-router-dom';
import { QRCodeSVG } from 'qrcode.react';
import ticketService from '../services/ticketService';
import './SuccessPage.css';

const SuccessPage = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const ticketIdFromState = location.state?.ticketId;
    
    const [ticket, setTicket] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        if (ticketIdFromState) {
            loadTicketInfo(ticketIdFromState);
        } else {
            setError('Информация о билете не найдена');
            setLoading(false);
        }
    }, [ticketIdFromState]);

    const loadTicketInfo = async (id) => {
        try {
            setLoading(true);
            const ticketData = await ticketService.getTicketById(id);
            setTicket(ticketData);
        } catch (err) {
            setError('Не удалось загрузить информацию о билете');
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    const handlePrint = () => {
        window.print();
    };

    const handleBackToFlights = () => {
        navigate('/flights');
    };

    if (loading) {
        return <div className="success-loading">Загрузка информации о билете...</div>;
    }

    if (error || !ticket) {
        return (
            <div className="success-error">
                <h2>Ошибка</h2>
                <p>{error || 'Билет не найден'}</p>
                <Link to="/flights" className="back-btn">Вернуться к списку рейсов</Link>
            </div>
        );
    }

    const calculatedPrice = ticket.calculatedPrice || (ticket.flight?.basePrice * ticket.seat?.priceMultiplier) || 0;
    const qrValue = JSON.stringify({
        ticketId: ticket.id,
        flightNumber: ticket.flight?.flightNumber,
        passenger: `${ticket.passenger?.firstName} ${ticket.passenger?.lastName}`,
        seat: ticket.seat?.seatNumber,
        departure: ticket.flight?.departureTime,
        status: ticket.bookingStatus
    });

    return (
        <div className="success-page">
            <div className="success-container">
                {/* Success Icon */}
                <div className="success-icon">
                    <svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                        <path d="M12 22C17.5228 22 22 17.5228 22 12C22 6.47715 17.5228 2 12 2C6.47715 2 2 6.47715 2 12C2 17.5228 6.47715 22 12 22Z" stroke="#10B981" strokeWidth="2"/>
                        <path d="M8 12L11 15L16 9" stroke="#10B981" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
                    </svg>
                </div>

                <h1 className="success-title">Оплата успешна!</h1>
                <p className="success-subtitle">Ваш билет успешно оплачен и подтвержден</p>

                {/* Ticket Card */}
                <div className="ticket-card" id="ticket-to-print">
                    <div className="ticket-header">
                        <div className="airline-logo">
                            <span>✈️ AirlineAPI</span>
                        </div>
                        <div className="ticket-status paid">
                            {ticket.bookingStatus}
                        </div>
                    </div>

                    <div className="ticket-body">
                        <div className="ticket-row">
                            <div className="ticket-info-block">
                                <span className="info-label">Рейс</span>
                                <span className="info-value flight-number">{ticket.flight?.flightNumber || `#${ticket.flightId}`}</span>
                            </div>
                            <div className="ticket-info-block">
                                <span className="info-label">Пассажир</span>
                                <span className="info-value">
                                    {ticket.passenger?.firstName} {ticket.passenger?.lastName}
                                </span>
                            </div>
                        </div>

                        <div className="ticket-route">
                            <div className="route-point departure">
                                <span className="airport-name">{ticket.flight?.departureAirport?.name}</span>
                                <span className="airport-location">{ticket.flight?.departureAirport?.location}</span>
                                <span className="departure-time">
                                    {new Date(ticket.flight?.departureTime).toLocaleTimeString('ru-RU', {
                                        hour: '2-digit',
                                        minute: '2-digit'
                                    })}
                                </span>
                                <span className="departure-date">
                                    {new Date(ticket.flight?.departureTime).toLocaleDateString('ru-RU', {
                                        day: '2-digit',
                                        month: 'short'
                                    })}
                                </span>
                            </div>
                            
                            <div className="route-arrow">
                                <span>→</span>
                            </div>
                            
                            <div className="route-point arrival">
                                <span className="airport-name">{ticket.flight?.arrivalAirport?.name}</span>
                                <span className="airport-location">{ticket.flight?.arrivalAirport?.location}</span>
                                <span className="arrival-time">
                                    {new Date(ticket.flight?.arrivalTime).toLocaleTimeString('ru-RU', {
                                        hour: '2-digit',
                                        minute: '2-digit'
                                    })}
                                </span>
                                <span className="arrival-date">
                                    {new Date(ticket.flight?.arrivalTime).toLocaleDateString('ru-RU', {
                                        day: '2-digit',
                                        month: 'short'
                                    })}
                                </span>
                            </div>
                        </div>

                        <div className="ticket-details-row">
                            <div className="ticket-info-block">
                                <span className="info-label">Место</span>
                                <span className="info-value seat">{ticket.seat?.seatNumber || `#${ticket.seatId}`}</span>
                                <span className="seat-class">{ticket.seat?.sector}</span>
                            </div>
                            <div className="ticket-info-block">
                                <span className="info-label">Самолёт</span>
                                <span className="info-value">{ticket.flight?.airplane?.model || 'N/A'}</span>
                            </div>
                            <div className="ticket-info-block price-block">
                                <span className="info-label">Оплачено</span>
                                <span className="info-value price">{calculatedPrice} ₽</span>
                            </div>
                        </div>

                        <div className="ticket-qr">
                            <QRCodeSVG 
                                value={qrValue}
                                size={120}
                                level="H"
                                includeMargin={true}
                            />
                            <span className="qr-label">Отсканируйте для проверки</span>
                        </div>
                    </div>

                    <div className="ticket-footer">
                        <span className="ticket-id">Билет № {ticket.id}</span>
                        <span className="booking-date">
                            Дата покупки: {new Date().toLocaleDateString('ru-RU')}
                        </span>
                    </div>
                </div>

                {/* Actions */}
                <div className="ticket-actions">
                    <button className="print-btn" onClick={handlePrint}>
                        📄 Скачать PDF / Распечатать
                    </button>
                    <button className="back-btn-secondary" onClick={handleBackToFlights}>
                        ← К списку рейсов
                    </button>
                </div>
            </div>
        </div>
    );
};

export default SuccessPage;
