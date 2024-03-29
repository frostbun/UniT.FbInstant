namespace UniT.FbInstant
{
    public class Result
    {
        public string Error { get; }

        public bool IsSuccess => this.Error is null;
        public bool IsError   => this.Error is { };

        internal Result(string error)
        {
            this.Error = error;
        }
    }

    public class Result<T> : Result
    {
        public T Data { get; }

        internal Result(T data, string error) : base(error)
        {
            this.Data = data;
        }
    }
}