using DynamicTableService.Components;

namespace DynamicTableService
{
    public class DynamicTableManager : DTManagerNoQuerier
    {
        public readonly DTQuerier Querier;

        public DynamicTableManager(string connectionString) : base(connectionString)
        {
            Querier = new DTQuerier(connectionString);
        }

        public DynamicTableManager() : this(getConnectionStringFromConfig())
        {
        }
    }
}