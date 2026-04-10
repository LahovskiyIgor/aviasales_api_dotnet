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

  create: async (flightData) => {
    const response = await api.post('/flight', flightData);
    return response.data;
  },

  update: async (id, flightData) => {
    const response = await api.put(`/flight/${id}`, flightData);
    return response.data;
  },

  delete: async (id) => {
    const response = await api.delete(`/flight/${id}`);
    return response.data;
  }
};

export default flightService;
