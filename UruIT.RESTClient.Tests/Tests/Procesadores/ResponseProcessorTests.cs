using Microsoft.VisualStudio.TestTools.UnitTesting;
using UruIT.Serialization.Core;
using Movistar.Online.Common.Types.ResultadosRest;
using UruIT.RESTClient.Clases;
using UruIT.RESTClient.Clases.Procesadores;
using UruIT.RESTClient.Interfaces;
using System;
using System.Globalization;
using System.Net;

namespace UruIT.RESTClient.Tests.Tests
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
			public void WhenDosProcesadoresPuedenProcesarThenProcessProcesaElPrimeroSolo()
			{
				// Arrange
				IRestResponse response = new RestResponse()
				{
					StatusCode = HttpStatusCode.InternalServerError,
					ContentType = "application/json",
					Content = "10"
				};
				var jsonConverter = new JsonMONConverter();
				var processor = new ProcessorStructure<string, IJsonConverter>(
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
			public void WhenEmptyProcessorThenAddNeutralProcessorRetornaSoloDefault()
			{
				// Arrange
				var processor = new ProcessorStructure<string, IJsonConverter>();

				// Act
				ProcessorUtilities.AddNeutralProcessorAtEndsForStructure(processor);

				// Assert
				Assert.AreEqual(1, processor.Count);
				Assert.IsInstanceOfType(processor[0], typeof(SuccessProcessor<string, IJsonConverter>));
			}

			[TestMethod]
			public void WhenUnProcessorThenAddNeutralProcessorRetornaDefaultAlFinal()
			{
				// Arrange
				var processor = new ProcessorStructure<string, IJsonConverter>(new ExampleSimpleProcessor());

				// Act
				ProcessorUtilities.AddNeutralProcessorAtEndsForStructure(processor);

				// Assert
				Assert.AreEqual(2, processor.Count);
				Assert.IsInstanceOfType(processor[0], typeof(ExampleSimpleProcessor));
				Assert.IsInstanceOfType(processor[1], typeof(SuccessProcessor<string, IJsonConverter>));
			}

			[TestMethod]
			public void WhenUnProcesadorEsDefaultConSuccessThenAddNeutralProcessorRetornaMismo()
			{
				// Arrange
				var processor = new ProcessorStructure<string, IJsonConverter>(new SuccessProcessor<string>());

				// Act
				ProcessorUtilities.AddNeutralProcessorAtEndsForStructure(processor);

				// Assert
				Assert.AreEqual(1, processor.Count);
				Assert.IsInstanceOfType(processor[0], typeof(SuccessProcessor<string, IJsonConverter>));
			}

			[TestMethod]
			public void WhenProcesadorRecursivoThenAddNeutralProcessorRetornaDefaultAlFinal()
			{
				// Arrange
				var processor = new ProcessorStructure<string, IJsonConverter>(
					new ExampleRecursiveProcessor().AddProcessors(
						new ExampleSimpleProcessor()
						)
					);

				// Act
				ProcessorUtilities.AddNeutralProcessorAtEndsForStructure(processor);

				// Assert
				Assert.AreEqual(2, processor.Count);
				Assert.IsInstanceOfType(processor[0], typeof(ExampleRecursiveProcessor));
				Assert.IsInstanceOfType(processor[1], typeof(SuccessProcessor<string, IJsonConverter>));

				var recNode = (ExampleRecursiveProcessor)processor[0];

				Assert.AreEqual(2, recNode.ProcessorStructure.Count);
				Assert.IsInstanceOfType(recNode.ProcessorStructure[0], typeof(ExampleSimpleProcessor));
				Assert.IsInstanceOfType(recNode.ProcessorStructure[1], typeof(SuccessProcessor<string, IJsonConverter>));
			}

			[TestMethod]
			public void WhenProcesadorRecursivoTieneDefaultThenAddNeutralProcessorRetornaMismo()
			{
				// Arrange
				var processor = new ProcessorStructure<string, IJsonConverter>(
					new ExampleRecursiveProcessor().AddProcessors(
						new SuccessProcessor<string>().Default()
						)
					);

				// Act
				ProcessorUtilities.AddNeutralProcessorAtEndsForStructure(processor);

				// Assert
				Assert.AreEqual(2, processor.Count);
				Assert.IsInstanceOfType(processor[0], typeof(ExampleRecursiveProcessor));
				Assert.IsInstanceOfType(processor[1], typeof(SuccessProcessor<string, IJsonConverter>));

				var recNode = (ExampleRecursiveProcessor)processor[0];

				Assert.AreEqual(1, recNode.ProcessorStructure.Count);
				Assert.IsInstanceOfType(recNode.ProcessorStructure[0], typeof(SuccessProcessor<string, IJsonConverter>));
			}

			[TestMethod]
			public void WhenProcesadorRecursivoConParametrosGenericosThenAddNeutralProcessorRetornaMismo()
			{
				// Arrange
				var processor = new ProcessorStructure<string, IJsonConverter>(
					new ExampleRecursiveProcessorAlternate<string>().AddProcessors(
						new ExampleSimpleProcessor()
						)
					);

				// Act
				ProcessorUtilities.AddNeutralProcessorAtEndsForStructure(processor);

				// Assert
				Assert.AreEqual(2, processor.Count);
				Assert.IsInstanceOfType(processor[0], typeof(ExampleRecursiveProcessorAlternate<string>));
				Assert.IsInstanceOfType(processor[1], typeof(SuccessProcessor<string, IJsonConverter>));

				var recNode = (ExampleRecursiveProcessorAlternate<string>)processor[0];

				Assert.AreEqual(2, recNode.ProcessorStructure.Count);
				Assert.IsInstanceOfType(recNode.ProcessorStructure[0], typeof(ExampleSimpleProcessor));
				Assert.IsInstanceOfType(recNode.ProcessorStructure[1], typeof(SuccessProcessor<string, IJsonConverter>));
			}

			[TestMethod]
			public void WhenProcesadorComplejoThenAddNeutralProcessorRetornaDefaultAlFinal()
			{
				// Arrange
				var processor = new ProcessorStructure<string, IJsonConverter>(
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
				Assert.IsInstanceOfType(processor[2], typeof(SuccessProcessor<string, IJsonConverter>));

				var recNode = (ExampleRecursiveProcessor)processor[0];

				Assert.AreEqual(2, recNode.ProcessorStructure.Count);
				Assert.IsInstanceOfType(recNode.ProcessorStructure[0], typeof(ExampleSimpleProcessor));
				Assert.IsInstanceOfType(recNode.ProcessorStructure[1], typeof(SuccessProcessor<string, IJsonConverter>));
			}
		}

		/// <summary>
		/// Tests de la estructura más común utilizada en los RESTClient
		/// </summary>
		[TestClass]
		public class CommonProcessorStructureTests
		{
			[TestMethod]
			public void WhenRespuesta200ThenReturnDatos()
			{
				// Arrange
				IRestResponse response = new RestResponse()
				{
					StatusCode = HttpStatusCode.OK,
					ContentType = "application/json",
					Content = "\"Hello\""
				};
				var jsonConverter = new JsonMONConverter();
				var processor = new ProcessorStructure<string, IJsonConverter>(
					new ExceptionResultadoProcessor<string>().Default());
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
			public void WhenRespuesta500ThenTiraExcepcion()
			{
				// Arrange
				IRestResponse response = new RestResponse()
				{
					StatusCode = HttpStatusCode.InternalServerError,
					ContentType = "application/json",
					Content = "\"Error\""
				};
				var jsonConverter = new JsonMONConverter();
				var processor = new ProcessorStructure<string, IJsonConverter>(
					new ExceptionResultadoProcessor<string>().Default());
				ProcessorUtilities.AddNeutralProcessorAtEndsForStructure(processor);
				ProcessorUtilities.SetErrorSerializerForStructure(processor, jsonConverter);

				// Act
				Assert.IsTrue(processor.CanProcess(response));
				processor.Process(response, jsonConverter);
			}
		}
	}

	/// <summary>
	/// Procesador de respuesta REST de ejemplo
	/// </summary>
	public class ExampleSimpleProcessor : ISimpleProcessorNode<string, IJsonConverter>
	{
		public bool CanProcess(IRestResponse response)
		{
			return response.StatusCode == HttpStatusCode.InternalServerError;
		}

		public string Process(IRestResponse response, IJsonConverter serializer)
		{
			var restResult = serializer.DeserializeObject<int>(response.Content);
			return restResult.ToString(CultureInfo.InvariantCulture);
		}
	}

	public class ExampleSimpleProcessorAlternate : ISimpleProcessorNode<string, IJsonConverter>
	{
		public bool CanProcess(IRestResponse response)
		{
			return response.StatusCode == HttpStatusCode.InternalServerError;
		}

		public string Process(IRestResponse response, IJsonConverter serializer)
		{
			var restResult = serializer.DeserializeObject<int>(response.Content);
			return (restResult + 10).ToString(CultureInfo.InvariantCulture);
		}
	}

	public class ExampleRecursiveProcessor : RecursiveProcessorNode<string, string, IJsonConverter>
	{
		protected override bool CanProcessSub(IRestResponse response)
		{
			return true;
		}

		protected override string ProcessSub(IRestResponse response, IJsonConverter serializer)
		{
			return ProcessorStructure.Process(response, serializer);
		}
	}

	public class ExampleRecursiveProcessorAlternate<TResult> : RecursiveProcessorNode<TResult, TResult, IJsonConverter>
	{
		protected override bool CanProcessSub(IRestResponse response)
		{
			return ProcessorStructure.CanProcess(response);
		}

		protected override TResult ProcessSub(IRestResponse response, IJsonConverter serializer)
		{
			return ProcessorStructure.Process(response, serializer);
		}
	}
}