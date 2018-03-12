using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Interface
{
    /// <summary>
    /// T is an TEntity 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Delete(int id);
        IQueryable<T> GetAll();
        T FindById(int id);

        RepositoryQuery<T> Query();

        IEnumerable<TEntity> ExecStoreProcedure<TEntity>(string query, params object[] parameters) where TEntity : class;

        
        void ExecuteStoreProcedure(string query, params object[] parameters);
    }
}
