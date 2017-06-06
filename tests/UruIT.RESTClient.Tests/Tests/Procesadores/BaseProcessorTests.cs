using Microsoft.VisualStudio.TestTools.UnitTesting;
using UruIT.Serialization;

namespace UruIT.RESTClient.Tests
{
    [TestClass]
    public class BaseProcessorTests<TSerializer>
        where TSerializer : ISerializer
    {
        protected IProcessorStructure<TResult, TSerializer> CreateProcessorStructure<TResult>(IProcessorNode<TResult, TSerializer> processor)
        {
            return new ProcessorStructure<TResult, TSerializer>(processor);
        }
    }

    [TestClass]
    public class BaseProcessorTests : BaseProcessorTests<IJsonSerializer>
    {
        protected readonly IJsonSerializer jsonConverter = new JsonSerializer();
    }
}