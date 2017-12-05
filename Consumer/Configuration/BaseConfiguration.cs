using System;

namespace Consumer.Configuration
{
    public abstract class BaseConfiguration
    {
        public string GetMandatoryAppSetting(string key)
        {
            throw new NotImplementedException();
        }
    }
}
