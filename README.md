<!-- <div style="text-align: center">

<img src="https://cdn.dribbble.com/users/42044/screenshots/3005802/media/e9d2cfc8f3ccdedebef7a8af171fbd08.jpg" width=15% height=15%>

</div> -->

# Microservice Project with .NET Core 6

<!-- [![Elastic Stack version](https://img.shields.io/badge/Elastic%20Stack-8.3.2-00bfb3?style=flat&logo=elastic-stack)](https://www.elastic.co/blog/category/releases)
[![Build Status](https://github.com/deviantony/docker-elk/workflows/CI/badge.svg?branch=main)](https://github.com/deviantony/docker-elk/actions?query=workflow%3ACI+branch%3Amain)
[![Join the chat at https://gitter.im/deviantony/docker-elk](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/deviantony/docker-elk?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) -->

## Asbtract

I was curious about Microservice Architectures and Technologies like Docker, Kubernetes, RabbitMQ and Asynchronous Programming. So I developed simple Microservice via .Net Core 6.

## Philosophy

Main goal was develop Mp4 to MP3 converter with Asynchronous way.

- I used Kong API Gateway. Because uploading a file to server might be dangerous. Some person can upload virus or trojan to server and person can access everything on server via virus file, this is gonna be devastation. So i did a precaution. (I made some precautions (like file extension checking, file size checking) on WebService that i developed .Net Core 6 but this is minor precaution)

- File uploading to webservice and converting process is resource consuming and response from webservice might be resolve as 'Timeout', you actually waited the response maybe a long time and you faced 'Timeout exception', this is bad and the person that use the webservice might be very angry about that. I find out that the solution is using queue well RabbitMQ. Actually you don't wait response, only send the request and do other work via asynchronous way. Eventually you will get desired result sound and safe. Timeout exception never gonna be problem. Sometimes maybe RabbitMQ Container gonna crash but this is not huge, you should use durable queues and persistent messages and this is important, you MUST use volumes on containers or you will lose everyting! With that way you will never lose requests and messages, the user eventually get the desired result late or early.

- I used Minio S3 Object Storage to store MP4 and MP3 files. Object Storages like Amazon S3 is very popular. They are cheap and fast. At first i decided to use SQL or MongoDB to store files but i thought this is very bad idea.

- I used ElasticSearch for logging purposes. Because ElasticSearch is very fast and popular for logging. It stores JSON files and uses indexes and you can get desired results with document scoring mechanism, you can create dashboards on Kibana. This is cool!

- I used .Net Core 6. Because I wanted to learn something from .Net Technologies.
  
- I used same network configuration on all containers (Kong, RabbitMQ, Minio, WebService and others). Because containers can communicate each other via same using same network.

## Contents

- [Microservice Project with .NET Core 6](#microservice-project-with-net-core-6)
  - [Asbtract](#asbtract)
  - [Philosophy](#philosophy)
  - [Contents](#contents)
  - [Features](#features)
  - [Requirements](#requirements)
    - [Docker installation](#docker-installation)
    - [ElasticSearch Installation](#elasticsearch-installation)
      - [Creation of Index on ElasticSearch-Kibana](#creation-of-index-on-elasticsearch-kibana)
    - [Minio Installation](#minio-installation)
    - [RabbitMQ Installation](#rabbitmq-installation)
    - [Kong API Gateway Installation](#kong-api-gateway-installation)
      - [Kong API Gateway Definitions](#kong-api-gateway-definitions)
        - [Kong API Snapshot](#kong-api-snapshot)
    - [Install project with Docker Container](#install-project-with-docker-container)
  - [Overall Architecture](#overall-architecture)
  - [Business Logic](#business-logic)
    - [Converter Microservice](#converter-microservice)
    - [Logger Microservice](#logger-microservice)
    - [Web Service](#web-service)
    - [Notification Microservice](#notification-microservice)
  - [Structure](#structure)
  - [Installation](#installation)
  - [Contributing](#contributing)
  - [Helper Documents](#helper-documents)
  - [Bug Reports \& Feature Requests](#bug-reports--feature-requests)
  - [RoadMap](#roadmap)

## Features

- Developed via **.Net Core 6**
- Uses **RabbitMQ** for Asynchronous Communication
- Uses **ElasticSearch** for logging request, responses, error messages and info messages
- Uses **Log4Net** File Logging due to missed ElasticSearch logs. The logs never missed.
- Uses **Minio S3 Object Storage** for store files
- Uses **Kong API Gateway** for API security
- Uses **FFMpeg** to convert MP4 to MP3
- Easy to deploy, just use Dockerfile to build image and use Docker Compose to deploy microservices
- Uses Environment Variables to more dynamic development (Test and Production environments)
- Can run on any platform (Mac, Linux and Windows wherever you want!)

## Requirements

> **Note** <br />
> Currently I use **Docker version 20.10.21** and **Docker Compose version v2.6.0** <br />
> Currently I use **Docker Compose version v2.12.2** <br />
> Currently I use **Linux Ubuntu 18.06 LTS** machine on Google Cloud. <br />

### Docker installation

Docker Engine and Docker Compose must be installed. Check out on Docker's offical site.

### ElasticSearch Installation

I followed instructions on this github repo: <https://github.com/deviantony/docker-elk> . Check readme, instructions are clear and simple.
Note: I used Elastic, Logstash and Kibana version **8.5.1** at the time of i wrote this readme

#### Creation of Index on ElasticSearch-Kibana

1. Open Dev Console on Kibana.

2. Run commands below on Kibana Dev Console to create indexes.

```bash
PUT /loggerservice_errorlogs
{
    "settings": {
        "index": {
            "number_of_shards": 1,
            "number_of_replicas": 0
        }
    },
    "mappings": {
        "properties": {
            "converterLog": {
                "type": "nested"
            },
            "loggerLog": {
                "type": "nested"
            },
            "notificationLog": {
                "type": "nested"
            },
            "queueLog": {
                "type": "nested",
                "properties": {
                    "date": {
                        "type": "date"
                    },
                    "exceptionMessage": {
                        "type": "text",
                        "fields": {
                            "keyword": {
                                "type": "keyword",
                                "ignore_above": 256
                            }
                        }
                    },
                    "exchangeName": {
                        "type": "text",
                        "fields": {
                            "keyword": {
                                "type": "keyword",
                                "ignore_above": 256
                            }
                        }
                    },
                    "operationType": {
                        "type": "text",
                        "fields": {
                            "keyword": {
                                "type": "keyword",
                                "ignore_above": 256
                            }
                        }
                    },
                    "queueName": {
                        "type": "text",
                        "fields": {
                            "keyword": {
                                "type": "keyword",
                                "ignore_above": 256
                            }
                        }
                    },
                    "routingKey": {
                        "type": "text",
                        "fields": {
                            "keyword": {
                                "type": "keyword",
                                "ignore_above": 256
                            }
                        }
                    }
                }
            },
            "storageLog": {
                "type": "nested"
            }
        }
    }
}

```

```bash
PUT /loggerservice_otherlogs
{
    "settings": {
        "index": {
            "number_of_shards": 1,
            "number_of_replicas": 0
        }
    },
    "mappings": {
        "properties": {
            "converterLog": {
                "type": "nested",
                "properties": {
                    "date": {
                        "type": "date"
                    },
                    "error": {
                        "type": "text",
                        "fields": {
                            "keyword": {
                                "type": "keyword",
                                "ignore_above": 256
                            }
                        }
                    },
                    "info": {
                        "type": "text",
                        "fields": {
                            "keyword": {
                                "type": "keyword",
                                "ignore_above": 256
                            }
                        }
                    }
                }
            },
            "queueLog": {
                "type": "nested",
                "properties": {
                    "date": {
                        "type": "date"
                    },
                    "exceptionMessage": {
                        "type": "text",
                        "fields": {
                            "keyword": {
                                "type": "keyword",
                                "ignore_above": 256
                            }
                        }
                    },
                    "exchangeName": {
                        "type": "text",
                        "fields": {
                            "keyword": {
                                "type": "keyword",
                                "ignore_above": 256
                            }
                        }
                    },
                    "message": {
                        "type": "text",
                        "fields": {
                            "keyword": {
                                "type": "keyword",
                                "ignore_above": 256
                            }
                        }
                    },
                    "operationType": {
                        "type": "text",
                        "fields": {
                            "keyword": {
                                "type": "keyword",
                                "ignore_above": 256
                            }
                        }
                    },
                    "queueName": {
                        "type": "text",
                        "fields": {
                            "keyword": {
                                "type": "keyword",
                                "ignore_above": 256
                            }
                        }
                    },
                    "routingKey": {
                        "type": "text",
                        "fields": {
                            "keyword": {
                                "type": "keyword",
                                "ignore_above": 256
                            }
                        }
                    }
                }
            },
            "storageLog": {
                "type": "nested",
                "properties": {
                    "bucketName": {
                        "type": "text",
                        "fields": {
                            "keyword": {
                                "type": "keyword",
                                "ignore_above": 256
                            }
                        }
                    },
                    "contentLength": {
                        "type": "long"
                    },
                    "contentType": {
                        "type": "text",
                        "fields": {
                            "keyword": {
                                "type": "keyword",
                                "ignore_above": 256
                            }
                        }
                    },
                    "date": {
                        "type": "date"
                    },
                    "exceptionMessage": {
                        "type": "text",
                        "fields": {
                            "keyword": {
                                "type": "keyword",
                                "ignore_above": 256
                            }
                        }
                    },
                    "objectName": {
                        "type": "text",
                        "fields": {
                            "keyword": {
                                "type": "keyword",
                                "ignore_above": 256
                            }
                        }
                    },
                    "operationType": {
                        "type": "text",
                        "fields": {
                            "keyword": {
                                "type": "keyword",
                                "ignore_above": 256
                            }
                        }
                    }
                }
            }
        }
    }
}
```

```bash
PUT /webservice_objstorage_logs
{
    "settings": {
        "index": {
            "number_of_shards": 1,
            "number_of_replicas": 0
        }
    },
    "mappings": {
        "properties": {
            "bucketName": {
                "type": "text"
            },
            "contentLength": {
                "type": "long"
            },
            "contentType": {
                "type": "text"
            },
            "date": {
                "type": "date"
            },
            "objectName": {
                "type": "text"
            }
        }
    }
}
```

```bash
PUT /webservice_requestresponse_logs
{
    "settings": {
        "index": {
            "number_of_shards": 1,
            "number_of_replicas": 0
        }
    },
    "mappings": {
        "properties": {
            "requestContentType": {
                "type": "text"
            },
            "requestDate": {
                "type": "date"
            },
            "requestFileDetails": {
                "type": "nested",
                "properties": {
                    "createdDate": {
                        "type": "date"
                    },
                    "length": {
                        "type": "text",
                        "fields": {
                            "keyword": {
                                "type": "keyword",
                                "ignore_above": 256
                            }
                        }
                    },
                    "name": {
                        "type": "text",
                        "fields": {
                            "keyword": {
                                "type": "keyword",
                                "ignore_above": 256
                            }
                        }
                    }
                }
            },
            "responseContentType": {
                "type": "text"
            },
            "responseDate": {
                "type": "date"
            },
            "responseMessage": {
                "type": "text"
            },
            "responseStatusCode": {
                "type": "integer"
            }
        }
    }
} 
```

```bash
PUT /webservice_queue_logs
{
    "settings": {
        "index": {
            "number_of_shards": 1,
            "number_of_replicas": 0
        }
    },
    "mappings": {
        "properties": {
            "date": {
                "type": "date"
            },
            "exchangeName": {
                "type": "text"
            },
            "message": {
                "type": "nested",
                "properties": {
                    "email": {
                        "type": "text",
                        "fields": {
                            "keyword": {
                                "type": "keyword",
                                "ignore_above": 256
                            }
                        }
                    },
                    "fileGuid": {
                        "type": "text",
                        "fields": {
                            "keyword": {
                                "type": "keyword",
                                "ignore_above": 256
                            }
                        }
                    }
                }
            },
            "queueName": {
                "type": "text"
            },
            "routingKey": {
                "type": "text"
            }
        }
    }
}
```

3. Go Index Management on Kibana. Make sure that indexes created successfully.

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

2. Create exchange called **'converter_exchange.direct'**. Type must be direct. After that, create queue called **'converter'** with properties of **"x-message-ttl: 43200000"** and **"durable: true"**. Bind queue to **'converter_exchange.direct'** exchange.

3. Create exchange called **'log_exchange.direct'**. Type must be direct. After that, create queue called **'errorlogs'** with property of **"durable: true"** and create queue **'otherlogs'** with property of **"durable: true"**. Bind queue to **'log_exchange.direct'** exchange.

4. Create exchange called **'notification_exchange.direct'**. Type must be direct. After that, create queue called **'notification'** with properties of **"x-message-ttl: 3600000"** and **"durable: true"**. Bind queue to **'notification_exchange.direct'** exchange.

### Kong API Gateway Installation

I followed instructions on this github repo: <https://github.com/Kuari/kong-konga-docker-compose> . Check readme, instructions are clear and simple.

For example, i used below docker-compose file.

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
    links:
      - kong-database:kong-database
    depends_on:
      - kong-migrations

  konga:
    image: pantsel/konga
    ports:
      - 1337:1337/tcp
    links:
      - kong:kong
    container_name: konga
    environment:
      - ${KONGA_ENVIRONMENT}

volumes:
  db-data-kong-postgres:

```

#### Kong API Gateway Definitions

There is some examples of service definitions in folder called **"\YAML-Files\Kong\Service-Route Definitions"**.

You should define exactly as i do in pictures.

<img src="./YAML-Files/Kong/Service-Route Definitions/ConverterServiceDefinition.png" >

Also there is example of route definition of service. You can define whatever you want. Just define route and bind to service.

##### Kong API Snapshot

The Snapshot file is in "YAML Files/Kong/Snapshot" folder. File format is JSON and you can import it in your Kong instance using Konga Admin Dashboard.

### Install project with Docker Container

Just use the Dockerfile to build image. After that use **"docker run"** command to run microservices.

## Overall Architecture

<img src="./ProjectArch.drawio.png" >

## Business Logic

### Converter Microservice

- Uses RabbitMQ for Asynchronous Communication.
- Gets messages that contains email and file guid data from queue.
- Fetches MP4 file's stream data from Minio Object storage via File Guid data that exists in queue message.
- Converts MP4 file's stream data to MP3 stream data via FFMpeg library (Basically converts MP4 file to MP3 file).
- New Guid is generated and MP3 stream data is saved as MP3 file in Minio Object Storage.
- Email and New File Guid is sent to queue as message with TTL. (I used 43200000 milliseconds = 12 hours as TTL). The reason is i wanted to limit the requests and don't occupy too much the services.
- Complete convert process until new message arrives to queue.
- Also error logs are sent to queue called errorlogs and info logs are sent to queue called otherlogs. They are ready to consumed by Logger Microservice.
- It uses Environment Variables. You can pass any RabbitMQ, Minio or ELK host, username, password into web service as environment variables in code and Docker Compose file. It is more dynamic for me because, host servers of Minio and other technologies can change during development (Like Test environment and Production environment).
- It uses Log4Net File Logging due to missed ElasticSearch logs (connection errors may happen during the process, i thought this is good idea to implement file logging. Logs never gonna be missed).

### Logger Microservice

- Uses RabbitMQ for Asynchronous Communication.
- Uses ElasticSearch for saving logs into indexes.
- Fetches message from two different queues. They called 'errorlogs' and 'otherlogs' queues.
- Saves logs into two different indexes in ElasticSearch. Errorlogs are saved into index that called 'loggerservice_errorlogs'. Infologs (Otherlogs) are saved into index that called 'loggerservice_otherlogs'.
- It is bassically logger.
- It uses Environment Variables. You can pass any RabbitMQ, Minio or ELK host, username, password into web service as environment variables in code and Docker Compose file. It is more dynamic for me because, host servers of Minio and other technologies can change during development (Like Test environment and Production environment).
- It uses Log4Net File Logging due to missed ElasticSearch logs (connection errors may happen during the process, i thought this is good idea to implement file logging. Logs never gonna be missed).

### Web Service

- It is web service that you can upload MP4 file (Max size of file is around 30MB is limited, this is handled by Environment Variable as well).
- It has only two controller (route). Health Controller is checking the health of API and its tools like ELK, RabbitMQ. Converter Controller is the main thing of the project, you can upload MP4 file for convert process.
- It is REST API.
- It uses RabbitMQ for queue the message that contains of email and Guid of uploaded MP4 file with TTL (I used 3600000 milliseconds = 1 hour as TTL). The reason is i wanted to limit the requests and don't occupy too much the services.
- It uses Minio Object Storage to store MP4 files.
- It uses ElasticSearch to log errors and infos.
- It uses Environment Variables. You can pass any RabbitMQ, Minio or ELK host, username, password into web service as environment variables in code and Docker Compose file. It is more dynamic for me because, host servers of Minio and other technologies can change during development (Like Test environment and Production environment).
- It uses Log4Net File Logging due to missed ElasticSearch logs (connection errors may happen during the process, i thought this is good idea to implement file logging. Logs never gonna be missed).

### Notification Microservice

- It uses RabbitMQ to fetch message that contains email address and MP3 File Guid from queue. Guid is generated in Converter Microservice.
- It uses Minio Object Storage to fetch MP3 files via Guid.
- It uses Environment Variables. You can pass any RabbitMQ, Minio or ELK host, username, password into web service as environment variables in code and Docker Compose file. It is more dynamic for me because, host servers of Minio and other technologies can change during development (Like Test environment and Production environment).
- It uses Google Gmail to send MP3 file in the attachment as email to user. This configuration is based on Environment Variables as well. You can pass working SMTP Host, Username, Password and Port to project via using Environment Variables.
- Error logs are sent to queue called errorlogs and info logs are sent to queue called otherlogs. They are ready to consumed by Logger Microservice.
- It uses Log4Net File Logging due to missed ElasticSearch logs (connection errors may happen during the process, i thought this is good idea to implement file logging. Logs never gonna be missed).

## Structure

````bash
root:.
|   cenk.doc
|   README.md
|   
+---Converter-Microservice
|   |   .dockerignore
|   |   .gitignore
|   |   Converter-Microservice.sln
|   |   
|   +---Configuration
|   |   |   Configuration.csproj
|   |   |   EnvVariablesHandler.cs  
|   |                       
|   +---Converter-Microservice
|   |   |   Converter-Microservice.csproj
|   |   |   Program.cs                            
|   |   |                   
|   |   \---Properties
|   |           launchSettings.json
|   |           
|   +---DataAccess
|   |   |   DataAccess.csproj
|   |   |   
|   |   +---bin
|   |   |   \---Debug
|   |   |       \---net6.0
|   |   |           |   
|   |   |           \---Repository
|   |   |                   App.config
|   |   |                   
|   |   +---Interfaces
|   |   |       IConverterRepository.cs
|   |   |       ILog4NetRepository.cs
|   |   |       IObjectStorageRepository.cs
|   |   |       IQueueRepository.cs     
|   |   |                   
|   |   +---Providers
|   |   |       LazyResolutionMiddlewareExtensions.cs
|   |   |       
|   |   \---Repository
|   |           App.config
|   |           ConverterRepository.cs
|   |           Log4NetRepository.cs
|   |           ObjectStorageRepository.cs
|   |           QueueRepository.cs
|   |           
|   +---Models
|   |   |   ConsumerException.cs
|   |   |   ConverterLog.cs
|   |   |   ElkConfiguration.cs
|   |   |   ErrorLog.cs
|   |   |   LoggerLog.cs
|   |   |   MinioConfiguration.cs
|   |   |   Models.csproj
|   |   |   NotificationLog.cs
|   |   |   ObjectDataModel.cs
|   |   |   ObjectStorageLog.cs
|   |   |   OtherLog.cs
|   |   |   QueueLog.cs
|   |   |   QueueMessage.cs
|   |   |   RabbitMqConfiguration.cs
|   |   |   
|   |   +---bin
|   |   |   \---Debug
|   |   |       \---net6.0
|   |   |               Models.deps.json
|   |   |               Models.dll
|   |   |               Models.pdb
|   |                  
|   |                       
|   \---Operation
|       |   Operation.csproj
|       |   
|       +---bin
|       |   \---Debug
|       |       \---net6.0
|       |           |   
|       |           \---Repository
|       |                   App.config
|       |                   
|       +---Interfaces
|       |       IObjectStorageOperation.cs
|       |       IQueueOperation.cs     
|       |                   
|       \---Operations
|               ObjectStorageOperation.cs
|               QueueOperation.cs
|               
+---LoggerMicroservice
|   |   .dockerignore
|   |   .gitignore
|   |   LoggerMicroservice.sln
|   |   
|   +---Configuration
|   |       Configuration.csproj
|   |       EnvVariablesHandler.cs
|   |       
|   +---DataAccess
|   |   |   App.config
|   |   |   DataAccess.csproj
|   |   |   
|   |   +---Interfaces
|   |   |       IILog4NetRepository.cs
|   |   |       ILoggingRepository.cs
|   |   |       IQueueRepository.cs
|   |   |       
|   |   +---Providers
|   |   |       LazyResolutionMiddlewareExtensions.cs
|   |   |       
|   |   \---Repository
|   |           Log4NetRepository.cs
|   |           LoggingRepository.cs
|   |           QueueRepository.cs
|   |           
|   +---LoggerMicroservice
|   |   |   LoggerMicroservice.csproj
|   |   |   Program.cs
|   |   |   
|   |   \---Properties
|   |           launchSettings.json
|   |           
|   +---Models
|   |       ConsumerException.cs
|   |       ConverterLog.cs
|   |       ElkConfiguration.cs
|   |       ErrorLog.cs
|   |       LoggerLog.cs
|   |       Models.csproj
|   |       NotificationLog.cs
|   |       ObjectStorageLog.cs
|   |       OtherLog.cs
|   |       QueueLog.cs
|   |       QueueMessage.cs
|   |       RabbitMqConfiguration.cs
|   |       
|   \---Operation
|       |   Operation.csproj
|       |   
|       +---Interfaces
|       |       ILoggingOperation.cs
|       |       IQueueOperation.cs
|       |       
|       \---Operations
|               LoggingOperation.cs
|               QueueOperation.cs
|               
+---Notification-Microservice
|   |   .dockerignore
|   |   .gitignore
|   |   Notification-Microservice.sln
|   |   
|   +---Configuration
|   |       Configuration.csproj
|   |       EnvVariablesHandler.cs
|   |       
|   +---DataAccess
|   |   |   DataAccess.csproj
|   |   |   
|   |   +---Interfaces
|   |   |       ILog4NetRepository.cs
|   |   |       IMailSenderRepository.cs
|   |   |       IObjectStorageRepository.cs
|   |   |       IQueueRepository.cs
|   |   |       
|   |   +---Providers
|   |   |       LazyResolutionMiddlewareExtensions.cs
|   |   |       QueueProviders.cs
|   |   |       
|   |   \---Repository
|   |           App.config
|   |           Log4NetRepository.cs
|   |           MailSenderRepository.cs
|   |           ObjectStorageRepository.cs
|   |           QueueRepository.cs
|   |           
|   +---Helpers
|   |       Helper.csproj
|   |       
|   +---Models
|   |       ConsumerException.cs
|   |       ConsumerExceptionModel.cs
|   |       ConverterLog.cs
|   |       ElkConfiguration.cs
|   |       ErrorLog.cs
|   |       LoggerLog.cs
|   |       MinioConfiguration.cs
|   |       Models.csproj
|   |       NotificationLog.cs
|   |       ObjectDataModel.cs
|   |       ObjectStorageLog.cs
|   |       OtherLog.cs
|   |       QueueLog.cs
|   |       QueueMessage.cs
|   |       RabbitMqConfiguration.cs
|   |       SmtpConfiguration.cs
|   |       
|   +---Notification-Microservice
|   |   |   Notification-Microservice.csproj
|   |   |   Program.cs
|   |   |   
|   |   \---Properties
|   |           launchSettings.json
|   |           
|   \---Operation
|       |   Operation.csproj
|       |   
|       +---Interfaces
|       |       IMailSenderOperation.cs
|       |       IObjectStorageOperation.cs
|       |       IQueueOperation.cs
|       |       
|       \---Operations
|               MailSenderOperation.cs
|               ObjectStorageOperation.cs
|               QueueOperation.cs
|               
\---WebService
    |   .dockerignore
    |   .gitignore
    |   WebService.sln
    |   
    +---APILayer
    |   |   APILayer.csproj
    |   |   
    |   +---Converters
    |   |       ConverterController.cs
    |   |       
    |   +---Health
    |   |       HealthController.cs
    |          
    +---DataLayer
    |   |   App.config
    |   |   DataLayer.csproj 
    |   +---DataAccess
    |   |       Log4NetRepository.cs
    |   |       LoggingRepository.cs
    |   |       LogOtherRepository.cs
    |   |       MinioStorageRepository.cs
    |   |       QueueRepository.cs
    |   |       
    |   +---Interfaces
    |   |       ILog4NetRepository.cs
    |   |       ILoggingRepository.cs
    |   |       ILogOtherRepository.cs
    |   |       IMinioStorageRepository.cs
    |   |       IQueueRepository.cs
    |          
    +---Helpers
    |   |   EmailFormatHelper.cs
    |   |   Helpers.csproj
    |   |   PingHelper.cs
    |   |   WebServiceException.cs  
    |   +---Interfaces
    |   |       IPingHelper.cs
    |          
    +---Middleware
    |   |   ErrorHandlerMiddleware.cs
    |   |   LoggingMiddleware.cs
    |   |   Middleware.csproj
    |   |   ReadableResponseStreamMiddleware.cs
    |   |   RequestValidationMiddleware.cs
    |      
    |   +---Contexts
    |   |   |   WebServiceContext.cs
    |   |   |   
    |   |   \---Interfaces
    |   |           IWebServiceContext.cs  
    |     
    +---Models
    |   |   ExceptionLogModel.cs
    |   |   HealthResponse.cs
    |   |   Models.csproj
    |   |   ObjectStorageLog.cs
    |   |   QueueLog.cs
    |   |   QueueMessage.cs
    |   |   RequestResponseLogModel.cs
    |   |   UploadMp4Response.cs
    |   |   WebServiceErrors.cs
    |
    +---ServiceLayer
    |   |   ServiceLayer.csproj
    |   |   
    |   +---Interfaces
    |   |       IHealthService.cs
    |   |       ILoggingService.cs
    |   |       IMinioStorageService.cs
    |   |       IQueueService.cs
    |   |       
    |   \---Services
    |           HealthService.cs
    |           LoggingService.cs
    |           MinioStorageService.cs
    |           QueueService.cs
    |           
    \---WebService
        |   appsettings.Development.json
        |   appsettings.json
        |   Dockerfile
        |   Program.cs
        |   WebService.csproj
        |
        +---obj
        |
        \---Properties
                launchSettings.json
````

## Installation

At the root of project, there is a folder called 'YAML Files'. Each folder name represents technologies that i used in this project. In that folders, there are docker-compose files corresponds to each related folder. Use them to install on Docker Compose Linux.

To install the Microservices, go to Microservices folder and each Microservice are represented as folder name. Inside of that folders there is docker-compose file. Use them via creating .env file in your workspace or what ever you want!

## Contributing

I am open every advice for my project. I am planning to improve myself on .NET Core 6, Microservices and Container Technologies. So don't hesitate comment on my project., every idea is plus for me.

## Helper Documents

ELK script and Docker Compose files added to project. Folder names are **'YAML Files'** and **'ELK Scripts'**.

## Bug Reports & Feature Requests

Please use the Github issues.

## RoadMap

- In the Future, i am planning to use Kubernetes and CI/CD Devops tools like Jenkins or Gitlab.
- I want to use different branches for test and production environments.
- Also i am planning to use S3 objects as TTL (Time to live). It will be more efficient way to save some HDD space on virtual machine.
- I am gonna fix RabbitMQ, Minio, ELK docker-compose.yaml files in readme (Ports section, use expose to use network property)
- Tech amblems will be added into arch picture
