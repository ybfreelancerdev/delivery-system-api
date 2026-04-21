using DeliverySystem.Data.Infrastructure;
using DeliverySystem.Data.Models.Entities;

namespace DeliverySystem.Data.Services
{
    public partial class ProductCategoryService : ServiceBase<ProductCategories>, IProductCategoryService
    {
        private readonly IUnitOfWork _uow;
        public ProductCategoryService(IDbFactory dbFactory, IUnitOfWork unitOfWork) : base(dbFactory, unitOfWork)
        {
            _uow = unitOfWork;
        }
    }

    public partial interface IProductCategoryService : IService<ProductCategories>
    {
    }
}
