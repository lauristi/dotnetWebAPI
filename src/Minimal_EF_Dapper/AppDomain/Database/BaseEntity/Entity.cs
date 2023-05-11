using Flunt.Notifications;

namespace Minimal_EF_Dapper.Domain.Database.BaseEntity
{
    public abstract class Entity : Notifiable<Notification>
    {
        //------------------------------------------------------------------------------------------------------
        // CLASSE PRINCIPAL ABSTRATA QUE SERA HERDADA POR TODAS AS ENTIDADES DO BANCO
        // EH HERDADA POR TODAS AS ENTITIDADES DE BANCO (TABELAS)
        // - Agrega o Notifiable do Flunt para que todas as entidades possam usar notification
        // - Agrega a criação de um Id Guid que será usando pelo EF na criação de novas entidades no banco
        //------------------------------------------------------------------------------------------------------
        public Entity()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        //Audity ------------------------------------
        public string CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }
        public string EditedBy { get; set; }
        public DateTime? EditedOn { get; set; }
    }
}