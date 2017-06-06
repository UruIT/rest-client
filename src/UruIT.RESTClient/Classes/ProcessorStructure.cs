using System;
using System.Collections.Generic;
using System.Linq;
using UruIT.Serialization;

namespace UruIT.RESTClient
{
    /// <summary>
    /// Represents de processing structure, using an internal list of nodes to maintain them.
    /// </summary>
    /// <typeparam name="TResult">Type of the result of the final processing</typeparam>
    /// <typeparam name="TSerializer">Type of the serializer</typeparam>
    public sealed class ProcessorStructure<TResult, TSerializer> : IProcessorStructure<TResult, TSerializer>
        where TSerializer : ISerializer
    {
        //Internal list of nodes
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
            //It can process the response if any node can process it
            return ProcessorList.Any(p => p.CanProcess(response));
        }

        public TResult Process(IRestResponse response, TSerializer serializer)
        {
            //Takes the first processor from the list that can process it, and gives it the response
            if (!ProcessorList.Any(p => p.CanProcess(response)))
                throw new InvalidOperationException("There is no processor that can process this response");

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