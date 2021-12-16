using OrderModels;
using System.ComponentModel.DataAnnotations;

namespace QueueStorageLibrary.Models
{
  /// <summary>
  /// "Lightweight" version of <see cref="Order"/> class
  /// </summary>
  public class OrderDTO
  {
    public int OrderId { get; set; }
    [Range(1, 10, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int RandomNumber { get; set; }
    public string OrderText { get; set; }
  }

}
