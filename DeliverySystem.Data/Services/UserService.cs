using DeliverySystem.Data.Infrastructure;
using DeliverySystem.Data.Models.Entities;

namespace DeliverySystem.Data.Services
{
    public partial class UserService : ServiceBase<User>, IUserService
    {
        private readonly IUnitOfWork _uow;
        public UserService(IDbFactory dbFactory, IUnitOfWork unitOfWork) : base(dbFactory, unitOfWork)
        {
            _uow = unitOfWork;
        }
    }

    public partial interface IUserService : IService<User>
    {
    }
}
