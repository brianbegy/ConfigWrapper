namespace ConfigWrapper
{
    interface IConfigWrapper
    {
       T Get<T>(string key, T value);
        T[] Get<T>(string key, T[] defaultValue, char[] delimiters);
    }
}
