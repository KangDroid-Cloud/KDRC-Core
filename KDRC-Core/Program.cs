using System.Reflection;
using KDRC_Core.Configurations;
using KDRC_Core.Models;
using KDRC_Core.Models.Data;
using KDRC_Core.Repositories;
using KDRC_Core.Services;
using KDRC_Models.EventMessages.Account;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Add Scoped Service
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<AuthService>();

// Add Repository Singleton Service
var mongoConfiguration = builder.Configuration.GetSection("MongoSection").Get<MongoConfiguration>();
builder.Services.AddSingleton(mongoConfiguration);
builder.Services.AddSingleton<MongoContext>();
builder.Services.AddSingleton<ICommonMongoRepository<Account>, AccountRepository>();
builder.Services.AddSingleton<ICommonMongoRepository<AccessToken>, AccessTokenRepository>();
builder.Services.AddScoped<IEventService, EventService>();

builder.Services.AddMassTransit(a =>
{
    a.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"], builder.Configuration["RabbitMq:VirtualHost"], h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.Message<AccountCreatedMessage>(x => x.SetEntityName("account.created"));
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseStaticFiles(new StaticFileOptions {ServeUnknownFileTypes = true});
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();