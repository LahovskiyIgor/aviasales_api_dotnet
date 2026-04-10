import { useAuth } from '../context/AuthContext';
import { Navigate } from 'react-router-dom';

const AdminRoute = ({ children }) => {
  const { isAdmin, loading } = useAuth();

  if (loading) {
    return <div className="loading">Загрузка...</div>;
  }

  return isAdmin ? children : <Navigate to="/login" />;
};

export default AdminRoute;
