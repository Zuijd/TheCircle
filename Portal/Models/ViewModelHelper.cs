namespace Portal.Models
{
    public class ViewModelHelper
    {
        public static byte[] ConvertClaimToKey(string key)
        {
            string convertedKey = key.ToString().Replace("\n", "").Replace("\r", "");
            return Convert.FromBase64String(convertedKey);
        }
    }
}