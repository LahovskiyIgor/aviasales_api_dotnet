import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import AdminHeader from '../components/AdminHeader';
import Footer from '../components/Footer';
import flightService from '../services/flightService';
import './AdminFlightsPage.css';

const AdminFlightsPage = () => {
  const navigate = useNavigate();
  const [flights, setFlights] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [showModal, setShowModal] = useState(false);
  const [editingFlight, setEditingFlight] = useState(null);
  
  // Форма для создания/редактирования рейса
  const [formData, setFormData] = useState({
    flightNumber: '',
    departureAirportId: '',
    arrivalAirportId: '',
    departureTime: '',
    arrivalTime: '',
    airplaneId: '',
    totalSeats: 0,
    basePrice: 100
  });

  useEffect(() => {
    loadFlights();
  }, []);

  const loadFlights = async () => {
    try {
      setLoading(true);
      const data = await flightService.getAll();
      setFlights(data);
    } catch (err) {
      setError('Не удалось загрузить список рейсов');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Вы уверены, что хотите удалить этот рейс?')) {
      return;
    }

    try {
      await flightService.delete(id);
      setFlights(flights.filter(f => f.id !== id));
    } catch (err) {
      alert('Ошибка при удалении рейса');
      console.error(err);
    }
  };

  const handleEdit = (flight) => {
    setEditingFlight(flight);
    setFormData({
      flightNumber: flight.flightNumber,
      departureAirportId: flight.departureAirport?.id || '',
      arrivalAirportId: flight.arrivalAirport?.id || '',
      departureTime: new Date(flight.departureTime).toISOString().slice(0, 16),
      arrivalTime: new Date(flight.arrivalTime).toISOString().slice(0, 16),
      airplaneId: flight.airplane?.id || '',
      totalSeats: flight.totalSeats,
      basePrice: flight.basePrice || 100
    });
    setShowModal(true);
  };

  const handleCreate = () => {
    setEditingFlight(null);
    setFormData({
      flightNumber: '',
      departureAirportId: '',
      arrivalAirportId: '',
      departureTime: '',
      arrivalTime: '',
      airplaneId: '',
      totalSeats: 0,
      basePrice: 100
    });
    setShowModal(true);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    const payload = {
      flightNumber: formData.flightNumber,
      departureAirportId: parseInt(formData.departureAirportId),
      arrivalAirportId: parseInt(formData.arrivalAirportId),
      departureTime: formData.departureTime,
      arrivalTime: formData.arrivalTime,
      airplaneId: parseInt(formData.airplaneId),
      totalSeats: parseInt(formData.totalSeats),
      basePrice: parseFloat(formData.basePrice) || 0
    };

    try {
      if (editingFlight) {
        await flightService.update(editingFlight.id, payload);
      } else {
        await flightService.create(payload);
      }
      setShowModal(false);
      loadFlights();
    } catch (err) {
      alert('Ошибка при сохранении рейса');
      console.error(err);
    }
  };

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    });
  };

  if (loading) {
    return <div className="loading">Загрузка рейсов...</div>;
  }

  if (error) {
    return <div className="error">{error}</div>;
  }

  return (
    <div className="admin-flights-page-wrapper">
      <AdminHeader activePage="flights" />
      <div className="admin-flights-page">
        <div className="page-header">
          <h1>Управление рейсами</h1>
          <button className="create-flight-btn" onClick={handleCreate}>
            + Создать рейс
          </button>
        </div>

        <div className="flights-table-container">
          <table className="flights-table">
            <thead>
              <tr>
                <th>№ рейса</th>
                <th>Откуда</th>
                <th>Куда</th>
                <th>Вылет</th>
                <th>Прилёт</th>
                <th>Самолёт</th>
                <th>Мест</th>
                <th>Действия</th>
              </tr>
            </thead>
            <tbody>
              {flights.map(flight => (
                <tr key={flight.id}>
                  <td>{flight.flightNumber}</td>
                  <td>{flight.departureAirport?.name || flight.departureAirport?.location}</td>
                  <td>{flight.arrivalAirport?.name || flight.arrivalAirport?.location}</td>
                  <td>{new Date(flight.departureTime).toLocaleString('ru-RU', {
                    day: '2-digit',
                    month: '2-digit',
                    hour: '2-digit',
                    minute: '2-digit'
                  })}</td>
                  <td>{new Date(flight.arrivalTime).toLocaleString('ru-RU', {
                    day: '2-digit',
                    month: '2-digit',
                    hour: '2-digit',
                    minute: '2-digit'
                  })}</td>
                  <td>{flight.airplane?.model}</td>
                  <td>{flight.totalSeats}</td>
                  <td className="actions-cell">
                    <button 
                      className="edit-btn"
                      onClick={() => handleEdit(flight)}
                    >
                      Изменить
                    </button>
                    <button 
                      className="delete-btn"
                      onClick={() => handleDelete(flight.id)}
                    >
                      Удалить
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
      <Footer />

      {/* Модальное окно для создания/редактирования */}
      {showModal && (
        <div className="modal-overlay" onClick={() => setShowModal(false)}>
          <div className="modal-content" onClick={(e) => e.stopPropagation()}>
            <h2>{editingFlight ? 'Редактировать рейс' : 'Создать рейс'}</h2>
            <form onSubmit={handleSubmit}>
              <div className="form-group">
                <label>Номер рейса:</label>
                <input
                  type="text"
                  name="flightNumber"
                  value={formData.flightNumber}
                  onChange={handleChange}
                  required
                />
              </div>
              <div className="form-group">
                <label>ID аэропорта отправления:</label>
                <input
                  type="number"
                  name="departureAirportId"
                  value={formData.departureAirportId}
                  onChange={handleChange}
                  required
                />
              </div>
              <div className="form-group">
                <label>ID аэропорта прибытия:</label>
                <input
                  type="number"
                  name="arrivalAirportId"
                  value={formData.arrivalAirportId}
                  onChange={handleChange}
                  required
                />
              </div>
              <div className="form-group">
                <label>Время вылета:</label>
                <input
                  type="datetime-local"
                  name="departureTime"
                  value={formData.departureTime}
                  onChange={handleChange}
                  required
                />
              </div>
              <div className="form-group">
                <label>Время прилёта:</label>
                <input
                  type="datetime-local"
                  name="arrivalTime"
                  value={formData.arrivalTime}
                  onChange={handleChange}
                  required
                />
              </div>
              <div className="form-group">
                <label>ID самолёта:</label>
                <input
                  type="number"
                  name="airplaneId"
                  value={formData.airplaneId}
                  onChange={handleChange}
                  required
                />
              </div>
              <div className="form-group">
                <label>Количество мест:</label>
                <input
                  type="number"
                  name="totalSeats"
                  value={formData.totalSeats}
                  onChange={handleChange}
                  required
                />
              </div>
              <div className="form-group">
                <label>Базовая цена:</label>
                <input
                  type="number"
                  name="basePrice"
                  value={formData.basePrice}
                  onChange={handleChange}
                  required
                />
              </div>
              <div className="modal-actions">
                <button type="button" className="cancel-btn" onClick={() => setShowModal(false)}>
                  Отмена
                </button>
                <button type="submit" className="submit-btn">
                  {editingFlight ? 'Сохранить' : 'Создать'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

export default AdminFlightsPage;
