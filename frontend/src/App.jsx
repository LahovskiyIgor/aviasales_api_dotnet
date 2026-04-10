import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/AuthContext';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import FlightsPage from './pages/FlightsPage';
import FlightDetailPage from './pages/FlightDetailPage';
import './App.css';

// Компонент для защиты маршрутов
const ProtectedRoute = ({ children }) => {
  const { isAuthenticated, loading } = useAuth();

  if (loading) {
    return <div className="loading">Загрузка...</div>;
  }

  return isAuthenticated ? children : <Navigate to="/login" />;
};

// Компонент для перенаправления авторизованных пользователей
const PublicRoute = ({ children }) => {
  const { isAuthenticated, loading } = useAuth();

  if (loading) {
    return <div className="loading">Загрузка...</div>;
  }

  return !isAuthenticated ? children : <Navigate to="/flights" />;
};

function AppRoutes() {
  return (
    <Routes>
      <Route
        path="/login"
        element={
          <PublicRoute>
            <LoginPage />
          </PublicRoute>
        }
      />
      <Route
        path="/register"
        element={
          <PublicRoute>
            <RegisterPage />
          </PublicRoute>
        }
      />
      <Route
        path="/flights"
        element={
          <ProtectedRoute>
            <FlightsPage />
          </ProtectedRoute>
        }
          />
          <Route
              path="/flights/:id"
              element={
                  <ProtectedRoute>
                      <FlightDetailPage />
                  </ProtectedRoute>
              }
          />
      <Route path="/" element={<Navigate to="/login" />} />
      <Route path="*" element={<Navigate to="/login" />} />
    </Routes>
  );
}

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <div className="app">
          <AppRoutes />
        </div>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;
