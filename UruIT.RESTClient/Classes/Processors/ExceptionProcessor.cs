using UruIT.Serialization.Core;
using UruIT.RESTClient.Interfaces;
using UruIT.RESTClient.Interfaces.Procesadores;
using UruIT.RESTClient.Interfaces.Proveedores;
using System;

namespace UruIT.RESTClient.Clases.Procesadores
{
	/// <summary>
	/// Procesador que lanza una excepción con la información procesada convertida en excepción
	/// </summary>
	/// <typeparam name="TResult">Tipo utilizado en el pipeline</typeparam>
	/// <typeparam name="TError">Tipo del error retornado</typeparam>
	/// <typeparam name="TException">Tipo de la excepción a lanzar</typeparam>
	/// <typeparam name="TSerializer">Tipo del serializador</typeparam>
	public class ExceptionProcessor<TResult, TError, TException, TSerializer>
		: RecursiveProcessorNode<TResult, TError, TSerializer>
		where TException : Exception
		where TSerializer : ISerializer
	{
		private readonly IExceptionProvider<TError, TException> exceptionProvider;

		public ExceptionProcessor(IExceptionProvider<TError, TException> exceptionProvider)
		{
			this.exceptionProvider = exceptionProvider;
		}

		protected override bool CanProcessSub(IRestResponse response)
		{
			return ProcessorStructure.CanProcess(response);
		}

		protected override TResult ProcessSub(IRestResponse response, TSerializer serializer)
		{
			var error = ProcessorStructure.Process(response, serializer);
			throw exceptionProvider.ProvideException(error);
		}
	}
}