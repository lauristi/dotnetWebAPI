using Controller_EF_Dapper_Repository_UnityOfWork.AppDomain.UnitOfWork.Interface;
using Controller_EF_Dapper_Repository_UnityOfWork.Domain.Database;
using Controller_EF_Dapper_Repository_UnityOfWork.Repository.Repositories.Interfaces;

namespace Controller_EF_Dapper_Repository_UnityOfWork.AppDomain.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        public IProductRepository Products { get; }
        public ICategoryRepository Categories { get; }
        public IOrderRepository Orders { get; }

        public UnitOfWork(ApplicationDbContext dbContext,
                          IProductRepository products,
                          ICategoryRepository categories,
                          IOrderRepository orders
            )

        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            Products = products ?? throw new ArgumentNullException(nameof(products));
            Categories = categories ?? throw new ArgumentNullException(nameof(categories));
            Orders = orders ?? throw new ArgumentNullException(nameof(orders));
        }

        public int Commit()
        {
            return _dbContext.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
        }
    }
}