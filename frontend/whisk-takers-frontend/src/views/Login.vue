<script setup>
import { ref } from "vue";
import api from "../api/axios";

const email = ref("");
const password = ref("");
const error = ref("");

const login = async () => {
  try {
    const res = await api.post("/auth/login", {
      email: email.value,
      password: password.value,
    });

    // ruaj token në localStorage
    localStorage.setItem("access_token", res.data.token);

    alert("Login success");
  } catch (err) {
    error.value = err.response?.data?.message || "Login failed";
  }
};
</script>

<template>
  <div class="flex flex-col items-center justify-center min-h-screen bg-gray-100">
    <div class="bg-white p-6 rounded shadow w-96">
      <h2 class="text-xl font-bold mb-4">Login</h2>
      <input v-model="email" type="email" placeholder="Email" class="border p-2 w-full mb-2" />
      <input v-model="password" type="password" placeholder="Password" class="border p-2 w-full mb-4" />
      <button @click="login" class="bg-blue-500 text-white px-4 py-2 rounded">Login</button>
      <p v-if="error" class="text-red-500 mt-2">{{ error }}</p>
    </div>
  </div>
</template>
