using System;

namespace UruIT.RESTClient
{
    /// <summary>
    /// Executer of the REST call, giving an HTTP response for a given HTTP request.
    /// </summary>
    public interface IRestClientExecuter
    {
        /// <summary>
        /// Obtains a REST response given a REST request.
        /// </summary>
        /// <param name="host">Host url</param>
        /// <param name="request">Request to send</param>
        /// <returns>Response gotten after execution of said request</returns>
        IRestResponse Execute(Uri host, IRestRequest request);
    }
}