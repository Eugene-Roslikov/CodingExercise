using System.Collections.Generic;
using Common;

namespace SupervisorApi.Services
{
    public interface IRepositoryService
    {
        IEnumerable<Confirmation> GetAllOrderConfirmations();
        Order SendOrder(Order order);
    }
}
