using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using JiraCloneMVC.Web.Errors;
using JiraCloneMVC.Web.Repositories.Interfaces;

namespace JiraCloneMVC.Web.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected ApplicationDbContext DbContext { get; set; }
        protected DbSet<T> Entries { get; set; }

        public GenericRepository(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
            Entries = dbContext.Set<T>();
        }

        public void Add(T entry)
        {
            Entries.Add(entry);
            DbContext.SaveChanges();
        }

        public void AddRange(IEnumerable<T> entries)
        {
            Entries.AddRange(entries);
            DbContext.SaveChanges();
        }

        public void Delete(T entry)
        {
            Entries.Remove(entry);
            DbContext.SaveChanges();
        }

        public void DeleteRange(IEnumerable<T> entries)
        {
            Entries.RemoveRange(entries);
            DbContext.SaveChanges();
        }

        public IEnumerable<T> GetAll()
        {
            return Entries.AsEnumerable();
        }

        public virtual T GetById(object id)
        {
            return Entries.Find(id);
        }

        public void Update(T entry)
        {
            DbContext.Set<T>().Attach(entry);
            DbContext.Entry(entry).State = EntityState.Modified;
            DbContext.SaveChanges();
        }

        public IEnumerable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return Entries.Where(predicate).AsEnumerable();
        }
    }
}