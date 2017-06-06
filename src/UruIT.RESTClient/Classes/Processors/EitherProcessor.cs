using Monad;
using UruIT.Serialization;

namespace UruIT.RESTClient.Processors
{
    /// <summary>
    /// Processor that processes eithers. It contains a serializer for errors or cases that are "Left". The successful cases are handled in the "Right" case.
    /// </summary>
    /// <typeparam name="TLeft">Type of the left result</typeparam>
    /// <typeparam name="TRight">Type of the right result</typeparam>
    /// <typeparam name="TSerializer">Type of the serializer</typeparam>
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
        /// Allows adding processing nodes to handle the Left case.
        /// </summary>
        /// <param name="processorNodes">Processor nodes</param>
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
            //It can process it only if either the Left or Right processors can
            return ProcessorStructure.CanProcess(response) || leftProcessorStructure.CanProcess(response);
        }

        protected override EitherStrict<TLeft, TRight> ProcessSub(IRestResponse response, TSerializer serializer)
        {
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