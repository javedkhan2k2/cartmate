using System.Security.Cryptography;

namespace CartMate.Domain.Lists;

public static class InviteCodeGenerator
{
    public const int Length = 8;

    // No 0/O or 1/I: codes are read aloud and typed on phones.
    private const string Alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    /// <summary>
    /// Cryptographically random, so codes can't be guessed. 32^8 ≈ 10^12 combinations;
    /// the database's unique index catches the astronomically rare collision.
    /// </summary>
    public static string Next() => RandomNumberGenerator.GetString(Alphabet, Length);
}
