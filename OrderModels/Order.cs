using System.ComponentModel.DataAnnotations;

namespace OrderModels
{
    /// <summary>
    /// Class used to represent an order received from a 3rd party application
    /// </summary>
    public class Order
    {
        public int OrderId { get; set; }
        [Range(1, 10, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int RandomNumber { get; set; }
        public string OrderText { get; set; }
    }
}
