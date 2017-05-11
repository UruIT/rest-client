using UruIT.Serialization;
using System.Collections.Generic;

namespace UruIT.RESTClient
{
	/// <summary>
    /// In charge of making validations and conversions of an HTTP response in the pipeline of the REST clien.
    /// It represents a processor or a chain/structure of processors that can check if it can process an HTTP response, and if it can then it processes it, returning a value.
    /// 
    /// This type IResponseProcessor&lt;T&gt; only acts as an abstraction of something that can process the response. The actual structure of processors is given by IProcessorStructure&lt;T&gt;
	/// </summary>
	/// <typeparam name="TResult">Type of result of the final processing</typeparam>
	/// <typeparam name="TSerializer">Type of the serializer of the body of the response</typeparam>
	public interface IResponseProcessor<TResult, TSerializer>
		where TSerializer : ISerializer
	{
		/// <summary>
        /// Verifies if it can process the response.
		/// </summary>
		/// <param name="response">HTTP response</param>
		bool CanProcess(IRestResponse response);

		/// <summary>
        /// Processes a rseponse.It returns the final value of the processing, deserializing the internal value of the response if necessary.
		/// </summary>
		/// <param name="response">HTTP response</param>
		/// <param name="serializer">Serializer of the body</param>
		/// <returns>Final result</returns>
		TResult Process(IRestResponse response, TSerializer serializer);
	}

	/// <summary>
    /// Represents the processing structure. Said structure is the following:
    /// 
    /// The structure maintains a sequence of processing nodes that are capable of completely processing a response. This sequence is essentially a list of IProcessorNodes. This list is ordered and represents alternative processors, where only one of them executes. Basically, if the 1st node of the list can process the response, then it processes it; if it can't, then it asks the 2nd node if it can process it, and so on until it arrives at a node that can process it or arrives at the end of the list.
    /// 
    /// Each processing node can be simple or recursive. A simple node only takes the response, processes it and returns the type  &lt;T&gt;. A recursive node is composed of an inner processing structure that returns another type &gt;R&lt;, and proceses this response to return the type &lt;T&gt;.
    /// 
    /// Thus, there are 2 ways to compose processors:
    ///     1. They are composed side by side, where if the 1st one can process the response it does, and if it can't then the 2nd one processes it.
    ///     2. They are composed in a nested way, where the 1st processor can use the 2nd one to verify if it can process the response, as well as it can use the 2nd one to partially process the response.
    ///     
    /// The notation could be the following:
    ///     "X -> Y": The processing node X is composed with the processing structure Y in parallel.
    ///     "X * Y": The recursive processing node X is nested with the processing structure Y.
    ///     
    /// E.j: (P1 -> (R1 * P2) -> P3 -> (R2 * (P4 -> P5))) -> P6)
    /// 
    /// An actual processing structure could be the following:
    ///     (OptionAsNotFoundProcessor * UnitAsSuccessProcessor) -> ExceptionProcessor -> DefaultProcessor
	/// </summary>
    /// <typeparam name="TResult">Type of result of the final processing</typeparam>
    /// <typeparam name="TSerializer">Type of the serializer of the body of the response</typeparam>
	public interface IProcessorStructure<TResult, TSerializer> :
		IResponseProcessor<TResult, TSerializer>, IList<IProcessorNode<TResult, TSerializer>>
		where TSerializer : ISerializer
	{
	}

	/// <summary>
    /// Represents a processing node that can be either simple or recursive.
	/// </summary>
    /// <typeparam name="TResult">Type of result of the final processing</typeparam>
    /// <typeparam name="TSerializer">Type of the serializer of the body of the response</typeparam>
	public interface IProcessorNode<TResult, TSerializer> : IResponseProcessor<TResult, TSerializer>
		where TSerializer : ISerializer
	{
	}

	/// <summary>
    /// Represents a simple processing node. It only takes a response and processes it itself, returning the corresponding value.
	/// </summary>
    /// <typeparam name="TResult">Type of result of the final processing</typeparam>
    /// <typeparam name="TSerializer">Type of the serializer of the body of the response</typeparam>
	public interface ISimpleProcessorNode<TResult, TSerializer> : IProcessorNode<TResult, TSerializer>
		where TSerializer : ISerializer
	{
	}

	/// <summary>
    /// Represents a recursive processing node. It allows composing itself with other processing nodes. In such cases, this recursive node only partially processes the response, and only partially verifies if it can process it. The node delegates to the rest of the nodes the rest of the verification and processing.
	/// </summary>
    /// <typeparam name="TResult">Type of result of the final processing</typeparam>
	/// <typeparam name="TRecursive">Type of the partial value resulting from the processing of the composed processing nodes.</typeparam>
    /// <typeparam name="TSerializer">Type of the serializer of the body of the response</typeparam>
	public abstract class RecursiveProcessorNode<TResult, TRecursive, TSerializer> : IProcessorNode<TResult, TSerializer>
		where TSerializer : ISerializer
	{
        /// <summary>
        /// Children processor structure.
        /// </summary>
		public IProcessorStructure<TRecursive, TSerializer> ProcessorStructure { get; set; }

        /// <summary>
        /// Creates a recursive processor with empty child processor nodes.
        /// </summary>
		protected RecursiveProcessorNode()
		{
			this.ProcessorStructure = new UruIT.RESTClient.ProcessorStructure<TRecursive, TSerializer>();
		}

        /// <summary>
        /// Creates a recursive processor with an already existing set of child processors
        /// </summary>
        /// <param name="processorStructure"></param>
		protected RecursiveProcessorNode(IProcessorStructure<TRecursive, TSerializer> processorStructure)
		{
			this.ProcessorStructure = processorStructure;
		}

		/// <summary>
        /// Adds a list of processing node to its children.
		/// </summary>
		/// <param name="processorNodes">List of processing nodes</param>
		public RecursiveProcessorNode<TResult, TRecursive, TSerializer> AddProcessors(params IProcessorNode<TRecursive, TSerializer>[] processorNodes)
		{
			foreach(var proc in processorNodes)
			{
				this.ProcessorStructure.Add(proc);
			}
			return this;
		}

		public bool CanProcess(IRestResponse response)
		{
			return CanProcessSub(response);
		}

		public TResult Process(IRestResponse response, TSerializer serializer)
		{
			return ProcessSub(response, serializer);
		}

		/// <summary>
        /// Abstract method that delegates the verification to the subclass.
		/// </summary>
		protected abstract bool CanProcessSub(IRestResponse response);

		/// <summary>
        /// Abstract method that delegates the processing to the subclass.
		/// </summary>
		protected abstract TResult ProcessSub(IRestResponse response, TSerializer serializer);
	}
}