using Core.Audio;
using Core.Services;
using Core.UI;
using Core.UI.Presenters;
using Core.UI.Views;

public class SettingsScreenPresenter : BasePresenter
{
    private SettingsScreenView _settingsScreenView;
    private IAudioManager _audioManager; 
    
    public SettingsScreenPresenter(IView view) : base(view)
    {
    }

    public override void Initialize()
    {
        _audioManager = ServiceLocator.Instance.Get<IAudioManager>();
        _settingsScreenView = (SettingsScreenView)view;
        SubscribeEvents();
        
        _settingsScreenView.SetUp(_audioManager.MusicVolume, _audioManager.SfxVolume);
    }


    public override void SubscribeEvents()
    {
        _settingsScreenView.OnCloseClicked += HandleSCloseClicked;
        _settingsScreenView.OnMusicVolumeChanged += HandeMusicVolumeChanged;
        _settingsScreenView.OnVFXVolumeChanged += HandeVFXVolumeChanged;
    }

    public override void UnSubscribeEvents()
    {
    }

    public override void Dispose()
    {
        UnSubscribeEvents();
    }
    
    
    private void HandleSCloseClicked()
    {
        eventBus.Publish(new HideView( ScreenId.Settings));
    }

    private void HandeVFXVolumeChanged(float vol)=> _audioManager.SetMusicVolume(vol);

    private void HandeMusicVolumeChanged(float vol) => _audioManager.SetSfxVolume(vol);
}
