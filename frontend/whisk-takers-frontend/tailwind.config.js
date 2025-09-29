/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      keyframes: {
        fadeIn: {
          "0%": { opacity: "0" },
          "100%": { opacity: "1" },
        },
        slideUp: {
          "0%": { transform: "translateY(20px)", opacity: "0" },
          "100%": { transform: "translateY(0)", opacity: "1" },
        },
        royalGlow: {
          "0%": { filter: "drop-shadow(0 0 2px #FFD700)" },
          "50%": { filter: "drop-shadow(0 0 6px #FFD700)" },
          "100%": { filter: "drop-shadow(0 0 2px #FFD700)" },
        },
      },
      animation: {
        "fade-in": "fadeIn 0.6s ease-out forwards",
        "slide-up": "slideUp 0.6s ease-out forwards",
        "royal-glow": "royalGlow 2s ease-in-out infinite",
      },
      colors: {
        gold: "#FFD700",
        "royal-green": "#0B3D2E",
        "royal-green-light": "#1F6F5A",
        "warm-white": "#FDFCF9",
      },
      boxShadow: {
        royal: "0 4px 20px rgba(0,0,0,0.25)",
        elegant: "0 2px 10px rgba(0,0,0,0.15)",
      },
      backgroundImage: {
        "gradient-royal": "linear-gradient(135deg, #0B3D2E, #1F6F5A)",
        "gradient-elegant": "linear-gradient(135deg, #FDFCF9, #E8E6E1)",
      },
    },
  },
  plugins: [],
}
