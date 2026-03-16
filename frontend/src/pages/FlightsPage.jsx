import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import flightService from '../services/flightService';
import airportService from '../services/airportService';
import AirportSuggestion from '../components/AirportSuggestion';
import './FlightsPage.css';

const FlightsPage = () => {
  const navigate = useNavigate();
  const [flights, setFlights] = useState([]);
  const [filteredFlights, setFilteredFlights] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  
  // Фильтры
  const [departureAirport, setDepartureAirport] = useState('');
  const [arrivalAirport, setArrivalAirport] = useState('');
  const [departureDate, setDepartureDate] = useState('');
  
  // Список аэропортов и состояние для выпадающих списков
  const [airports, setAirports] = useState([]);
  const [showDepartureDropdown, setShowDepartureDropdown] = useState(false);
  const [showArrivalDropdown, setShowArrivalDropdown] = useState(false);
  const [filteredDepartureAirports, setFilteredDepartureAirports] = useState([]);
  const [filteredArrivalAirports, setFilteredArrivalAirports] = useState([]);

  useEffect(() => {
    loadAirports();
    loadFlights();
  }, []);

  const loadAirports = async () => {
    try {
      const data = await airportService.getAll();
      setAirports(data);
    } catch (err) {
      console.error('Не удалось загрузить список аэропортов:', err);
    }
  };

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
          flight.departureAirport?.name.toLowerCase().includes(departureAirport.toLowerCase())
      );
    }

    if (arrivalAirport) {
      result = result.filter(flight => 
          flight.arrivalAirport?.name.toLowerCase().includes(arrivalAirport.toLowerCase())
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
    setFilteredDepartureAirports([]);
    setFilteredArrivalAirports([]);
  };

  // Обработчики для поля "Откуда"
  const handleDepartureChange = (e) => {
    const value = e.target.value;
    setDepartureAirport(value);
    
    if (value.trim()) {
      const filtered = airports.filter(airport => 
        airport.name.toLowerCase().includes(value.toLowerCase()) ||
        airport.location.toLowerCase().includes(value.toLowerCase())
      );
      setFilteredDepartureAirports(filtered);
      setShowDepartureDropdown(true);
    } else {
      setFilteredDepartureAirports([]);
      setShowDepartureDropdown(false);
    }
  };

  const handleDepartureSelect = (airport) => {
    setDepartureAirport(airport.name);
    setShowDepartureDropdown(false);
    setFilteredDepartureAirports([]);
  };

  const handleDepartureBlur = () => {
    setTimeout(() => {
      setShowDepartureDropdown(false);
      setFilteredDepartureAirports([]);
    }, 200);
  };

  // Обработчики для поля "Куда"
  const handleArrivalChange = (e) => {
    const value = e.target.value;
    setArrivalAirport(value);
    
    if (value.trim()) {
      const filtered = airports.filter(airport => 
        airport.name.toLowerCase().includes(value.toLowerCase()) ||
        airport.location.toLowerCase().includes(value.toLowerCase())
      );
      setFilteredArrivalAirports(filtered);
      setShowArrivalDropdown(true);
    } else {
      setFilteredArrivalAirports([]);
      setShowArrivalDropdown(false);
    }
  };

  const handleArrivalSelect = (airport) => {
    setArrivalAirport(airport.name);
    setShowArrivalDropdown(false);
    setFilteredArrivalAirports([]);
  };

  const handleArrivalBlur = () => {
    setTimeout(() => {
      setShowArrivalDropdown(false);
      setFilteredArrivalAirports([]);
    }, 200);
  };

  if (loading) {
    return <div className="loading">Загрузка рейсов...</div>;
  }

  if (error) {
    return <div className="error">{error}</div>;
  }

  return (
    <div className="flights-page">
      <h1>Доступные рейсы</h1>
      
      <div className="filters-container">
        <div className="filter-group">
          <label htmlFor="departure">Откуда:</label>
          <div className="autocomplete-container">
            <input
              id="departure"
              type="text"
              placeholder="Город или аэропорт отправления"
              value={departureAirport}
              onChange={handleDepartureChange}
              onFocus={() => {
                if (departureAirport.trim()) {
                  const filtered = airports.filter(airport => 
                    airport.name.toLowerCase().includes(departureAirport.toLowerCase()) ||
                    airport.location.toLowerCase().includes(departureAirport.toLowerCase())
                  );
                  setFilteredDepartureAirports(filtered);
                  setShowDepartureDropdown(true);
                }
              }}
              onBlur={handleDepartureBlur}
            />
            {showDepartureDropdown && filteredDepartureAirports.length > 0 && (
              <ul className="autocomplete-dropdown">
                {filteredDepartureAirports.map(airport => (
                    <AirportSuggestion
                        key={airport.id}
                        airport={airport}
                        onClick={handleDepartureSelect}
                    />
                ))}
              </ul>
            )}
          </div>
        </div>

        <div className="filter-group">
          <label htmlFor="arrival">Куда:</label>
          <div className="autocomplete-container">
            <input
              id="arrival"
              type="text"
              placeholder="Город или аэропорт прибытия"
              value={arrivalAirport}
              onChange={handleArrivalChange}
              onFocus={() => {
                if (arrivalAirport.trim()) {
                  const filtered = airports.filter(airport => 
                    airport.name.toLowerCase().includes(arrivalAirport.toLowerCase()) ||
                    airport.location.toLowerCase().includes(arrivalAirport.toLowerCase())
                  );
                  setFilteredArrivalAirports(filtered);
                  setShowArrivalDropdown(true);
                }
              }}
              onBlur={handleArrivalBlur}
            />
            {showArrivalDropdown && filteredArrivalAirports.length > 0 && (
              <ul className="autocomplete-dropdown">
                {filteredArrivalAirports.map(airport => (
                    <AirportSuggestion
                        key={airport.id}
                        airport={airport}
                        onClick={handleArrivalSelect}
                    />
                ))}
              </ul>
            )}
          </div>
        </div>

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
  );
};

export default FlightsPage;
