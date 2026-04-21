using DeliverySystem.Data.Infrastructure;
using DeliverySystem.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DeliverySystem.Data.Services
{
    public partial class OrderService : ServiceBase<Order>, IOrderService
    {
        private readonly IUnitOfWork _uow;
        public OrderService(IDbFactory dbFactory, IUnitOfWork unitOfWork) : base(dbFactory, unitOfWork)
        {
            _uow = unitOfWork;
        }
    }

    public partial interface IOrderService : IService<Order>
    {
    }
}
