using PBVCC.DAL.AppData;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBVCC.DAL
{
    public class UnitOfWork : IDisposable
    {
        private TradeBulkEntities context = new TradeBulkEntities();

        public Type TheType { get; set; }

        public GenericRepository<TEntityType> GetRepoInstance<TEntityType>() where TEntityType:class
        {
                return new GenericRepository<TEntityType>(context);
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}