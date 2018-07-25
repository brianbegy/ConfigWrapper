# ConfigWrapper	

A simple configuration wrapper to abstract config sources and offer sensible defaults.

## Getting Started

Available on Nuget

### Prerequisites

.net 4.0 + 

## Use

### Interfaces

The IConfigWrapper provides the contract for reading config values.  
The IWritableConfigWrapper provides the contract for reading and writing values. 

### Simple values

In this example, we return 1000 if there is no configured value in the app.config or web.config file.
``` 
IConfigWrapper = new AppSettingsConfigWrapper();
var sleepMs = configWrapper.Get<int>("sleep-time-in-ms", 1000);
```

### Arrays

In this example, we return an array of strings from the Windows registry, delimited by , or |.  
If we have no values for the specified key, we get the array ["pork", "beans"]
``` 
IConfigWrapper = new WindowsRegistryConfigWrapper();
var items = configWrapper.Get<string[]>("HKCU/topkey/subkey/valuekey", new [] {"pork", "beans"}, []{',''|'});
```

### Exceptions

By default, the neither the AppSettingsConfigWrapper nor the WindowsRegistryConfigWrapper throws an exception if a config has a value of the wrong type. 
If it cannot cast, it returns the default.

Example: 
for a config entry
```
<appSettings>
	<add key="sleep-time-in-ms" value="chicken sandwich"/>
</appSettings>
```

and the code
``` 
IConfigWrapper = new AppSettingsConfigWrapper();
var sleepMs = configWrapper.Get<int>("sleep-time-in-ms", 1000);
// sleepMs = 1000
```

We try to cast the value "chicken sandwich" to an int.  This fails, so we return 1000.

To throw an exception, pass in true for errorOnWrongType.
``` 
IConfigWrapper = new AppSettingsConfigWrapper();
var sleepMs = configWrapper.Get<int>("sleep-time-in-ms", 1000, true);
//throws an Exception
```

### Required config values
If you do not pass in a default, it will error on missing values or values that cannot be cast.

``` 
IConfigWrapper = new AppSettingsConfigWrapper();
var sleepMs = configWrapper.Get<int>("this-key-is-not-in-the-config");
//  throws an exception
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
IConfigWrapper configWrapper = new AppSettingsConfigWrapper();
var sleepMs = configWrapper.Get<int>("sleep-time-in-ms", 1000);
// sleepMs = 5000
```

#### WindowsRegistryConfigWrapper

Loads from the Windows registry, defining a root key for my application.

``` 
IWritableConfigWrapper = new WindowsRegistryConfigWrapper("HKLM/MyApplication/");
configWrapper.Set<int>("MyKey", 5000); 
var sleepMs = configWrapper.Get<int>("sleep-time-in-ms", 5000);
// sleepMs = 5000
```

##### WARNING

While the WindowsRegistryConfigWrapper will not modify root level keys e.g. "`", it should be used with care.  It is a sharp knife, be careful.

#### IniConfigWrapper

Supports ini files with values like 
```
myvalue=foo
```

#### JSONConfigWrapper
The json wrapper is in a separate nuget package: ConfigWrapper.Json.  It has a dependency on Newtonsoft, so it is distributed separately from ConfigWrapper.


```
IConfigWrapper configWrapper = new JsonConfigWrapper("../data.json");
var sleepMs = configWrapper.Get<int>("sleep-time-in-ms", 5000);
// sleepMs = 5000

```

#### InMemoryConfigWrapper 
The in memory wrapper is useful for unit test scenarios or where we want to hold onto a config temporarily. 


## Authors

See the list of [contributors](https://github.com/brianbegy/ConfigWrapper/contributors) who participated in this project.

## License

This project is licensed under the MIT License 

## Acknowledgments

* Hat tip to the teams at Spotlite (RallyHealth), Guaranteed Rate, GrubHub, and Endava for their feedback.
