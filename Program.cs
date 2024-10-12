using System.Text;
using BlogAPI.Contracts;
using BlogAPI.Entities;
using BlogAPI.Helpers;
using BlogAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

static void AddPostsToUser(User user, params Post[] posts)
{
    foreach (var post in posts)
    {
        post.AddPost(user);
    }
}

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddScoped<NHibernateHelper>();
builder.Services.AddScoped<HashHelper>();
builder.Services.AddScoped<AuthHelper>();
builder.Services.AddScoped<CustomUserClaim>();
builder.Services.AddScoped<CustomRoleClaim>();
builder.Services.AddScoped<RoleManager<Role>>();
builder.Services.AddScoped<UserManager<User>>();
builder.Services.AddScoped<INhibernateHelper, NHibernateHelper>();
builder.Services.AddScoped<IUserPasswordStore<User>, CustomUserStore>();
builder.Services.AddScoped<IRoleStore<Role>, CustomRoleStore>();
builder.Services.AddScoped<IPasswordHasher<User>, CustomPasswordHasher>();
builder.Services.AddScoped<IUserClaimStore<User>, CustomUserStore>();
builder.Services.AddScoped<IHashHelper, HashHelper>();
builder.Services.AddIdentity<User, Role>().AddUserStore<CustomUserStore>().AddRoleStore<CustomRoleStore>();
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddSwaggerGen(c => 
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JWTToken_Auth_API",
        Version = "v1"
    });

    // Add the Bearer token security definition
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer your_token_here\"",
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer" 
                }
            },
            new string[] {}
        }
    });
});


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

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
