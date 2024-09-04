using Newtonsoft.Json.Linq;

namespace ReupVirtualTwin.dataModels
{
    public class TaskResult
    {
        public bool isSuccess { get; }
        public string error { get; }

        protected TaskResult(bool isSuccess, string error)
        {
            this.isSuccess = isSuccess;
            this.error = error;
        }

        public static TaskResult Success() => new TaskResult(true, null);

        public static TaskResult Failure(string error) => new TaskResult(false, error);

        public static TaskResult CombineResults(params TaskResult[] results)
        {
            foreach (var result in results)
            {
                if (!result.isSuccess)
                {
                    return TaskResult.Failure(result.error);
                }
            }
            return TaskResult.Success();
        }
    }
}