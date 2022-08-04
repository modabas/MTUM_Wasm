using MTUM_Wasm.Shared.Core.Common.Result;
using MTUM_Wasm.Shared.Core.Identity.Dto;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Client.Core.Identity.Service
{
    internal interface IIdentityService
    {
        public Task<IServiceResult> Login(LoginRequest loginRequest, CancellationToken cancellationToken);
        public Task<IServiceResult<RefreshResponse>> RefreshToken(CancellationToken cancellationToken);
        public Task<IServiceResult> Logout(LogoutTypeEnum logoutType, CancellationToken cancellationToken);
        public Task<IServiceResult> ChangePassword(ChangePasswordRequest request, CancellationToken cancellationToken);
        public Task<IServiceResult> ForgotPassword(ForgotPasswordRequest request, CancellationToken cancellationToken);
        public Task<IServiceResult> ResetPassword(ResetPasswordRequest request, CancellationToken cancellationToken);
    }
}