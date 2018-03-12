using DataAccessLibrary.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary
{
    /// <summary>
    /// This is make sure all the Database Operations Including CRUD happens
    /// under one DbContext instance per page request
    /// </summary>
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly DbContext _context;

        private bool _disposed;
        private Hashtable _repositories;

        public UnitOfWork(DbContext context)
        {
            _context = context;
        }

        public UnitOfWork()
        {
            //_context = new 

        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }



        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing)
                    _context.Dispose();

            _disposed = true;
        }

        public void Save()
        {
            _context.SaveChanges();
        }


        /// <summary>
        /// This will give the required repository instance.
        /// we are storing all the activated instances of repositories for each and every requests and on request we will first check
        /// too see if our hastable(container to hold all of our activated repository instances) has been created, if not we create our container.
        /// Next, we’ll scan our container to see if the requested repository instance has already been created, if it has, then will return it, if it hasn’t,
        /// we will activate the requested repository instance, store it in our container, and then return it.
        /// We can think of this as  lazy loading our repository instances(only creating repository instances on demand)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IRepository<T> Repository<T>() where T : class
        {
            if (_repositories == null)
                _repositories = new Hashtable();

            var type = typeof(T).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(Repository<>);

                var repositoryInstance =
                    Activator.CreateInstance(repositoryType
                            .MakeGenericType(typeof(T)), _context);

                _repositories.Add(type, repositoryInstance);
            }

            return (IRepository<T>)_repositories[type];
        }

    }
}
