using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wallet.Application.Dtos.Requests;
using Wallet.Domain.Entities;

namespace Wallet.Application.Mappers.IdentityProfiles;

public class UserProfile : Profiles
{
    public UserProfile()
    {
        #region Register
        CreateMap<RegisterRequestDto, User>()
            .ForMember(dest=>dest.UserName, opt=>opt.MapFrom(src=>src.MobileNumber));
        #endregion
        #region Login

        #endregion
    }
}
