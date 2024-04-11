namespace ReupVirtualTwin.dataModels
{
    public class PaginationResult<T>
    {
        public int count;
        public string next;
        public string previous;
        public T[] results;
    }
}
