using System;
using System.Linq;
using System.Reflection;
using UruIT.RESTClient.Processors;
using UruIT.Serialization;

namespace UruIT.RESTClient
{
    /// <summary>
    /// Utilities for processors
    /// </summary>
    public static class ProcessorUtilities
    {
        /// <summary>
        /// Takes a processing node and determines if it extends other recursive nodes. If it does, then it calls the callback with the recursive node.
        /// </summary>
        /// <typeparam name="TResult">Type of the result of the processor</typeparam>
        /// <typeparam name="TSerializer">Type of the serializer</typeparam>
        /// <param name="processor">Processor</param>
        /// <param name="callbackName">Name of the method in the class, must be the same that calls it</param>
        public static void ProcessNode<TResult, TSerializer>(
            IProcessorNode<TResult, TSerializer> processor, string callbackName, params object[] args)
            where TSerializer : ISerializer
        {
            if (processor is ISimpleProcessorNode<TResult, TSerializer>)
            {
                //We don't have to do anything with a simple processor
                return;
            }
            else
            {
                //We check if it's a recursive processor
                if (!InheritsFromRecursiveNode(processor.GetType()))
                    throw new ArgumentException(string.Format("The type '{0}' must be recursive", processor.GetType().Name), "processor");

                //We cast to the recursive node
                var recType = ObtainSuperClass(processor.GetType(), IsRecursiveNode);

                //We obtain the remaining list of processors
                var rest = recType.GetProperty("ProcessorStructure").GetValue(processor, null);

                //Gets the generic types
                Type r1Type = recType.GetGenericArguments()[1];
                Type r2Type = recType.GetGenericArguments()[2];

                //We apply recursion over the internal list of processors
                var methodArgs = new[] { rest }.Concat(args).ToArray();
                typeof(ProcessorUtilities)
                    .GetMethod(callbackName, BindingFlags.Public | BindingFlags.Static)
                    .MakeGenericMethod(new[] { r1Type, r2Type })
                    .Invoke(null, methodArgs);
            }
        }

        /// <summary>
        /// Returns the first superclass that fulfills the specified condition.
        /// </summary>
        private static Type ObtainSuperClass(Type type, Func<Type, bool> callback)
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
        /// Checks if a node is recursive or not.
        /// </summary>
        private static bool IsRecursiveNode(Type type)
        {
            if (!type.IsGenericType)
                return false;

            var gtd = type.GetGenericTypeDefinition();
            return gtd == typeof(RecursiveProcessorNode<,,>);
        }

        /// <summary>
        /// Verifies if a node inherits from a recursive one.
        /// </summary>
        private static bool InheritsFromRecursiveNode(Type type)
        {
            return ObtainSuperClass(type, IsRecursiveNode) != null;
        }

        private static bool IsExceptionProcessorNode(Type type)
        {
            if (!type.IsGenericType)
                return false;

            var gtd = type.GetGenericTypeDefinition();
            return gtd == typeof(ExceptionProcessor<,,,>);
        }

        /// <summary>
        /// Verifies if the type inhetids form ExceptionProcessor.
        /// </summary>
        private static bool InheritsFromExceptionProcessor(Type type)
        {
            return ObtainSuperClass(type, IsExceptionProcessorNode) != null;
        }

        /// <summary>
        /// Takes a processing structure from a REST response, and it adds a default processor to the end of each leaf.
        /// Because the default processor is the identity when composed on the right, it can be added without problems and it's only done so there exists at least one node to force the processing of a response.
        ///
        /// Ej:
        /// 	(P1 -> P2) goes to (P1 -> P2 -> DEF)
        /// 	(R1 * P1) -> P2 goes to (R1 * (P1 -> DEF)) -> P2 -> DEF
        /// </summary>
        /// <typeparam name="TResult">Type of the result</typeparam>
        /// <typeparam name="TSerializer">Type of the serializer</typeparam>
        /// <param name="processor">Processor structure</param>
        public static void AddNeutralProcessorAtEndsForStructure<TResult, TSerializer>(
            IProcessorStructure<TResult, TSerializer> processor)
            where TSerializer : ISerializer
        {
            //If it only has a single processor or it is one by default, then we don't do anything
            if (processor.Count == 1 && processor[0] is SuccessProcessor<TResult, TSerializer>)
                return;

            //It adds the default processing node to all its nodes
            foreach (var node in processor)
            {
                if (!InheritsFromExceptionProcessor(node.GetType()))
                {
                    ProcessNode<TResult, TSerializer>(node, "AddNeutralProcessorAtEndsForStructure");
                }
            }

            //Then it adds it to the end of the structure itself
            processor.Add(new SuccessProcessor<TResult, TSerializer>().Default());
        }

        /// <summary>
        /// Takes a processing structure, finds all nodes that implement IErrorProcessor and sets the error serializer with a specific one.
        /// </summary>
        /// <typeparam name="TResult">Type of the result</typeparam>
        /// <typeparam name="TSerializer">Type of the error serializer</typeparam>
        /// <param name="processor">Processor to add serializer to</param>
        /// <param name="serializer">Serializer to add to processors</param>
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
        /// Verifies if the HTTP code represents success (2xx).
        /// </summary>
        public static bool IsSuccessful(this HttpStatusCode code)
        {
            return (int)code >= 200 && (int)code < 300;
        }
    }
}