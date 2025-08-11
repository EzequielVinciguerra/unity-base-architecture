using Core.Services;

namespace Core.Installers
{
    public interface IInstaller
    {
        void Install(ServiceLocator serviceLocator);
        void Uninstall(ServiceLocator serviceLocator);
    }
}
