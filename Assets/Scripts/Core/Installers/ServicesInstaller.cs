using System;
using System.ComponentModel;
using UnityEngine;
using Core.Services;

namespace Core.Installers
{
    [DefaultExecutionOrder(-100)]
    public class ServicesInstaller : MonoBehaviour
    {
        public MonoBehaviour[] installersInOrder;

        [Tooltip("If enabled, the object persists between scenes.")]
        public bool dontDestroyOnLoad = true;

        private void Awake()
        {
            if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);

            if (installersInOrder == null) return;

            foreach (var mb in installersInOrder)
            {
                if (mb is not IInstaller installer)
                {
                    if (mb != null)
                        Debug.LogWarning($"'{mb.name}' Does not implement IInstaller, skipping it", mb);
                    continue;
                }

                installer.Install(ServiceLocator.Instance);
            }
        }

        private void OnDestroy()
        {
            Uninstall();
        }

        private void Uninstall()
        {
            foreach (Installer installer in installersInOrder)
            {
                installer.Uninstall(ServiceLocator.Instance);
            }
        }
    }
}

