import axios from "axios";

const api = axios.create({
  baseURL: "http://localhost:5000/api", // backend API base URL
  headers: {
    "Content-Type": "application/json",
  },
});

// optional: attach token automatically
api.interceptors.request.use((config) => {
  const token = localStorage.getItem("access_token");
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export default api;
