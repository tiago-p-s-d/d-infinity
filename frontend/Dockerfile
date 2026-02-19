FROM node:22-alpine
WORKDIR /app

# Instalamos o Angular CLI globalmente no contêiner
RUN npm install -g @angular/cli

COPY package*.json ./
RUN npm install

# Copiamos TUDO (necessário para o ng serve)
COPY . .

EXPOSE 4000

# O ng serve mantém o processo vivo e aceita conexões externas
CMD ["ng", "serve", "--host", "0.0.0.0", "--port", "4000", "--disable-host-check"]FROM node:22-alpine
WORKDIR /app

# Instalamos o Angular CLI globalmente no contêiner
RUN npm install -g @angular/cli

COPY package*.json ./
RUN npm install

# Copiamos TUDO (necessário para o ng serve)
COPY . .

EXPOSE 4000

# O ng serve mantém o processo vivo e aceita conexões externas
CMD ["ng", "serve", "--host", "0.0.0.0", "--port", "4000"]