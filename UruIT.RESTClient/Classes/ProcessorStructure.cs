using UruIT.Serialization.Core;
using UruIT.RESTClient.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UruIT.RESTClient.Clases
{
	/// <summary>
	/// Representa la estructura misma de procesadores, utilizando una lista de nodos interna para mantenerlos.
	///
	/// Ver doc de IProcessorStructure por más información.
	/// </summary>
	/// <typeparam name="TResult">Tipo del resultado del procesamiento final</typeparam>
	/// <typeparam name="TSerializer">Tipo del serializador</typeparam>
	public sealed class ProcessorStructure<TResult, TSerializer> : IProcessorStructure<TResult, TSerializer>
		where TSerializer : ISerializer
	{
		//Lista interna de nodos de procesamiento
		public IList<IProcessorNode<TResult, TSerializer>> ProcessorList { get; private set; }

		public ProcessorStructure()
		{
			this.ProcessorList = new List<IProcessorNode<TResult, TSerializer>>();
		}

		public ProcessorStructure(IList<IProcessorNode<TResult, TSerializer>> processorList)
		{
			this.ProcessorList = processorList;
		}

		public ProcessorStructure(params IProcessorNode<TResult, TSerializer>[] processorList)
		{
			this.ProcessorList = processorList.ToList();
		}

		public bool CanProcess(IRestResponse response)
		{
			//Puede procear solamente si algún elemento de la lista puede hacerlo
			return ProcessorList.Any(p => p.CanProcess(response));
		}

		public TResult Process(IRestResponse response, TSerializer serializer)
		{
			//Toma el primer procesador de la lista que puede procesar, y lo procesa
			if (!ProcessorList.Any(p => p.CanProcess(response)))
				throw new InvalidOperationException("No existe ningún procesador que pueda procesar esta respuesta");

			return ProcessorList.First(p => p.CanProcess(response)).Process(response, serializer);
		}

		#region IList<IProcessorNode<TResult, TSerializer>> implementation

		public IProcessorNode<TResult, TSerializer> this[int index]
		{
			get
			{
				return ProcessorList[index];
			}
			set
			{
				ProcessorList[index] = value;
			}
		}

		public int IndexOf(IProcessorNode<TResult, TSerializer> item)
		{
			return ProcessorList.IndexOf(item);
		}

		public void Insert(int index, IProcessorNode<TResult, TSerializer> item)
		{
			ProcessorList.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			ProcessorList.RemoveAt(index);
		}

		#endregion IList<IProcessorNode<TResult, TSerializer>> implementation

		#region ICollection<IProcessorNode<TResult, TSerializer>> implementation

		public int Count { get { return ProcessorList.Count; } }

		public bool IsReadOnly { get { return ProcessorList.IsReadOnly; } }

		public void Add(IProcessorNode<TResult, TSerializer> item)
		{
			ProcessorList.Add(item);
		}

		public void Clear()
		{
			ProcessorList.Clear();
		}

		public bool Contains(IProcessorNode<TResult, TSerializer> item)
		{
			return ProcessorList.Contains(item);
		}

		public void CopyTo(IProcessorNode<TResult, TSerializer>[] array, int arrayIndex)
		{
			ProcessorList.CopyTo(array, arrayIndex);
		}

		public bool Remove(IProcessorNode<TResult, TSerializer> item)
		{
			return ProcessorList.Remove(item);
		}

		#endregion ICollection<IProcessorNode<TResult, TSerializer>> implementation

		#region IEnumerable<IProcessorNode<TResult, TSerializer>> implementation

		public IEnumerator<IProcessorNode<TResult, TSerializer>> GetEnumerator()
		{
			return ProcessorList.GetEnumerator();
		}

		#endregion IEnumerable<IProcessorNode<TResult, TSerializer>> implementation

		#region IEnumerable implementation

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return ProcessorList.GetEnumerator();
		}

		#endregion IEnumerable implementation
	}
}