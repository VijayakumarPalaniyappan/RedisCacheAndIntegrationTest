# RedisCache and Integration Test

Sample Application for Redis Cache including Integration Test

Caching mechanism to store frequently accessed data in a data store temporarily and retrieve this data for subsequent requests instead of extracting it from the original data source. The Azure Redis Cache is a high-performance caching service that provides in-memory data store for faster retrieval of data. It is based on the open-source implementation Redis cache. 

We used in our application (Core Auth Service), the API service tries to read data from the cache. If the requested data not available in the cache, the AUTH service gets the data from the actual data source (Baldo Service). Then the AUTH service stores that data in the cache for subsequent requests. When the next request comes to the AUTH service, it retrieves data from the cache without going to the actual data source (Baldo Service). This process improves the application performance because data lives in memory.

## Redis Configuration prerequisites before implementation
Redis configuration provides a native capability to use it in API services. We must use machine configuration to manage Redis, you must enable the prerequisites for the service. There are two possible aspects to configure Redis. One is local machine and another one is deployment servers(Dev, Staging, QA or PROD)

### Enable machine configuration (Local Machine)
Before doing the configuration, we must install Docker Desktop and make it accessible. How to download and install Docker Desktop?

After installing Docker Desktop, use docker compose file(available from AUTH service) to enable Redis server using docker commands.

#### Docker Compose
Find the docker compose file from AUTH Service REPO and use it. To run your redis docker image, execute the command:

````bash
docker compose -f docker-compose-redis.yml up -d
````

##### How to verify and test?
##### Code Configuration
##### IServiceCollection
##### IRedisCache & RedisCache
##### GetCacheData
##### SetCacheData
##### RemoveData
##### Integration Test
##### BaseIntegrationApiTest
##### IntegrationTestFactory
##### Important to note
### Enable deployment servers configuration (Dev, Staging, QA and PROD)
#### ARO - Red Hat OpenShift on Azure
##### Setup
###### Distributed cache - Azure Cache for Redis
As developer, before implement Redis cache make sure to understand how the Redis Cache is functioning through ARO.

Use the following referrence page to get to know how to create Azure cache for Redis
https://learn.microsoft.com/en-us/azure/redis/dotnet-core-quickstart?pivots=azure-cache-redis

##### Setting-Up Redis Cache - ARO (Azure Redhat OpenShift)
The following README.md file gives us to understand better about setup Redis at ARO step by step.

[[https://github.com/VijayakumarPalaniyappan/utility/tree/main/redis-client/README.md](https://github.com/VijayakumarPalaniyappan/utility/tree/main/redis-client/README.md)

We could not test Azure cache for Redis from our local development and only possible to verify and test it from ARO(Azure Redhat OpenShift). 

##### How to do the test from Local Development
Once push the code into REPO, deploy it into staging environment. Once the build succeeded, connect ARO through PowerShell using ARO token.
#### Create and Enable Cluster
#### Deployment
#### How to verify and test?
##### How to Get the ARO Token
Login into 
(https://oauth-openshift.apps.aro-delta.euw-hub02.azure.xxxx.net/oauth/authorize?client_id=console&redirect_uri=https%3A%2F%2Fconsole-openshift-console.apps.aro-delta.euw-hub02.azure.xxxxx.net%2Fauth%2Fcallback&response_type=code&scope=user)

1. Click user and select "Copy Login command".
2. Copy the token and run it from PowerShell. 
3. Once the login succeeded, ensure the deployed staging pod and service is running in ARO.
   
````bash
oc get pods

oc get services
````

Map the auth service port to **8080**
````bash
 oc port-forward service/core-authservice-staging 8080:8080
````
Once the port is mapped, send the request and ensure whether the cache is working fine or not.
http://localhost:8080/XXXX/authorizationsXXXXXXX
