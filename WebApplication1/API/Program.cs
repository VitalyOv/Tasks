using System.Text.Json.Serialization;
using Domain.DatabaseContext;
using Domain.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
services.AddDbContext<DatabaseContext>(
    contextBuilder => contextBuilder.UseNpgsql(builder.Configuration.GetConnectionString("TasksDB")));

services.AddScoped<ITasksService, TasksService>();
services.AddHostedService<TaskWorkerService>();

var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors(configurePolicy =>
{
    configurePolicy
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
});

app.MapControllers();

app.Run();