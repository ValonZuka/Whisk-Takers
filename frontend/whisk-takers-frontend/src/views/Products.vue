<script setup lang="ts">
import { ref, onMounted } from "vue";
import api from "../api/axios";

const products = ref<any[]>([]);

onMounted(async () => {
  try {
    const res = await api.get("/products");
    console.log("API response:", res.data); 
    products.value = res.data;
  } catch (err) {
    console.error("API error:", err); 
  }
});
</script>

<template>
  <div class="p-4">
    <h1 class="text-2xl font-bold mb-4">Products</h1>
    <ul>
      <li v-for="p in products" :key="p.id" class="border-b py-2">
        {{ p.name }} - ${{ p.price }}
      </li>
    </ul>
  </div>
</template>
