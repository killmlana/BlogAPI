using BlogAPI.Contracts;
using BlogAPI.Entities;
using BlogAPI.Helpers;
using Microsoft.AspNetCore.Authentication;

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
builder.Services.AddScoped<INhibernateHelper, NHibernateHelper>();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
