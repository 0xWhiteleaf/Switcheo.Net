# ![Icon](https://github.com/Zaliro/Switcheo.Net/blob/master/Resources/switcheo-coin.png?raw=true) Switcheo.Net 

![Build status](https://api.travis-ci.org/Zaliro/Switcheo.Net.svg?branch=master)

A .Net wrapper for the Switcheo API as described on [Switcheo](https://docs.switcheo.network/), including all features the API provides using clear and readable objects.

**If you think something is broken, something is missing or have any questions, please open an [Issue](https://github.com/Zaliro/Switcheo.Net/issues)**

---

## Donations
Donations are greatly appreciated and a motivation to keep improving.

**BTC**: 1FWRA22DneeUzbzgCbC8uWgAR7ahBmm42t  
**ETH**: 0x133db000bc2a1a5c503ea6997cb4a791a41ce782  
**NEO**: Aaqd8tnrRtLtjec4yCf5Sf1YQ1tF8gPXuZ  
**NANO**: xrb_1q1fnmnw87a5zqbb7shrkwsfxmea6569w4saw76k8eyor9dxt18wiu7ocddk  

## Installation
![Nuget version](https://img.shields.io/nuget/v/switcheo.net.svg)  ![Nuget downloads](https://img.shields.io/nuget/dt/Switcheo.Net.svg)
Available on [Nuget](https://www.nuget.org/packages/Switcheo.Net/).
```
pm> Install-Package Switcheo.Net
```
To get started with Switcheo.Net first you will need to get the library itself. The easiest way to do this is to install the package into your project using  [NuGet](https://www.nuget.org/packages/Switcheo.Net/). Using Visual Studio this can be done in two ways.

### Using the package manager
In Visual Studio right click on your solution and select 'Manage NuGet Packages for solution...'. A screen will appear which initially shows the currently installed packages. In the top bit select 'Browse'. This will let you download net package from the NuGet server. In the search box type 'Switcheo.Net' and hit enter. The Switcheo.Net package should come up in the results. After selecting the package you can then on the right hand side select in which projects in your solution the package should install. After you've selected all project you wish to install and use Switcheo.Net in hit 'Install' and the package will be downloaded and added to you projects.

### Using the package manager console
In Visual Studio in the top menu select 'Tools' -> 'NuGet Package Manager' -> 'Package Manager Console'. This should open up a command line interface. On top of the interface there is a dropdown menu where you can select the Default Project. This is the project that Switcheo.Net will be installed in. After selecting the correct project type  `Install-Package Switcheo.Net`  in the command line interface. This should install the latest version of the package in your project.

After doing either of above steps you should now be ready to actually start using Switcheo.Net.
## Getting started
After installing it's time to actually use it. To get started we have to add the Switcheo.Net namespace:  `using Switcheo.Net;`.

Switcheo.Net provides `SwitcheoClient` to interact with all rest calls of the Switcheo API. This client is disposable and as such can be used in a  `using`statement.

Most API methods are available in two flavors, sync and async:
````C#
public void NonAsyncMethod()
{
    using (var client = new SwitcheoClient())
    {
        var result = client.Ping();
    }
}

public async Task AsyncMethod()
{
    using (var client = new SwitcheoClient())
    {
        var result2 = await client.PingAsync();
    }
}
````

## Examples
Examples can be found in the Examples folder.


## Response handling
All API requests will respond with an CallResult object. This object contains whether the call was successful, the data returned from the call and an error if the call wasn't successful. As such, one should always check the Success flag when processing a response.
For example:
```C#
using (var client = new SwitcheoClient())
{
    var result = client.GetServerTime();
    if (result.Success)
      Console.WriteLine($"Server time: {result.Data}");
    else
      Console.WriteLine($"Error: {result.Error.Message}");
}
```
## Options & Authentication
The default behavior of client can be changed by providing options to the constructor, or using the `SetDefaultOptions` before creating a new client. Api credentials can be provided in the options.

## Acknowledgments
* [JKorf](https://github.com/JKorf/),
  * For his awesome work on [CryptoExchange.Net](https://github.com/JKorf/CryptoExchange.Net/).
  
## Release notes
* Version 1.0.4 - 28 oct 2018 
	* Support of encrypted private keys (*NEP2* format)
* Version 1.0.3 - 20 aug 2018 
	* Add support for endpoint `exchange/tokens`
	* Assets in objects are now of type SwitcheoToken
	* Assets in objects are now resolved dynamically through this new endpoint
	* Amounts in objects are calculated dynamically through SwitcheoToken.Precision
* Version 1.0.2 - 13 aug 2018 
	* Change Default encoding to UTF8
	* Fixed versions for libraries
* Version 1.0.1 - 12 aug 2018 
	* WithdrawalStatus added to SwitcheoWithdrawalExecutedResult
	* Parameters for endpoint `candlesticks` revised
* Version 1.0.0 - 08 aug 2018 
	* Initial version 
---
You also check other [JKorf](https://github.com/JKorf/)'s exchange API wrappers:
<table>
<tr>
<td><img src="https://github.com/JKorf/Binance.Net/blob/master/Resources/binance-coin.png?raw=true">
<br /><a href="https://github.com/JKorf/Binance.Net">Binance</a></td>
<td><img src="https://github.com/JKorf/Bittrex.Net/blob/master/Resources/icon.png?raw=true">
<br /><a href="https://github.com/JKorf/Bittrex.Net">Bittrex</a></td>
<td><img src="https://github.com/JKorf/Bitfinex.Net/blob/master/Resources/icon.png?raw=true">
<br /><a href="https://github.com/JKorf/Bitfinex.Net">Bitfinex</a></td>
<td><img src="https://github.com/JKorf/CoinEx.Net/blob/master/Resources/icon.png?raw=true">
<br /><a href="https://github.com/JKorf/CoinEx.Net">CoinEx</a></td>
</table>
