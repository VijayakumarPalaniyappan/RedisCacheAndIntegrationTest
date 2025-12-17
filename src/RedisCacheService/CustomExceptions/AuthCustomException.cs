using System.Diagnostics.CodeAnalysis;

namespace RedisCacheService.CustomExceptions
{
  [ExcludeFromCodeCoverage]
  [SuppressMessage("SonarLint", "S3925", Justification = "ISerializable is deprecated in .NET 8.")]
  [SuppressMessage("SonarLint", "S4027", Justification = "ISerializable is deprecated in .NET 8.")]
  public class AuthCustomException : Exception
  {
    public AuthCustomException() : base() { }

    public AuthCustomException(string message) : base(message) { }
  }
}
