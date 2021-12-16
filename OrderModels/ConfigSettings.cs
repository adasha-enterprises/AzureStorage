namespace OrderModels
{
    /// <summary>
    /// Configuration Settings used within the solution
    /// </summary>
    public class ConfigSettings
    {
        public string StorageConnectionString { get; set; }
        public string ConfirmationTableName { get; set; }
        public string OrderIdTableName { get; set; }
    }
}
