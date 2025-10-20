using Microsoft.AspNetCore.Http;

namespace solidhardware.storeCore.ServiceContract
{
    public interface IMailingService
    {
        Task SendMessageAsync(string mailTo, string subject, string body, IList<IFormFile>? attach);
    }
}
