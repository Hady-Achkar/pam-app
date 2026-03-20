# build stage
FROM node:22-alpine AS build

ENV PNPM_HOME="/pnpm"
ENV PATH="$PNPM_HOME:$PATH"
RUN corepack enable

WORKDIR /app

# copy lockfile first for layer caching
COPY package.json pnpm-lock.yaml ./
RUN pnpm install --frozen-lockfile

# VITE_* env vars are baked into the bundle at build time
ARG VITE_API_URL
ENV VITE_API_URL=$VITE_API_URL

COPY . .
RUN pnpm build

# nginx to serve static files
FROM nginx:alpine

RUN rm /etc/nginx/conf.d/default.conf
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
