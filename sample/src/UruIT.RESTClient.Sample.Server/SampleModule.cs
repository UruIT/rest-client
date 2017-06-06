using Nancy;
using Nancy.Responses;
using Nancy.Serialization.JsonNet;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace UruIT.RESTClient.Sample.Server
{
    public class SampleModule : NancyModule
    {
        public SampleModule()
            : base("/Sample")
        {
            Get["/WithNotFoundAndError/{Value}"] = p =>
            {
                string val = (string)p.Value;

                var results = new Dictionary<string, Nancy.Response>
                {
                    {
                        "NotFound", new Nancy.Response { StatusCode = HttpStatusCode.NotFound }
                    },
                    {
                        "InternalError", new Nancy.Response { StatusCode = HttpStatusCode.InternalServerError}
                    },
                    {
                        "InternalErrorWithResult",
                        new JsonResponse(
                            new ErrorResult
                            {
                                StatusCode = HttpStatusCode.InternalServerError,
                                Message = "InternalServerError",
                                Details = "InternalServerError - Detail"
                            },
                        new JsonNetSerializer(new JsonSerializer()))
                        {
                            StatusCode = HttpStatusCode.InternalServerError
                        }
                    },
                    {
                        "Ok", new JsonResponse(
                            new OperationResult
                            {
                                Value1 = "Value1",
                                Value2 = 10
                            },
                        new JsonNetSerializer(new JsonSerializer()))
                        {
                            StatusCode = HttpStatusCode.OK
                        }
                    },
                    {
                        "BadRequest", new Nancy.Response { StatusCode = HttpStatusCode.BadRequest}
                    },
                    {
                        "BadRequestWithResult",
                        new JsonResponse(
                            new ErrorResult
                            {
                                StatusCode = HttpStatusCode.BadRequest,
                                Message = "BadRequest",
                                Details = "BadRequest - Detail"
                            },
                        new JsonNetSerializer(new JsonSerializer()))
                        {
                            StatusCode = HttpStatusCode.BadRequest
                        }
                    },
                };

                return results[val];
            };

            Post["/NoContentWithError/{Value}"] = p =>
            {
                string val = (string)p.Value;

                var results = new Dictionary<string, Nancy.Response>
                {
                    {
                        "InternalError", new Nancy.Response { StatusCode = HttpStatusCode.InternalServerError}
                    },
                    {
                        "InternalErrorWithResult",
                        new JsonResponse(
                            new ErrorResult
                            {
                                StatusCode = HttpStatusCode.InternalServerError,
                                Message = "InternalServerError",
                                Details = "InternalServerError - Detail"
                            },
                        new JsonNetSerializer(new JsonSerializer()))
                        {
                            StatusCode = HttpStatusCode.InternalServerError
                        }
                    },
                    {
                        "NoContent", new Nancy.Response { StatusCode = HttpStatusCode.NoContent }
                    },
                    {
                        "BadRequest", new Nancy.Response { StatusCode = HttpStatusCode.BadRequest}
                    },
                    {
                        "BadRequestWithResult",
                        new JsonResponse(
                            new ErrorResult
                            {
                                StatusCode = HttpStatusCode.BadRequest,
                                Message = "BadRequest",
                                Details = "BadRequest - Detail"
                            },
                        new JsonNetSerializer(new JsonSerializer()))
                        {
                            StatusCode = HttpStatusCode.BadRequest
                        }
                    },
                };

                return results[val];
            };
        }
    }

    public class OperationResult
    {
        public string Value1 { get; set; }
        public int Value2 { get; set; }
    }

    public class ErrorResult
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
    }
}