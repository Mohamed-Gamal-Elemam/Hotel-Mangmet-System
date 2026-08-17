namespace Shared.Resposne
{
    public class GenaricResponse<T>
    {
        public int StutesCode { get; set; }
        public string Message { get; set; }=null!;
        public T? Data { get; set; }
    }
}
