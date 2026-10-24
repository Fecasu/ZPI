import { defineConfig } from 'vite';

export default defineConfig({
  build: { outDir: '../wwwroot', emptyOutDir: true },
  server: { host: '127.0.0.1' }
});
