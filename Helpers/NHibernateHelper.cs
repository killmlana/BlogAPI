using BlogAPI.Contracts;
using BlogAPI.Entities;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Linq;
using NHibernate.Tool.hbm2ddl;
using ISession = NHibernate.ISession;

namespace BlogAPI.Helpers;

public class NHibernateHelper : INhibernateHelper
{
    private ISessionFactory _sessionFactory;

    public NHibernateHelper(ISessionFactory sessionFactory)
    {
        _sessionFactory = sessionFactory;
    }

    private ISessionFactory SessionFactory
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
    private void BuildSchema(Configuration config)
    {
        // delete the existing db on each run
        if (File.Exists("firstProject.db"))
            File.Delete("firstProject.db");

        // this NHibernate tool takes a configuration (with mapping info in)
        // and exports a database schema from it
        new SchemaExport(config)
            .Create(false, true);
    }
    public ISession OpenSession()
    {
        return _sessionFactory.OpenSession();
    }

    public async Task SetUsername(User user, string? username, ISession? session=null)
    {
        if (username == null) return;
        if (session == null)
        {
            try
            {
                session = OpenSession();
                await SetUsername(user, username, session);
                return;
            }
            catch (Exception e) 
            {
                Console.WriteLine("Error: " + e);
                return;
            }
        } 
        using (var transaction = session.BeginTransaction())
        {
            try
            {
                var userToUpdate = await FindByuserId(user.Id, session);
                if (userToUpdate == null)
                {
                    Console.WriteLine("Error Occured while trying to query user.");
                    return;
                }
                userToUpdate.Username = username;
                await session.SaveAsync(userToUpdate);
                await transaction.CommitAsync();
                await session.FlushAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                Console.WriteLine("Error occured: " + e.Message);
            }
        }
    }

    public async Task<User?> FindByuserId(string id, ISession? s=null) // returns null if no user found.
    {
        if (s == null)
        {
            try
            {
                s = OpenSession();
                return await FindByuserId(id, s);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
                return null;
            }
        }
        try
        {
            var queryUser = await s.GetAsync<User?>(id);
            return queryUser;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public async Task DeleteUser(User user, ISession? session = null)
    {
        if (session == null)
        {
            try
            {
                session = OpenSession();
                await DeleteUser(user, session);
                return;
            }
            catch (Exception e) 
            {
                Console.WriteLine("Error: " + e);
                return;
            }
        }

        using (var transaction = session.BeginTransaction())
        {
            try
            {
                var userToDelete = await session.GetAsync<User>(user.Id);
                if (userToDelete == null) return;
                await session.DeleteAsync(userToDelete);
                await transaction.CommitAsync();
                await session.FlushAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
                await transaction.RollbackAsync();
            }
        }
    }

    public async Task<User?> FindByName(string name, ISession? session = null)
    {
        if (session == null)
        {
            try
            {
                session = OpenSession();
                return await FindByName(name, session);
            }
            catch (Exception e) 
            {
                Console.WriteLine("Error: " + e);
                return null;
            }
        }

        try
        {
            return await session.Query<User>().FirstOrDefaultAsync(u => u.Username.ToLower() == name);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e);
            return null;
        }
    }
}
            