using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Tribal.Backend.CreditLine.Domain.Intefaces;

namespace Tribal.Backend.CreditLine.Infrastructure.DataRepositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected List<T> _context;

        public Repository(List<T> context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Delete(T entity)
        {
            _context.Remove(entity);
        }

        public IQueryable<T> Find(Expression<Func<T, bool>> expression)
        {
            return _context.AsQueryable<T>().Where(expression);
        }

        public T Get(object id)
        {
            throw new NotImplementedException();
            //return _context.Find(x => x == id);
        }

        public void Insert(T entity)
        {
            _context.Add(entity);
        }

        public void Update(T entity)
        {
            throw new NotImplementedException();

            //if(_context != null)
            //{
            //    foreach(T item in _context)
            //    {
            //        predicate(item);
            //    }
            //}

            ////if (_context.Contains(entity))
            ////    _context.Find(x => x == entity)
            //_context.Remove(entity);
            //_context.Add(entity);
            //_context.AsEnumerable<T>().Update()
        }
    }
}
