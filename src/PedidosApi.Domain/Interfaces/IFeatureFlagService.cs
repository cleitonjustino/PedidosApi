namespace PedidosApi.Domain.Interfaces;

public interface IFeatureFlagService
{
    Task<bool> IsFeatureEnabledAsync(string featureName);
}