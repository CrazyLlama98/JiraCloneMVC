using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace JiraCloneMVC.Web.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        void Add(T entry);
        void AddRange(IEnumerable<T> entries);
        void Update(T entry);
        void Delete(T entry);
        void DeleteRange(IEnumerable<T> entries);
        IEnumerable<T> GetAll();
        T GetById(object id);
        IEnumerable<T> Where(Expression<Func<T, bool>> predicate);
        T FirstOrDefault(Expression<Func<T, bool>> predicate);
    }
}
