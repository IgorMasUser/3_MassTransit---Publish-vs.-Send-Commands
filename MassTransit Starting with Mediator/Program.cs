using MassTransit;
using Sample.Components.Consumers;
using Sample.Contracts;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Text;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.SetInMemorySagaRepositoryProvider();
    x.AddConsumer<SubmitOrderConsumer>();
    //x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq());
    ////////
    x.AddRequestClient<SubmitOrder>(new Uri($"exchange:{KebabCaseEndpointNameFormatter.Instance.Consumer<SubmitOrderConsumer>()}"));
    ///////
    x.UsingRabbitMq((cxt, cfg) =>

    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");

        });

        cfg.ConfigureEndpoints(cxt);

    });

});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApiDocument(cfg => cfg.PostProcess = d => d.Info.Title = "Sample API Site");


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    app.UseSwaggerUi3();
    app.UseOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseRouting();

app.Run();
