namespace Library.Utils
{
    public class ActionResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = null!;
        public object? Data { get; set; }
    }
}
