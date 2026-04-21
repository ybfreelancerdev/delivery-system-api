using DeliverySystem.Data.Infrastructure;
using DeliverySystem.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeliverySystem.Data.Services
{
    public partial class OrderItemService : ServiceBase<OrderItem>, IOrderItemService
    {
        private readonly IUnitOfWork _uow;
        public OrderItemService(IDbFactory dbFactory, IUnitOfWork unitOfWork) : base(dbFactory, unitOfWork)
        {
            _uow = unitOfWork;
        }
    }

    public partial interface IOrderItemService : IService<OrderItem>
    {
    }
}
