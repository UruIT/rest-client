using Monad;
using UruIT.Serialization.Core;
using Movistar.Online.Common.Types.Resultados;
using Movistar.Online.Common.Types.ResultadosRest;
using UruIT.RESTClient.Clases.Proveedores;
using UruIT.RESTClient.Interfaces.Proveedores;
using System;

namespace UruIT.RESTClient.Clases.Procesadores
{
    /// <summary>
    /// Extensiones de EitherProcessor para crear los encadenamientos usados por defecto
    /// </summary>
    public static class EitherProcessorExtensions
    {
        /// <summary>
        /// Configura un eitherProcessor con los procesadores necesarios para retornar con TError (TLeft)
        /// cuando la respuesta del servidor es error.
        /// </summary>
        /// <typeparam name="TError">Tipo Left que representa el error de negocio</typeparam>
        /// <typeparam name="TResult">Tipo Right que representa el caso de éxito</typeparam>
        /// <typeparam name="TErrorRest">Tipo del error que retorna el servidor</typeparam>
        /// <typeparam name="TException">Tipo de la excepción que contendrá al error de negocio</typeparam>
        /// <typeparam name="TSerializer">Tipo del serializador</typeparam>
        /// <param name="eitherProcessor">EitherProcessor a configurar</param>
        /// <param name="exceptionProvider">Crea una excepción a partir del error de negocio</param>
        /// <param name="errorConverterProvider">Convierte del error que se espera del servidor al error de negocio</param>
        /// <returns>El mismo EitherProcessor ya configurado</returns>
        public static EitherProcessor<TError, TResult, TSerializer> Default<TError, TResult, TErrorRest, TException, TSerializer>(
            this EitherProcessor<TError, TResult, TSerializer> eitherProcessor,
            IExceptionProvider<TError, TException> exceptionProvider,
            IErrorConverterProvider<TError, TErrorRest> errorConverterProvider)
            where TSerializer : ISerializer
            where TException : Exception
        {
            var errorConverterProcessor = new ErrorConverterProcessor<TError, TErrorRest, TSerializer>(errorConverterProvider);
            var errorProcessor = new ErrorProcessor<OptionStrict<TErrorRest>, TSerializer>().Default();

            eitherProcessor.AddLeftProcessors(
                errorConverterProcessor.AddProcessors(
                    errorProcessor));

            return eitherProcessor;
        }

        /// <summary>
        /// Configura un eitherResultadoProcessor con los procesadores necesarios para retornar con TResultado (TLeft)
        /// cuando la respuesta del servidor es error.
        /// </summary>
        /// <typeparam name="TResultado">Tipo Left que representa el error de negocio</typeparam>
        /// <typeparam name="TResult">Tipo Right que representa el caso de éxito</typeparam>
        /// <typeparam name="TResultadoRest">Tipo del error que retorna el servidor</typeparam>
        /// <typeparam name="TRestException">Tipo de la excepción que contendrá al error de negocio</typeparam>
        /// <typeparam name="TSerializer">Tipo del serializador</typeparam>
        /// <param name="eitherResultadoProcessor">EitherResultadoProcessor a configurar</param>
        /// <returns>El mismo EitherResultadoProcessor ya configurado</returns>
        public static EitherResultadoProcessor<TResultado, TResult, TResultadoRest, TSerializer> Default<TResultado, TResult, TResultadoRest, TRestException, TSerializer>(
            this EitherResultadoProcessor<TResultado, TResult, TResultadoRest, TSerializer> eitherResultadoProcessor)
            where TSerializer : ISerializer
            where TResultado : RestBusinessError<TResultado, TResultadoRest>
            where TResultadoRest : RestHttpError<TResultado, TResultadoRest>
            where TRestException : RestException<TResultado, TResultadoRest>
        {
            eitherResultadoProcessor.Default<TResultado, TResult, TResultadoRest, TRestException, TSerializer>(new ExceptionResultadoProvider<TResultado, TResultadoRest, TRestException>(),
                new ErrorResultadoConverterProvider<TResultado, TResultadoRest>());

            return eitherResultadoProcessor;
        }

        /// <summary>
        /// Configura un eitherResultadoProcessor con los procesadores necesarios para retornar con Resultado (TLeft)
        /// cuando la respuesta del servidor es error.
        /// </summary>
        /// <typeparam name="TResult">Tipo Right que representa el caso de éxito</typeparam>
        /// <typeparam name="TSerializer">Tipo del serializador</typeparam>
        /// <param name="eitherResultadoProcessor">EitherResultadoProcessor a configurar</param>
        /// <returns>El mismo EitherResultadoProcessor ya configurado</returns>
        public static EitherResultadoProcessor<TResult, TSerializer> Default<TResult, TSerializer>(
            this EitherResultadoProcessor<TResult, TSerializer> eitherResultadoProcessor)
            where TSerializer : ISerializer
        {
            eitherResultadoProcessor.Default<RestBusinessError, TResult, RestHttpError, RestException, TSerializer>();

            return eitherResultadoProcessor;
        }
    }
}