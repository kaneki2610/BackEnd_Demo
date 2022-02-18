namespace BackendSession2.Core.Models
{
    public class ResponseModel
    {
        public string msg { get; set; }
        public int err { get; set; }

        public dynamic data { get; set; }
    }

    public class NewCacheEntryRequest
    {
        public string key { get; set; }
        public string value { get; set; }
    }
}