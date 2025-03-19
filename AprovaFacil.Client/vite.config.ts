import path from "path"
import react from "@vitejs/plugin-react"
import { defineConfig } from "vite"
// import fs from "fs"

export default defineConfig({
  plugins: [react()],
  server: {
    // https: {
    //   key: fs.readFileSync('E:\\Desenvolvimento\\CSharp\\AprovaFacil\\certificado\\key.pem'),
    //   cert: fs.readFileSync('E:\\Desenvolvimento\\CSharp\\AprovaFacil\\certificado\\cert.pem')
    // },
    port: 5173
  },
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./src"),
    },
  },
})
