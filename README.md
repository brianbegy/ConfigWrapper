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

### Arrays

``` 
	var configWrapper = new AppSettingsConfigWrapper();
	var items = configWrapper.Get<int>("sample-items", new [] {"pork", "beans"}, []{',''|'});
```
In this example, we return an array of strings from the config, delimited by , or |.  If we have no values in config we get the array ["pork", "beans"]


## Authors

See the list of [contributors](https://github.com/brianbegy/ConfigWrapper/contributors) who participated in this project.

## License

This project is licensed under the MIT License 

## Acknowledgments

* Hat tip to the teams at Spotlite (RallyHealth) and Guaranteed Rate where I wrote earlier versions of these concepts.
