### Build stage
FROM node:20-alpine AS build
WORKDIR /app

# Install dependencies
COPY frontend/package.json frontend/package-lock.json* ./
RUN npm ci --silent

# Copy source and build
COPY frontend/ ./
RUN npm run build --silent

### Production stage
FROM nginx:stable-alpine AS runtime

# Remove default nginx config and content
RUN rm -rf /usr/share/nginx/html/* /etc/nginx/conf.d/default.conf

# Copy custom nginx config
COPY frontend/nginx.conf /etc/nginx/conf.d/default.conf

# Copy built Angular app (dist/<project-name>/browser for Angular 21+)
COPY --from=build /app/dist/frontend/browser/ /usr/share/nginx/html/

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
