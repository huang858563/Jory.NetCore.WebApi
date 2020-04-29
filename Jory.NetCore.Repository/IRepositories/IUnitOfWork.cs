using SqlSugar;

namespace Jory.NetCore.Repository.IRepositories
{
    public interface IUnitOfWork
    {
        SqlSugarClient GetDbClient();

        void BeginTran();

        void CommitTran();

        void RollbackTran();
    }
}
