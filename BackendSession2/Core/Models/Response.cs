namespace BackendSession2.Core.Models
{
    public class ResponseModel
    {
        public string msg { get; set; }
        public int err { get; set; }

        public dynamic data { get; set; }
    }
}