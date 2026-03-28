namespace MusicApp.Domain.Common;

public static class Guard
{
    public static class Against
    {
        public static void NullOrWhiteSpace(string? value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{parameterName} cannot be null or empty.", parameterName);
        }

        public static void InvalidEmail(string? email, string parameterName)
        {
            NullOrWhiteSpace(email, parameterName);
            if (!email!.Contains('@'))
                throw new ArgumentException("Invalid email format.", parameterName);
        }

        public static void Negative(int value, string parameterName)
        {
            if (value < 0)
                throw new ArgumentException($"{parameterName} cannot be negative.", parameterName);
        }
    }
}
