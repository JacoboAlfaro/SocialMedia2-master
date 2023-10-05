namespace SocialMedia.Infrastructure.Options
{
    public class PasswordOptions
    {
        public int SaltSize { get; set; } = 16;
        public int KeySize { get; set; } = 32;
        public int Iterations { get; set; } = 10000;

    }
}
