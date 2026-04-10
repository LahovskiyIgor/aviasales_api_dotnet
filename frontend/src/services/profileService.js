import api from './api';

const profileService = {
    // Получить данные текущего пассажира
    getMyProfile: async () => {
        const response = await api.get('/passenger/my');
        return response.data;
    },

    // Обновить данные пассажира
    updateProfile: async (passengerData) => {
        const response = await api.put('/passenger/my', passengerData);
        return response.data;
    },

    // Получить мои билеты
    getMyTickets: async () => {
        const response = await api.get('/ticket/my');
        return response.data;
    },

    // Отменить билет
    cancelTicket: async (ticketId) => {
        const response = await api.post(`/ticket/${ticketId}/cancel`);
        return response.data;
    }
};

export default profileService;
