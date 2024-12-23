namespace NanoHealthLoggingSystem.Configrations
{
    public class JWTConfiguration
    {
        public string Key { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public double AvailavleDays { get; set; }
    }
}
