using Flunt.Validations;
using Minimal_EF_Dapper.Domain.Database.BaseEntity;

namespace Minimal_EF_Dapper.Domain.Database.Entities.Product
{
    public class Order : Entity
    {
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public List<Product> Products { get; set; }
        public decimal Total { get; set; }

        private void Validate()
        {
            //validacao com flunt
            var contract = new Contract<Order>()
                .IsNotNullOrEmpty(ClientId, "ClientId", "Id é obrigatório")
                .IsNotNullOrEmpty(ClientName, "ClientName", "Nome é obrigatório");

            AddNotifications(contract);
        }

        public Order()
        {
            // use uum construtor vazio sempre que operar com o ef
        }

        public void AddOrder(string clientId, string clientName, List<Product> products)
        {
            ClientId = clientId;
            ClientName = clientName;

            Products = products;

            //Audity -------------------------------

            CreatedBy = clientName;
            CreatedOn = DateTime.Now;
            EditedBy = clientName;
            EditedOn = DateTime.Now;

            foreach (var product in Products)
            {
                Total += product.Price;
            }

            Validate();
        }

        public void EditOrder(string clientId, string clientName, List<Product> products)
        {
            ClientId = clientId;
            ClientName = clientName;

            Products = products;

            //Audity -------------------------------

            CreatedBy = clientName;
            CreatedOn = DateTime.Now;
            EditedBy = clientName;
            EditedOn = DateTime.Now;

            foreach (var product in Products)
            {
                Total += product.Price;
            }

            Validate();
        }
    }
}