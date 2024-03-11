using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using ISession = NHibernate.ISession;

namespace BlogAPI.Helpers;

public static class NHibernateHelper
{
    private static ISessionFactory _sessionFactory;

    private static ISessionFactory SessionFactory
    {
        get
        {
            return _sessionFactory = Fluently.Configure()
                .Database(
                    SQLiteConfiguration.Standard
                        .UsingFile("firstProject.db")
                )
                .Mappings(m =>
                    m.FluentMappings.AddFromAssemblyOf<Program>())
                .ExposeConfiguration(BuildSchema)
                .BuildSessionFactory();
        }
    }
    private static void BuildSchema(Configuration config)
    {
        // delete the existing db on each run
        if (File.Exists("firstProject.db"))
            File.Delete("firstProject.db");

        // this NHibernate tool takes a configuration (with mapping info in)
        // and exports a database schema from it
        new SchemaExport(config)
            .Create(false, true);
    }
    public static ISession OpenSession()
    {
        return SessionFactory.OpenSession();
    }
}
            