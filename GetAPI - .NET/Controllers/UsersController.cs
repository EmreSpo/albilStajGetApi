using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace YourNamespace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;
        private readonly string _cacheKey = "usersCache";
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

        public UsersController(HttpClient httpClient, IMemoryCache memoryCache)
        {
            _httpClient = httpClient;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> Get()
        {
            // Cache'de veri var mı kontrol et
            if (!_memoryCache.TryGetValue(_cacheKey, out List<User>? users))
            {
                // Cache'de veri yoksa API'den veriyi çek
                var response = await _httpClient.GetAsync("https://jsonplaceholder.typicode.com/users");

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "API request failed.");
                }

                var json = await response.Content.ReadAsStringAsync();
                users = JsonSerializer.Deserialize<List<User>>(json);

                if (users == null || users.Count == 0)
                {
                    return NotFound("No users found.");
                }

                // Veriyi cache'e ekle
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _cacheExpiration,
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                };
                _memoryCache.Set(_cacheKey, users, cacheEntryOptions);

                // Cache'e eklendiğine dair log yaz
                Console.WriteLine("Cache miss - Veri API'den alındı ve cache'e eklendi.");
            }
            else
            {
                // Cache'den alındığına dair log yaz
                Console.WriteLine("Cache hit - Veri cache'den alındı.");
            }

            return Ok(users);
        }
    }
}
