<div style="text-align: center">

<img src="https://cdn.dribbble.com/users/42044/screenshots/3005802/media/e9d2cfc8f3ccdedebef7a8af171fbd08.jpg" width=15% height=15%>

</div>

# Microservice Project with .NET Core 6

<!-- [![Elastic Stack version](https://img.shields.io/badge/Elastic%20Stack-8.3.2-00bfb3?style=flat&logo=elastic-stack)](https://www.elastic.co/blog/category/releases)
[![Build Status](https://github.com/deviantony/docker-elk/workflows/CI/badge.svg?branch=main)](https://github.com/deviantony/docker-elk/actions?query=workflow%3ACI+branch%3Amain)
[![Join the chat at https://gitter.im/deviantony/docker-elk](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/deviantony/docker-elk?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) -->

## Philosophy

I was curious about Microservice Architectures and Technologies like Docker, Kubernetes, RabbitMQ and Asynchronous Programming. So I developed simple microservice via .Net Core 6.

## Contents

- [Microservice Project with .NET Core 6](#microservice-project-with-net-core-6)
  - [Philosophy](#philosophy)
  - [Contents](#contents)
  - [Features](#features)
  - [Requirements](#requirements)
    - [Docker installation](#docker-installation)
    - [ElasticSearch Installation](#elasticsearch-installation)
      - [Creation of Index on ElasticSearch-Kibana](#creation-of-index-on-elasticsearch-kibana)
    - [Kibana Installation](#kibana-installation)
    - [Minio Installation](#minio-installation)
    - [Postman Collection](#postman-collection)
    - [RabbitMQ Installation](#rabbitmq-installation)
    - [Kong API Gateway Installation](#kong-api-gateway-installation)
    - [Install project with Docker Container ???](#install-project-with-docker-container-)
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
- Can run on any platform (Mac, Linux ve Windows)

## Requirements

> **Note** <br />
> Currently I use **Docker version 20.10.21** and **Docker Compose version v2.6.0** <br />
> Currently I use **Docker Compose version v2.12.2** <br />
> Currently I use **Linux Ubuntu 18.06 LTS** machine on Google Cloud. <br />

### Docker installation

Docker Engine and Docker Compose must be installed. Check out on Docker's offical site.

### ElasticSearch Installation

I followed instructions on this github repo: https://github.com/deviantony/docker-elk . Check readme, instructions are clear and simple.
Note: I used Elastic, Logstash and Kibana version 8.5.1 at the time of i wrote this readme

#### Creation of Index on ElasticSearch-Kibana

1.  Open Dev Console on Kibana.

2.  Run commands below on Kibana Dev Console to create indexes.

    ```bash
    $ PUT /loggerservice_errorlogs
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
    $ PUT /loggerservice_otherlogs
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

    $ PUT /webservice_objstorage_logs
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
    }

    ````

    ```bash
    $ PUT /webservice_queue_logs
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
    ````

        ```bash

    $ PUT /webservice_requestresponse_logs
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

    ```

3.  Go Index Management on Kibana. Make sure that indexes created successfully.

### Kibana Installation

Follow the instructions below.

1. After installation of Logstash, ElasticSearch and Kibana from https://github.com/deviantony/docker-elk repository, Kibana will occur errors. To fix that, stop and remove kibana docker container.

   ```bash
   $ docker stop kibana_container_id
   $ docker container rm  kibana_container_id
   ```

2. Run commands below.

   ```bash
   $ docker-compose up -d #Run this command at the location of Kibana Folder (location of docker-compose.yml). Kibana Folder is in Docs folder. (Docs/Kibana/docker-compose.yml)
   ```

3. After installation of Kibana, you need to reset ceredentials of Kibana user in ElasticSearch. (Source: https://github.com/deviantony/docker-elk). This instruction have been written in README.

   ```bash
   $ docker-compose exec elasticsearch bin/elasticsearch-reset-password --batch --user kibana_system #Run this command and get credentials for kibana
   ```

### Minio Installation

```bash
$
$
$
$
$
```

### Postman Collection

Collection file is in Docs/Postman Collection. Import and test API!

### RabbitMQ Installation

Rabbitmq

### Kong API Gateway Installation

Kong API Gateway

### Install project with Docker Container ???

Follow the instructions below.

```bash
$ git clone
```

## Structure

(Will be edited in the future. Delete dlls etc. only necessary files) (FILE STRUCTURE)

## Contributing

I am open every advice for my project. I am planning to improve myself on .NET Core 6. So don't hesitate comment on my project.

## Bug Reports & Feature Requests

Please use the Github issues.

```

```
