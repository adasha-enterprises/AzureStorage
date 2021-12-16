using QueueStorageLibrary.Models;
using System.Threading.Tasks;

namespace SupervisorService.Interfaces
{
    public interface ISupervisorService
    {
        /// <summary>
        /// Method to insert an order into the queue and then retrieve the confirmation from an Azure storage table
        /// </summary>
        /// <param name="order">The order object to be added to the queue</param>
        /// <returns><see cref="ConfirmationDTO"/></returns>
        Task<ConfirmationDTO> InsertOrderAsync(OrderDTO order);

        /// <summary>
        /// Method to retrieve an order confirmation from an Azure storage table
        /// </summary>
        /// <param name="orderId">The id of the order to retrieve</param>
        /// <returns><see cref="ConfirmationDTO"/></returns>
        Task<ConfirmationDTO> RetrieveAsync(string orderId);
    }
}
