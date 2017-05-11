using Monad;
using UruIT.Serialization.Core;
using UruIT.RESTClient.Interfaces;
using UruIT.RESTClient.Interfaces.Procesadores;

namespace UruIT.RESTClient.Clases.Procesadores
{
	/// <summary>
	/// Procesador que permite tener un procesador izquierda
	/// Si puede procesar con el recursivo (derecha) lo hace y sino trata con el procesador izquierda
	/// </summary>
	/// <typeparam name="TLeft">Tipo izquierda, resultante del procesador izquierda</typeparam>
	/// <typeparam name="TRight">Tipo derecha, resultante del procesador derecha</typeparam>
	/// <typeparam name="TSerializer">Tipo del serializador</typeparam>
	public class EitherProcessor<TLeft, TRight, TSerializer>
		: RecursiveProcessorNode<EitherStrict<TLeft, TRight>, TRight, TSerializer>, IErrorProcessor<TSerializer>
		where TSerializer : ISerializer
	{
		public TSerializer ErrorSerializer
		{
			set { ProcessorUtilities.SetErrorSerializerForStructure(leftProcessorStructure, value); }
		}

		private IProcessorStructure<TLeft, TSerializer> leftProcessorStructure;

		public EitherProcessor()
		{
			leftProcessorStructure = new ProcessorStructure<TLeft, TSerializer>();
		}

		public EitherProcessor(IProcessorStructure<TRight, TSerializer> processorStructure)
			: base(processorStructure)
		{
			leftProcessorStructure = new ProcessorStructure<TLeft, TSerializer>();
		}

		/// <summary>
		/// Permite agregar nodos de procesamiento en el árbol izquierda
		/// </summary>
		/// <param name="processorNodes">Nodos de procesamiento</param>
		/// <returns></returns>
		public EitherProcessor<TLeft, TRight, TSerializer> AddLeftProcessors(params IProcessorNode<TLeft, TSerializer>[] processorNodes)
		{
			foreach (var proc in processorNodes)
			{
				leftProcessorStructure.Add(proc);
			}
			return this;
		}

		protected override bool CanProcessSub(IRestResponse response)
		{
			//Procesa si el procesador derecha o el procesador izquierda puede procesar
			return ProcessorStructure.CanProcess(response) || leftProcessorStructure.CanProcess(response);
		}

        protected override EitherStrict<TLeft, TRight> ProcessSub(IRestResponse response, TSerializer serializer)
		{
			//Procesa si procesador derecha puede procesar, sino lo delega al procesador izquierda
			if (ProcessorStructure.CanProcess(response))
			{
				return ProcessorStructure.Process(response, serializer);
			}
			else
			{
				return leftProcessorStructure.Process(response, serializer);
			}
		}
	}
}
