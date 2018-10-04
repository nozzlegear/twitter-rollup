FROM kkarczmarczyk/node-yarn:8.0
WORKDIR /app

# copy project and restore as distinct layers
COPY package.json ./
COPY yarn.lock ./
RUN yarn install

# copy everything else and build
COPY . ./
RUN yarn build

# Expose the /etc/twitter-rollup folder as a volume, which lets the host machine persist the tweet history file to disk
RUN mkdir -p /etc/twitter-rollup 
VOLUME /etc/twitter-rollup

CMD ["node", "/app/dist/index.js"]
