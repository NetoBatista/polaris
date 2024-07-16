using System.Security.Cryptography;
using System.Text;

public static class CryptographyUtil
{
    public static string ConvertToMD5(string input)
    {
        var md5Hash = MD5.Create();

        var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

        var sBuilder = new StringBuilder();

        for (int index = 0; index < data.Length; index++)
        {
            sBuilder.Append(data[index].ToString("x2"));
        }

        return sBuilder.ToString();
    }
}