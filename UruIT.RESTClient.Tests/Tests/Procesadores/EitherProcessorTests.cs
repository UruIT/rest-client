using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monad;
using Movistar.Online.Common.Monad;
using UruIT.Serialization.Core;
using Movistar.Online.Common.Types.Resultados;
using Movistar.Online.Common.Types.ResultadosRest;
using UruIT.RESTClient.Clases;
using UruIT.RESTClient.Clases.Procesadores;
using UruIT.RESTClient.Interfaces;
using System.Net;

namespace UruIT.RESTClient.Tests.Tests
{
	[TestClass]
	public class EitherProcessorTests
	{
		protected IJsonConverter jsonConverter;

		public EitherProcessorTests()
		{
			jsonConverter = new JsonMONConverter();
		}

		[TestClass]
		public class BasicEitherResultadoProcessorTests : EitherProcessorTests
		{
			[TestMethod]
			public void WhenOkThenRetornarRight()
			{
				// Arrange
				IRestResponse response = new RestResponse()
				{
					StatusCode = HttpStatusCode.OK,
					ContentType = "application/json",
					Content = "10"
				};
				var processor = new ProcessorStructure<EitherStrict<RestBusinessError, int>, IJsonConverter>(
					new EitherResultadoProcessor<int>().Default().AddProcessors(
						new SuccessProcessor<int>().Default()));
				ProcessorUtilities.SetErrorSerializerForStructure(processor, jsonConverter);

				// Act
				Assert.IsTrue(processor.CanProcess(response));
				var res = processor.Process(response, jsonConverter);

				// Assert
				Assert.IsTrue(res.IsRight);
				Assert.AreEqual(10, res.Right);
			}

			[TestMethod]
			public void WhenErrorNoRestThenRetornarLeft()
			{
				// Arrange
				IRestResponse response = new RestResponse()
				{
					StatusCode = HttpStatusCode.InternalServerError,
					ContentType = "application/json",
					Content = "ErrorGenerico"
				};

				var processor = new ProcessorStructure<EitherStrict<RestBusinessError, int>, IJsonConverter>(
						new EitherResultadoProcessor<int>().Default()
							.AddProcessors(new SuccessProcessor<int>().Default())
					);
				ProcessorUtilities.SetErrorSerializerForStructure(processor, jsonConverter);

				// Act
				Assert.IsTrue(processor.CanProcess(response));
				var res = processor.Process(response, jsonConverter);

				// Assert
				Assert.IsTrue(res.IsLeft);
				Assert.AreEqual(RestErrorType.InternalError, res.Left.Resultado);
				Assert.AreEqual("ErrorGenerico", res.Left.Detalle);
			}

			[TestMethod]
			public void WhenErrorRestThenRetornarLeft()
			{
				// Arrange
				IRestResponse response = new RestResponse()
				{
					StatusCode = HttpStatusCode.InternalServerError,
					ContentType = "application/json",
					Content = "{'StatusCode': 400, 'Message': 'Mensaje', 'Details': 'Detalles'}"
				};

				var processor = new ProcessorStructure<EitherStrict<RestBusinessError, int>, IJsonConverter>(
						new EitherResultadoProcessor<int>().Default()
							.AddProcessors(new SuccessProcessor<int>().Default())
					);
				ProcessorUtilities.SetErrorSerializerForStructure(processor, jsonConverter);

				// Act
				Assert.IsTrue(processor.CanProcess(response));
				var res = processor.Process(response, jsonConverter);

				// Assert
				Assert.IsTrue(res.IsLeft);
				Assert.AreEqual(RestErrorType.ValidationError, res.Left.Resultado);
				Assert.AreEqual("Mensaje", res.Left.Mensaje);
				Assert.AreEqual("Detalles", res.Left.Detalle);
			}
		}

		[TestClass]
		public class EitherResultadoWithOptionProcessorTests : EitherProcessorTests
		{
			[TestMethod]
			public void WhenOkThenRetornarRight()
			{
				// Arrange
				IRestResponse response = new RestResponse()
				{
					StatusCode = HttpStatusCode.OK,
					ContentType = "application/json",
					Content = "10"
				};
                var processor = new ProcessorStructure<EitherStrict<RestBusinessError, OptionStrict<int>>, IJsonConverter>(
						new EitherResultadoProcessor<OptionStrict<int>>().Default()
							.AddProcessors(new OptionAsNotFoundProcessor<int>()
								.AddProcessors(new SuccessProcessor<int>().Default()))
					);
				ProcessorUtilities.SetErrorSerializerForStructure(processor, jsonConverter);

				// Act
				Assert.IsTrue(processor.CanProcess(response));
				var res = processor.Process(response, jsonConverter);

				// Assert
				Assert.IsTrue(res.IsRight);
				Assert.IsTrue(res.Right.HasValue);
				Assert.AreEqual(10, res.Right.Value);
			}

			[TestMethod]
			public void WhenNotFoundThenRetornarNothing()
			{
				// Arrange
				IRestResponse response = new RestResponse()
				{
					StatusCode = HttpStatusCode.NotFound,
					ContentType = "application/json"
				};
                var processor = new ProcessorStructure<EitherStrict<RestBusinessError, OptionStrict<int>>, IJsonConverter>(
                        new EitherResultadoProcessor<OptionStrict<int>>().Default()
							.AddProcessors(new OptionAsNotFoundProcessor<int>()
								.AddProcessors(new SuccessProcessor<int>().Default()))
					);
				ProcessorUtilities.SetErrorSerializerForStructure(processor, jsonConverter);

				// Act
				Assert.IsTrue(processor.CanProcess(response));
				var res = processor.Process(response, jsonConverter);

				// Assert
				Assert.IsTrue(res.IsRight);
				Assert.IsFalse(res.Right.HasValue);
			}

			[TestMethod]
			public void WhenErrorThenRetornarLeft()
			{
				// Arrange
				IRestResponse response = new RestResponse()
				{
					StatusCode = HttpStatusCode.InternalServerError,
					ContentType = "application/json",
					Content = "{'StatusCode': 400, 'Message': 'Mensaje', 'Details': 'Detalles'}"
				};
                var processor = new ProcessorStructure<EitherStrict<RestBusinessError, OptionStrict<int>>, IJsonConverter>(
                        new EitherResultadoProcessor<OptionStrict<int>>().Default()
							.AddProcessors(new OptionAsNotFoundProcessor<int>()
								.AddProcessors(new SuccessProcessor<int>().Default()))
					);
				ProcessorUtilities.SetErrorSerializerForStructure(processor, jsonConverter);

				// Act
				Assert.IsTrue(processor.CanProcess(response));
				var res = processor.Process(response, jsonConverter);

				// Assert
				Assert.IsTrue(res.IsLeft);
			}
		}
	}
}