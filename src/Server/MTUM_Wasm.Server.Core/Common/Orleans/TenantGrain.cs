using MTUM_Wasm.Server.Core.Common.Entity;
using MTUM_Wasm.Server.Core.Common.ExpiringData;
using MTUM_Wasm.Server.Core.Common.Service;
using MTUM_Wasm.Server.Core.SystemAdmin.Dto;
using MTUM_Wasm.Server.Core.SystemAdmin.Service;
using MTUM_Wasm.Shared.Core.Common.Result;
using MTUM_Wasm.Shared.Core.Identity.Entity;
using Orleans;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MTUM_Wasm.Server.Core.Common.Orleans
{
    internal class TenantGrain : Grain, ITenantGrain
    {
        private ExpiringData<TenantEntity>? _expiringData;
        private const int TenantExpiresInSeconds = 60 * 30; //30 minutes

        private readonly ITenantRepo _tenantRepo;

        public TenantGrain(ITenantRepo tenantRepo)
        {
            _tenantRepo = tenantRepo;
        }

        public async Task<IServiceResult<TenantEntity>> GetTenant(GrainCancellationToken grainCancellationToken)
        {
            if (_expiringData is null || _expiringData.IsExpired())
            {
                var tenantId = this.GetPrimaryKey();

                using (var cancellationTokenSource = new CancellationTokenSource())
                {
                    //create a timeout token
                    var cancellationToken = cancellationTokenSource.Token;
                    //link grain token to cancellation token source for this scope, so token will be cancelled if grain token is cancelled
                    using (grainCancellationToken.CancellationToken.Register(() => cancellationTokenSource.Cancel()))
                    {
                        var tenantResult = await _tenantRepo.GetTenant(tenantId, cancellationToken);
                        if (tenantResult.Succeeded)
                        {
                            _expiringData = new ExpiringData<TenantEntity>(tenantResult.Data, TimeSpan.FromSeconds(TenantExpiresInSeconds));
                        }
                        //deactivate grain if returned error
                        else
                        {
                            DeactivateOnIdle();
                        }
                        return tenantResult;
                    }
                }
            }
            else
            {
                return Result<TenantEntity>.Success(_expiringData.Data);
            }
        }

        public async Task<IServiceResult> UpdateTenant(TenantEntity tenant, GrainCancellationToken grainCancellationToken)
        {
            var tenantId = this.GetPrimaryKey();

            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                //create a timeout token
                var cancellationToken = cancellationTokenSource.Token;
                //link grain token to cancellation token source for this scope, so token will be cancelled if grain token is cancelled
                using (grainCancellationToken.CancellationToken.Register(() => cancellationTokenSource.Cancel()))
                {
                    var updateResult = await _tenantRepo.UpdateTenant(tenant, cancellationToken);
                    if (updateResult.Succeeded)
                    {
                        _expiringData = new ExpiringData<TenantEntity>(updateResult.Data, TimeSpan.FromSeconds(TenantExpiresInSeconds));
                    }
                    //deactivate grain if returned error
                    else
                    {
                        DeactivateOnIdle();
                    }
                    return updateResult.ToBaseResult();
                }
            }
        }

        public async Task<IServiceResult> TenantExists(GrainCancellationToken grainCancellationToken)
        {
            var tenantResult = await GetTenant(grainCancellationToken);
            return tenantResult.ToBaseResult();
        }

        public async Task<IServiceResult> UpdateTenantNacPolicy(NacPolicy? nacPolicy, GrainCancellationToken grainCancellationToken)
        {
            var tenantId = this.GetPrimaryKey();

            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                //create a timeout token
                var cancellationToken = cancellationTokenSource.Token;
                //link grain token to cancellation token source for this scope, so token will be cancelled if grain token is cancelled
                using (grainCancellationToken.CancellationToken.Register(() => cancellationTokenSource.Cancel()))
                {
                    var updateResult = await _tenantRepo.UpdateTenantNacPolicy(tenantId, nacPolicy, cancellationToken);
                    if (updateResult.Succeeded)
                    {
                        _expiringData = new ExpiringData<TenantEntity>(updateResult.Data, TimeSpan.FromSeconds(TenantExpiresInSeconds));
                    }
                    //deactivate grain if returned error
                    else
                    {
                        DeactivateOnIdle();
                    }
                    return updateResult.ToBaseResult();
                }
            }
        }
    }
}
