import path from "path"
import react from "@vitejs/plugin-react"
import { defineConfig } from "vite"
// import fs from "fs"

export default defineConfig({
  plugins: [react()],
  server: {
    allowedHosts: ["optimal-pleasantly-spaniel.ngrok-free.app"],
    port: 5173
  },
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./src"),
    },
  },
})
