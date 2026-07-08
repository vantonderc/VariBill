using System.Threading.Tasks;

namespace VariBillWebAPI.Services.Interfaces;

public interface ITokenService
{
    Task<string> GetAccessTokenAsync();
}
