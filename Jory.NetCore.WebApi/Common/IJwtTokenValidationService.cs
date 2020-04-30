using Jory.NetCore.Model.Entities;
using System.Threading.Tasks;
using Jory.NetCore.Core.Interfaces;

namespace Jory.NetCore.WebApi.Common
{
    public interface IJwtTokenValidationService:IService
    {
        string GenerateToken(ADUserT user);

        Task<string> GenerateTokenAsync(ADUserT user);
    }
}