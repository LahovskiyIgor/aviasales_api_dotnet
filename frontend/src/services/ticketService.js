import api from './api';

const ticketService = {
    // Получить доступные места на рейсе
    getAvailableSeats: async (flightId) => {
        const response = await api.get(`/ticket/flight/${flightId}/available-seats`);
        return response.data;
    },

    // Получить занятые места на рейсе
    getOccupiedSeats: async (flightId) => {
        const response = await api.get(`/ticket/flight/${flightId}/occupied-seats`);
        return response.data;
    },

    // Забронировать место
    reserveSeat: async (flightId, seatId) => {
        const response = await api.post('/ticket/reserve', {
            flightId,
            seatId
        });
        return response.data;
    },

    // Получить мои билеты
    getMyTickets: async () => {
        const response = await api.get('/ticket/my');
        return response.data;
    },

    // Получить билет по ID
    getTicketById: async (ticketId) => {
        const response = await api.get(`/ticket/my/${ticketId}`);
        return response.data;
    },

    // Оплатить билет (подтвердить бронирование)
    confirmBooking: async (ticketId) => {
        const response = await api.post(`/ticket/${ticketId}/pay`);
        return response.data;
    },

    // Отменить бронирование или билет
    cancelTicket: async (ticketId) => {
        const response = await api.post(`/ticket/${ticketId}/cancel`);
        return response.data;
    }
};

export default ticketService;