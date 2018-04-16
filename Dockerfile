FROM kkarczmarczyk/node-yarn:8.0
WORKDIR /app

# copy project and restore as distinct layers
COPY package.json ./
COPY yarn.lock ./
RUN yarn install

# copy everything else and build
COPY . ./
RUN yarn build

CMD ["node", "/app/dist/index.js"]
