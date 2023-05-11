namespace Controller_EF_Dapper_Repository_UnityOfWork.Business.Models.Product
{
    public class OrderDetailed
    {
        public OrderDetailed()
        {
        }

        public OrderDetailed(string clientId, string clientName, IEnumerable<OrderProduct> products)
        {
            this.ClientId = clientId;
            this.ClientName = clientName;
            this.Products = products;
        }

        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public IEnumerable<OrderProduct> Products { get; set; }
    }
}