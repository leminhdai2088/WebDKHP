using dev_DKHP.CoreModule.Dto;
using dev_DKHP.CoreModule.Dto.Common;
using dev_DKHP.CoreModule.Helper.Authorization;
using dev_DKHP.CoreModule.Helper.EmailSender;
using dev_DKHP.CoreModule.Helper.Procedure;
using dev_DKHP.CoreModule.Model;
using dev_DKHP.Impls;
using dev_DKHP.Intfs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
string? connectionString = configuration.GetSection("ConnectionStrings:Default").Value;
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSignalR();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
           builder =>
           {
               builder.WithOrigins("http://localhost:4200")
                      .AllowAnyHeader()
                      .AllowAnyMethod();
           });
});



builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API for WEB DKHP", Version = "v1" });

    // Configure JWT authentication for Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. 
                        Enter 'Bearer' [space] and then your token in the text input below.
                        Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

builder.Services.AddDbContext<DKHPDbContext>(
    options => options.UseSqlServer(connectionString));

builder.Services.AddIdentity<TL_USER_ENTITY, IdentityRole<string>>()
    .AddEntityFrameworkStores<DKHPDbContext>()
    .AddDefaultTokenProviders();

// START REGISTER TokenAuthConfiguration 
builder.Services.Configure<TokenAuthConfiguration>(builder.Configuration.GetSection("JwtBearer"));
builder.Services.AddSingleton(resolver =>
        resolver.GetRequiredService<IOptions<TokenAuthConfiguration>>().Value);
// END REGISTER TokenAuthConfiguration 

// START REGISTER EmailConfiguration 
builder.Services.Configure<EmailConfiguration>(builder.Configuration.GetSection("EmailConfiguration"));
builder.Services.AddSingleton(resolver =>
        resolver.GetRequiredService<IOptions<EmailConfiguration>>().Value);
// END REGISTER EmailConfiguration 

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var tokenConfig = builder.Configuration.GetSection("JwtBearer").Get<TokenAuthConfiguration>();
    var secretKeyBytes = Encoding.UTF8.GetBytes(tokenConfig.SecurityKey);
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = tokenConfig.Issuer,
        ValidAudience = tokenConfig.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes)
    };
});

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
// START ADD SCOPE
builder.Services.AddScoped<IStudentAppService, StudentAppService>();
builder.Services.AddScoped<IAuthenticationAppService, AuthenticationAppService>();
builder.Services.AddScoped<IEmailAppService, EmailAppService>();
builder.Services.AddScoped<IBaseAppService, BaseAppService>();
builder.Services.AddScoped<IMakerAppService, MakerAppService>();
builder.Services.AddScoped<ICheckerAppService, CheckerAppService>();
builder.Services.AddScoped<IClassSubjectAppService, ClassSubjectAppService>();
builder.Services.AddScoped<IStoredProcedureProvider, StoredProcedureProvider>();

// END ADD SCOPE


var app = builder.Build();
// app.MapIdentityApi<IdentityUser>();

app.UseCors("AllowSpecificOrigin");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseSession();

app.UseExceptionHandler(c =>
{
    c.Run(async context =>
    {
        context.Response.ContentType = "application/json";
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        if (exception is CustomException customException)
        {
            context.Response.StatusCode = 400; 
            var errorResponse = new CustomedExceptionDto
            {
                STATUS_CODE = customException.STATUS_CODE,
                ERROR_MESSAGE = customException.ERROR_MESSAGE,
                DATA = customException.DATA
            };
            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    });
});

app.MapControllers();

app.Run();
