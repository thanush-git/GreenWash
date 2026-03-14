namespace GreenWash.DTO
{
    public class SubmitRatingRequest
    {
        public long OrderId { get; set; }
        public int Score { get; set; }
        public string? Comment { get; set; }
        // RevieweeId is NOT accepted from the client — it is derived server-side
        // from the order to prevent spoofing (customer always rates washer, washer always rates customer)
    }
}
