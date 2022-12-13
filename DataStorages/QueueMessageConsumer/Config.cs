using System.Text.Json;

namespace QueueMessageConsumer;

public class Config
{
	public string? ConnectionString { get; set; }

	public static Config Bind()
	{
		var json = File.ReadAllText("config.json");
		return JsonSerializer.Deserialize<Config>(json) ?? throw new Exception("Cannot bind configuration");
	}
}
