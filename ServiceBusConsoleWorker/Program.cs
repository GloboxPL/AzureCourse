// See https://aka.ms/new-console-template for more information
using Azure.Messaging.ServiceBus;
using System.Text.Json;

Console.WriteLine("Service Bus Consumer");

var config = Config.Bind();
var client = new ServiceBusClient(config.ConnectionString);
var processor = client.CreateProcessor("user-registered", "subscription2");
processor.ProcessMessageAsync += MessageHandler;
processor.ProcessErrorAsync += ErrorHandler;

Console.WriteLine("Start processing");
await processor.StartProcessingAsync();
Console.ReadKey();

Task ErrorHandler(ProcessErrorEventArgs arg)
{
	Console.WriteLine(arg.ErrorSource);
	Console.WriteLine(arg.Exception.ToString());
	return Task.CompletedTask;
}

Task MessageHandler(ProcessMessageEventArgs arg)
{
	var body = arg.Message.Body.ToString();
	Console.WriteLine(body);
	return Task.CompletedTask;
}

class Config
{
	public string? ConnectionString { get; set; }

	public static Config Bind()
	{
		var json = File.ReadAllText("config.json");
		return JsonSerializer.Deserialize<Config>(json) ?? throw new Exception("Cannot bind configuration");
	}
}