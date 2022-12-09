using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ServiceBusFunction;

internal record User(string FirstName, string LastName, string Email);

public class Function1
{
	private readonly ILogger<Function1> _logger;

	public Function1(ILogger<Function1> log)
	{
		_logger = log;
	}

	[FunctionName("Function1")]
	public void Run([ServiceBusTrigger("user-registered", "subscription1", Connection = "ServiceBusConnection")] string mySbMsg)
	{
		var user = JsonSerializer.Deserialize<User>(mySbMsg);
		_logger.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg}");
	}
}
