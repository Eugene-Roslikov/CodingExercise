
using Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SupervisorApi.Services;
using System;
using System.Collections.Generic;

namespace SupervisorApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IRepositoryService repositoryService;

        public OrderController(IRepositoryService repositoryService)
        {
            this.repositoryService = repositoryService;
        }

        [HttpGet]
        [Produces("application/json")]
        public IEnumerable<Confirmation> GetAllOrderConfirmations()
        {
            return repositoryService.GetAllOrderConfirmations();
        }

        /// <summary>Send an order and returns its ID.</summary>
        /// <param name="order">An order object.</param>
        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(201)]
        public ActionResult<int> SendOrder([FromBody] Order order)
        {
            var createdOrder = repositoryService.SendOrder(order);
            return Created(String.Empty, createdOrder.OrderId);
        }
    }
}
