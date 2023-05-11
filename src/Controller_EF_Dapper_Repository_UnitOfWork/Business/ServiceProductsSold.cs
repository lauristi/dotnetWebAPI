using Controller_EF_Dapper_Repository_UnityOfWork.Business.Interface;
using Controller_EF_Dapper_Repository_UnityOfWork.Business.Models.Product;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Controller_EF_Dapper_Repository_UnityOfWork.Business
{
    public class ServiceProductSold : IServiceProductSold
    {
        public IConfiguration configuration { get; }

        public ServiceProductSold(IConfiguration configuration)
        {
            //Necessario para recuperar as configuracoes de conexao parao Dapper
            this.configuration = configuration;
        }

        public async Task<IEnumerable<ProductSold>> GetAll()
        {
            var db = new SqlConnection(configuration["Database:SQlServer"]);

            var query = @" SELECT A.ID,
                                  C.NAME,
                                  COUNT(*) AMOUNT
                             FROM ORDERS A
                            INNER JOIN ORDERPRODUCT B ON
                                  A.ID = B.ORDERSID
                            INNER JOIN PRODUCTS C ON
                                  C.ID = B.PRODUCTSID
                            GROUP BY A.ID, C.NAME
                            ORDER BY AMOUNT DESC";

            return await db.QueryAsync<ProductSold>(query);
        }
    }
}