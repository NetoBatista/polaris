using Polaris.Domain.Dto.Member;
using Polaris.Domain.Entity;
using Polaris.Domain.Interface.Repository;
using Polaris.Domain.Interface.Validator;
using Polaris.Domain.Model;

namespace Polaris.Domain.Validator.Application
{
    public class MemberRemoveValidator : IValidator<MemberRemoveRequestDTO>
    {
        private ValidatorResultModel _resultModel = new();
        private MemberRemoveRequestDTO _instance = new();
        private readonly IMemberRepository _memberRepository;
        public MemberRemoveValidator(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }

        public ValidatorResultModel Validate(MemberRemoveRequestDTO request)
        {
            _instance = request;
            ExistsMemberValidate();
            return _resultModel;
        }

        private void ExistsMemberValidate()
        {
            var entity = new Member
            {
                Id = _instance.Id
            };
            var exists = _memberRepository.Exists(entity).Result;
            if (!exists)
            {
                _resultModel.Errors.Add($"Member not found");
            }
        }
    }
}
