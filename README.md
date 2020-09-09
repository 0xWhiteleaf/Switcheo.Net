# ![Icon](https://github.com/Zaliro/Switcheo.Net/blob/master/Resources/switcheo-coin.png?raw=true) Switcheo.Net 

![Build status](https://api.travis-ci.org/Zaliro/Switcheo.Net.svg?branch=master)

A .Net wrapper for the Switcheo API as described on [Switcheo](https://docs.switcheo.network/), including all features the API provides using clear and readable objects.

**If you think something is broken, something is missing or have any questions, please open an [Issue](https://github.com/Zaliro/Switcheo.Net/issues)**

---

## Donations
Donations are greatly appreciated and a motivation to keep improving.

**NEO / NEP-5**: ANvghqzo47faaeAbuuUJcqDhenxx38Wyuy

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
After installing it's time to actually use it. To get started we have to add the Switcheo.Net namespace:  `using Switcheo.Net;`

Switcheo.Net provides `SwitcheoClient` to interact with all rest calls of the Switcheo API. This client is disposable and as such can be used in a `using` statement.

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
The default behavior of client can be changed by providing options to the constructor, or using the `SetDefaultOptions` before creating a new client.  
Api credentials can be provided in the options. To instantiate a new `ApiCredentials` object, you can provide your private key simply in hexadecimal format or you can use an encrypted private key with a passphrase.  
  
For example:
```C#
// Using a private key in hex format
ApiCredentials = new ApiCredentials(new PrivateKey("<YOUR_PRIVATE_KEY_HEX>".ToSecureString()))

// Using a private key in Nep2 format
ApiCredentials = new ApiCredentials(new PrivateKey("<YOUR_ENCRYPTED_PRIVATE_KEY>".ToSecureString(), "<YOUR_PASSPHRASE>".ToSecureString()))
```

## Acknowledgments
* [JKorf](https://github.com/JKorf/),
  * For his awesome work on [CryptoExchange.Net](https://github.com/JKorf/CryptoExchange.Net/).
  
## Release notes
* Version 1.0.6 - 01 nov 2018 
	* Fixing `GetMyWalletBalances` method
* Version 1.0.5 - 01 nov 2018 
	* Adding a method `GetMyWalletBalances` to fecth user's wallet balance
	* Renaming `GetMyBalances` to `GetMyContractBalances`
	* Add support for endpoint `network/best_node`
	* Expose *WalletInformations* to *SwitcheoClient*
	* Updating endpoint `exchange/tokens`
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
<td><a href="https://github.com/JKorf/Bittrex.Net"><img src="https://github.com/JKorf/Bittrex.Net/blob/master/Bittrex.Net/Icon/icon.png?raw=true"></a>
<br />
<a href="https://github.com/JKorf/Bittrex.Net">Bittrex</a>
</td>
<td><a href="https://github.com/JKorf/Bitfinex.Net"><img src="https://github.com/JKorf/Bitfinex.Net/blob/master/Bitfinex.Net/Icon/icon.png?raw=true"></a>
<br />
<a href="https://github.com/JKorf/Bitfinex.Net">Bitfinex</a>
</td>
<td><a href="https://github.com/JKorf/CoinEx.Net"><img src="https://github.com/JKorf/CoinEx.Net/blob/master/CoinEx.Net/Icon/icon.png?raw=true"></a>
<br />
<a href="https://github.com/JKorf/CoinEx.Net">CoinEx</a>
</td>
<td><a href="https://github.com/JKorf/Huobi.Net"><img src="https://github.com/JKorf/Huobi.Net/blob/master/Huobi.Net/Icon/icon.png?raw=true"></a>
<br />
<a href="https://github.com/JKorf/Huobi.Net">Huobi</a>
</td>
<td><a href="https://github.com/JKorf/Kucoin.Net"><img src="https://github.com/JKorf/Kucoin.Net/blob/master/Kucoin.Net/Icon/icon.png?raw=true"></a>
<br />
<a href="https://github.com/JKorf/Kucoin.Net">Kucoin</a>
</td>
<td><a href="https://github.com/JKorf/Kraken.Net"><img src="https://github.com/JKorf/Kraken.Net/blob/master/Kraken.Net/Icon/icon.png?raw=true"></a>
<br />
<a href="https://github.com/JKorf/Kraken.Net">Kraken</a>
</td>
</tr>
</table>
And implementations from third parties (like this repository):
<table>
<tr>
<td><a href="https://github.com/ridicoulous/LiquidQuoine.Net"><img src="https://github.com/ridicoulous/LiquidQuoine.Net/blob/master/Resources/icon.png?raw=true"></a>
<br />
<a href="https://github.com/ridicoulous/LiquidQuoine.Net">Liquid</a>
</td>
<td><a href="https://github.com/burakoner/OKEx.Net"><img src="https://raw.githubusercontent.com/burakoner/OKEx.Net/master/Okex.Net/Icon/icon.png"></a>
<br />
<a href="https://github.com/burakoner/OKEx.Net">OKEx</a>
</td>
<td><a href="https://github.com/ridicoulous/Bitmex.Net"><img src="https://github.com/ridicoulous/Bitmex.Net/blob/master/Bitmex.Net/Icon/icon.png"></a>
<br />
<a href="https://github.com/ridicoulous/Bitmex.Net">Bitmex</a>
</td>
</tr>
</table>
