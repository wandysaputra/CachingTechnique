# CachingTechnique

## [InMemoryCaching](https://github.com/wandysaputra/CachingTechnique/blob/master/Domain.Data/InMemoryRepository.cs)
Example implementation => Domain.Data/InMemoryRepository.cs


## [DistributeCaching](https://github.com/wandysaputra/CachingTechnique/blob/master/Domain.Data/DistributeCacheRepository.cs)
Example implementation => Domain.Data/DistributeCacheRepository.cs


## Response Caching
Response Caching Restrictions
- Must be GET or HEAD request
- Cannot have an Authorization header
- Not for server-side UI apps (Razor Pages, MVC)
- Can for anonymous API call or static HTTP assets

Example implementation => `[ResponseCache(Duration = 90, VaryByQueryKeys = new [] { "category" })]`
![image](https://user-images.githubusercontent.com/42372928/223712179-443983cf-1ac8-45e1-b8a9-dbca0220e056.png)


## Output Caching

Output Caching Restrictions
- Must be GET or HEAD requests
- Authenticated requests are not cached
- Uses MemoryCache by default (due to ETags that requires atomic operations within a cache)
	- Don't use IDistributedCache
	- Can create custom IOutputCacheStore
	
