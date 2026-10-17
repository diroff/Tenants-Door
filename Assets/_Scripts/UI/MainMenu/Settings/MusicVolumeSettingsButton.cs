using UI.Settings;

namespace UI.MainMenu.Settings
{
    public sealed class MusicVolumeSettingsButton : AudioSettingsButtonBase
    {
        protected override float GetCurrentValue()
        {
            return AudioSettingsService.MusicVolume;
        }

        protected override void ApplyValue(float value)
        {
            AudioSettingsService.SetMusicVolume(value);
        }
    }
}