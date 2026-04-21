using DeliverySystem.Data.Infrastructure;
using DeliverySystem.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeliverySystem.Data.Services
{
    public partial class CartService : ServiceBase<Cart>, ICartService
    {
        private readonly IUnitOfWork _uow;
        public CartService(IDbFactory dbFactory, IUnitOfWork unitOfWork) : base(dbFactory, unitOfWork)
        {
            _uow = unitOfWork;
        }
    }

    public partial interface ICartService : IService<Cart>
    {
    }
}
