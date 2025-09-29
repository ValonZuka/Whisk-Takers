import axios from "axios";
import { getToken } from "../store/auth";

const api = axios.create({
  baseURL: "http://localhost:5058/api", 
  headers: {
    "Content-Type": "application/json",
  },
});

// Add JWT token to requests
api.interceptors.request.use((config) => {
  const token = getToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export default api;
