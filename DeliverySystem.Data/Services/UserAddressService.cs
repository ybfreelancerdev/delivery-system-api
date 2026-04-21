using DeliverySystem.Data.Infrastructure;
using DeliverySystem.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeliverySystem.Data.Services
{
    public partial class UserAddressService : ServiceBase<UserAddress>, IUserAddressService
    {
        private readonly IUnitOfWork _uow;
        public UserAddressService(IDbFactory dbFactory, IUnitOfWork unitOfWork) : base(dbFactory, unitOfWork)
        {
            _uow = unitOfWork;
        }
    }

    public partial interface IUserAddressService : IService<UserAddress>
    {
    }
}
