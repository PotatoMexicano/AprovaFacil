import path from "path"
import react from "@vitejs/plugin-react"
import { defineConfig } from "vite"
import fs from "fs"

export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/api': {
        target: 'https://localhost:7296',
        changeOrigin: false,
        secure: false
      },
    },
    https: {
      key: fs.readFileSync('../certificado/localhost+2-key.pem'),
      cert: fs.readFileSync('../certificado/localhost+2.pem')
    },
    host: '127.0.0.1',
    port: 5173
  },
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./src"),
    },
  },
})
