FROM node:16.13.0

WORKDIR /usr/src/app

COPY .env ./
COPY package*.json ./
RUN npm install --only=production
COPY ./src ./src

CMD npm start