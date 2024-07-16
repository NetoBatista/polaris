namespace Polaris.Domain.Model
{
    public class ValidatorResultModel
    {
        public bool IsValid { get => Errors.Count == 0; }
        public List<string> Errors { get; set; } = [];
    }
}
