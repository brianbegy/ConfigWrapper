namespace ConfigWrapper
{
    public class AppSettingsConfigWrapper:IConfigWrapper
    {        public T Get<T>(string key, T defaultValue){

           return System.Configuration.ConfigurationManager.AppSettings[key].CastAsT<T>(defaultValue);
            
        }
    }
}
