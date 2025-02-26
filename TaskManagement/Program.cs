using AspNet.Security.OAuth.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TaskManagement.DataAccess;
using TaskManagement.Interfaces;
using TaskManagement.Middlewares;
using TaskManagement.Models;
using TaskManagement.Repositories;
using TaskManagement.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TaskManagementContext>(options =>
options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection"),
    b => b.MigrationsAssembly(typeof(TaskManagementContext).Assembly.FullName)));

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IGenericRepository<User>, UserRepository>();
builder.Services.AddScoped<IGenericRepository<Category>, CategoryRepository>();
builder.Services.AddScoped<IGenericRepository<TaskManagement.Models.Task>, TaskRepository>();
builder.Services.AddScoped<IGenericRepository<TaskComment>, TaskCommentRepository>();
builder.Services.AddScoped<IGenericRepository<Label>, LabelRepository>();
builder.Services.AddScoped<IGenericRepository<TaskLabel>, TaskLabelRepository>();

builder.Services.AddAuthentication(cfg =>
{
    cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    cfg.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8
            .GetBytes(builder.Configuration["ApplicationSettings:JWT_Secret"])
        ),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };

    x.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("token"))
            {
                context.Token = context.Request.Cookies["token"];
            }

            return System.Threading.Tasks.Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseRequestLoggingMiddleware();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

