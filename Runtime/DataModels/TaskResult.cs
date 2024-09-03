using Newtonsoft.Json.Linq;

namespace ReupVirtualTwin.dataModels
{
    public class TaskResult
    {
        public bool IsSuccess { get; }
        public string Error { get; }

        protected TaskResult(bool isSuccess, string error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static TaskResult Success() => new TaskResult(true, null);

        public static TaskResult Failure(string error) => new TaskResult(false, error);

        public static TaskResult CombineResults(params TaskResult[] results)
        {
            foreach (var result in results)
            {
                if (!result.IsSuccess)
                {
                    return TaskResult.Failure(result.Error);
                }
            }
            return TaskResult.Success();
        }
    }
}