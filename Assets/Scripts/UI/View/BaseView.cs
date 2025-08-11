using System.Collections;
using System.Collections.Generic;
using Core.UI.Views;
using UnityEngine;

namespace Core.UI.Views
{

    public abstract class BaseView : MonoBehaviour, IView
    {
        [SerializeField] protected GameObject root;

        public abstract void Initialize();
        public abstract void DestroyFeature();
        public abstract void ActiveView(bool active);
    }
}
