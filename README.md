## About

StatusWatchdog is an API-first status page designed to be easily self-hostable and look great. A design goal of the application is to be as easy to integrate with as possible for your own monitoring scripts, allowing you to programmatically update service status, as well as create and resolve incidents.

## Deployment

The recommended way to deploy StatusWatchdog is to run it in a docker container. You can build the docker image for yourself using the included dockerfile.

```
$ docker build -t statuswatchdog:latest ./src/StatusWatchdog/Dockerfile
```

In order to run, StatusWatchdog needs to be provided an API key and credentials to a PostgreSQL database. This is typically done through environment variables when deploying through docker.

First, generate a random string to serve as your API key:

```
$ openssl rand -base64 32
```

To make setup of the container easier, we'll be using `docker-compose`:

```yml
version: "3"
services:
  web:
    image: statuswatchdog:latest
    ports:
      - "5000:80"
    environment:
      - API_KEY=foo
      - PG_HOST=db
      - PG_USERNAME=postgres
      - PG_PASSWORD=postgres
      - PG_DATABASE=postgres
    depends_on:
      - "db"
  db:
    image: postgres
    volumes:
      - persist:/var/lib/postgresql
    environment:
      - POSTGRES_PASSWORD=postgres
volumes:
  persist:
```

Running `docker-compose up -d` at this point should result in an instance of StatusWatchdog becoming available at `http://localhost:5000`.

## Screenshots
![status-page-screenshot](img/status-page.png)
![issue-report-screenshot](img/issue-report.png)

## Development

When working on StatusWatchdog you'll want two CLI sessions running for rapid feedback during development:

In src/StatusWatchdog
```
$ dotnet watch run
```

In src/StatusWatchdog/client-app
```
$ npm run watch
```
