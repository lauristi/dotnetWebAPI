using Controller_EF_Dapper_Repository_UnityOfWork.AppDomain.Database.Entities;
using Flunt.Notifications;
using Microsoft.EntityFrameworkCore;

namespace Controller_EF_Dapper_Repository_UnityOfWork.Domain.Database
{
    public class ApplicationDbContext : DbContext
    {
        //------------------------------------------------------------------------
        //TABELAS MAPEADAS
        //------------------------------------------------------------------------

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }

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
            //TABELA PRODUTO - PRODUCT
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
            //TABELA CATEGORIA - CATEGORY
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
            //TABELA PEDIDOS - ORDER
            //------------------------------------------------------------------------------------------------

            builder.Entity<Order>()
           .Property(o => o.ClientId).IsRequired(true);

            builder.Entity<Order>()
           .Property(o => o.ClientName).IsRequired(true);

            //Audity---------------------------------------------------

            builder.Entity<Order>()
                .Property(c => c.CreatedBy).IsRequired(false);

            builder.Entity<Order>()
                   .Property(c => c.CreatedOn).IsRequired(false);

            builder.Entity<Order>()
                   .Property(c => c.EditedBy).IsRequired(false);

            builder.Entity<Order>()
                   .Property(c => c.EditedOn).IsRequired(false);

            //O entity cria uma tabela de muitos para muitos nesta situação
            builder.Entity<Order>()
           .HasMany(o => o.Products)
           .WithMany(p => p.Orders)
           .UsingEntity(t => t.ToTable("OrderProduct"));
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configuration)
        {
            //Define configuracoes globais que serão aplicadas pelo EF

            configuration.Properties<string>()
                         .HaveMaxLength(100);
        }
    }
}