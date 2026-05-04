namespace GreenWash.DTO
{
    public class ProcessPayment
    {
        public long OrderId { get; set; }
        public int PaymentMethodId { get; set; }
        public string? PromoCode { get; set; }  // optional - applied at payment time
    }
}
