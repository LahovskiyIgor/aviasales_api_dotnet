import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/AuthContext';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import FlightsPage from './pages/FlightsPage';
import FlightDetailPage from './pages/FlightDetailPage';
import CheckoutPage from './pages/CheckoutPage';
import SuccessPage from './pages/SuccessPage';
import ProfilePage from './pages/ProfilePage';
import AdminFlightsPage from './pages/AdminFlightsPage';
import AdminAirplanesPage from './pages/AdminAirplanesPage';
import AdminAirportsPage from './pages/AdminAirportsPage';
import AdminRoute from './components/AdminRoute';
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
    if (loading) return <div>Загрузка...</div>;

    // Редирект на корень, чтобы сработал RoleBasedRoute
    return !isAuthenticated ? children : <Navigate to="/" replace />;
};

// Компонент для маршрутизации на основе роли
const RoleBasedRoute = () => {
  const { isAdmin, isPassenger, loading } = useAuth();

  if (loading) {
    return <div className="loading">Загрузка...</div>;
  }

  if (isAdmin) {
    return <Navigate to="/admin/flights" />;
  }

  if (isPassenger) {
    return <Navigate to="/flights" />;
  }

  return <Navigate to="/login" />;
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
        path="/"
        element={<RoleBasedRoute />}
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
          <Route
              path="/checkout/:ticketId"
              element={
                  <ProtectedRoute>
                      <CheckoutPage />
                  </ProtectedRoute>
              }
          />
          <Route
              path="/success"
              element={
                  <ProtectedRoute>
                      <SuccessPage />
                  </ProtectedRoute>
              }
          />
          <Route
              path="/profile"
              element={
                  <ProtectedRoute>
                      <ProfilePage />
                  </ProtectedRoute>
              }
          />
          <Route
              path="/admin/flights"
              element={
                  <AdminRoute>
                      <AdminFlightsPage />
                  </AdminRoute>
              }
          />
          <Route
              path="/admin/airplanes"
              element={
                  <AdminRoute>
                      <AdminAirplanesPage />
                  </AdminRoute>
              }
          />
          <Route
              path="/admin/airports"
              element={
                  <AdminRoute>
                      <AdminAirportsPage />
                  </AdminRoute>
              }
          />
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
