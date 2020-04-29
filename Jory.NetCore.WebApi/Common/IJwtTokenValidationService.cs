using Jory.NetCore.Model.Entities;
using System.Threading.Tasks;

namespace Jory.NetCore.WebApi.Common
{
    public interface IJwtTokenValidationService
    {
        string GenerateToken(ADUserT user);

        Task<string> GenerateTokenAsync(ADUserT user);
    }
}