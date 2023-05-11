using Controler_EF_Dapper.Domain.Database.BaseEntity;
using Flunt.Validations;

namespace Controler_EF_Dapper.Domain.Database.Entities.Product
{
    public class Category : Entity
    {
        public string Name { get; set; }
        public bool Active { get; set; }

        private void Validate()
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
            // use uum construtor vazio sempre que operar com o ef
        }

        public void AddCategory(string name, string createdBy)
        {
            Name = name;
            Active = true;

            //Audity ------------------------------------
            CreatedBy = createdBy;
            CreatedOn = DateTime.Now;

            Validate();
        }

        public void EditInfo(string name, bool active, string editedBy)
        {
            Active = active;
            Name = name;

            //Audity ------------------------------------
            EditedBy = editedBy;
            EditedOn = DateTime.Now;

            Validate();
        }

        public static implicit operator Category(Task<Category?> v)
        {
            throw new NotImplementedException();
        }
    }
}