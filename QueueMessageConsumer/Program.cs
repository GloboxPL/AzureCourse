using Azure.Storage.Queues;
using QueueMessageConsumer;

var config = Config.Bind();
var queueName = "returns";
var queueClient = new QueueClient(config.ConnectionString, queueName);

while (true)
{
	var message = queueClient.ReceiveMessage();
	if (message.Value != null)
	{
		var returnDto = message.Value.Body.ToObjectFromJson<ReturnDTO>();
		Process(returnDto);
		await queueClient.DeleteMessageAsync(message.Value.MessageId, message.Value.PopReceipt);
	}

	await Task.Delay(1000);
}

void Process(ReturnDTO returnDto)
{
	Console.WriteLine($"Id: {returnDto.Id}\n\tUser: {returnDto.User}\n\tAddress: {returnDto.Address}");
}