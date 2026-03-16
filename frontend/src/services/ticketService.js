import api from './api';

const ticketService = {
  // Получить все билеты текущего пользователя
  getMyTickets: async () => {
    const response = await api.get('/ticket/my');
    return response.data;
  },

  // Получить билет по ID (доступно владельцу или админу)
  getMyTicketById: async (id) => {
    const response = await api.get(`/ticket/my/${id}`);
    return response.data;
  },

  // Получить занятые места на рейсе
  getOccupiedSeats: async (flightId) => {
    const response = await api.get(`/ticket/flight/${flightId}/occupied-seats`);
    return response.data;
  },

  // Получить доступные места на рейсе
  getAvailableSeats: async (flightId) => {
    const response = await api.get(`/ticket/flight/${flightId}/available-seats`);
    return response.data;
  },

  // Зарезервировать билет с выбором места
  reserveTicket: async (flightId, seatNumber) => {
    const response = await api.post('/ticket/reserve', {
      flightId,
      seatNumber
    });
    return response.data;
  },

  // Отменить резервирование
  cancelReservation: async (ticketId) => {
    const response = await api.post(`/ticket/${ticketId}/cancel-reservation`);
    return response.data;
  },

  // Оплатить билет
  payTicket: async (ticketId) => {
    const response = await api.post(`/ticket/${ticketId}/pay`);
    return response.data;
  },

  // Отменить билет (резервацию или оплаченный)
  cancelTicket: async (ticketId) => {
    const response = await api.post(`/ticket/${ticketId}/cancel`);
    return response.data;
  }
};

export default ticketService;
