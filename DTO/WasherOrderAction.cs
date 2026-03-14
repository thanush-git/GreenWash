namespace GreenWash.DTO
{
    /// <summary>
    /// Body for PATCH /api/washers/orders/{orderId}
    /// Action must be one of: accept, decline, start, complete
    /// </summary>
    public class WasherOrderAction
    {
        public string Action { get; set; } = string.Empty;
    }
}
