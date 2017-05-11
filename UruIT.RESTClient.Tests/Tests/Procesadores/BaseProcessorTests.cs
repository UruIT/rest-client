using Microsoft.VisualStudio.TestTools.UnitTesting;
using UruIT.Serialization.Core;
using UruIT.RESTClient.Clases;
using UruIT.RESTClient.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UruIT.RESTClient.Tests.Tests
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
	public class BaseProcessorTests : BaseProcessorTests<IJsonConverter>
	{
		protected readonly IJsonConverter jsonConverter = new JsonMONConverter();
	}
}
