# RedisCache and Integration Test

Sample Application for Redis Cache including Integration Test

Caching mechanism to store frequently accessed data in a data store temporarily and retrieve this data for subsequent requests instead of extracting it from the original data source. The Azure Redis Cache is a high-performance caching service that provides in-memory data store for faster retrieval of data. It is based on the open-source implementation Redis cache. 

We used in our application (Core Auth Service), the API service tries to read data from the cache. If the requested data not available in the cache, the AUTH service gets the data from the actual data source (Baldo Service). Then the AUTH service stores that data in the cache for subsequent requests. When the next request comes to the AUTH service, it retrieves data from the cache without going to the actual data source (Baldo Service). This process improves the application performance because data lives in memory.

We have one Azure Cache for Redis per environment: https://portal.azure.com/#view/HubsExtension/BrowseResource.ReactView/resourceType/Microsoft.Cache%2FRedis

**Keep in mind that the Azure Cache for Redis is using Redis version 6.0**

## Redis Configuration prerequisites before implementation
Redis configuration provides a native capability to use it in API services. We must use machine configuration to manage Redis, you must enable the prerequisites for the service. There are two possible aspects to configure Redis. One is local machine and another one is deployment servers(Dev, Staging, QA or PROD)

### Enable machine configuration (Local Machine)
Before doing the configuration, we must install Docker Desktop and make it accessible. How to download and install Docker Desktop?

After installing Docker Desktop, use docker compose file(available from AUTH service) to enable Redis server using docker commands.

#### Docker
Follow the official documentation: https://redis.io/docs/latest/operate/oss_and_stack/install/install-stack/docker/

If adding Redis to a service, add it to the docker-compose.yml file in the repository. Example docker-compose file:

````yml
version: '3.8'
services:
  redis:
    image: redis:6.0
    container_name: redis-local
    ports:
      - "6379:6379"
    command: ["redis-server", "--appendonly", "yes"]
    volumes:
      - redis-data:/data

volumes:
  redis-data:
````

````bash
docker compose -f docker-compose-redis.yml up -d
````

##### How to verify and test?
Once push the code into REPO, deploy it into staging environment. Once the build succeeded, connect ARO through PowerShell using ARO token.
How to Get the ARO Token
Login into (https://oauth-openshift.apps.aro-delta.euw-hub02.azure.volvo.net/oauth/authorize?client_id=console&redirect_uri=https%3A%2F%2Fconsole-openshift-console.apps.aro-delta.euw-hub02.azure.volvo.net%2Fauth%2Fcallback&response_type=code&scope=user)

Click user and select "Copy Login command".
Copy the token and run it from PowerShell.
Once the login succeeded, ensure the deployed staging pod and service is running in ARO.

````bash
oc get pods

oc get services

redis-cli -h oneview-114458-redis-dev.redis.cache.windows.net -p 6379 ping
````
NOTE: Port 6380 is designated for SSL connections in Azure Cache for Redis. However, testing via CLI reveals that port 6379 is actually handling the connection role.


Map the auth service port to 8080

````bash
oc port-forward service/core-authservice-staging 8080:8080
````
Once the port is mapped, send the request and ensure whether the cache is working fine or not. http://localhost:8080/user/authorizations
##### Code Configuration
- Add a reference to the Microsoft.Azure.StackExchangeRedis NuGet package in your Redis client project.
- In your Redis connection code, first create a ConfigurationOptions instance. You can use the .Parse() method to create an instance from a Redis connection string or the cache host name alone
  
````csharp
var configurationOptions = ConfigurationOptions.Parse($"{cacheHostName}:6380");
````
- Use one of the ConfigureForAzure* extension methods supplied by this package to configure the authentication options
  
````csharp
// Service principal secret
    configurationOptions.ConfigureForAzureWithServicePrincipalAsync(clientId, tenantId, secret).Result;
````
- Create the connection, passing in the ConfigurationOptions instance
  
````csharp
var connectionMultiplexer = ConnectionMultiplexer.Connect(configurationOptions);
````
- Use the connectionMultiplexer to interact with Redis as you normally would.
##### IServiceCollection
The extension method of IServiceCollection helps to Redis for creating Connection multiplexer. Redis is the ConnectionMultiplexer class in the StackExchange. The object that hides away the details of multiple servers. Because the ConnectionMultiplexer does a lot, it is designed to be shared and reused between callers. You should not create a ConnectionMultiplexer per operation.

So that, it was implemented on extension method of IServiceCollection. ConnectionMultiplexer used to connect Redis using Redis host name along with respective of valid configurations.
Based on Basic, Standard, and Premium tiers, TLS access is enabled by default and non-TLS access is disabled in new caches. Non-clustered caches use port 6380 for TLS access or port 6379 for non-TLS access. Therefore, SSL should be enabled.

configurationOptions.Ssl = true;
Redis cache holds database, which is used to store data into Redis cache and get data from cache using cache key.
##### Integration Test
For integration test, some basic steps needs to be set that helps to avoid issues on continuous test case executions. Every execution, the Redis cache holds data into Redis database that impact on each test execution. Every test case execution, the Redis database should be cleared. Otherwise, the Redis Database holds the cache details and carry forward same details into next test case execution. 

IntegrationTestFactory
Purpose:
The IntegrationTestFactory class initializes Redis once at the start of the test suite.
It sets the initial configuration for Redis, which remains consistent across all test cases.
This approach ensures:

Performance efficiency (no repeated flush/reconfigure per test).
Stable environment for integration tests.
Behavior:
- Redis is configured during the first execution of the test suite.
- The same configuration persists for all subsequent test cases.
- No flush or reinitialization occurs between tests unless explicitly triggered.

BaseIntegrationApiTest
**Per‑test Redis reset and configuration**

For every test case, the test runner ensures a clean Redis environment:
- Flush the database to remove keys from the previous test run.
- Apply test‑specific configuration so each test executes against a newly configured Redis instance.
- (Optional) Re-create required fixtures/seed data before assertions.
Example policy: With 10 test cases, Redis is flushed and reconfigured 10 times—once before each test executes. This guarantees isolation and prevents state leakage across tests.
##### Important to note
The following code helps to define initial configuration

````csharp
public class IntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
  private RedisContainer redisContainer = new RedisBuilder().WithImage("redis:latest").WithPortBinding(6379, true).Build(); // Create new container
  ...
  ...
  protected override void ConfigureWebHost(IWebHostBuilder builder)
  {
    ...
    ...
    builder.ConfigureTestServices(service =>
	{
  		service.RemoveAll<IConnectionMultiplexer>();
  		service.AddSingleton<IConnectionMultiplexer>(sp =>
  		{
   		 var options = ConfigurationOptions.Parse(redisContainer.GetConnectionString());
    	// Before flushing all caching server, it should have admin access. Stated clearly: BaseIntegrationApiTest class
    	options.AllowAdmin = true;

    	return ConnectionMultiplexer.Connect(options);
  		}); 
      });
    }
}
````
IAsyncLifetime - Used to implements both InitializeAsync and IAsyncLifetime.DisposeAsync for generating Redis cache DB and disposing it.

````csharp
  public BaseIntegrationApiTest(IntegrationTestFactory factory)
    {
		...
		...	    
      // Should be flushed all database from ConnectionMultiplexer. Otherwise, the key will be reatined at database
      // It's very safe to flush the database to pass the subsequent integration test cases
      foreach (var server in connectionMultiplexer?.GetServers())
      {
        server?.FlushDatabase();
      }
		...
		...
	}
````
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
