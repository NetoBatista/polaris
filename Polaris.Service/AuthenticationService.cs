﻿using FirebaseAdmin;
using FirebaseAdmin.Auth;
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
using Polaris.Domain.Model.Authentication;
using Polaris.Domain.Model.Event;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Polaris.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private static readonly ConcurrentDictionary<string, FirebaseApp> FirebaseApps = new();
        private readonly IEventService _eventService;
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IValidator<AuthenticationRequestDTO> _authenticatorValidator;
        private readonly IValidator<AuthenticationFirebaseRequestDTO> _authenticatorFirebaseValidator;
        private readonly IValidator<AuthenticationRefreshTokenRequestDTO> _authenticatorRefreshTokenValidator;
        private readonly IValidator<AuthenticationGenerateCodeRequestDTO> _authenticatorGenerateCodeValidator;
        private readonly IValidator<AuthenticationChangePasswordRequestDTO> _authenticationChangePasswordValidator;
        public AuthenticationService(IAuthenticationRepository authenticationRepository,
                                     IUserRepository userRepository,
                                     IMemberRepository memberRepository,
                                     IEventService eventService,
                                     IRefreshTokenRepository refreshTokenRepository,
                                     IValidator<AuthenticationRequestDTO> authenticatorValidator,
                                     IValidator<AuthenticationFirebaseRequestDTO> authenticatorFirebaseValidator,
                                     IValidator<AuthenticationGenerateCodeRequestDTO> authenticatorGenerateCodeValidator,
                                     IValidator<AuthenticationRefreshTokenRequestDTO> authenticatorRefreshTokenValidator,
                                     IValidator<AuthenticationChangePasswordRequestDTO> authenticationChangePasswordValidator)
        {
            _authenticationRepository = authenticationRepository;
            _userRepository = userRepository;
            _memberRepository = memberRepository;
            _eventService = eventService;
            _refreshTokenRepository = refreshTokenRepository;
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

            var content = new EventAuthenticationModel
            {
                UserId = entity.MemberNavigation.UserNavigation.Id,
                UserEmail = entity.MemberNavigation.UserNavigation.Email,
                ApplicationId = entity.MemberNavigation.ApplicationId
            };
            await _eventService.SendMessage(EventConstant.AuthenticateCode, content);
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

            var isValid = false;

            try
            {
                var firebaseApp = GetFirebaseApp(request.FirebaseAppId, request.JsonCredentials);
                var decodedToken = await FirebaseAuth.GetAuth(firebaseApp).VerifyIdTokenAsync(request.TokenFirebase);
                decodedToken.Claims.TryGetValue(ClaimConstant.Email, out object? email);
                request.Email = email?.ToString() ?? string.Empty;
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

            var content = new EventAuthenticationModel
            {
                UserId = entity.MemberNavigation.UserNavigation.Id,
                UserEmail = entity.MemberNavigation.UserNavigation.Email,
                ApplicationId = entity.MemberNavigation.ApplicationId
            };
            await _eventService.SendMessage(EventConstant.AuthenticateFirebase, content);
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
            await _authenticationRepository.ClearCodeConfirmation(entity);
            var token = GenerateToken(UserMapper.ToResponseDTO(userResponse!));
            var refreshToken = await _refreshTokenRepository.Create(entity.Id);
            var response = new AuthenticationResponseDTO
            {
                Expire = TokenConfiguration.Expire * 60,
                Token = token,
                RefreshToken = refreshToken.Token
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

            var authentication = await _authenticationRepository.GenerateCode(entity);
            var content = new EventGenerateCodeModel
            {
                UserId = entity.MemberNavigation.UserNavigation.Id,
                UserEmail = entity.MemberNavigation.UserNavigation.Email,
                ApplicationId = entity.MemberNavigation.ApplicationId,
                Code = authentication.Code!,
                ApplicationName = entity.MemberNavigation.ApplicationNavigation.Name
            };
            await _eventService.SendMessage(EventConstant.GenerateCode, content);
            return ResponseBaseModel.Ok();
        }

        public async Task<ResponseBaseModel> RefreshToken(AuthenticationRefreshTokenRequestDTO request)
        {
            var responseValidate = _authenticatorRefreshTokenValidator.Validate(request);
            if (!responseValidate.IsValid)
            {
                return ResponseBaseModel.BadRequest(responseValidate.Errors);
            }

            var entity = new RefreshToken { Token = request.RefreshToken.ToString() };
            var refreshTokenEntity = await _refreshTokenRepository.Get(entity);
            if (refreshTokenEntity == null)
            {
                return ResponseBaseModel.BadRequest("Invalid credentials");
            }

            await _refreshTokenRepository.Remove(refreshTokenEntity);
            if (DateTime.UtcNow > refreshTokenEntity.Expiration)
            {
                return ResponseBaseModel.BadRequest("Invalid credentials");
            }

            refreshTokenEntity = await _refreshTokenRepository.Create(refreshTokenEntity.AuthenticationId);
            var authentication = await _authenticationRepository.GetById(refreshTokenEntity.AuthenticationId);
            if (authentication == null)
            {
                return ResponseBaseModel.BadRequest("Invalid credentials");
            }

            await _authenticationRepository.ClearCodeConfirmation(new Authentication { Id = authentication.Id });
            var memberEntity = new Member
            {
                Id = authentication.MemberId
            };
            var member = await _memberRepository.Get(memberEntity);
            var token = GenerateToken(UserMapper.ToResponseDTO(member.First().UserNavigation));
            var response = new AuthenticationResponseDTO
            {
                Expire = TokenConfiguration.Expire * 60,
                Token = token,
                RefreshToken = refreshTokenEntity.Token
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
                Id = entity!.Id,
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

            var key = Encoding.ASCII.GetBytes(TokenConfiguration.Secret);

            var claims = new List<Claim>
            {
                new(ClaimConstant.Email, user.Email),
                new(ClaimConstant.Identifier, user.Id.ToString()),
            };
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(TokenConfiguration.Expire),
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
