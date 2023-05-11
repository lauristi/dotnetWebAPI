using Controller_EF_Dapper_Repository_UnityOfWork.AppDomain.Database.BaseEntity;
using Flunt.Validations;

namespace Controller_EF_Dapper_Repository_UnityOfWork.AppDomain.Database.Entities
{
    public class Category : Entity
    {
        public string Name { get; set; }
        public bool Active { get; set; }

        public void Validate()
        {
            //validacao com flunt
            var contract = new Contract<Category>()
                .IsNotNullOrEmpty(Name, "Name", "Nome é obrigatório")
                .IsGreaterOrEqualsThan(Name, 3, "Name");

            //.IsNotNullOrEmpty(CreatedBy, "CreatedBy", "O usuario criador é obrigatório")
            //.IsNotNullOrEmpty(EditedBy, "EditedBy", "O usuario alterador é obrigatório");

            AddNotifications(contract);
        }

        public Category()
        {
            // use um construtor vazio sempre que operar com o ef
        }
    }
}