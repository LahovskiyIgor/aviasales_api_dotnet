import api from './api';

const airplaneService = {
  getAll: async () => {
    const response = await api.get('/airplane');
    return response.data;
  },

  getById: async (id) => {
    const response = await api.get(`/airplane/${id}`);
    return response.data;
  },

  create: async (airplaneData) => {
    const response = await api.post('/airplane', airplaneData);
    return response.data;
  },

  update: async (id, airplaneData) => {
    const response = await api.put(`/airplane/${id}`, airplaneData);
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/airplane/${id}`);
    return response.data;
  }
};

export default airplaneService;
