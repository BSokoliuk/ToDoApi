using Application.DTOs;
using NBomber.CSharp;
using NBomber.Http.CSharp;
using System.Text.Json;

namespace API.Tests.Performance;

public class TodoFlowPerformanceTest
{
    public static void Main(string[] args)
    {
        Http.GlobalJsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        using var httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };

        // Example of a simple scenario with 4 steps
        // 1. POST a new TodoItem and capture the ID
        // 2. GET the TodoItem by ID
        // 3. PUT (update) the TodoItem
        // 4. DELETE the TodoItem (Clean up)
        // Note: Ensure that the API is running and accessible at the specified base address
        var scenario = Scenario.Create("TodoItem_Flow", async context =>
        {
            // Step 1: POST and capture new ID
            var postStep = await Step.Run<int>("Post_TodoItem", context, async () =>
            {
                var dto = new TodoItemDto
                {
                    Title = "Performance Test Item",
                    Description = "Created during performance test",
                    ExpiryDateTime = DateTime.UtcNow.AddDays(1)
                };

                var request = Http.CreateRequest("POST", "/api/TodoItem")
                                  .WithJsonBody(dto);

                var response = await Http.Send<TodoItemDto>(httpClient, request);
                if (!response.IsError)
                {
                    var created = response.Payload.Value.Data;
                    return Response.Ok(payload: created.Id);
                }

                return Response.Fail<int>("POST failed");
            });

            var todoId = postStep.Payload.Value;

            // Step 2: GET by ID
            await Step.Run("Get_TodoItem", context, async () =>
            {
                var req = Http.CreateRequest("GET", $"/api/TodoItem/{todoId}")
                              .WithHeader("Accept", "application/json");
                var res = await Http.Send(httpClient, req);
                return res.IsError ? Response.Fail() : Response.Ok();
            });

            // Step 3: PUT update
            await Step.Run("Put_TodoItem", context, async () =>
            {
                var updateDto = new TodoItemDto
                {
                    Id = todoId,
                    Title = "Updated Performance Test Item",
                    Description = "Updated during performance test",
                    ExpiryDateTime = DateTime.UtcNow.AddDays(2)
                };

                var req = Http.CreateRequest("PUT", $"/api/TodoItem/{todoId}")
                              .WithJsonBody(updateDto);

                var res = await Http.Send(httpClient, req);
                return res.IsError ? Response.Fail() : Response.Ok();
            });

            // Step 4: DELETE
            await Step.Run("Delete_TodoItem", context, async () =>
            {
                var req = Http.CreateRequest("DELETE", $"/api/TodoItem/{todoId}")
                              .WithHeader("Accept", "application/json");
                var res = await Http.Send(httpClient, req);
                return res.IsError ? Response.Fail() : Response.Ok();
            });

            return Response.Ok();
        })
        .WithLoadSimulations(
            Simulation.Inject(rate: 5, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(30))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .WithReportFileName("TodoItemFlowReport")
            .Run();
    }
}
