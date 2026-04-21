using DeliverySystem.Api.Extensions;
using DeliverySystem.Api.Middleware;
using DeliverySystem.Data.Context;
using DeliverySystem.Data.Helpers;
using DeliverySystem.Data.Infrastructure;
using DeliverySystem.Data.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DeliverySystemContext>
    (options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//This code will setup the database context
string connString = builder.Configuration["ConnectionStrings:DefaultConnection"];
DeliverySystemContext.ConnectionString = connString;
DeliverySystemContext.SecretKey = builder.Configuration["JwtConfig:SecretKey"];
DeliverySystemContext.ValidIssuer = builder.Configuration["JwtConfig:ValidIssuer"];
DeliverySystemContext.ValidAudience = builder.Configuration["JwtConfig:ValidAudience"];
DeliverySystemContext.TokenExpireMinute = builder.Configuration["JwtConfig:TokenExpireMinute"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JwtConfig:ValidAudience"],
        ValidIssuer = builder.Configuration["JwtConfig:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:SecretKey"]))
    };
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
    });

    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            // KEY: The reference to your scheme
            new OpenApiSecuritySchemeReference("Bearer", document), 
        
            // VALUE: An empty list of strings (for Bearer) or required scopes (for OAuth2)
            new List<string>()
        }
    });

});

// Add services to the container.
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IDbFactory, DbFactory>();
builder.Services.AddTransient<IErrorLogsService, ErrorLogsService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IUserAddressService, UserAddressService>();
builder.Services.AddTransient<IProductCategoryService, ProductCategoryService>();
builder.Services.AddTransient<IFileService, FileService>();
builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddTransient<ICartService, CartService>();
builder.Services.AddTransient<ICartItemService, CartItemService>();
builder.Services.AddTransient<IOrderService, OrderService>();
builder.Services.AddTransient<IOrderItemService, OrderItemService>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

var app = builder.Build();

await app.SeedDatabaseAsync();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
