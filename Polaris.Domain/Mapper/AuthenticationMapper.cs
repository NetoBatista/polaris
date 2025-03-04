using Polaris.Domain.Dto.Authentication;
using Polaris.Domain.Model.Authentication;

namespace Polaris.Domain.Mapper
{
    public static class AuthenticationMapper
    {
        public static AuthenticationPasswordModel ToModel(AuthenticationRequestDTO request)
        {
            return new AuthenticationPasswordModel
            {
                Email = request.Email,
                Password = request.Password!,
                ApplicationId = request.ApplicationId
            };
        }



    }
}
