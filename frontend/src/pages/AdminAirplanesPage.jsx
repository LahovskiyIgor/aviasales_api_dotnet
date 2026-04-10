import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import AdminHeader from '../components/AdminHeader';
import Footer from '../components/Footer';
import airplaneService from '../services/airplaneService';
import './AdminAirplanesPage.css';

const AdminAirplanesPage = () => {
  const navigate = useNavigate();
  const [airplanes, setAirplanes] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [deleteError, setDeleteError] = useState(null);
  const [showModal, setShowModal] = useState(false);
  const [editingAirplane, setEditingAirplane] = useState(null);
  
  // Форма для создания/редактирования самолёта
  const [formData, setFormData] = useState({
    model: '',
    capacity: 0
  });

  useEffect(() => {
    loadAirplanes();
  }, []);

  const loadAirplanes = async () => {
    try {
      setLoading(true);
      setDeleteError(null);
      const data = await airplaneService.getAll();
      setAirplanes(data);
    } catch (err) {
      setError('Не удалось загрузить список самолётов');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Вы уверены, что хотите удалить этот самолёт?')) {
      return;
    }

    try {
      await airplaneService.delete(id);
      setAirplanes(airplanes.filter(a => a.id !== id));
      setDeleteError(null);
    } catch (err) {
      const errorMessage = err.response?.data || 'Ошибка при удалении самолёта. Возможно, он используется в рейсах.';
      setDeleteError(errorMessage);
      console.error(err);
    }
  };

  const handleEdit = (airplane) => {
    setEditingAirplane(airplane);
    setFormData({
      model: airplane.model,
      capacity: airplane.capacity
    });
    setShowModal(true);
  };

  const handleCreate = () => {
    setEditingAirplane(null);
    setFormData({
      model: '',
      capacity: 0
    });
    setShowModal(true);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    const payload = {
      ...formData,
      id: editingAirplane?.id,
      capacity: parseInt(formData.capacity)
    };

    try {
      if (editingAirplane) {
        await airplaneService.update(editingAirplane.id, payload);
      } else {
        await airplaneService.create(payload);
      }
      setShowModal(false);
      loadAirplanes();
    } catch (err) {
      const errorMessage = err.response?.data || 'Ошибка при сохранении самолёта';
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
    return <div className="loading">Загрузка самолётов...</div>;
  }

  if (error) {
    return <div className="error">{error}</div>;
  }

  return (
    <div className="admin-airplanes-page-wrapper">
      <AdminHeader activePage="airplanes" />
      <div className="admin-airplanes-page">
        <div className="page-header">
          <h1>Управление самолётами</h1>
          <button className="create-airplane-btn" onClick={handleCreate}>
            + Создать самолёт
          </button>
        </div>

        {deleteError && (
          <div className="error-banner">
            {deleteError}
            <button className="close-error-btn" onClick={() => setDeleteError(null)}>×</button>
          </div>
        )}

        <div className="airplanes-table-container">
          <table className="airplanes-table">
            <thead>
              <tr>
                <th>ID</th>
                <th>Модель</th>
                <th>Вместимость</th>
                <th>Действия</th>
              </tr>
            </thead>
            <tbody>
              {airplanes.map(airplane => (
                <tr key={airplane.id}>
                  <td>{airplane.id}</td>
                  <td>{airplane.model}</td>
                  <td>{airplane.capacity}</td>
                  <td className="actions-cell">
                    <button 
                      className="edit-btn"
                      onClick={() => handleEdit(airplane)}
                    >
                      Изменить
                    </button>
                    <button 
                      className="delete-btn"
                      onClick={() => handleDelete(airplane.id)}
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
            <h2>{editingAirplane ? 'Редактировать самолёт' : 'Создать самолёт'}</h2>
            <form onSubmit={handleSubmit}>
              <div className="form-group">
                <label>Модель:</label>
                <input
                  type="text"
                  name="model"
                  value={formData.model}
                  onChange={handleChange}
                  required
                />
              </div>
              <div className="form-group">
                <label>Вместимость:</label>
                <input
                  type="number"
                  name="capacity"
                  value={formData.capacity}
                  onChange={handleChange}
                  required
                />
              </div>
              <div className="modal-actions">
                <button type="button" className="cancel-btn" onClick={() => setShowModal(false)}>
                  Отмена
                </button>
                <button type="submit" className="submit-btn">
                  {editingAirplane ? 'Сохранить' : 'Создать'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

export default AdminAirplanesPage;
