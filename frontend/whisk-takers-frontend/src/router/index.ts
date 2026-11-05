import { createRouter, createWebHistory } from "vue-router";
import type { RouteRecordRaw } from "vue-router";
import { getCurrentUser } from "../store/auth";

const routes: RouteRecordRaw[] = [
  { path: "/", name: "Intro", component: () => import("../views/Intro.vue") },
  { path: "/login", name: "Login", component: () => import("../views/Login.vue") },
  { path: "/signup", name: "WorkerSignup", component: () => import("../views/WorkerSignup.vue") },
  { path: "/client", name: "Client", component: () => import("../views/ClientDashboard.vue") },
  { path: "/about", name: "About", component: () => import("../views/About.vue") },
  { path: "/waiter", name: "Waiter", component: () => import("../views/WaiterDashboard.vue") },
  { path: "/chef", name: "Chef", component: () => import("../views/ChefDashboard.vue") },
  { path: "/admin", name: "Admin", component: () => import("../views/AdminDashboard.vue") },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

router.beforeEach((to) => {
  const user = getCurrentUser()
  const publicPaths = new Set(["Intro", "Login", "About", "WorkerSignup", "Client"])
  if (!user && !publicPaths.has(to.name as string)) {
    return { name: 'Intro' }
  }

  if (user) {
    // Prevent cross-role navigation via URL
    if (to.name === 'Waiter' && user.role !== 'waiter') return { name: 'Intro' }
    if (to.name === 'Chef' && user.role !== 'chef') return { name: 'Intro' }
    if (to.name === 'Admin' && user.role !== 'admin') return { name: 'Intro' }
  }
})

export default router;
