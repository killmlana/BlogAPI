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

    public NHibernateHelper()
    {
        _sessionFactory = Fluently.Configure()
            .Database(
                SQLiteConfiguration.Standard
                    .UsingFile("firstProject.db")
            )
            .Mappings(m =>
                m.FluentMappings.AddFromAssemblyOf<Program>())
            .ExposeConfiguration(BuildSchema)
            .BuildSessionFactory();
    } 
    
    private void BuildSchema(Configuration config)
    {
        // this NHibernate tool takes a configuration (with mapping info in)
        // and exports a database schema from it
        new SchemaExport(config)
            .Create(false, true);
    }

    public ISession OpenSession()
    {
        return _sessionFactory.OpenSession();
    }
    
    public ISession GetSession()
    {
        return _sessionFactory.GetCurrentSession();
    }

    public async Task Dispose()
    {
        _sessionFactory.Dispose();
    }

    public async Task SetUsername(User user, string? username)
        {
            if (username == null) return;
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                try
                {
                    var userToUpdate = await session.GetAsync<User>(user.Id);
                    if (userToUpdate == null)
                    {
                        Console.WriteLine("Error Occured while trying to query user.");
                        return;
                    }
                    userToUpdate.Username = username;
                    await session.SaveOrUpdateAsync(userToUpdate);
                    await transaction.CommitAsync();
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine("Error occured: " + e.Message);
                }
            }
        }

    public async Task<User?> FindByuserId(string id) // returns null if no user found.
    {
        using (var s = OpenSession())
        {
            User? queryUser = await s.GetAsync<User?>(id);
            return queryUser;
        }
    }

    public async Task DeleteUser(User user)
    {
        using (var session = OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            try
            {
                var userToDelete = await session.GetAsync<User>(user.Id);
                if (userToDelete == null) throw new QueryException("User not found.");
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

    public async Task Update(User user)
    {
        using (var session = _sessionFactory.OpenSession())
        using (var transaction = session.BeginTransaction())
        {
            await session.UpdateAsync(user);
            await session.FlushAsync();
            await transaction.CommitAsync();
        }    
    }

    public async Task<User?> FindByName(string name) // returns null when no user is found
    {
        using (var session = _sessionFactory.OpenSession())
        {
            User? userToFind = await session.Query<User>().FirstOrDefaultAsync(u => u.Username.ToLower() == name);
            return userToFind;
        }
    }

    public async Task CreateUser(User user)
    {
        try
        {
            using (var session = _sessionFactory.OpenSession())
            using (var transaction = session.BeginTransaction())
            {
                await session.MergeAsync(user);
                await transaction.CommitAsync();
            }
        } catch (Exception e)
        {
            throw;
        }
    } 
}
            