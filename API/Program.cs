using System.Text.Json.Serialization;
using Domain.DatabaseContext;
using Domain.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
services.AddDbContextPool<DatabaseContext>(
    contextBuilder => contextBuilder.UseNpgsql(builder.Configuration.GetConnectionString("TasksDB")));

services.AddScoped<ITasksService, TasksService>();
services.AddHostedService<TaskWorkerService>();

var app = builder.Build();

app.UseRouting();
app.UseHttpsRedirection();
app.UseCors(configurePolicy =>
{
    configurePolicy
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
});

app.MapControllers();

app.Run();