namespace UruIT.Serialization
{
    /// <summary>
    /// JSON.NET's contract resolver to manage JSON contracts
    /// </summary>
    public interface IContractResolver
    {
        /// <summary>
        /// Allows creating and modifying properties
        /// </summary>
        IPropertyContract PropertyContract { get; set; }

        /// <summary>
        /// Allows creating and modifying objects
        /// </summary>
        IObjectContract ObjectContract { get; set; }

        /// <summary>
        /// Creates an exact copy
        /// </summary>
        IContractResolver Clone();
    }
}