namespace efilling_api.Models
{
    public class FilingDetails
    {
        public string filingId { get; set; }
    }
    public class FilingListRequest
    {
        public string courtlocation { get; set; }
        public string userId { get; set; }
    }
}
