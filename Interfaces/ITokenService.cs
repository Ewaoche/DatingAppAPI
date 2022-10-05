using FinalDatingApp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalDatingApp.Interfaces
{
   public  interface ITokenService
    {

        string CreateToken(AppUser user);
    }
}
