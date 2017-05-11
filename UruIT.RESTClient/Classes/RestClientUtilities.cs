using UruIT.Serialization.Core;
using UruIT.RESTClient.Clases.Procesadores;
using UruIT.RESTClient.Interfaces;
using UruIT.RESTClient.Interfaces.Procesadores;
using System;
using System.Reflection;
using System.Linq;

namespace UruIT.RESTClient.Clases
{
	/// <summary>
	/// Funciones útiles de procesadores
	/// </summary>
	public static class ProcessorUtilities
	{
		/// <summary>
		/// Toma un nodo de procesamiento y determina si extiende de nodos recursivos.
		/// Si se cumple, traslada la llamada al callback definido con la estructura que el nodo recursivo contiene
		/// </summary>
		/// <typeparam name="TResult">Tipo que retorna el procesador</typeparam>
		/// <typeparam name="TSerializer">Tipo del serializador</typeparam>
		/// <param name="processor">Procesador</param>
		/// <param name="callbackName">Nombre del método en esta clase, debe ser el mismo que lo llama</param>
		public static void ProcessNode<TResult, TSerializer>(
			IProcessorNode<TResult, TSerializer> processor, string callbackName, params object[] args)
			where TSerializer : ISerializer
		{
			if (processor is ISimpleProcessorNode<TResult, TSerializer>)
			{
				//No se hace nada, es un procesador simple que no depende de la estructura
				return;
			}
			else
			{
				//Si no es un procesador simple, hay que verificar que sea del tipo RecursiveProcessorNode
				if (!HeredaDeNodoRecursivo(processor.GetType()))
					throw new ArgumentException(string.Format("El tipo '{0}' debe ser RecursiveProcessorNode o ISimpleProcessor", processor.GetType().Name), "processor");

				//Se castea a ese tipo de nodo recursivo
				var recType = ObtenerSuperclase(processor.GetType(), EsNodoRecursivo);

				//Se obtienen el resto de la lista de procesadores
				var rest = recType.GetProperty("ProcessorStructure").GetValue(processor, null);

				//Tipo <R> genérico
				Type r1Type = recType.GetGenericArguments()[1];
				Type r2Type = recType.GetGenericArguments()[2];

				//Aplico la recursión sobre la lista de procesadores interna
				var methodArgs = new[] { rest }.Concat(args).ToArray();
				typeof(ProcessorUtilities)
					.GetMethod(callbackName, BindingFlags.Public | BindingFlags.Static)
					.MakeGenericMethod(new[] { r1Type, r2Type })
					.Invoke(null, methodArgs);
			}
		}

		/// <summary>
		/// Retorna la primer superclase que cumple la condición del callback
		/// </summary>
		private static Type ObtenerSuperclase(Type type, Func<Type, bool> callback)
		{
			Type superType = type;
			while (superType != typeof(Object))
			{
				if (callback(superType))
					return superType;

				superType = superType.BaseType;
			}

			return null;
		}

		/// <summary>
		/// Permite saber si un tipo el nodo recursivo de procesamiento
		/// </summary>
		private static bool EsNodoRecursivo(Type type)
		{
			if (!type.IsGenericType)
				return false;

			var gtd = type.GetGenericTypeDefinition();
			return gtd == typeof(RecursiveProcessorNode<,,>);
		}

		/// <summary>
		/// Retorna true si el tipo hereda del nodo recursivo de procesamiento
		/// </summary>
		private static bool HeredaDeNodoRecursivo(Type type)
		{
			return ObtenerSuperclase(type, EsNodoRecursivo) != null;
		}

		private static bool EsNodoExceptionProcessor(Type type)
		{
			if (!type.IsGenericType)
				return false;

			var gtd = type.GetGenericTypeDefinition();
			return gtd == typeof(ExceptionProcessor<,,,>);
		}

		/// <summary>
		/// Retorna true si el tipo hereda del nodo ExceptionProcessor
		/// </summary>
		private static bool HeredaDeExceptionProcessor(Type type)
		{
			return ObtenerSuperclase(type, EsNodoExceptionProcessor) != null;
		}

		/// <summary>
		/// Toma una estructura de procesamiento de una respuesta REST, y le agrega el procesador por defecto y el successful al final de cada "punta".
		/// Como el procesador por defecto es equivalente a la identidad cuando se componen lado a lado, se puede agregar sin problemas, y solamente se hace para que por lo menos exista un nodo que fuerce a que se procese este nodo siempre
		/// Ej:
		///		(P1 -> P2) pasa a (P1 -> P2 -> DEF)
		///		(R1 * P1) -> P2 pasa a (R1 * (P1 -> DEF)) -> P2 -> DEF
		/// </summary>
		/// <typeparam name="TResult">Tipo que retorna la estructura de procesadores</typeparam>
		/// <typeparam name="TSerializer">Tipo del serializador</typeparam>
		/// <param name="processor">Estructura de procesadores</param>
		public static void AddNeutralProcessorAtEndsForStructure<TResult, TSerializer>(
			IProcessorStructure<TResult, TSerializer> processor)
			where TSerializer : ISerializer
		{
			//Si se tiene un único proceador y es el de por defecto, no se hace nada
			if (processor.Count == 1 && processor[0] is SuccessProcessor<TResult, TSerializer>)
				return;

			//Primero le agrega el procesador por defecto a todos los nodos
			foreach (var node in processor)
			{
				if (!HeredaDeExceptionProcessor(node.GetType()))
				{
					ProcessNode<TResult, TSerializer>(node, "AddNeutralProcessorAtEndsForStructure");
				}
			}

			//Luego lo agrega al final de la estructura misma
			processor.Add(new SuccessProcessor<TResult, TSerializer>().Default());
		}

		/// <summary>
		/// Toma una estructura de procesamiento de una respuesta REST, y busca nodos que implementen IErrorProcessor
		/// A estos les setea el ErrorSerializer para que lo usen al momento de deserializar el error
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <typeparam name="TSerializer"></typeparam>
		/// <param name="processor"></param>
		/// <param name="serializer"></param>
		public static void SetErrorSerializerForStructure<TResult, TSerializer>(
			IProcessorStructure<TResult, TSerializer> processor, TSerializer serializer)
			where TSerializer : ISerializer
		{
			foreach (var node in processor)
			{
				if (node is IErrorProcessor<TSerializer>)
				{
					(node as IErrorProcessor<TSerializer>).ErrorSerializer = serializer;
				}
				ProcessNode<TResult, TSerializer>(node, "SetErrorSerializerForStructure", serializer);
			}
		}
	}
}

namespace System.Net
{
	public static class HttpCodeExtensions
	{
		/// <summary>
		/// Valida si el código HTTP representa éxito (2XX)
		/// </summary>
		public static bool IsSuccessful(this HttpStatusCode code)
		{
			return (int)code >= 200 && (int)code < 300;
		}
	}
}