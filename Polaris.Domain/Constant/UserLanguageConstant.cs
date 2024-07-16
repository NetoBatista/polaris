namespace Polaris.Domain.Constant
{
    public class UserLanguageConstant
    {
        public const string PT_BR = "PT_BR";
        public const string EN_US = "EN_US";

        public static bool IsValid(string value)
        {
            return value == PT_BR ||
                   value == EN_US;
        }
    }
}
