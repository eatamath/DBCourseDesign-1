using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using No9Gallery.Models;

namespace No9Gallery.Services
{
    public interface ILoginServiceInterface
    {
        Task<LoginUser> CheckLogin(string ID, string password);
    }
}
