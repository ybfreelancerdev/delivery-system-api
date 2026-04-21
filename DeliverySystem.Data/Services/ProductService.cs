using DeliverySystem.Data.Infrastructure;
using DeliverySystem.Data.Models.Entities;

namespace DeliverySystem.Data.Services
{
    public partial class ProductService : ServiceBase<Product>, IProductService
    {
        private readonly IUnitOfWork _uow;
        public ProductService(IDbFactory dbFactory, IUnitOfWork unitOfWork) : base(dbFactory, unitOfWork)
        {
            _uow = unitOfWork;
        }
    }

    public partial interface IProductService : IService<Product>
    {
    }
}
