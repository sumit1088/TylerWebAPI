using efilling_api.Models;
using efilling_api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// # dbcontext injection
var connectionString = builder.Configuration.GetConnectionString("EfilingDB");
builder.Services.AddDbContextPool<EFilling_DBContext>(
    //option => option.UseSqlServer(connectionString));
    options => options.UseNpgsql(connectionString));

builder.Services.AddSingleton<JwtService>(new JwtService("b9a5643b1f6274fe7efe42edad3b0704966c5b7799420a72261303853931df34260aa3c919f3174b2b003e53c1a31668275475861fa2a641b86fab9e015bfa60bc3b5c9d86e730ccd300d53825311345ae4dc86900a0ca469499b70bc2bac4e915d531deb645200a8b7be397bff8091b65704eb81935eceff7e54b713736cb811181293792f3f08892497b0f8a64dd224fe6073d6b2ef18720ce264e986125d4472e068dfe4e41ee8ab43dc25242cb19c3fe66dfbf92852a5d59f73b0f61bb3a3b622b32d83e6a74b7bb410989ef398a030946731b5a5060237112c756938b06182a77d1837dd83355e89d4f771f933441b8df39e2b05dc46a2fc301e509097e"));
builder.Services.AddTransient<UserService>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"], //"yourIssuer",
            ValidAudience = builder.Configuration["Jwt:Audience"], //"yourAudience",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("b9a5643b1f6274fe7efe42edad3b0704966c5b7799420a72261303853931df34260aa3c919f3174b2b003e53c1a31668275475861fa2a641b86fab9e015bfa60bc3b5c9d86e730ccd300d53825311345ae4dc86900a0ca469499b70bc2bac4e915d531deb645200a8b7be397bff8091b65704eb81935eceff7e54b713736cb811181293792f3f08892497b0f8a64dd224fe6073d6b2ef18720ce264e986125d4472e068dfe4e41ee8ab43dc25242cb19c3fe66dfbf92852a5d59f73b0f61bb3a3b622b32d83e6a74b7bb410989ef398a030946731b5a5060237112c756938b06182a77d1837dd83355e89d4f771f933441b8df39e2b05dc46a2fc301e509097e"))
        };
    });

// Add services to the container.
// # add jsonpatch for using PATCH method
// # SerializerSettings.ReferenceLoopHandling:  Make it a default setting in ASP.NET Core serialization when you're returning an object result in controller actions, (but this could make it slow) 
builder.Services.AddControllers().AddNewtonsoftJson(
    options => { options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore; }
    );
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddHttpClient<FilingService>();
//builder.Services.AddScoped<FilingService>();
//builder.Services.AddHostedService<FilingScheduler>();



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseAuthentication();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseCors("AllowAll");
app.Run();
