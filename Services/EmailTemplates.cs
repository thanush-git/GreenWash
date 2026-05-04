namespace GreenWash.Services
{
    /// Simple HTML email templates for GreenWash.
    /// Each method returns a (subject, htmlBody) tuple.
    public static class EmailTemplates
    {
        private static string Wrap(string bodyHtml) => $@"<!DOCTYPE html>
<html>
<body style=""font-family:Arial,sans-serif;font-size:14px;color:#333;"">
  <h2 style=""color:#2e7d32;"">GreenWash</h2>
  <hr>
  {bodyHtml}
  <hr>
  <p>GreenWash Team</p>
</body>
</html>";

        public static (string subject, string html) CustomerWelcome(string firstName) => (
            "Welcome to GreenWash",
            Wrap($@"<p>Hi {firstName},</p>
  <p>Thanks for signing up for GreenWash.</p>
  <p>You can now add your cars and book your first wash.</p>"));

        public static (string subject, string html) WasherWelcome(string firstName, string email, string tempPassword) => (
            "Your GreenWash washer account",
            Wrap($@"<p>Hi {firstName},</p>
  <p>Your washer account has been created.</p>
  <p><b>Email:</b> {email}<br>
     <b>Temporary Password:</b> {tempPassword}</p>
  <p>Please change your password after your first login.</p>"));

        public static (string subject, string html) OrderAccepted(
            string customerFirstName, long orderId, string washerName, string serviceAddress) => (
            "Washer assigned to your order",
            Wrap($@"<p>Hi {customerFirstName},</p>
  <p>Your order <b>#{orderId}</b> has been accepted.</p>
  <p><b>Washer:</b> {washerName}<br>
     <b>Location:</b> {serviceAddress}</p>
  <p>Your washer will arrive shortly.</p>"));

        public static (string subject, string html) OrderDeclined(
            string customerFirstName, long orderId) => (
            "Update on your order",
            Wrap($@"<p>Hi {customerFirstName},</p>
  <p>The washer assigned to order <b>#{orderId}</b> was unable to take the request.</p>
  <p>Your order will be reassigned to another washer shortly.</p>"));

        public static (string subject, string html) WashStarted(
            string customerFirstName, long orderId, string serviceAddress) => (
            "Wash started",
            Wrap($@"<p>Hi {customerFirstName},</p>
  <p>Your car wash for order <b>#{orderId}</b> has started.</p>
  <p><b>Location:</b> {serviceAddress}</p>"));

        public static (string subject, string html) WashCompleted(
            string customerFirstName, long orderId, decimal totalAmount) => (
            "Wash completed",
            Wrap($@"<p>Hi {customerFirstName},</p>
  <p>Your car wash for order <b>#{orderId}</b> is complete.</p>
  <p><b>Amount:</b> Rs. {totalAmount:F2}</p>
  <p>Please ignore if payment is already done and rate your washer.</p>"));

        public static (string subject, string html) OrderCancelled(
            string customerFirstName, long orderId) => (
            "Order cancelled",
            Wrap($@"<p>Hi {customerFirstName},</p>
  <p>Your order <b>#{orderId}</b> has been cancelled.</p>
  <p>If you have any questions, please contact support.</p>"));

        public static (string subject, string html) InvoiceGenerated(
            string customerFirstName, long orderId, int invoiceId,
            decimal totalAmount, string afterWashImageUrl, DateTime generatedAt) => (
            "Your GreenWash invoice",
            Wrap($@"<p>Hi {customerFirstName},</p>
  <p>Your invoice has been generated.</p>
  <p><b>Invoice ID:</b> {invoiceId}<br>
     <b>Order ID:</b> {orderId}<br>
     <b>Amount:</b> Rs. {totalAmount:F2}</p>
  <p><a href=""{afterWashImageUrl}"">View after-wash photo</a></p>"));

        public static (string subject, string html) PaymentConfirmed(
            string customerFirstName, long orderId, decimal amount) => (
            "Payment received",
            Wrap($@"<p>Hi {customerFirstName},</p>
  <p>We received your payment for order <b>#{orderId}</b>.</p>
  <p><b>Amount paid:</b> Rs. {amount:F2}</p>
  <p>Thank you for using GreenWash.</p>"));

        public static (string subject, string html) ScheduledWashReminder(
            string customerFirstName, long orderId,
            DateTime scheduledAt, string serviceAddress) => (
            "Upcoming wash reminder",
            Wrap($@"<p>Hi {customerFirstName},</p>
  <p>This is a reminder for your scheduled wash.</p>
  <p><b>Order:</b> #{orderId}<br>
     <b>Time:</b> {scheduledAt:dd MMM yyyy HH:mm}<br>
     <b>Location:</b> {serviceAddress}</p>"));
    }
}
