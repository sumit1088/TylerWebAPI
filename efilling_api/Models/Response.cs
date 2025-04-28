namespace efilling_api.Models
{

    public class CommonResponse
    {
        public bool success { get; set; }
        public int status { get; set; }
        public string? message { get; set; }
        public dynamic? data { get; set; }
       
    }

}
