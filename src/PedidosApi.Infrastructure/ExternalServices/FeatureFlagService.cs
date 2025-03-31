using ConfigCat.Client;
using PedidosApi.Domain.Interfaces;

namespace PedidosApi.Infrastructure.ExternalServices;

public class FeatureFlagService(IConfigCatClient client) : IFeatureFlagService
{
    public Task<bool> IsFeatureEnabledAsync(string featureName)
    {
        return client.GetValueAsync(featureName, false);
    }
}