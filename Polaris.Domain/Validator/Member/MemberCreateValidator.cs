﻿using Polaris.Domain.Constant;
using Polaris.Domain.Dto.Member;
using Polaris.Domain.Entity;
using Polaris.Domain.Interface.Repository;
using Polaris.Domain.Interface.Validator;
using Polaris.Domain.Model;

namespace Polaris.Domain.Validator.Application
{
    public class MemberCreateValidator : IValidator<MemberCreateRequestDTO>
    {
        private ValidatorResultModel _resultModel = new();
        private MemberCreateRequestDTO _instance = new();
        private readonly IApplicationRepository _applicationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMemberRepository _memberRepository;
        public MemberCreateValidator(IApplicationRepository applicationRepository,
                                     IUserRepository userRepository,
                                     IMemberRepository memberRepository)
        {
            _applicationRepository = applicationRepository;
            _userRepository = userRepository;
            _memberRepository = memberRepository;
        }

        public ValidatorResultModel Validate(MemberCreateRequestDTO request)
        {
            _instance = request;
            AuthenticationPasswordValidate();
            AuthenticationTypeValidate();
            UserValidate();
            ApplicationValidate();
            AlreadyMemberValidate();
            return _resultModel;
        }

        private void AuthenticationPasswordValidate()
        {
            if (_instance.AuthenticationType == AuthenticationTypeConstant.EmailPassword)
            {
                if (string.IsNullOrEmpty(_instance.Password))
                {
                    _resultModel.Errors.Add($"Password is required");
                }
                else if (_instance.Password.Length < 6)
                {
                    _resultModel.Errors.Add($"Password must have at least 6 characters");
                }
            }
            else if (_instance.AuthenticationType == AuthenticationTypeConstant.EmailOnly)
            {
                if (!string.IsNullOrEmpty(_instance.Password))
                {
                    _resultModel.Errors.Add($"For {AuthenticationTypeConstant.EmailOnly} the password must be empty.");
                }
            }
        }

        private void AuthenticationTypeValidate()
        {
            if (!AuthenticationTypeConstant.IsValid(_instance.AuthenticationType))
            {
                _resultModel.Errors.Add($"Authentication type must be {AuthenticationTypeConstant.EmailOnly} or {AuthenticationTypeConstant.EmailPassword}");
            }
        }

        private void UserValidate()
        {
            var entity = new User { Id = _instance.UserId };
            var exists = _userRepository.Exists(entity).Result;
            if (!exists)
            {
                _resultModel.Errors.Add($"User not found");
            }
        }

        private void ApplicationValidate()
        {
            var entity = new Entity.Application { Id = _instance.ApplicationId };
            var exists = _applicationRepository.Exists(entity).Result;
            if (!exists)
            {
                _resultModel.Errors.Add($"Application not found");
            }
        }

        private void AlreadyMemberValidate()
        {
            var entity = new Member
            {
                ApplicationId = _instance.ApplicationId,
                UserId = _instance.UserId
            };
            var exists = _memberRepository.Exists(entity).Result;
            if (exists)
            {
                _resultModel.Errors.Add($"User already member of application");
            }
        }
    }
}