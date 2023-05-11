using Dapper;
using Microsoft.Data.SqlClient;
using Minimal_EF_Dapper.Business.Models;

namespace Minimal_EF_Dapper.Business
{
    public class ServiceAllProductsSold : Interface.IServiceAllProductsSold
    {
        public ServiceAllProductsSold(IConfiguration configuration)
        {
            //Necessario para recuperar as configuracoes de conexao para
            //usar com Dapper
            this.configuration = configuration;
        }

        public IConfiguration configuration { get; }

        public async Task<IEnumerable<ProductSold>> Execute()
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