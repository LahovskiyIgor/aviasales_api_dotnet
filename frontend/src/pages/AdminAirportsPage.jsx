import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import AdminHeader from '../components/AdminHeader';
import Footer from '../components/Footer';
import airportService from '../services/airportService';
import './AdminAirportsPage.css';

const AdminAirportsPage = () => {
  const navigate = useNavigate();
  const [airports, setAirports] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [deleteError, setDeleteError] = useState(null);
  const [showModal, setShowModal] = useState(false);
  const [editingAirport, setEditingAirport] = useState(null);
  
  // Форма для создания/редактирования аэропорта
  const [formData, setFormData] = useState({
    name: '',
    location: ''
  });

  useEffect(() => {
    loadAirports();
  }, []);

  const loadAirports = async () => {
    try {
      setLoading(true);
      setDeleteError(null);
      const data = await airportService.getAll();
      setAirports(data);
    } catch (err) {
      setError('Не удалось загрузить список аэропортов');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Вы уверены, что хотите удалить этот аэропорт?')) {
      return;
    }

    try {
      await airportService.delete(id);
      setAirports(airports.filter(a => a.id !== id));
      setDeleteError(null);
    } catch (err) {
      const errorMessage = err.response?.data || 'Ошибка при удалении аэропорта. Возможно, он используется в рейсах.';
      setDeleteError(errorMessage);
      console.error(err);
    }
  };

  const handleEdit = (airport) => {
    setEditingAirport(airport);
    setFormData({
      name: airport.name,
      location: airport.location
    });
    setShowModal(true);
  };

  const handleCreate = () => {
    setEditingAirport(null);
    setFormData({
      name: '',
      location: ''
    });
    setShowModal(true);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    const payload = {
      ...formData,
      id: editingAirport?.id
    };

    try {
      if (editingAirport) {
        await airportService.update(editingAirport.id, payload);
      } else {
        await airportService.create(payload);
      }
      setShowModal(false);
      loadAirports();
    } catch (err) {
      const errorMessage = err.response?.data || 'Ошибка при сохранении аэропорта';
      alert(errorMessage);
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
    return <div className="loading">Загрузка аэропортов...</div>;
  }

  if (error) {
    return <div className="error">{error}</div>;
  }

  return (
    <div className="admin-airports-page-wrapper">
      <AdminHeader activePage="airports" />
      <div className="admin-airports-page">
        <div className="page-header">
          <h1>Управление аэропортами</h1>
          <button className="create-airport-btn" onClick={handleCreate}>
            + Создать аэропорт
          </button>
        </div>

        {deleteError && (
          <div className="error-banner">
            {deleteError}
            <button className="close-error-btn" onClick={() => setDeleteError(null)}>×</button>
          </div>
        )}

        <div className="airports-table-container">
          <table className="airports-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Название</th>
                <th>Расположение</th>
                <th>Действия</th>
              </tr>
            </thead>
            <tbody>
              {airports.map(airport => (
                <tr key={airport.id}>
                  <td>{airport.id}</td>
                  <td>{airport.name}</td>
                  <td>{airport.location}</td>
                  <td className="actions-cell">
                    <button 
                      className="edit-btn"
                      onClick={() => handleEdit(airport)}
                    >
                      Изменить
                    </button>
                    <button 
                      className="delete-btn"
                      onClick={() => handleDelete(airport.id)}
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
            <h2>{editingAirport ? 'Редактировать аэропорт' : 'Создать аэропорт'}</h2>
            <form onSubmit={handleSubmit}>
              <div className="form-group">
                <label>Название:</label>
                <input
                  type="text"
                  name="name"
                  value={formData.name}
                  onChange={handleChange}
                  required
                />
              </div>
              <div className="form-group">
                <label>Расположение:</label>
                <input
                  type="text"
                  name="location"
                  value={formData.location}
                  onChange={handleChange}
                  required
                />
              </div>
              <div className="modal-actions">
                <button type="button" className="cancel-btn" onClick={() => setShowModal(false)}>
                  Отмена
                </button>
                <button type="submit" className="submit-btn">
                  {editingAirport ? 'Сохранить' : 'Создать'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

export default AdminAirportsPage;
