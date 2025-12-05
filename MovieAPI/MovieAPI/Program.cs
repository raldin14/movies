using System.IdentityModel.Tokens.Jwt;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MovieAPI.Repository;
using Microsoft.OpenApi.Models;
using MovieAPI;
using MovieAPI.ApiBehavior;
using MovieAPI.Filters;
using MovieAPI.Utils;
using NetTopologySuite;
using NetTopologySuite.Geometries;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddSingleton<GeometryFactory>(NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326));

builder.Services.AddSingleton(provider =>
    new MapperConfiguration(config =>
    {
        var geometryFactory = provider.GetRequiredService<GeometryFactory>();
        config.AddProfile(new AutoMapperProfile(geometryFactory));
    }).CreateMapper());

builder.Services.AddTransient<IStorageAzureStorage, StorageAzureStorage>();
builder.Services.AddDbContext<ApplicationDBContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServer => sqlServer.UseNetTopologySuite()));

builder.Services.AddOpenApi();
builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(ParseBadRequest));
}).ConfigureApiBehaviorOptions(BehaviorBadRequest.Parse);

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1",
        Description = "Example API using ASP.NET Core 8 Minimal APIs"
    });
});
builder.Services.AddResponseCaching();
builder.Services.AddCors(options =>
{
    var frontend = builder.Configuration.GetValue<string>("frontend_url");
    options.AddDefaultPolicy(build =>
    {
        build.WithOrigins(frontend).AllowAnyMethod().AllowAnyHeader()
            .WithExposedHeaders(new string[]{"totalRecord"});
    });
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDBContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtKey"])),
            ClockSkew = TimeSpan.Zero
        });

builder.Services.AddAuthorization(options => 
    options.AddPolicy("isAdmin", policy => policy.RequireClaim("role", "admin"))
    );
builder.Services.AddTransient<IInMemoryRepository,InMemoryRepository>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
/*app.UseEndpoints(endPoints =>
{
    endPoints.MapControllers();
});*/
app.MapControllers();

app.Run();