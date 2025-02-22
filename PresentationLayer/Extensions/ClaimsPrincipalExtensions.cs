using System.Security.Claims;

namespace PresentationLayer.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetProfessorId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (claim == null)
            {
                throw new InvalidOperationException("User ID claim not found.");
            }
            return int.Parse(claim);
        }
    }
}
