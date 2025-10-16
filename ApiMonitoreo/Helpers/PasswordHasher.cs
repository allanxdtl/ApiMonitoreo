namespace ApiMonitoreo.Helpers
{
    public static class PasswordHasher
    {
        // Genera hash (bcrypt incluye salt internamente)
        public static string Hash(string password, int workFactor = 12)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor);
        }

        // Verifica
        public static bool Verify(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }
    }
}
