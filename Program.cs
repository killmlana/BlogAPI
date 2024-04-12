using System.Configuration;
using System.Text;
using BlogAPI.Contracts;
using BlogAPI.Entities;
using BlogAPI.Helpers;
using BlogAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

static void AddPostsToUser(User user, params Post[] posts)
{
    foreach (var post in posts)
    {
        post.AddPost(user);
    }
}

var builder = WebApplication.CreateBuilder(args);
var secret = builder.Configuration["JWT:Signing-Key"];


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
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
         ValidateIssuer = false, //change to true in prod
         ValidateAudience = false, //change to true in prod
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         ValidIssuer = builder.Configuration["JWT:Issuer"],
         ValidAudience = builder.Configuration["JWT:Audience"],
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
     };
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
