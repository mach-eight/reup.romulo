using Newtonsoft.Json.Linq;

namespace ReupVirtualTwin.dataModels
{
    public class Result
    {
        public bool IsSuccess { get; }
        public string Error { get; }
        public JToken RequestTimestamp { get; }

        protected Result(bool isSuccess, string error, JToken requestTimestamp)
        {
            IsSuccess = isSuccess;
            Error = error;
            RequestTimestamp = requestTimestamp;
        }

        public static Result Success(JToken requestTimestamp = null) => new Result(true, null, requestTimestamp);

        public static Result Failure(string error, JToken requestTimestamp = null) => new Result(false, error, requestTimestamp);
    }
}