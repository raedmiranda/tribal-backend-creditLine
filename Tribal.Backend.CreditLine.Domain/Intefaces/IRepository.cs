using System;
using System.Linq;
using System.Linq.Expressions;

namespace Tribal.Backend.CreditLine.Domain.Intefaces
{
    public interface IRepository<T>
        where T : class
    {

        /// <summary>
        /// Search for entities in the context according to the expression provided.
        /// </summary>
        /// <param name="expression">Expression with which the search will be carried out</param>
        /// <returns></returns>
        IQueryable<T> Find(Expression<Func<T, bool>> expression);

        /// <summary>
        /// Obtains entities by means of their primary key
        /// </summary>
        /// <param name="id">Entity Identifier</param>
        /// <returns>Entity</returns>
        T Get(object id);

        /// <summary>
        /// Add an entity to the context
        /// </summary>
        /// <param name="entity">Entity to be added</param>
        void Insert(T entity);
        /// <summary>
        /// Update a context entity
        /// </summary>
        /// <param name="entity">Entity to be updated</param>
        void Update(T entity);

        /// <summary>
        /// Delete an entity in context.
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        void Delete(T entity);
    }
}
