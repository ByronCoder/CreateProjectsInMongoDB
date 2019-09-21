using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectsAPI.Services
{
    public interface IEmailService
    {
        Task SendEmail(string name, string email, string phoneNumber, string subject, string message);
    }
}
