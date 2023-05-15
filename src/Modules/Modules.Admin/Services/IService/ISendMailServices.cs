using InfrastructureCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Modules.Admin.Services.IService
{
    public interface ISendMailServices
    {
        Task<Result> SendEmailTo(IEnumerable<string> listEmail, string bodyContent, string subject, bool isBodyHtml);

        Task<Result> SendEmailWithFileTo(List<string> filesPath, IEnumerable<string> listEmail, string bodyContent, string subject, bool isBodyHtml, string SenderMailAddress, string SenderEmailPassword, string SenderEmailSmtpServer);

    }
}
