using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.ConfigurationReader
{
    public class ConfigurationReader
    {
        public static T LoadConfigurationSetting<T>(string keyname, T defaultvalue)
        {
            T result = defaultvalue;
            try
            {
                result = (T)Convert.ChangeType(ConfigurationManager.AppSettings[keyname], typeof(T));
            }
            catch
            {
                result = defaultvalue;
            }
            if (result == null)
                result = defaultvalue;
            return result;
        }
    }
}
