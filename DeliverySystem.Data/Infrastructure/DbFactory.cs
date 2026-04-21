using DeliverySystem.Data;
using DeliverySystem.Data.Context;

namespace DeliverySystem.Data.Infrastructure

{
    public class DbFactory : Disposable, IDbFactory
    {
        DeliverySystemContext dbContext;
        public DeliverySystemContext Init()
        {
            return dbContext ?? (dbContext = new DeliverySystemContext());
        }
        protected override void DisposeCore()
        {
            if (dbContext != null)
            {
                dbContext.Dispose();
            }
        }
    }
    public interface IDbFactory : IDisposable
    {
        DeliverySystemContext Init();
    }
}
