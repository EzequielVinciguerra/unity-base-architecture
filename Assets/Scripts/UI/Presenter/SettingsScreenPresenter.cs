using System.Collections;
using System.Collections.Generic;
using Core.UI.Presenters;
using Core.UI.Views;
using UnityEngine;

public class SettingsScreenPresenter : BasePresenter
{
    SettingsScreenPresenter _settingsScreenPresenter;
    
    public SettingsScreenPresenter(IView view) : base(view)
    {
        _settingsScreenPresenter = (SettingsScreenPresenter)view;
        SubscribeEvents();
    }

    public override void Initialize()
    {
    }

    public override void SubscribeEvents()
    {
    }

    public override void UnSubscribeEvents()
    {
    }

    public override void Dispose()
    {
    }
}
