using System.Collections.Concurrent;
using Microsoft.IdentityModel.Tokens;
using Polaris.Domain.Configuration;
using Polaris.Domain.Constant;
using Polaris.Domain.Dto.Authentication;
using Polaris.Domain.Dto.User;
using Polaris.Domain.Entity;
using Polaris.Domain.Interface.Repository;
using Polaris.Domain.Interface.Service;
using Polaris.Domain.Interface.Validator;
using Polaris.Domain.Mapper;
using Polaris.Domain.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FirebaseAdmin;
using FirebaseAdmin.Auth;

namespace Polaris.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private static readonly ConcurrentDictionary<string, FirebaseApp> FirebaseApps = new();

        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IValidator<AuthenticationRequestDTO> _authenticatorValidator;
        private readonly IValidator<AuthenticationFirebaseRequestDTO> _authenticatorFirebaseValidator;
        private readonly IValidator<AuthenticationRefreshTokenRequestDTO> _authenticatorRefreshTokenValidator;
        private readonly IValidator<AuthenticationGenerateCodeRequestDTO> _authenticatorGenerateCodeValidator;
        private readonly IValidator<AuthenticationChangePasswordRequestDTO> _authenticationChangePasswordValidator;
        public AuthenticationService(IAuthenticationRepository authenticationRepository,
                                     IUserRepository userRepository,
                                     IMemberRepository memberRepository,
                                     IValidator<AuthenticationRequestDTO> authenticatorValidator,
                                     IValidator<AuthenticationFirebaseRequestDTO> authenticatorFirebaseValidator,
                                     IValidator<AuthenticationGenerateCodeRequestDTO> authenticatorGenerateCodeValidator,
                                     IValidator<AuthenticationRefreshTokenRequestDTO> authenticatorRefreshTokenValidator,
                                     IValidator<AuthenticationChangePasswordRequestDTO> authenticationChangePasswordValidator)
        {
            _authenticationRepository = authenticationRepository;
            _userRepository = userRepository;
            _memberRepository = memberRepository;
            _authenticatorValidator = authenticatorValidator;
            _authenticatorGenerateCodeValidator = authenticatorGenerateCodeValidator;
            _authenticatorRefreshTokenValidator = authenticatorRefreshTokenValidator;
            _authenticationChangePasswordValidator = authenticationChangePasswordValidator;
            _authenticatorFirebaseValidator = authenticatorFirebaseValidator;
        }

        public async Task<ResponseBaseModel> Authenticate(AuthenticationRequestDTO request)
        {
            var responseValidate = _authenticatorValidator.Validate(request);
            if (!responseValidate.IsValid)
            {
                return ResponseBaseModel.BadRequest(responseValidate.Errors);
            }

            var model = new AuthenticationByUserApplicationModel
            {
                ApplicationId = request.ApplicationId,
                Email = request.Email,
            };
            var entity = await _authenticationRepository.GetByEmailApplication(model);

            if (entity == null)
            {
                return ResponseBaseModel.BadRequest("Email or application not found");
            }

            var isValid = false;

            if (!string.IsNullOrEmpty(request.Password))
            {
                var authenticationPasswordModel = new AuthenticationPasswordModel
                {
                    ApplicationId = request.ApplicationId,
                    Email = request.Email,
                    Password = CryptographyUtil.ConvertToMD5(request.Password!)
                };
                isValid = await _authenticationRepository.AuthenticatePassword(authenticationPasswordModel);
            }
            else if (!string.IsNullOrEmpty(request.Code))
            {
                var canValidate = await _authenticationRepository.CanValidateCode(entity);
                if (!canValidate)
                {
                    return ResponseBaseModel.BadRequest("Excessive attempt or expired code, please generate a new code.");
                }

                var authenticationEmailOnlyModel = new Authentication
                {
                    Id = entity.Id,
                    Code = request.Code
                };
                isValid = await _authenticationRepository.AuthenticateCode(authenticationEmailOnlyModel);
            }

            if (!isValid)
            {
                return ResponseBaseModel.BadRequest("Invalid credentials");
            }

            var response = await GenerateAuthenticationResponseDto(request.Email, entity);
            return ResponseBaseModel.Ok(response);
        }

        public async Task<ResponseBaseModel> AuthenticateFirebase(AuthenticationFirebaseRequestDTO request)
        {
            var responseValidate = _authenticatorFirebaseValidator.Validate(request);
            if (!responseValidate.IsValid)
            {
                return ResponseBaseModel.BadRequest(responseValidate.Errors);
            }

            var model = new AuthenticationByUserApplicationModel
            {
                ApplicationId = request.ApplicationId,
                Email = request.Email,
            };
            var entity = await _authenticationRepository.GetByEmailApplication(model);

            if (entity == null)
            {
                return ResponseBaseModel.BadRequest("Email or application not found");
            }

            var isValid = false;

            try
            {
                var firebaseApp = GetFirebaseApp(request.FirebaseAppId, request.JsonCredentials);
                var decodedToken = await FirebaseAuth.GetAuth(firebaseApp).VerifyIdTokenAsync(request.TokenFirebase);
                isValid = decodedToken != null;
            }
            catch
            {
                isValid = false;
            }

            if (!isValid)
            {
                return ResponseBaseModel.BadRequest("Invalid credentials");
            }

            var response = await GenerateAuthenticationResponseDto(request.Email, entity);
            return ResponseBaseModel.Ok(response);
        }

        private async Task<AuthenticationResponseDTO> GenerateAuthenticationResponseDto(string email, Authentication entity)
        {
            var userEntity = new User
            {
                Email = email
            };
            var userResponse = await _userRepository.Get(userEntity);
            var authentication = await _authenticationRepository.RefreshToken(entity);
            await _authenticationRepository.ClearCodeConfirmation(entity);
            var token = GenerateToken(UserMapper.ToResponseDTO(userResponse!));
            var response = new AuthenticationResponseDTO
            {
                Expire = TokenConfig.Expire * 60,
                Token = token,
                RefreshToken = authentication.RefreshToken!
            };
            return response;
        }

        public async Task<ResponseBaseModel> GenerateCode(AuthenticationGenerateCodeRequestDTO request)
        {
            var responseValidate = _authenticatorGenerateCodeValidator.Validate(request);
            if (!responseValidate.IsValid)
            {
                return ResponseBaseModel.BadRequest(responseValidate.Errors);
            }

            var model = new AuthenticationByUserApplicationModel
            {
                ApplicationId = request.ApplicationId,
                Email = request.Email,
            };
            var entity = await _authenticationRepository.GetByEmailApplication(model);

            if (entity == null)
            {
                return ResponseBaseModel.BadRequest("Email or application not found");
            }

            await _authenticationRepository.GenerateCode(entity);

            return ResponseBaseModel.Ok();
        }

        public async Task<ResponseBaseModel> RefreshToken(AuthenticationRefreshTokenRequestDTO request)
        {
            var responseValidate = _authenticatorRefreshTokenValidator.Validate(request);
            if (!responseValidate.IsValid)
            {
                return ResponseBaseModel.BadRequest(responseValidate.Errors);
            }

            var entity = new Authentication { RefreshToken = request.RefreshToken.ToString() };
            var authentication = await _authenticationRepository.GetByRefreshToken(entity);
            if (authentication == null)
            {
                return ResponseBaseModel.BadRequest("Invalid credentials");
            }

            authentication = await _authenticationRepository.RefreshToken(authentication);
            await _authenticationRepository.ClearCodeConfirmation(authentication);
            var memberEntity = new Member
            {
                Id = authentication.MemberId
            };
            var member = await _memberRepository.Get(memberEntity);
            var token = GenerateToken(UserMapper.ToResponseDTO(member.First().UserNavigation));
            var response = new AuthenticationResponseDTO
            {
                Expire = TokenConfig.Expire * 60,
                Token = token,
                RefreshToken = authentication.RefreshToken!
            };
            return ResponseBaseModel.Ok(response);
        }

        public async Task<ResponseBaseModel> ChangePassword(AuthenticationChangePasswordRequestDTO request)
        {
            var responseValidate = _authenticationChangePasswordValidator.Validate(request);
            if (!responseValidate.IsValid)
            {
                return ResponseBaseModel.BadRequest(responseValidate.Errors);
            }

            var model = new AuthenticationByUserApplicationModel
            {
                ApplicationId = request.ApplicationId,
                Email = request.Email
            };
            var entity = await _authenticationRepository.GetByEmailApplication(model);

            var authenticationEmailOnlyModel = new Authentication
            {
                Id = entity.Id,
                Code = request.Code
            };

            var isValid = await _authenticationRepository.AuthenticateCode(authenticationEmailOnlyModel);
            if (!isValid)
            {
                return ResponseBaseModel.BadRequest("Invalid credentials");
            }

            entity!.Password = CryptographyUtil.ConvertToMD5(request.Password!);
            await _authenticationRepository.ChangePassword(entity);
            await _authenticationRepository.ClearCodeConfirmation(entity);

            return ResponseBaseModel.Ok();
        }

        private string GenerateToken(UserResponseDTO user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(TokenConfig.Secret);

            var claims = new List<Claim>
            {
                new(ClaimConstant.Email, user.Email),
                new(ClaimConstant.Identifier, user.Id.ToString()),
            };
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(TokenConfig.Expire),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private FirebaseApp GetFirebaseApp(string appId, string jsonFirebase)
        {
            if (!FirebaseApps.TryGetValue(appId, out var firebaseApp))
            {
                firebaseApp = FirebaseApp.Create(
                    new AppOptions
                    {
                        Credential = Google.Apis.Auth.OAuth2.GoogleCredential.FromJson(jsonFirebase)
                    }, appId);
                FirebaseApps[appId] = firebaseApp;
            }

            return firebaseApp;
        }
    }
}
