using Controller_EF_Dapper_Repository_UnityOfWork.AppDomain.Database.Entities;
using Controller_EF_Dapper_Repository_UnityOfWork.Business.Interface;
using Controller_EF_Dapper_Repository_UnityOfWork.Business.Models.Product;
using Controller_EF_Dapper_Repository_UnityOfWork.Domain.Database;
using Controller_EF_Dapper_Repository_UnityOfWork.Repository.Base;
using Controller_EF_Dapper_Repository_UnityOfWork.Repository.Repositories.Interfaces;

namespace Controller_EF_Dapper_Repository_UnityOfWork.Repository.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly IServiceProductSold _serviceProductSold;

        public ProductRepository(ApplicationDbContext dbContext,
                                 IServiceProductSold serviceAllProductSold) : base(dbContext)
        {
            _serviceProductSold = serviceAllProductSold;
        }

        public Task<IEnumerable<ProductSold>> GetSoldProducts()
        {
            var productsSold = _serviceProductSold.GetAll();

            return productsSold;
        }
    }
}