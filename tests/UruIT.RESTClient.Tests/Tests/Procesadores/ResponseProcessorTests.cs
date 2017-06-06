using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using System.Net;
using UruIT.RESTClient.Processors;
using UruIT.Serialization;

namespace UruIT.RESTClient.Tests
{
    [TestClass]
    public sealed class ResponseProcessorTests
    {
        private ResponseProcessorTests()
        {
        }

        [TestClass]
        public class ProcessorStructureTests
        {
            [TestMethod]
            public void WhenTwoProcessorsCanProcessItThenOnlyFirstOneDoes()
            {
                // Arrange
                IRestResponse response = new RestResponse()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ContentType = "application/json",
                    Content = "10"
                };
                var jsonConverter = new JsonSerializer();
                var processor = new ProcessorStructure<string, IJsonSerializer>(
                        new ExampleSimpleProcessorAlternate(),
                        new ExampleSimpleProcessor()
                        );

                // Act
                var res = processor.Process(response, jsonConverter);

                // Assert
                Assert.AreEqual("20", res);
            }
        }

        [TestClass]
        public class AddNeutralProcessorAtEndsTests
        {
            [TestMethod]
            public void WhenEmptyProcessorThenAddNeutralProcessorReturnsDefault()
            {
                // Arrange
                var processor = new ProcessorStructure<string, IJsonSerializer>();

                // Act
                ProcessorUtilities.AddNeutralProcessorAtEndsForStructure(processor);

                // Assert
                Assert.AreEqual(1, processor.Count);
                Assert.IsInstanceOfType(processor[0], typeof(SuccessProcessor<string, IJsonSerializer>));
            }

            [TestMethod]
            public void WhenSingleProcessorThenAddNeutralProcessorReturnsDefaultAtTheEnd()
            {
                // Arrange
                var processor = new ProcessorStructure<string, IJsonSerializer>(new ExampleSimpleProcessor());

                // Act
                ProcessorUtilities.AddNeutralProcessorAtEndsForStructure(processor);

                // Assert
                Assert.AreEqual(2, processor.Count);
                Assert.IsInstanceOfType(processor[0], typeof(ExampleSimpleProcessor));
                Assert.IsInstanceOfType(processor[1], typeof(SuccessProcessor<string, IJsonSerializer>));
            }

            [TestMethod]
            public void WhenSingleProcessorIsDefaultWithSuccessThenAddNeutralProcessorReturnsTheSameOne()
            {
                // Arrange
                var processor = new ProcessorStructure<string, IJsonSerializer>(new SuccessProcessor<string>());

                // Act
                ProcessorUtilities.AddNeutralProcessorAtEndsForStructure(processor);

                // Assert
                Assert.AreEqual(1, processor.Count);
                Assert.IsInstanceOfType(processor[0], typeof(SuccessProcessor<string, IJsonSerializer>));
            }

            [TestMethod]
            public void WhenRecursiveProcessorThenAddNeutralProcessorReturnsDefaultAtTheEnd()
            {
                // Arrange
                var processor = new ProcessorStructure<string, IJsonSerializer>(
                    new ExampleRecursiveProcessor().AddProcessors(
                        new ExampleSimpleProcessor()
                        )
                    );

                // Act
                ProcessorUtilities.AddNeutralProcessorAtEndsForStructure(processor);

                // Assert
                Assert.AreEqual(2, processor.Count);
                Assert.IsInstanceOfType(processor[0], typeof(ExampleRecursiveProcessor));
                Assert.IsInstanceOfType(processor[1], typeof(SuccessProcessor<string, IJsonSerializer>));

                var recNode = (ExampleRecursiveProcessor)processor[0];

                Assert.AreEqual(2, recNode.ProcessorStructure.Count);
                Assert.IsInstanceOfType(recNode.ProcessorStructure[0], typeof(ExampleSimpleProcessor));
                Assert.IsInstanceOfType(recNode.ProcessorStructure[1], typeof(SuccessProcessor<string, IJsonSerializer>));
            }

            [TestMethod]
            public void WhenRecursiveProcessorHasDefaultThenAddNeutralProcesorReturnsTheSame()
            {
                // Arrange
                var processor = new ProcessorStructure<string, IJsonSerializer>(
                    new ExampleRecursiveProcessor().AddProcessors(
                        new SuccessProcessor<string>().Default()
                        )
                    );

                // Act
                ProcessorUtilities.AddNeutralProcessorAtEndsForStructure(processor);

                // Assert
                Assert.AreEqual(2, processor.Count);
                Assert.IsInstanceOfType(processor[0], typeof(ExampleRecursiveProcessor));
                Assert.IsInstanceOfType(processor[1], typeof(SuccessProcessor<string, IJsonSerializer>));

                var recNode = (ExampleRecursiveProcessor)processor[0];

                Assert.AreEqual(1, recNode.ProcessorStructure.Count);
                Assert.IsInstanceOfType(recNode.ProcessorStructure[0], typeof(SuccessProcessor<string, IJsonSerializer>));
            }

            [TestMethod]
            public void WhenRecursiveProcessorWithGenericParametersThenAddNeutralProcessorReturnsTheSame()
            {
                // Arrange
                var processor = new ProcessorStructure<string, IJsonSerializer>(
                    new ExampleRecursiveProcessorAlternate<string>().AddProcessors(
                        new ExampleSimpleProcessor()
                        )
                    );

                // Act
                ProcessorUtilities.AddNeutralProcessorAtEndsForStructure(processor);

                // Assert
                Assert.AreEqual(2, processor.Count);
                Assert.IsInstanceOfType(processor[0], typeof(ExampleRecursiveProcessorAlternate<string>));
                Assert.IsInstanceOfType(processor[1], typeof(SuccessProcessor<string, IJsonSerializer>));

                var recNode = (ExampleRecursiveProcessorAlternate<string>)processor[0];

                Assert.AreEqual(2, recNode.ProcessorStructure.Count);
                Assert.IsInstanceOfType(recNode.ProcessorStructure[0], typeof(ExampleSimpleProcessor));
                Assert.IsInstanceOfType(recNode.ProcessorStructure[1], typeof(SuccessProcessor<string, IJsonSerializer>));
            }

            [TestMethod]
            public void WhenComplexProcessorThenAddNeutralProcessorReturnsDefaultAtTheEnd()
            {
                // Arrange
                var processor = new ProcessorStructure<string, IJsonSerializer>(
                    new ExampleRecursiveProcessor().AddProcessors(
                        new ExampleSimpleProcessor()
                        ),
                    new ExampleSimpleProcessor()
                    );

                // Act
                ProcessorUtilities.AddNeutralProcessorAtEndsForStructure(processor);

                // Assert
                Assert.AreEqual(3, processor.Count);
                Assert.IsInstanceOfType(processor[0], typeof(ExampleRecursiveProcessor));
                Assert.IsInstanceOfType(processor[1], typeof(ExampleSimpleProcessor));
                Assert.IsInstanceOfType(processor[2], typeof(SuccessProcessor<string, IJsonSerializer>));

                var recNode = (ExampleRecursiveProcessor)processor[0];

                Assert.AreEqual(2, recNode.ProcessorStructure.Count);
                Assert.IsInstanceOfType(recNode.ProcessorStructure[0], typeof(ExampleSimpleProcessor));
                Assert.IsInstanceOfType(recNode.ProcessorStructure[1], typeof(SuccessProcessor<string, IJsonSerializer>));
            }
        }

        [TestClass]
        public class CommonProcessorStructureTests
        {
            [TestMethod]
            public void When200ThenReturnsData()
            {
                // Arrange
                IRestResponse response = new RestResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    ContentType = "application/json",
                    Content = "\"Hello\""
                };
                var jsonConverter = new JsonSerializer();
                var processor = new ProcessorStructure<string, IJsonSerializer>(
                    new RestExceptionProcessor<string>().Default());
                ProcessorUtilities.AddNeutralProcessorAtEndsForStructure(processor);
                ProcessorUtilities.SetErrorSerializerForStructure(processor, jsonConverter);

                // Act
                Assert.IsTrue(processor.CanProcess(response));
                var res = processor.Process(response, jsonConverter);

                // Assert
                Assert.AreEqual("Hello", res);
            }

            [TestMethod]
            [ExpectedException(typeof(RestException))]
            public void When500ThenThrowsException()
            {
                // Arrange
                IRestResponse response = new RestResponse()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ContentType = "application/json",
                    Content = "\"Error\""
                };
                var jsonConverter = new JsonSerializer();
                var processor = new ProcessorStructure<string, IJsonSerializer>(
                    new RestExceptionProcessor<string>().Default());
                ProcessorUtilities.AddNeutralProcessorAtEndsForStructure(processor);
                ProcessorUtilities.SetErrorSerializerForStructure(processor, jsonConverter);

                // Act
                Assert.IsTrue(processor.CanProcess(response));
                processor.Process(response, jsonConverter);
            }
        }
    }

    public class ExampleSimpleProcessor : ISimpleProcessorNode<string, IJsonSerializer>
    {
        public bool CanProcess(IRestResponse response)
        {
            return response.StatusCode == HttpStatusCode.InternalServerError;
        }

        public string Process(IRestResponse response, IJsonSerializer serializer)
        {
            var restResult = serializer.DeserializeObject<int>(response.Content);
            return restResult.ToString(CultureInfo.InvariantCulture);
        }
    }

    public class ExampleSimpleProcessorAlternate : ISimpleProcessorNode<string, IJsonSerializer>
    {
        public bool CanProcess(IRestResponse response)
        {
            return response.StatusCode == HttpStatusCode.InternalServerError;
        }

        public string Process(IRestResponse response, IJsonSerializer serializer)
        {
            var restResult = serializer.DeserializeObject<int>(response.Content);
            return (restResult + 10).ToString(CultureInfo.InvariantCulture);
        }
    }

    public class ExampleRecursiveProcessor : RecursiveProcessorNode<string, string, IJsonSerializer>
    {
        protected override bool CanProcessSub(IRestResponse response)
        {
            return true;
        }

        protected override string ProcessSub(IRestResponse response, IJsonSerializer serializer)
        {
            return ProcessorStructure.Process(response, serializer);
        }
    }

    public class ExampleRecursiveProcessorAlternate<TResult> : RecursiveProcessorNode<TResult, TResult, IJsonSerializer>
    {
        protected override bool CanProcessSub(IRestResponse response)
        {
            return ProcessorStructure.CanProcess(response);
        }

        protected override TResult ProcessSub(IRestResponse response, IJsonSerializer serializer)
        {
            return ProcessorStructure.Process(response, serializer);
        }
    }
}