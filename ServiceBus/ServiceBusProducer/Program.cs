using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();


var connectionString = builder.Configuration.GetValue<string>("ServiceBusConnectionString");

app.MapPost("/register", async (User user) =>
{
	var client = new ServiceBusClient(connectionString);
	var sender = client.CreateSender("user-registered");
	var message = new ServiceBusMessage(JsonSerializer.Serialize(user));
	await sender.SendMessageAsync(message);
}).WithOpenApi();

app.Run();

internal record User(string FirstName, string LastName, string Email);
