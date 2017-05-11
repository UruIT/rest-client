using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monad;
using UruIT.Serialization.Core;
using UruIT.RESTClient.Clases;
using UruIT.RESTClient.Clases.Procesadores;
using UruIT.RESTClient.Interfaces;

namespace UruIT.RESTClient.Tests.Tests
{
	[TestClass]
	public class ErrorProcessorTests : BaseProcessorTests
	{
        protected IProcessorStructure<OptionStrict<T>, IJsonConverter> CreateProcessorStructure<T>()
		{
            return base.CreateProcessorStructure(new ErrorProcessor<OptionStrict<T>, IJsonConverter>().Default());
		}

		[TestClass]
		public class CanProcess : ErrorProcessorTests
		{
			class Error
			{
				public string Detalle { get; set; }
			}

			[TestMethod]
			public void WhenSuccessThenFalse()
			{
				// arrange
				var response = new RestResponse
				{
					StatusCode = System.Net.HttpStatusCode.OK,
					ContentType = "application/json",
					Content = "{ 'Detalle':'detalle del error' }",
				};
				var processor = CreateProcessorStructure<Error>();
				
				// act
				var resultado = processor.CanProcess(response);

				// assert
				Assert.AreEqual(false, resultado);
			}

			[TestMethod]
			public void WhenErrorThenTrue()
			{
				// arrange
				var response = new RestResponse
				{
					StatusCode = System.Net.HttpStatusCode.InternalServerError,
					ContentType = "application/json",
					Content = "{ 'Detalle':'detalle del error' }",
				};
				var processor = CreateProcessorStructure<Error>();

				// act
				var resultado = processor.CanProcess(response);

				// assert
				Assert.AreEqual(true, resultado);
			}
		}

		[TestClass]
		public class Process : ErrorProcessorTests
		{
			class Error
			{
				public string Detalle { get; set; }
			}

			[TestMethod]
			public void WhenErrorNoCorrespondeThenNothing()
			{
				// arrange
				var response = new RestResponse
				{
					Content = "{ 'Error':'detalle del error' }"
				};
				var processor = CreateProcessorStructure<Error>();
				ProcessorUtilities.SetErrorSerializerForStructure(processor, jsonConverter);

				// act
				var resultado = processor.Process(response, jsonConverter);

				// assert
                Assert.AreEqual(OptionStrict<Error>.Nothing, resultado);
			}

			[TestMethod]
			public void WhenErrorCorrespondeThenErrorParseado()
			{
				// arrange
				var response = new RestResponse
				{
					Content = "{ 'Detalle':'detalle del error' }"
				};
				var processor = CreateProcessorStructure<Error>();
				ProcessorUtilities.SetErrorSerializerForStructure(processor, jsonConverter);

				// act
				var resultado = processor.Process(response, jsonConverter);

				// assert
				Assert.AreEqual("detalle del error", resultado.Value.Detalle);
			}
		}
	}
}
