using System.Collections.Generic;
using Core.Audio;
using Core.Localization;
using Core.Services;
using Core.UI;
using Core.UI.Presenters;
using Core.UI.Views;

public class SettingsScreenPresenter : BasePresenter
{
    private SettingsScreenView _settingsScreenView;
    private readonly IAudioManager _audioManager; 
    private readonly ILocalizationService _localization;
    
    public SettingsScreenPresenter(IView view) : base(view)
    {
        _audioManager = ServiceLocator.Instance.Get<IAudioManager>();
        _localization = ServiceLocator.Instance.Get<ILocalizationService>();
    }

    public override void Initialize()
    {
        _settingsScreenView = (SettingsScreenView)view;
        SubscribeEvents();
        
        _settingsScreenView.SetUp(_audioManager.MusicVolume, _audioManager.SfxVolume);
        
        var options = BuildLanguageOptions();
        _settingsScreenView.PopulateLanguages(options, _localization.CurrentLanguageId);
    }


    public override void SubscribeEvents()
    {
        _settingsScreenView.OnCloseClicked += HandleSCloseClicked;
        _settingsScreenView.OnMusicVolumeChanged += HandeMusicVolumeChanged;
        _settingsScreenView.OnVFXVolumeChanged += HandeVFXVolumeChanged;
        _settingsScreenView.OnLanguageSelected += HandleLanguageSelected;
        
        eventBus.Subscribe<LanguageChanged>(OnLanguageChanged);
    }

    public override void UnSubscribeEvents()
    {
        _settingsScreenView.OnCloseClicked -= HandleSCloseClicked;
        _settingsScreenView.OnMusicVolumeChanged -= HandeMusicVolumeChanged;
        _settingsScreenView.OnVFXVolumeChanged -= HandeVFXVolumeChanged;
        _settingsScreenView.OnLanguageSelected -= HandleLanguageSelected;
        
        eventBus.Unsubscribe<LanguageChanged>(OnLanguageChanged);
    }

    public override void Dispose()
    {
        UnSubscribeEvents();
    }
    
    
    private void HandleSCloseClicked()
    {
        eventBus.Publish(new HideView( ScreenId.Settings));
    }

    private void HandeVFXVolumeChanged(float vol)=> _audioManager.SetSfxVolume(vol);

    private void HandeMusicVolumeChanged(float vol) => _audioManager.SetMusicVolume(vol);
    
    private void HandleLanguageSelected(LanguageId id)
    {
        _localization.SetLanguage(id);
    }

    private List<(LanguageId id, string label)> BuildLanguageOptions()
    {
        var loc = ServiceLocator.Instance.Get<ILocalizationService>();

        return new List<(LanguageId, string)>
        {
            (LanguageId.English, loc.Get("LANG_NAME_EN")),
            (LanguageId.Spanish, loc.Get("LANG_NAME_ES")),
            (LanguageId.Italian, loc.Get("LANG_NAME_IT")),
            (LanguageId.French,  loc.Get("LANG_NAME_FR")),
            (LanguageId.Russian, loc.Get("LANG_NAME_RU"))
        };
    }
    
    private void OnLanguageChanged(LanguageChanged _)
    {
        _settingsScreenView.PopulateLanguages(BuildLanguageOptions(), _localization.CurrentLanguageId);
    }
}
