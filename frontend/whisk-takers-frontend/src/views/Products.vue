<script setup>
import { ref, onMounted } from "vue";
import api from "../api/axios.ts"; // kjo lidhet me axios.js

const products = ref([]);

onMounted(async () => {
  const res = await api.get("/products");
  products.value = res.data;
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
