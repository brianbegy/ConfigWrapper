namespace ConfigWrapper
{
    interface IConfigWrapper
    {
       T Get<T>(string key, T value);
    }
}
