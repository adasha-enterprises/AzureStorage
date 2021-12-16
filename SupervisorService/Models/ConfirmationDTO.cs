using OrderModels;
using System;

namespace SupervisorService.Models
{
    /// <summary>
    /// "Lightweight" version of <see cref="Confirmation"/> class
    /// </summary>
    public class ConfirmationDTO
    {
        public int OrderId { get; set; }
        public Guid AgentId { get; set; }
        public string OrderStatus { get; set; }
    }
}
