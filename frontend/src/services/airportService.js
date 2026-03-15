import api from './api';

const airportService = {
  getAll: async () => {
    const response = await api.get('/airport');
    return response.data;
  }
};

export default airportService;
