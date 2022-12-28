using SnowFlakeFactory.Service;
using SnowFlakeFactory.Model;
using Microsoft.Extensions.Options;
using SnowFlakeFactory.Interface;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<SnowFlakeModel>(builder.Configuration.GetRequiredSection("SnowflakeId"));
builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddSingleton<SnowFlakeModel>(x => x.GetRequiredService<IOptions<SnowFlakeModel>>().Value);
builder.Services.AddSingleton<SnowFlakeIdService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/snowflake-id", (SnowFlakeIdService snowflakeIdService) => 
    Results.Ok(new SnowflakeId(snowflakeIdService.CreateSnowflakeId())))
    .WithName("SnowflakeId")
    .Produces(StatusCodes.Status200OK, typeof(SnowflakeId));

app.Run();

internal record class SnowflakeId(ulong id);