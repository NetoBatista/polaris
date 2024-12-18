using Polaris.Domain.Constant;
using Polaris.Domain.Dto.Member;
using Polaris.Domain.Entity;
using Polaris.Domain.Interface.Repository;
using Polaris.Domain.Interface.Service;
using Polaris.Domain.Interface.Validator;
using Polaris.Domain.Mapper;
using Polaris.Domain.Model;

namespace Polaris.Service
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly IValidator<MemberCreateRequestDTO> _createValidator;
        private readonly IValidator<MemberRemoveRequestDTO> _removeValidator;

        public MemberService(IMemberRepository memberRepository,
                             IAuthenticationRepository authenticationRepository,
                             IValidator<MemberCreateRequestDTO> createValidator,
                             IValidator<MemberRemoveRequestDTO> removeValidator)
        {
            _memberRepository = memberRepository;
            _authenticationRepository = authenticationRepository;
            _createValidator = createValidator;
            _removeValidator = removeValidator;
        }

        public async Task<ResponseBaseModel> Create(MemberCreateRequestDTO request)
        {
            var responseValidate = _createValidator.Validate(request);
            if (!responseValidate.IsValid)
            {
                return ResponseBaseModel.BadRequest(responseValidate.Errors);
            }

            var entity = MemberMapper.ToEntity(request);
            var member = await _memberRepository.Create(entity);

            string? password = null;
            if (!string.IsNullOrEmpty(request.Password))
            {
                password = CryptographyUtil.ConvertToMD5(request.Password!);
            }
            var entityAuthentication = new Authentication
            {
                MemberId = member.Id,
                Password = password
            };
            await _authenticationRepository.Create(entityAuthentication);

            return ResponseBaseModel.Ok();
        }

        public async Task<ResponseBaseModel> Remove(MemberRemoveRequestDTO request)
        {
            var responseValidate = _removeValidator.Validate(request);
            if (!responseValidate.IsValid)
            {
                return ResponseBaseModel.BadRequest(responseValidate.Errors);
            }

            var entity = MemberMapper.ToEntity(request);
            await _memberRepository.Remove(entity);
            return ResponseBaseModel.Ok();
        }

        public async Task<ResponseBaseModel> GetByUser(MemberGetUserRequestDTO request)
        {
            var entity = new Member { UserId = request.UserId };
            var response = await _memberRepository.Get(entity);

            if (response.Count == 0)
            {
                return ResponseBaseModel.Ok();
            }

            MemberUserResponseDTO member = new();

            foreach (var item in response.GroupBy(x => x.UserId))
            {
                var firstEntity = item.First();
                member = new MemberUserResponseDTO
                {
                    Email = firstEntity.UserNavigation.Email,
                    Applications = item.Select(x => new MemberItemApplicationResponseDTO
                    {
                        MemberId = x.Id,
                        Id = x.ApplicationNavigation.Id,
                        Name = x.ApplicationNavigation.Name,
                    }).ToList()
                };
            }

            return ResponseBaseModel.Ok(member);
        }

        public async Task<ResponseBaseModel> GetByApplication(MemberGetApplicationRequestDTO request)
        {
            var entity = new Member { ApplicationId = request.ApplicationId };
            var response = await _memberRepository.Get(entity);
            if (response.Count == 0)
            {
                return ResponseBaseModel.Ok();
            }

            var members = new List<MemberApplicationResponseDTO>();
            List<Guid> applications = [];
            foreach (var item in response.GroupBy(x => x.ApplicationId))
            {
                applications.Add(item.Key);
            }

            foreach (var item in applications)
            {
                var application = response.First(x => x.ApplicationId == item).ApplicationNavigation;
                var member = new MemberApplicationResponseDTO
                {
                    Id = application.Id,
                    Name = application.Name,
                    Users = response.Where(x => x.ApplicationId == item)
                                    .Select(x => new MemberItemUserResponseDTO
                                    {
                                        MemberId = x.Id,
                                        Email = x.UserNavigation.Email
                                    }).ToList()
                };
                members.Add(member);
            }

            members = members.OrderBy(x => x.Name).ToList();

            return ResponseBaseModel.Ok(members);
        }
    }
}