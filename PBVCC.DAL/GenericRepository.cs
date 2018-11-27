using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using PBVCC.DAL.AppData;
using System.Linq.Expressions;

namespace PBVCC.DAL
{
    public class GenericRepository<TEntity> where TEntity : class
    {
        internal TradeBulkEntities context;
        internal DbSet<TEntity> dbSet;

        public GenericRepository(TradeBulkEntities context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }

        public virtual IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query);
            }
            else
            {
                return query;
            }
        }
        public virtual IQueryable<TEntity> GetAllExpressions(Expression<Func<TEntity, bool>> filter = null,Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,params Expression<Func<TEntity, object>>[] naProperties)
        {
            IQueryable<TEntity> dbQuery = dbSet;

            if (filter != null)
            {
                dbQuery = dbQuery.Where(filter);
            }
            if (naProperties != null)
                foreach (Expression<Func<TEntity, object>> nProperty in naProperties)
                {
                    if(nProperty!=null)
                    dbQuery = dbQuery.Include<TEntity, object>(nProperty);
                }

            if (orderBy != null)
            {
                dbQuery = orderBy(dbQuery);
            }

            return dbQuery;
        }


        public virtual TEntity GetByID(object id)
        {
            return dbSet.Find(id);
        }

        public virtual void Insert(TEntity entity)
        {
            dbSet.Add(entity);
        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            context.Entry(entityToUpdate).State = EntityState.Modified;
        }
    }
}