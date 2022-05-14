using KDRC_Core.Configurations;
using KDRC_Core.Models;
using KDRC_Core.Models.Data;
using KDRC_Core.Repositories;
using KDRC_Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Scoped Service
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<AuthService>();

// Add Repository Singleton Service
var mongoConfiguration = builder.Configuration.GetSection("MongoSection").Get<MongoConfiguration>();
builder.Services.AddSingleton(mongoConfiguration);
builder.Services.AddSingleton<MongoContext>();
builder.Services.AddSingleton<ICommonMongoRepository<Account>, AccountRepository>();
builder.Services.AddSingleton<ICommonMongoRepository<AccessToken>, AccessTokenRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseStaticFiles(new StaticFileOptions {ServeUnknownFileTypes = true});
    app.UseSwagger();
    app.UseSwaggerUI(a => a.SwaggerEndpoint("/core.yaml", "KangDroid-Cloud Core Definition"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();