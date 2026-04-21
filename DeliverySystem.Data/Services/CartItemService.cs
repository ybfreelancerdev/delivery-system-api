using DeliverySystem.Data.Infrastructure;
using DeliverySystem.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeliverySystem.Data.Services
{
    public partial class CartItemService : ServiceBase<CartItem>, ICartItemService
    {
        private readonly IUnitOfWork _uow;
        public CartItemService(IDbFactory dbFactory, IUnitOfWork unitOfWork) : base(dbFactory, unitOfWork)
        {
            _uow = unitOfWork;
        }
    }

    public partial interface ICartItemService : IService<CartItem>
    {
    }
}
