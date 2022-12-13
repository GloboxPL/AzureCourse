using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;

namespace WebApiFunction;

internal class ToDoFunctions
{
	private readonly ILogger<ToDoFunctions> _logger;
	private readonly ToDoStore _toDoStore;

	public ToDoFunctions(ILoggerFactory loggerFactory, ToDoStore toDoStore)
	{
		_logger = loggerFactory.CreateLogger<ToDoFunctions>();
		this._toDoStore = toDoStore;
	}

	[Function("GetAllToDos")]
	[OpenApiOperation(operationId: "GetAllToDos", Summary = "Get all toDos")]
	[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<ToDo>))]
	public async Task<HttpResponseData> GetAllToDos([HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "todo/all")] HttpRequestData request)
	{
		_logger.LogInformation("Getting toDos");
		var toDos = _toDoStore.GetToDos();

		var response = request.CreateResponse(HttpStatusCode.OK);
		await response.WriteAsJsonAsync(toDos);

		return response;
	}

	[Function("CreateToDo")]
	[OpenApiOperation(operationId: "CreateToDo", Summary = "Create new toDo")]
	[OpenApiRequestBody(contentType: "application/json", bodyType: typeof(ToDoDto), Required = true, Description = "Description of a new toDo")]
	[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.Created)]
	public async Task<HttpResponseData> CreateToDo([HttpTrigger(AuthorizationLevel.Anonymous, "POST", Route = "todos")] HttpRequestData request)
	{
		_logger.LogInformation("Creating toDo");
		var toDos = _toDoStore.GetToDos();

		var dto = await request.ReadFromJsonAsync<ToDoDto>();
		_toDoStore.AddToDo(dto.Description);

		var response = request.CreateResponse(HttpStatusCode.Created);

		return response;
	}

	[Function("MarkToDoAsComplited")]
	[OpenApiOperation(operationId: "MarkToDoAsComplited", Summary = "Mark existing toDo as finished")]
	[OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Summary = "ToDo id")]
	[OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NoContent)]
	public Task<HttpResponseData> MarkToDoAsComplited([HttpTrigger(AuthorizationLevel.Anonymous, "PATCH", Route = "todos/{id}")] HttpRequestData request, Guid id)
	{
		_logger.LogInformation("Making toDo as complited");
		_toDoStore.MarkComplited(id);

		var response = request.CreateResponse(HttpStatusCode.NoContent);

		return Task.FromResult(response);
	}

	public record ToDoDto(string Description);
}
