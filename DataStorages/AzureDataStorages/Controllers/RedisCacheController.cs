using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace AzureDataStorages.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RedisCacheController : ControllerBase
{
	private readonly DatabaseMock _databaseMock;
	private readonly IDatabase _redisDb;
	private const string CountriesKey = "countries";

	public RedisCacheController(IOptions<AzureConnectionStrings> connectionStrings)
	{
		_databaseMock = new DatabaseMock();
		var redis = ConnectionMultiplexer.Connect(connectionStrings.Value.RedisCahce);
		_redisDb = redis.GetDatabase();
	}

	[HttpGet]
	public async Task<IActionResult> Get()
	{
		if (_redisDb.KeyExists(CountriesKey))
		{
			RedisValue countriesValues = await _redisDb.StringGetAsync(CountriesKey);
			var countries = JsonSerializer.Deserialize<Dictionary<string, string>>(countriesValues);
			return Ok(countries);
		}
		var countriesFromDb = await _databaseMock.GetAllCountriesAsync();
		var json = JsonSerializer.Serialize(countriesFromDb);
		_redisDb.StringSet(CountriesKey, json);
		_redisDb.KeyExpire(CountriesKey, new TimeSpan(0, 10, 0));
		return Ok(countriesFromDb);
	}

	[HttpPost]
	public IActionResult Post([FromQuery] string code, [FromQuery] string name)
	{
		_databaseMock.AddCountry(code, name);
		_redisDb.KeyDelete(CountriesKey);
		return Accepted();
	}
}
