
using Polaris.Domain.Model;

namespace Polaris.Domain.Interface.Validator
{
    public interface IValidator<T>
    {
        ValidatorResultModel Validate(T instance);
    }
}
