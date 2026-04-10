import { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import ticketService from '../services/ticketService';
import './CheckoutPage.css';

const CheckoutPage = () => {
    const { ticketId } = useParams();
    const navigate = useNavigate();
    
    const [ticket, setTicket] = useState(null);
    const [loading, setLoading] = useState(false);
    const [processing, setProcessing] = useState(false);
    const [error, setError] = useState(null);
    
    // Данные карты
    const [cardNumber, setCardNumber] = useState('');
    const [expiryDate, setExpiryDate] = useState('');
    const [cvv, setCvv] = useState('');
    const [cardHolder, setCardHolder] = useState('');

    useEffect(() => {
        loadTicketInfo();
    }, [ticketId]);

    const loadTicketInfo = async () => {
        try {
            setLoading(true);
            const ticketData = await ticketService.getTicketById(ticketId);
            setTicket(ticketData);
        } catch (err) {
            setError('Не удалось загрузить информацию о билете');
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    // Маска для номера карты (#### #### #### ####)
    const handleCardNumberChange = (e) => {
        let value = e.target.value.replace(/\D/g, '');
        value = value.substring(0, 16);
        // Добавляем пробелы каждые 4 символа
        value = value.replace(/(\d{4})(?=\d)/g, '$1 ');
        setCardNumber(value);
    };

    // Маска для срока действия (MM/YY)
    const handleExpiryDateChange = (e) => {
        let value = e.target.value.replace(/\D/g, '');
        value = value.substring(0, 4);
        if (value.length >= 2) {
            value = value.substring(0, 2) + '/' + value.substring(2);
        }
        setExpiryDate(value);
    };

    // Маска для CVV (только цифры, макс 3)
    const handleCvvChange = (e) => {
        let value = e.target.value.replace(/\D/g, '');
        value = value.substring(0, 3);
        setCvv(value);
    };

    const handlePay = async () => {
        // Валидация
        if (!cardNumber || cardNumber.replace(/\s/g, '').length !== 16) {
            setError('Введите корректный номер карты (16 цифр)');
            return;
        }
        if (!expiryDate || expiryDate.length !== 5) {
            setError('Введите корректный срок действия (MM/YY)');
            return;
        }
        if (!cvv || cvv.length !== 3) {
            setError('Введите корректный CVV (3 цифры)');
            return;
        }
        if (!cardHolder.trim()) {
            setError('Введите имя держателя карты');
            return;
        }

        // Имитация обработки платежа
        setProcessing(true);
        setError(null);

        // Ждем 2-3 секунды
        await new Promise(resolve => setTimeout(resolve, 2500));

        // 90% успех, 10% ошибка
        const isSuccess = Math.random() < 0.9;

        if (isSuccess) {
            try {
                await ticketService.confirmBooking(ticketId);
                navigate('/success', { state: { ticketId } });
            } catch (err) {
                setError('Ошибка при подтверждении платежа. Попробуйте снова.');
                setProcessing(false);
            }
        } else {
            const errors = [
                'Недостаточно средств на карте',
                'Ошибка связи с банком',
                'Карта отклонена',
                'Превышен лимит операций'
            ];
            const randomError = errors[Math.floor(Math.random() * errors.length)];
            setError(randomError);
            setProcessing(false);
        }
    };

    if (loading) {
        return <div className="checkout-loading">Загрузка информации о билете...</div>;
    }

    if (!ticket) {
        return (
            <div className="checkout-error">
                <h2>Билет не найден</h2>
                <Link to="/flights">Вернуться к списку рейсов</Link>
            </div>
        );
    }

    const calculatedPrice = ticket.calculatedPrice || (ticket.flight?.basePrice * ticket.seat?.priceMultiplier) || 0;
    
    // Расчет оставшегося времени для таймера
    const getRemainingTime = () => {
        if (ticket.bookingStatus !== 'Зарезервирован' || !ticket.reservedAt) return null;
        
        const now = new Date().getTime();
        const reservedAt = new Date(ticket.reservedAt).getTime();
        const elapsed = Math.floor((now - reservedAt) / 1000);
        const remaining = Math.max(0, 600 - elapsed);
        
        if (remaining <= 0) return 'Истекло';
        
        const minutes = Math.floor(remaining / 60);
        const seconds = remaining % 60;
        return `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
    };
    
    const [reservationTimeLeft, setReservationTimeLeft] = useState(getRemainingTime());
    
    useEffect(() => {
        if (ticket.bookingStatus !== 'Зарезервирован' || !ticket.reservedAt) return;
        
        const timer = setInterval(() => {
            setReservationTimeLeft(getRemainingTime());
        }, 1000);
        
        return () => clearInterval(timer);
    }, [ticket]);

    return (
        <div className="checkout-page">
            <div className="checkout-container">
                <h1 className="checkout-title">Оплата билета</h1>
                
                {/* Информация о билете */}
                <div className="ticket-summary">
                    <h2>Информация о рейсе</h2>
                    <div className="ticket-summary-content">
                        <div className="summary-row">
                            <span className="label">Рейс:</span>
                            <span className="value">{ticket.flight?.flightNumber || `#${ticket.flightId}`}</span>
                        </div>
                        <div className="summary-row">
                            <span className="label">Маршрут:</span>
                            <span className="value">
                                {ticket.flight?.departureAirport?.name} → {ticket.flight?.arrivalAirport?.name}
                            </span>
                        </div>
                        <div className="summary-row">
                            <span className="label">Дата вылета:</span>
                            <span className="value">
                                {new Date(ticket.flight?.departureTime).toLocaleDateString('ru-RU', {
                                    day: '2-digit',
                                    month: 'long',
                                    year: 'numeric'
                                })}
                                {' '}
                                {new Date(ticket.flight?.departureTime).toLocaleTimeString('ru-RU', {
                                    hour: '2-digit',
                                    minute: '2-digit'
                                })}
                            </span>
                        </div>
                        <div className="summary-row">
                            <span className="label">Место:</span>
                            <span className="value">
                                {ticket.seat?.seatNumber || `#${ticket.seatId}`} 
                                ({ticket.seat?.sector || 'N/A'})
                            </span>
                        </div>
                        <div className="summary-row">
                            <span className="label">Класс:</span>
                            <span className="value">{ticket.seat?.sector || 'N/A'}</span>
                        </div>
                        {ticket.bookingStatus === 'Зарезервирован' && reservationTimeLeft && (
                            <div className="summary-row">
                                <span className="label">⏱️ Время на оплату:</span>
                                <span className={`value ${reservationTimeLeft === 'Истекло' ? 'time-expired' : 'time-remaining'}`}>
                                    {reservationTimeLeft}
                                </span>
                            </div>
                        )}
                        <div className="summary-row total">
                            <span className="label">К оплате:</span>
                            <span className="value price">{calculatedPrice} ₽</span>
                        </div>
                    </div>
                </div>

                {/* Форма оплаты */}
                <div className="payment-form">
                    <h2>Данные карты</h2>
                    
                    <div className="form-group">
                        <label htmlFor="cardNumber">Номер карты</label>
                        <input
                            type="text"
                            id="cardNumber"
                            placeholder="0000 0000 0000 0000"
                            value={cardNumber}
                            onChange={handleCardNumberChange}
                            maxLength={19}
                            disabled={processing}
                        />
                    </div>

                    <div className="form-row">
                        <div className="form-group">
                            <label htmlFor="expiryDate">Срок действия</label>
                            <input
                                type="text"
                                id="expiryDate"
                                placeholder="MM/YY"
                                value={expiryDate}
                                onChange={handleExpiryDateChange}
                                maxLength={5}
                                disabled={processing}
                            />
                        </div>
                        <div className="form-group">
                            <label htmlFor="cvv">CVV</label>
                            <input
                                type="password"
                                id="cvv"
                                placeholder="123"
                                value={cvv}
                                onChange={handleCvvChange}
                                maxLength={3}
                                disabled={processing}
                            />
                        </div>
                    </div>

                    <div className="form-group">
                        <label htmlFor="cardHolder">Имя держателя карты</label>
                        <input
                            type="text"
                            id="cardHolder"
                            placeholder="IVAN IVANOV"
                            value={cardHolder}
                            onChange={(e) => setCardHolder(e.target.value.toUpperCase())}
                            disabled={processing}
                        />
                    </div>

                    {error && (
                        <div className="error-message">
                            ⚠️ {error}
                        </div>
                    )}

                    <button 
                        className="pay-button" 
                        onClick={handlePay}
                        disabled={processing}
                    >
                        {processing ? 'Обработка...' : `Оплатить ${calculatedPrice} ₽`}
                    </button>
                </div>

                <Link to="/flights" className="back-link">← Вернуться к рейсам</Link>
            </div>

            {/* Full-screen Loader */}
            {processing && (
                <div className="processing-overlay">
                    <div className="processing-content">
                        <div className="spinner"></div>
                        <p className="processing-text">Обработка платежа...</p>
                        <p className="processing-subtext">Пожалуйста, не закрывайте страницу</p>
                    </div>
                </div>
            )}
        </div>
    );
};

export default CheckoutPage;
