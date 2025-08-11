using Core.Services;
using UnityEngine;

namespace Core.Installers
{
    public abstract class Installer : MonoBehaviour, IInstaller
    {
        public abstract void Install(ServiceLocator serviceLocator);

        public abstract void Uninstall(ServiceLocator serviceLocator);
    }

}
