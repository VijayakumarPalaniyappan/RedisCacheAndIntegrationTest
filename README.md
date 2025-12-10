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
#### Create and Enable Cluster
#### Deployment
#### How to verify and test?
