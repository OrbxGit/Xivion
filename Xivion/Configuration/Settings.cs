using Microsoft.Win32;

namespace Xivion
{
    public class Settings
    {
        public static Settings Default => new Settings("Vixion");
        private readonly RegistryKey RegKey;
        
        public Settings(string Name)
            => RegKey = Registry.CurrentUser.OpenSubKey($"SOFTWARE\\{Name}", true) ?? Registry.CurrentUser.CreateSubKey(Name, true);

        public bool AlwaysOnTop
        {
            get => RegKey.GetValue<bool>();
            set => RegKey.SetValue(nameof(AlwaysOnTop), value);
        }

        public bool AutoAttach
        {
            get => RegKey.GetValue<bool>();
            set => RegKey.SetValue(nameof(AutoAttach), value);
        }

        public bool AutoExecute
        {
            get => RegKey.GetValue<bool>();
            set => RegKey.SetValue(nameof(AutoExecute), value);
        }
    }
}
