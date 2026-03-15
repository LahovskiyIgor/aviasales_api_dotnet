import api from './api';

const airportService = {
  getAll: async () => {
    const response = await api.get('/airport');
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/airport/${id}`);
    return response.data;
  }
};

export default airportService;
