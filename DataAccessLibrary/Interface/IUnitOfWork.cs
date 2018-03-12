using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLibrary.Interface
{
    public interface IUnitOfWork
    {
        IRepository<T> Repository<T>() where T : class;

        void Dispose();
        void Dispose(bool disposing);

        void Save();
    }
}
