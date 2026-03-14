namespace GreenWash.DTO
{
    public class ApplyPromoRequest
    {
        public long OrderId { get; set; }
        public string PromoCode { get; set; } = string.Empty;
    }
}
