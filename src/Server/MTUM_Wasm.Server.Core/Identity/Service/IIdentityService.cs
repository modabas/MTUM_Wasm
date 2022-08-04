using MTUM_Wasm.Server.Core.Identity.Dto;
using MTUM_Wasm.Shared.Core.Common.Result;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.Identity.Service;

internal interface IIdentityService
{
    Task<IServiceResult<LoginOutput>> Login(LoginInput requestDto, CancellationToken cancellationToken);
    Task<IServiceResult<RefreshOutput>> Refresh(RefreshInput requestDto, CancellationToken cancellationToken);
    Task<IServiceResult> Logout(LogoutInput requestDto, CancellationToken cancellationToken);
    Task<IServiceResult> ChangePassword(ChangePasswordInput requestDto, CancellationToken cancellationToken);
    Task<IServiceResult> ForgotPassword(ForgotPasswordInput requestDto, CancellationToken cancellationToken);
    Task<IServiceResult> ResetPassword(ResetPasswordInput requestDto, CancellationToken cancellationToken);
}
