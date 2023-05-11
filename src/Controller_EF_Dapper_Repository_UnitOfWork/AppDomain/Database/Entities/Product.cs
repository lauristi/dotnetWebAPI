using Controller_EF_Dapper_Repository_UnityOfWork.AppDomain.Database.BaseEntity;
using Flunt.Validations;

namespace Controller_EF_Dapper_Repository_UnityOfWork.AppDomain.Database.Entities
{
    public class Product : Entity
    {
        // usar private no set restringe o acesso
        // aos metodos somente pelo construtor da classe

        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool Active { get; set; } = true;

        //Categorias -------------------------------------------------
        public Guid CategoryId { get; set; }

        public Category Category { get; set; }

        //Pedidos ----------------------------------------------------
        public ICollection<Order> Orders { get; set; }

        public Product()
        {
            // use um construtor vazio sempre que operar com o ef
        }

        public void Validate()
        {
            //validacao com flunt
            var contract = new Contract<Product>()
                .IsNotNullOrEmpty(Name, "Name", "Nome é obrigatório")
                .IsGreaterOrEqualsThan(Name, 3, "Name")
                .IsGreaterOrEqualsThan(Price, 1, "Price")
                .IsNotNull(Category, "Category", "Category not found")
                .IsNotNullOrEmpty(Description, "Description")
                .IsGreaterOrEqualsThan(Description, 3, "Description");

            //.IsNotNullOrEmpty(CreatedBy, "CreatedBy", "O usuario criador é obrigatório")
            //.IsNotNullOrEmpty(EditedBy, "EditedBy", "O usuario alterador é obrigatório");

            AddNotifications(contract);
        }
    }
}