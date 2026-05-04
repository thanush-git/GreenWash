namespace GreenWash.Interfaces
{
    public interface IEmailService
    {
        Task<(string email, string firstName)> GetCustomerContactAsync(long customerId);
        Task SendAsync(string toEmail, string toName, string subject, string htmlBody);
    }
}
