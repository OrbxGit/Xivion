using Microsoft.Win32;
using System;
using System.Linq;

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
            get
            {
                if (!RegKey.GetValueNames().Contains(nameof(AlwaysOnTop)))
                    RegKey.SetValue(nameof(AlwaysOnTop), false);

                return Convert.ToBoolean(RegKey.GetValue(nameof(AlwaysOnTop)));
            }
            set => RegKey.SetValue(nameof(AlwaysOnTop), value);
        }

        public bool AutoAttach
        {
            get
            {
                if (!RegKey.GetValueNames().Contains(nameof(AutoAttach)))
                    RegKey.SetValue(nameof(AutoAttach), false);

                return Convert.ToBoolean(RegKey.GetValue(nameof(AutoAttach)));
            }
            set => RegKey.SetValue(nameof(AutoAttach), value);
        }

        public bool AutoExecute
        {
            get
            {
                if (!RegKey.GetValueNames().Contains(nameof(AutoExecute)))
                    RegKey.SetValue(nameof(AutoExecute), false);

                return Convert.ToBoolean(RegKey.GetValue(nameof(AutoExecute)));
            }
            set => RegKey.SetValue(nameof(AutoExecute), value);
        }
    }
}
