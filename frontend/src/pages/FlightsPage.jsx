import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import flightService from '../services/flightService';
import Header from '../components/Header';
import AirportSelect from '../components/AirportSelect';
import './FlightsPage.css';

const FlightsPage = () => {
  const navigate = useNavigate();
  const [flights, setFlights] = useState([]);
  const [filteredFlights, setFilteredFlights] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  
  // Фильтры
  const [departureAirport, setDepartureAirport] = useState(null);
  const [arrivalAirport, setArrivalAirport] = useState(null);
  const [departureDate, setDepartureDate] = useState('');

  useEffect(() => {
    loadFlights();
  }, []);

  useEffect(() => {
    applyFilters();
  }, [flights, departureAirport, arrivalAirport, departureDate]);

  const loadFlights = async () => {
    try {
      setLoading(true);
      const data = await flightService.getAll();
      setFlights(data);
      setFilteredFlights(data);
    } catch (err) {
      setError('Не удалось загрузить список рейсов');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const applyFilters = () => {
    let result = [...flights];

    if (departureAirport) {
      result = result.filter(flight => 
        flight.departureAirport?.id === departureAirport.id
      );
    }

    if (arrivalAirport) {
      result = result.filter(flight => 
        flight.arrivalAirport?.id === arrivalAirport.id
      );
    }

    if (departureDate) {
      result = result.filter(flight => {
        const flightDate = new Date(flight.departureTime);
        return flightDate.toDateString() === new Date(departureDate).toDateString();
      });
    }

    setFilteredFlights(result);
  };

  const handleFlightClick = (id) => {
    navigate(`/flights/${id}`);
  };

  const clearFilters = () => {
    setDepartureAirport('');
    setArrivalAirport('');
    setDepartureDate('');
  };

  if (loading) {
    return <div className="loading">Загрузка рейсов...</div>;
  }

  if (error) {
    return <div className="error">{error}</div>;
  }

  return (
    <div className="flights-page">
      <Header />
      
      <div className="content-container">
        <h1>Доступные рейсы</h1>
        
        <div className="filters-container">
          <AirportSelect
            label="Откуда:"
            value={departureAirport}
            onChange={setDepartureAirport}
            placeholder="Город или аэропорт отправления"
          />

          <AirportSelect
            label="Куда:"
            value={arrivalAirport}
            onChange={setArrivalAirport}
            placeholder="Город или аэропорт прибытия"
          />

          <div className="filter-group">
            <label htmlFor="date">Дата вылета:</label>
            <input
              id="date"
              type="date"
              value={departureDate}
              onChange={(e) => setDepartureDate(e.target.value)}
            />
          </div>

          <button className="clear-filters-btn" onClick={clearFilters}>
            Сбросить фильтры
          </button>
        </div>

      <div className="flights-list">
        {filteredFlights.length === 0 ? (
          <p className="no-flights">Рейсы не найдены</p>
        ) : (
          filteredFlights.map(flight => (
            <div 
              key={flight.id} 
              className="flight-card"
              onClick={() => handleFlightClick(flight.id)}
            >
              <div className="flight-header">
                <span className="flight-number">{flight.flightNumber}</span>
                <span className="airplane-model">{flight.airplane?.model}</span>
              </div>
              
              <div className="flight-route">
                <div className="airport departure">
                  <span className="airport-code">{flight.departureAirport?.name}</span>
                  <span className="airport-city">{flight.departureAirport?.location}</span>
                </div>
                <div className="route-arrow">→</div>
                <div className="airport arrival">
                  <span className="airport-code">{flight.arrivalAirport?.name}</span>
                  <span className="airport-city">{flight.arrivalAirport?.location}</span>
                </div>
              </div>

              <div className="flight-time">
                <div className="departure-time">
                  <span className="time-label">Вылет:</span>
                  <span className="time-value">
                    {new Date(flight.departureTime).toLocaleString('ru-RU', {
                      day: '2-digit',
                      month: '2-digit',
                      hour: '2-digit',
                      minute: '2-digit'
                    })}
                  </span>
                </div>
                <div className="arrival-time">
                  <span className="time-label">Прилёт:</span>
                  <span className="time-value">
                    {new Date(flight.arrivalTime).toLocaleString('ru-RU', {
                      day: '2-digit',
                      month: '2-digit',
                      hour: '2-digit',
                      minute: '2-digit'
                    })}
                  </span>
                </div>
              </div>

              <div className="flight-seats">
                <span>Всего мест: {flight.totalSeats}</span>
                <span>Продано: {flight.soldTickets}</span>
                <span>Забронировано: {flight.reservedTickets}</span>
                <span className="available-seats">
                  Доступно: {flight.totalSeats - flight.soldTickets - flight.reservedTickets}
                </span>
              </div>

              <button className="select-flight-btn">Выбрать рейс</button>
            </div>
          ))
        )}
      </div>
      </div>
    </div>
  );
};

export default FlightsPage;
