namespace Project_back_end.JwtBearerConfig
{
    public class JwtBearerTokenSettings
    {
        public string SecretKey
        {
            get;
            set;
        }
        public string Audience
        { get; set; }
        public string Issuer
        { get; set; }
        public int ExpireTimeInSeconds { get; set; }
    }
}
