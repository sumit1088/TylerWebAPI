namespace efilling_api.Models
{
    public class LoginResponse
    {
        public bool success { get; set; }
        public int status { get; set; }
        public string? message { get; set; }
        public string? access_token { get; set; }
        public dynamic? data { get; set; }
    }
}
