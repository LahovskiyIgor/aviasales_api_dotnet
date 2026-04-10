import api from './api';

const airportService = {
  getAll: async () => {
    const response = await api.get('/airport');
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/airport/${id}`);
    return response.data;
  },

  create: async (airportData) => {
    const response = await api.post('/airport', airportData);
    return response.data;
  },

  update: async (id, airportData) => {
    const response = await api.put(`/airport/${id}`, airportData);
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/airport/${id}`);
    return response.data;
  }
};

export default airportService;
