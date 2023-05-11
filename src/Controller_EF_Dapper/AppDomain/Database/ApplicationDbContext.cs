using Controler_EF_Dapper.Domain.Database.Entities.Product;
using Flunt.Notifications;
using Microsoft.EntityFrameworkCore;

namespace Controler_EF_Dapper.Domain.Database
{
    public class ApplicationDbContext : DbContext
    {
        //------------------------------------------------------------------------
        //TABELAS MAPEADAS
        //------------------------------------------------------------------------

        //==> Virtual pe necessario para o uso de Moq no teste unitário

        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Order> Orders { get; set; }

        public ApplicationDbContext()
        {
            //Necessario para que o contexto possa ser mokado no teste unitário
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //Chama da clase pai (adicona a modelagem do pai (identity) na criação da migration
            base.OnModelCreating(builder);

            //Necessario para o flunt funcionar corretamente
            //The entity type 'Notification' requires a primary key to be defined.
            //If you intended to use a keyless entity type, call 'HasNoKey' in 'OnModelCreating'
            builder.Ignore<Notification>();

            //Define configuracoes individuais que serão aplicadas pelo EF

            //------------------------------------------------------------------------------------------------
            //TABELA PRODUTO
            //------------------------------------------------------------------------------------------------
            builder.Entity<Product>()
                         .Property(p => p.Description).HasMaxLength(255)
                                                    .IsRequired(false);

            builder.Entity<Product>()
                         .Property(p => p.Price).HasColumnType("decimal(10,2)").IsRequired();

            builder.Entity<Product>()
                   .Property(p => p.Name).IsRequired(true);

            builder.Entity<Product>()
                   .Property(p => p.Name).IsRequired(true);

            //Audity---------------------------------------------------

            builder.Entity<Product>()
                .Property(p => p.CreatedBy).IsRequired(false);

            builder.Entity<Product>()
                   .Property(p => p.CreatedOn).IsRequired(false);

            builder.Entity<Product>()
                   .Property(p => p.EditedBy).IsRequired(false);

            builder.Entity<Product>()
                   .Property(p => p.EditedOn).IsRequired(false);

            //------------------------------------------------------------------------------------------------
            //TABELA CATEGORIA
            //------------------------------------------------------------------------------------------------

            builder.Entity<Category>()
                   .Property(c => c.Name).IsRequired(true);

            //Audity---------------------------------------------------

            builder.Entity<Category>()
                .Property(c => c.CreatedBy).IsRequired(false);

            builder.Entity<Category>()
                   .Property(c => c.CreatedOn).IsRequired(false);

            builder.Entity<Category>()
                   .Property(c => c.EditedBy).IsRequired(false);

            builder.Entity<Category>()
                   .Property(c => c.EditedOn).IsRequired(false);

            //------------------------------------------------------------------------------------------------
            //TABELA PEDIDOS
            //------------------------------------------------------------------------------------------------

            builder.Entity<Order>()
           .Property(o => o.ClientId).IsRequired(true);

            builder.Entity<Order>()
           .Property(o => o.ClientName).IsRequired(true);

            //O entity cria uma tabela de muitos para muitos nesta situação
            builder.Entity<Order>()
           .HasMany(o => o.Products)
           .WithMany(p => p.Orders)
           .UsingEntity(x => x.ToTable("OrderProduct"));
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configuration)
        {
            //Define configuracoes globais que serão aplicadas pelo EF

            configuration.Properties<string>()
                         .HaveMaxLength(100);
        }
    }
}