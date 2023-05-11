using Controller_EF_Dapper_Repository_UnityOfWork.Repository.Repositories.Interfaces;

namespace Controller_EF_Dapper_Repository_UnityOfWork.AppDomain.UnitOfWork.Interface
{
    // Usamos a interface IDisposable de forma a poder liberar os recursos não gerenciados
    // de maneira determinística.No entanto, Dispose não remove o próprio objeto da memória.
    // O objeto será removido quando o coletor de lixo achar conveniente.

    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        ICategoryRepository Categories { get; }
        IOrderRepository Orders { get; }

        int Commit();
    }
}