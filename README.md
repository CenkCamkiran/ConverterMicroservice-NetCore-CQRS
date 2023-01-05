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

First of all i used Kong API Gateway. Because uploading a file to server might be dangerous. Some person can upload virus or trojan to server and person can access everything on server via virus file, this is gonna be devastation. So i did a precaution. (I made some precautions (like file extension checking, file size checking) on WebService that i developed .Net Core 6 but this is minor precaution)

Second, File uploading to webservice and converting process is resource consuming and response from webservice might be resolve as 'Timeout', you actually waited the response maybe a long time and you faced 'Timeout exception', this is bad and the person that use the webservice might be very angry about that. I find out that the solution is using queue well RabbitMQ. Actually you don't wait response, only send the request and do other work via asynchronous way. Eventually you will get desired result sound and safe. Timeout exception never gonna be problem. Sometimes maybe RabbitMQ Container gonna crash but this is not huge, you should use durable queues and persistent messages and this is important, you MUST use volumes on containers or you will lose everyting! With that way you will never lose requests and messages, the user eventually get the desired result late or early.

Third, I used Minio S3 Object Storage to store MP4 and Mp3 files. Object Storages like Amazon S3 is very popular. They are cheap and fast. At first i decided to use SQL or MongoDB to store files but i thought this is very bad idea.

Fourth, i used ElasticSearch for logging purposes. Because ElasticSearch is very fast and popular for logging. It stores JSON files and uses indexes and you can get desired results with document scoring mechanism, you can create dashboards on Kibana. This is cool!

Fifth, I used .Net Core 6. Because I wanted to learn something from .Net Technologies.

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
    - [Postman Collection](#postman-collection)
    - [RabbitMQ Installation](#rabbitmq-installation)
    - [Kong API Gateway Installation](#kong-api-gateway-installation)
    - [Install project with Docker Container](#install-project-with-docker-container)
  - [Structure](#structure)
  - [Contributing](#contributing)
  - [Bug Reports \& Feature Requests](#bug-reports--feature-requests)

## Features

- Developed via .Net Core 6
- Uses RabbitMQ for Asynchronous Communication
- Uses ElasticSearch for logging request, responses, error messages and info messages
- Uses Minio S3 Object Storage for store files
- Uses Kong API Gateway for security
- Easy to deploy, just use Dockerfile
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
Note: I used Elastic, Logstash and Kibana version 8.5.1 at the time of i wrote this readme

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
    #networks:
      #- overlay
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

#networks:
  #overlay:

```

### Postman Collection

Collection file is in Docs/Postman Collection. Import and test API!

### RabbitMQ Installation

Use below docker-compose file.

```bash

version: "3.9"

services:
  rabbitmq:
    image: rabbitmq:3-management
    volumes:
      - rabbitmq-volume:/var/lib/rabbitmq
    #networks:
      #- overlay
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

```

### Kong API Gateway Installation

I followed instructions on this github repo: <https://github.com/Kuari/kong-konga-docker-compose> . Check readme, instructions are clear and simple.

### Install project with Docker Container

Follow the instructions below.

```bash
git clone
```

## Structure

(Will be edited in the future. Delete dlls etc. only necessary files) (FILE STRUCTURE)

## Contributing

I am open every advice for my project. I am planning to improve myself on .NET Core 6. So don't hesitate comment on my project.

## Bug Reports & Feature Requests

Please use the Github issues.
