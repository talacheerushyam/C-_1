using DataAccessLibrary.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary
{
    /// <summary>
    /// This Class will give the requested Repository by passing the Entity
    /// and has all the required LINQ to Entity Queries 
    /// </summary>
    public class Repository<T> : IRepository<T> where T : class
    {
        #region Properties
        protected DbContext DbContext { get; set; }

        protected DbSet<T> DbSet { get; set; }

        #endregion

        #region LifeCycle
        public Repository(DbContext dbContext)
        {
            if(dbContext==null)
                throw new ArgumentNullException("DbContext is NULL");
            DbContext = dbContext;
            DbSet= DbContext.Set<T>();
            DbContext.Database.Log = s => System.Diagnostics.Debug.WriteLine(s); // This will log the sql generated in VS OutputWindow
            DbContext.Database.Log = Console.WriteLine; //this will generate logging in console window
        }

        #endregion

        #region IRepository Implementation

        public virtual void Add(T entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry(entity);
            if (dbEntityEntry.State != EntityState.Detached)
            {
                dbEntityEntry.State = EntityState.Added;
            }
            else
            {
                DbSet.Add(entity);
            }
        }

        public virtual void Delete(int id)
        {
            var entity = FindById(id);
            if (entity == null) return; 
            Delete(entity);
        }

        public virtual void Delete(T entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry(entity);
            if (dbEntityEntry.State != EntityState.Deleted)
            {
                dbEntityEntry.State = EntityState.Deleted;
            }
            else
            {
                DbSet.Attach(entity);
                DbSet.Remove(entity);
            }
        }

        public virtual IQueryable<T> GetAll()
        {
            return DbSet;
        }

        public virtual T FindById(int id)
        {
            return DbSet.Find(id);
        }


        public virtual void Update(T entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry(entity);
            if(dbEntityEntry.State==EntityState.Detached)
            {
                DbSet.Attach(entity);
            }
            dbEntityEntry.State = EntityState.Modified;
        }


        /// <summary>
        /// T is TEntity
        /// </summary>
        public virtual RepositoryQuery<T> Query()
        {
            var repositoryGetFluentHelper =
                new RepositoryQuery<T>(this);

            return repositoryGetFluentHelper;
        }

        /// <summary>
        
        /// The Get Method here handles querying the data supporting a filtering, ordering, paging, 
        /// and eager loading of child types, so that we can make one round trip and eager load the required Entities
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="includeProperties"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        internal IQueryable<T> Get(
           Expression<Func<T, bool>> filter = null,
           Func<IQueryable<T>,
               IOrderedQueryable<T>> orderBy = null,
           List<Expression<Func<T, object>>>
               includeProperties = null,
           int? page = null,
           int? pageSize = null)
        {
            IQueryable<T> query = DbSet;

            if (includeProperties != null)
                includeProperties.ForEach(i => { query = query.Include(i); });

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            if (page != null && pageSize != null)
                query = query
                    .Skip((page.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value);

            return query;
        }

        

        public virtual IEnumerable<TEntity> ExecStoreProcedure<TEntity>(string query, params object[] parameters) where TEntity: class
        {
            return DbContext.Database.SqlQuery<TEntity>(query,parameters);
        }




        public virtual void ExecuteStoreProcedure(string query, params object[] parameters)
        {
           DbContext.Database.ExecuteSqlCommand(query, parameters);
           
        }

        #endregion
    }


}
