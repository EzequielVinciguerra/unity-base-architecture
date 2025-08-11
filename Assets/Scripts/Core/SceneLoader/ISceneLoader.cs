using System.Threading;
using System.Threading.Tasks;

public interface ISceneLoader
{
    Task Load(string sceneName, bool additive = false, CancellationToken ct = default);
    Task Unload(string sceneName, CancellationToken ct = default);
    Task ReloadActive(CancellationToken ct = default);
}
