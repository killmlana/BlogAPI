using System.Text.RegularExpressions;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using ISession = Microsoft.AspNetCore.Http.ISession;

namespace BlogAPI.Factories;

using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class SessionFactory 
{
    private static readonly Lazy<ISessionFactory> _sessionFactory = new (() =>
    {
        return Fluently.Configure()
            .Database(SQLiteConfiguration.Standard.UsingFile("firstProject.db"))
            .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Program>())
            .ExposeConfiguration(BuildSchema)
            .BuildSessionFactory();
    });

    private static void BuildSchema(Configuration config)
    {
        new SchemaExport(config).Create(false, false);
    }

    public ISession OpenSession()
    {
        return _sessionFactory.Value.OpenSession();
    }

    public string GenerateGuid()
    {
        string id = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        id = Regex.Replace(id, "[^0-9a-zA-Z]+", "");
        return id;
    }

    public ISession GetSession()
    {
        return _sessionFactory.Value.GetCurrentSession();
    }

    public async ValueTask Dispose()
    {
        if (_sessionFactory.IsValueCreated)
        {
            await Task.Run(() => _sessionFactory.Value.Dispose());
        }
    }
}
