namespace Polaris.Domain.Constant
{
    public class AuthenticationTypeConstant
    {
        public const string EmailOnly = "EMAIL_ONLY";
        public const string EmailPassword = "EMAIL_PASSWORD";

        public static bool IsValid(string value)
        {
            return value == EmailOnly ||
                   value == EmailPassword;
        }
    }
}
