using Cysharp.Threading.Tasks;
using UI.Settings;

namespace UI.MainMenu.Settings
{
    public sealed class SfxVolumeSettingsButton : AudioSettingsButtonBase
    {
        protected override float GetCurrentValue()
        {
            return AudioSettingsService.SfxVolume;
        }

        protected override void ApplyValue(float value)
        {
            AudioSettingsService.SetSfxVolume(value).Forget();
        }
    }
}