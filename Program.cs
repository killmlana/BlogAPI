using BlogAPI.Entities;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

static ISessionFactory CreateSessionFactory()
{
    return Fluently.Configure()
        .Database(
            SQLiteConfiguration.Standard
                .UsingFile("firstProject.db")
        )
        .Mappings(m =>
            m.FluentMappings.AddFromAssemblyOf<Program>())
        .ExposeConfiguration(BuildSchema)
        .BuildSessionFactory();
}

static void BuildSchema(Configuration config)
{
    // delete the existing db on each run
    if (File.Exists("firstProject.db"))
        File.Delete("firstProject.db");

    // this NHibernate tool takes a configuration (with mapping info in)
    // and exports a database schema from it
    new SchemaExport(config)
        .Create(false, true);
}

static void AddPostsToUser(User user, params Post[] posts)
{
    foreach (var post in posts)
    {
        post.AddPost(user);
    }
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

app.MapPost("/addUser", () =>
{
    var sessionFactory = CreateSessionFactory();
    using (var session = sessionFactory.OpenSession())
    {
        using (var transaction = session.BeginTransaction())
        {
            var tempUser = new User("wldhalhwdnawd", "killmlana", "aojwdajwpak", 10, 198273);
            var tempPost = new Post("sladwlajd", tempUser, "test", "test", 1982022, 028302, 1);
            AddPostsToUser(tempUser, tempPost);
            session.SaveOrUpdate(tempUser);
            transaction.Commit();
        }
    }
})
.WithName("SetUser")
.WithOpenApi();

app.Run();
