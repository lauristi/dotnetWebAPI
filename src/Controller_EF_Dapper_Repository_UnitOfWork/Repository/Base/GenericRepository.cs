using Controller_EF_Dapper_Repository_UnityOfWork.Domain.Database;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static Controller_EF_Dapper_Repository_UnityOfWork.Repository.Base.IGenericRepository;

namespace Controller_EF_Dapper_Repository_UnityOfWork.Repository.Base
{
    public abstract class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly DbContext _context;

        protected GenericRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<T> Get(Guid id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task Add(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        // Esse método usa a expressão LINQ Where para filtrar os objetos do tipo T que
        // atendem ao predicado fornecido. Em seguida, o resultado é convertido
        // em uma lista com o método ToListAsync e retornado como um Task
        // que representa o resultado da operação assíncrona.

        public async Task<List<T>> Find(Expression<Func<T, bool>> filter)
        {
            return await _context.Set<T>().Where(filter).ToListAsync();
        }
    }
}