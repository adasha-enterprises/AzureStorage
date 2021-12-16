using OrderModels;
using QueueStorageLibrary.Models;
using System.Threading.Tasks;

namespace QueueStorageLibrary.Interfaces
{
  public interface ITableStorageCommands
  {
    /// <summary>
    /// Method to retrieve an order confirmation from an Azure storage table
    /// </summary>
    /// <param name="orderId">The id of the order to retrieve</param>
    /// <returns><see cref="Confirmation"/></returns>
    Task<Confirmation> RetrieveAsync(string orderId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="confirmation"></param>
    /// <returns>bool</returns>
    Task<bool> InsertConfirmation(ConfirmationDTO confirmation);
  }
}
