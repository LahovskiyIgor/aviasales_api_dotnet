import api from './api';

const flightService = {
  getAll: async () => {
    const response = await api.get('/flight');
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/flight/${id}`);
    return response.data;
  },

  getDetails: async (id) => {
    const response = await api.get(`/flight/${id}/details`);
    return response.data;
  },

  getSeats: async (id) => {
    const response = await api.get(`/flight/${id}/seats`);
    return response.data;
  }
};

export default flightService;
