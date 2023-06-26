<!-- <div style="text-align: center">

<img src="https://cdn.dribbble.com/users/42044/screenshots/3005802/media/e9d2cfc8f3ccdedebef7a8af171fbd08.jpg" width=15% height=15%>

</div> -->

# Microservice Project with .NET Core 6 using CQRS Design Pattern

<!-- [![Elastic Stack version](https://img.shields.io/badge/Elastic%20Stack-8.3.2-00bfb3?style=flat&logo=elastic-stack)](https://www.elastic.co/blog/category/releases)
[![Build Status](https://github.com/deviantony/docker-elk/workflows/CI/badge.svg?branch=main)](https://github.com/deviantony/docker-elk/actions?query=workflow%3ACI+branch%3Amain)
[![Join the chat at https://gitter.im/deviantony/docker-elk](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/deviantony/docker-elk?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) -->

## Abstract

I was curious about Microservice Architectures and Technologies like **Docker, Kubernetes, RabbitMQ and Asynchronous Programming**. So I developed **Microservice via .NET Core 6** using CQRS Design Pattern.

## Philosophy

Main goal is:  Develop MP4 to MP3 converter with Asynchronous way.

- I used **Kong API Gateway**. Because uploading a file to server might be dangerous. Some person can upload virus or trojan to server and person can access everything on server via virus file, this is gonna be devastation. So I did a precaution. (I made some precautions (like file extension checking, file size checking) on WebService that I developed **.NET Core 6** but this is minor precaution)

- File uploading to WebService and converting process is resource consuming and response from WebService might be resolve as **'Timeout'**, you actually waited the response maybe a long time and you faced **'Timeout exception'**, this is bad and the person that use the WebService might be very angry about that. I find out that the solution is using queue well **RabbitMQ**. Actually you don't wait response, only send the request and do other work via **asynchronous way**. Eventually you will get desired result sound and safe. **Timeout** **exception** never gonna be problem. Sometimes maybe **RabbitMQ Container** gonna crash but this is not huge, you should use **durable queues and persistent messages** and this is important, **you MUST use volumes on containers or you will lose everyting!** With that way you will never lose requests and messages, the user eventually get the desired result late or early.

- I used **Minio S3 Object Storage** to store **MP4 and MP3 files**. Object Storages like **Amazon S3** is very popular. They are cheap and fast. At first I decided to use **SQL** or **MongoDB** to store files but I thought this is very bad idea.

- I used **ElasticSearch** for logging purposes. Because **ElasticSearch** is very fast and popular for logging. It stores JSON files and uses indexes and you can get desired results with document scoring mechanism, you can create dashboards on **Kibana**. This is cool!

- I used **.NET Core 6**. Because I wanted to learn something from **.NET Technologies**.
  
- I used same network configuration on all containers **(Kong, RabbitMQ, Minio, WebService and others)**. Because containers can communicate each other via same using same network.
  
- I used **CQRS Design Pattern** on Web Service and other Microservices.

- I used **Initilization Service** for building required elements such as **create Bucket in Minio**, **create indexes in ELK** and **create queues-exchanges and binding them each other in RabbitMQ**. It runs **only once.**

## What is CQRS?

Check the link: <https://learn.microsoft.com/tr-tr/azure/architecture/patterns/cqrs>

## Contents

- [Microservice Project with .NET Core 6 using CQRS Design Pattern](#microservice-project-with-net-core-6-using-cqrs-design-pattern)
  - [Abstract](#abstract)
  - [Philosophy](#philosophy)
  - [What is CQRS?](#what-is-cqrs)
  - [Contents](#contents)
  - [Features](#features)
  - [Requirements](#requirements)
    - [Docker installation](#docker-installation)
    - [ElasticSearch Installation](#elasticsearch-installation)
    - [Minio Installation](#minio-installation)
    - [RabbitMQ Installation](#rabbitmq-installation)
    - [Kong API Gateway Installation](#kong-api-gateway-installation)
      - [Kong API Gateway Definitions](#kong-api-gateway-definitions)
        - [Kong API Snapshot](#kong-api-snapshot)
    - [Install project with Docker Container](#install-project-with-docker-container)
  - [Crontab definition](#crontab-definition)
  - [Overall Architecture](#overall-architecture)
  - [Controllers (API Endpoints)](#controllers-api-endpoints)
    - [`GET` /api/v1/Health](#get-apiv1health)
    - [`POST` /api/v1/main/Converter](#post-apiv1mainconverter)
  - [Business Logic](#business-logic)
    - [Converter Microservice](#converter-microservice)
    - [Logger Microservice](#logger-microservice)
    - [Web Service](#web-service)
    - [Notification Microservice](#notification-microservice)
    - [Initilization Microservice](#initilization-microservice)
  - [Installation](#installation)
  - [Contributing](#contributing)
  - [Helper Documents](#helper-documents)
  - [Bug Reports \& Feature Requests](#bug-reports--feature-requests)

## Features

- Developed via **.Net Core 6**
- Uses **RabbitMQ** for Asynchronous Communication
- Uses **ElasticSearch** for logging request, responses, error messages and info messages
- Uses **Microsoft ILogging** for Console Logging
- Uses **Minio S3 Object Storage** for store files
- Uses **Kong API Gateway** for API security
- Uses **FFMpeg** to convert MP4 to MP3
- Uses Initilization service that builds required elements for the project and you are ready to go.
- Easy to deploy, just use Dockerfile to build image and use Docker Compose to deploy microservices
- Uses Environment Variables to more dynamic development (Test and Production environments)
- Can run on any platform (Mac, Linux and Windows wherever you want!)

## Requirements

> **Note** <br />
> Currently I use **Docker version 20.10.21** and **Docker Compose version v2.6.0** <br />
> Currently I use **Linux Ubuntu 18.06 LTS** machine on Google Cloud. <br />

### Docker installation

Docker Engine and Docker Compose must be installed. Check out on Docker's offical site.

### ElasticSearch Installation

I followed instructions on this github repo: <https://github.com/deviantony/docker-elk> . Check readme, instructions are clear and simple.
Note: I used **Elastic, Logstash and Kibana version 8.5.1** at the time of I wrote this readme

### Minio Installation

Use below docker-compose file.

```bash
version: "3.9"

services:
  minio:
    image: quay.io/minio/minio
    volumes:
      - minio-volume:/data
    networks:
      default: null
    restart: always
    container_name: minio_object_storage
    environment:
      MINIO_ROOT_USER: ${MINIOUSER}
      MINIO_ROOT_PASSWORD: ${MINIOPASSWD}
    ports:
      - 9000:9000
      - 9001:9001
    command: server --console-address ":9001" /data
volumes:
  minio-volume:

networks:
  default:
    name: kong_default

```

### RabbitMQ Installation

1. Use below docker-compose file.

```bash
version: "3.9"

services:
  rabbitmq:
    image: rabbitmq:3-management
    volumes:
      - rabbitmq-volume:/var/lib/rabbitmq
    networks:
      default: null
    restart: always
    hostname: my_rabbitmq
    container_name: rabbitmq
    environment:
      RABBITMQ_DEFAULT_USER: ${MQUSER}
      RABBITMQ_DEFAULT_PASS: ${MQPWD}
    ports:
      - 15672:15672
      - 5672:5672
volumes:
  rabbitmq-volume:

networks:
  default:
    name: kong_default

```

### Kong API Gateway Installation

I followed instructions on this github repo: <https://github.com/Kuari/kong-konga-docker-compose> . Check readme, instructions are clear and simple.

For example, I used below docker-compose file.

```bash
version: "3"

services:
  kong-database:
    image: postgres:9.6
    container_name: kong-database
    expose:
      - "5432"
    environment:
      - POSTGRES_USER=${KONGDBUSER}
      - POSTGRES_DB=${KONGDBNAME}
      - POSTGRES_PASSWORD=${KONGPWD}
    networks:
      default: null
    volumes:
      - "db-data-kong-postgres:/var/lib/postgresql/data"

  kong-migrations:
    image: kong
    environment:
      - KONG_DATABASE=postgres
      - KONG_PG_HOST=kong-database
      - KONG_PG_PASSWORD=${KONGPWD}
      - KONG_CASSANDRA_CONTACT_POINTS=kong-database
    command: kong migrations bootstrap
    restart: on-failure
    depends_on:
      - kong-database

  kong:
    image: kong
    container_name: kong
    environment:
      - LC_CTYPE=en_US.UTF-8
      - LC_ALL=en_US.UTF-8
      - KONG_DATABASE=postgres
      - KONG_PG_HOST=kong-database
      - KONG_PG_USER=${KONGDBUSER}
      - KONG_PG_PASSWORD=${KONGPWD}
      - KONG_CASSANDRA_CONTACT_POINTS=kong-database
      - KONG_PROXY_ACCESS_LOG=/dev/stdout
      - KONG_ADMIN_ACCESS_LOG=/dev/stdout
      - KONG_PROXY_ERROR_LOG=/dev/stderr
      - KONG_ADMIN_ERROR_LOG=/dev/stderr
      - KONG_ADMIN_LISTEN=${KONG_ADMINHOST}:8001, ${KONG_ADMINHOST}:8444 ssl
    restart: on-failure
    expose:
      - 8001
      - 8444
    ports:
      - 8000:8000
      - 8443:8443
    networks:
      default: null
    links:
      - kong-database:kong-database
    depends_on:
      - kong-migrations

  konga:
    image: pantsel/konga
    ports:
      - 1337:1337/tcp
    networks:
      default: null
    links:
      - kong:kong
    container_name: konga
    environment:
      - ${KONGA_ENVIRONMENT}

volumes:
  db-data-kong-postgres:

networks:
  default:
    name: kong_default

```

#### Kong API Gateway Definitions

There is some examples of service definitions in folder called **"\YAML-Files\Kong\Service-Route Definitions"**.

You should define exactly as I do in pictures.

<img src="./YAML-Files/Kong/Service-Route Definitions/ConverterServiceDefinition.png" >

Also there is example of route definition of service. You can define whatever you want. Just define route and bind to service.

##### Kong API Snapshot

The Snapshot file is in **"YAML Files/Kong/Snapshot"** folder. File format is JSON and you can import it in your **Kong** instance using **Konga Admin Dashboard**.

### Install project with Docker Container

Just use the Dockerfile to build image. After that use **"docker run"** command to run microservices.

## Crontab definition

You can use below crontab definition in your Linux Environment. You can edit whatever you like.

```bash
*/7 * * * * docker restart notification_service #container name can be edited

*/7 * * * * docker restart logger_service #container name can be edited

*/7 * * * * docker restart converter_service #container name can be edited
```

## Overall Architecture

<img src="./ProjectArch.png" >

## Controllers (API Endpoints)

### `GET` /api/v1/Health

**Parameters**

|          Name | Required |  Type   | Description                                                                                                                                                           |
| -------------:|:--------:|:-------:| --------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
|     n/a | n/a | n/a | <br/> n/a <br/><br/>                                                                     |

**Response**

```

//The tools that is used by webservice and microservices is running (online)
{
    "HostStatus": "Host is working!"
}

//The tools that is used by webservice and microservices is not running or online
{
    "HostStatus": "Host is not working!"
}

```

### `POST` /api/v1/main/Converter

**Parameters**

|          Name | Required |  Type   | Description                                                                                                                                                           |
| -------------:|:--------:|:-------:| --------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
|     `file` | required | Form Data  | <br/> MP4 file to upload to API <br/><br/>                                                                     |
|     `email` | required | Form Data  | <br/> Email address to receive converted from MP4 file that sent to MP3 <br/><br/>                                                                     |

**Response**

```
//File uploaded succesfully
{
    "responseCode": 200,
    "message": "File uploaded!",
    "errorMessage": "",
    "errorCode": 0
}

//File format error
{
    "ErrorMessage": "File format must be mp4!",
    "ErrorCode": 400
}

//Error occurred while uploading the file
{
    "ErrorMessage": "Some error text",
    "ErrorCode": 500
}
```

<!-- > **Note** <br />
> :exclamation: :exclamation: :exclamation:  :exclamation: :exclamation: :exclamation: <br /> -->

## Business Logic

I used CQRS Design Pattern Web Service and other Microservices.

### Converter Microservice

- Uses **RabbitMQ** for **Asynchronous Communication**.
- Gets messages that contains email and file guid data from queue.
- Fetches **MP4** file's stream data from **Minio Object Storage** via File Guid data that exists in queue message.
- Converts **MP4** file's stream data to **MP3** stream data via **FFMpeg Library** (Basically converts **MP4** file to **MP3** file).
- New Guid is generated and **MP3** stream data is saved as **MP3** file in **Minio Object Storage**.
- Email and New File Guid is sent to queue as message with **TTL**. (I used 43200000 milliseconds = 12 hours as TTL). The reason is I wanted to limit the requests and don't occupy too much the services.
- Complete convert process until new message arrives to queue.
- Also error logs are sent to queue called **'errorlogs**' and info logs are sent to queue called **'otherlogs'**. They are ready to consumed by **Logger Microservice**.
- It uses **Environment Variables**. You can pass any **RabbitMQ, Minio or ELK host, username, password into web service as environment variables in code and Docker Compose file**. It is more dynamic for me because, host servers of Minio and other technologies can change during development (Like Test environment and Production environment).

### Logger Microservice

- Uses **RabbitMQ** for **Asynchronous Communication**.
- Uses **ElasticSearch** for saving logs into indexes.
- Fetches message from two different queues. They called **'errorlogs'** and **'otherlogs'** queues.
- Saves logs into two different indexes in **ElasticSearch**. **Errorlogs** are saved into index that called **'loggerservice_errorlogs'**. **Infologs** (Otherlogs) are saved into index that called **'loggerservice_otherlogs'**.
- It is bassically logger.
- It uses **Environment Variables**. You can pass any **RabbitMQ, Minio or ELK host, username, password into web service as environment variables in code and Docker Compose file**. It is more dynamic for me because, host servers of Minio and other technologies can change during development (Like Test environment and Production environment).

### Web Service

- It is web service that you can upload **MP4** file (Max size of file is around **30MB** is limited, this is handled by **Environment Variable** as well).
- It has only two controller (route). **Health Controller** is checking the health of API and its tools like ELK, RabbitMQ. **Converter Controller** is the main thing of the project, you can upload **MP4** file for convert process.
- It is **REST API**.
- It uses **RabbitMQ** for queue the message that contains of email and Guid of uploaded **MP4** file with **TTL** (I used 3600000 milliseconds = 1 hour as TTL). The reason is I wanted to limit the requests and don't occupy too much the services.
- It uses **Minio Object Storage** to store **MP4** files.
- It uses **ElasticSearch** to log errors and infos.
- It uses **Environment Variables**. You can pass any **RabbitMQ, Minio or ELK host, username, password into web service as environment variables in code and Docker Compose file**. It is more dynamic for me because, host servers of Minio and other technologies can change during development. It is Like Test environment and Production environment.

### Notification Microservice

- It uses **RabbitMQ** to fetch message that contains email address and **MP3** File Guid from queue. Guid is generated in **Converter Microservice**.
- It uses **Minio Object Storage** to fetch **MP3** files via Guid.
- It uses **Environment Variables**. You can pass any **RabbitMQ, Minio or ELK host, username, password into web service as environment variables in code and Docker Compose file**. It is more dynamic for me because, host servers of Minio and other technologies can change during development (Like Test environment and Production environment).
- It uses **Google Gmail** to send **MP3** file in the attachment as email to user. This configuration is based on **Environment Variables** as well. You can pass working **SMTP Host, Username, Password and Port to project via using Environment Variables**.
- Error logs are sent to queue called **'errorlogs'** and info logs are sent to queue called **'otherlogs'**. They are ready to consumed by **Logger Microservice**.

### Initilization Microservice

- It is basically Initilization service for the whole project. It builds required elements for the project. It runs only once.
- It creates required Minio Bucket.
- It creates required RabbitMQ Queue - Exchanges and binds them each other. Assigns **message TTL** property to exchanges.
- It creates required ELK Indexes.

## Installation

At the root of project, there is a folder called **'YAML-Files'**. Each folder name represents technologies that I used in this project. In that folders, there are docker-compose files corresponds to each related folder. Use them to install on **Docker Compose**.

To install the Microservices, go to **Microservices folder** and each Microservice are represented as folder name. Inside of that folders there is **docker-compose** file. Use them via creating **.env** file in your workspace or whatever you want!

Install **InitilizationService** **first and install the other services second!**

> **Note** <br />
> :exclamation: :exclamation: :exclamation: Install **InitilizationService** **first and install the other services second!** :exclamation: :exclamation: :exclamation: <br />

## Contributing

I am open every advice for my project. I am planning to improve myself on **.NET Core 6, Microservices and Container Technologies**. So don't hesitate comment on my project. Every idea is plus for me.

## Helper Documents

ELK script and Docker Compose files added to project. Folder names are **'YAML Files'** and **'ELK Scripts'**.

## Bug Reports & Feature Requests

Please use the Github issues.
