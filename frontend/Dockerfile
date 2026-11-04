# ============================================================================
# Etapa 1: Compilación de la aplicación Angular 18 con Node.js
# ============================================================================
FROM node:20-alpine AS build
WORKDIR /app

# Copiar manifiestos e instalar dependencias
COPY package*.json ./
RUN npm install

# Copiar código fuente y compilar bundle para producción
COPY . ./
RUN npm run build

# ============================================================================
# Etapa 2: Servidor web ultraligero Nginx para servir los estáticos y enrutar
# ============================================================================
FROM nginx:alpine
WORKDIR /usr/share/nginx/html

# Limpiar archivos por defecto de Nginx
RUN rm -rf ./*

# Copiar configuración personalizada de Nginx
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Copiar bundle compilado de Angular (dist/comerciantes-frontend/browser)
COPY --from=build /app/dist/comerciantes-frontend/browser ./

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
