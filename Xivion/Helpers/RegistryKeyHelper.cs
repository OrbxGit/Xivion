using Microsoft.Win32;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Xivion
{
    public static class RegistryKeyHelper
    {
        public static T ConvertGeneric<T>(string value)
        {
            try { return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(value); }
            catch { return default; }
        }

        public static T GetValue<T>(this RegistryKey RegKey, [CallerMemberName] string KeyName = null)
        {
            if (!RegKey.GetValueNames().Contains(KeyName))
                RegKey.SetValue(KeyName, default(T));

            object value = RegKey.GetValue(KeyName);
            T actValue = ConvertGeneric<T>(value.ToString());

            if (value == null || !actValue.GetType().Equals(typeof(T)))
            {
                RegKey.SetValue(KeyName, default(T));

                value = RegKey.GetValue(KeyName);
                actValue = ConvertGeneric<T>(value.ToString());
            }

            return actValue;
        }
    }
}
