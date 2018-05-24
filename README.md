# ConfigWrapper	

A simple configuration wrapper to abstract config sources and offer sensible defaults.

## Getting Started

Available on Nuget

### Prerequisites

.net 4.0 + 

## Use
### Simple values

``` 
var configWrapper = new AppSettingsConfigWrapper();
var sleepMs = configWrapper.Get<int>("sleep-time-in-ms", 1000);
```

In this example, we return 1000 if there is no configured value in the app.config or web.config file.

### Simple values

``` 
var configWrapper = new AppSettingsConfigWrapper();
var sleepMs = configWrapper.Get<int>("sleep-time-in-ms", 1000);
```

In this example, we return 1000 if there is no configured value in the app.config or web.config file.

### Arrays

``` 
var configWrapper = new AppSettingsConfigWrapper();
var items = configWrapper.Get<string[]>("sample-items", new [] {"pork", "beans"}, []{',''|'});
```

In this example, we return an array of strings from the config, delimited by , or |.  

If we have no values in config we get the array ["pork", "beans"]

### Exceptions
By default, the AppSettingsConfigWrapper does not throw an exception if a config has a value of the wrong type: it returns the default.

For a config entry:
```
<appSettings>
	<add key="sleep-time-in-ms" value="chicken sandwich"/>
</appSettings>
```

``` 
var configWrapper = new AppSettingsConfigWrapper();
var sleepMs = configWrapper.Get<int>("sleep-time-in-ms", 1000);
// sleepMs = 1000
```

We try to cast the value "chicken sandwich" to an int, fail and return 1000.

To throw an exception, pass in true for errorOnWrongType.
``` 
var configWrapper = new AppSettingsConfigWrapper();
var sleepMs = configWrapper.Get<int>("sleep-time-in-ms", 1000, true);
//throws an Exception
```
### Sources
Each config wrapper loads from a different source. 

#### AppSettingsConfigWrapper 

Loads from the current application's app.config or web.config

```
<appSettings>
	<add key="sleep-time-in-ms" value="5000"/>
</appSettings>
```

``` 
var configWrapper = new AppSettingsConfigWrapper();
var sleepMs = configWrapper.Get<int>("sleep-time-in-ms", 1000);
// sleepMs = 5000
```

#### WindowsRegistryConfigWrapper 

``` 
var configWrapper = new WindowsRegistryConfigWrapper();
configWrapper.Set<int>("HKLM/MyApplication/MyKey", 5000); 
var sleepMs = configWrapper.Get<int>("sleep-time-in-ms", 5000);
// sleepMs = 5000
```
##### WARNING
While the WindowsRegistryConfigWrapper will not modify root level keys e.g. "HKLM.", it can be used to damage your system and should be used with care.

## Authors

See the list of [contributors](https://github.com/brianbegy/ConfigWrapper/contributors) who participated in this project.

## License

This project is licensed under the MIT License 

## Acknowledgments

* Hat tip to the teams at Spotlite (RallyHealth) and Guaranteed Rate where I wrote earlier versions of these concepts.
