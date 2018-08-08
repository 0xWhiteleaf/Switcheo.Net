using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Logging;
using Switcheo.Net;
using Switcheo.Net.Helpers;
using Switcheo.Net.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace ClientConsole
{
    class Program
    {
        private const string privateKeyHex = "<YOUR_PRIVATE_KEY_HEX>";

        static void Main(string[] args)
        {
            SwitcheoClient.SetDefaultOptions(new SwitcheoClientOptions()
            {
                // Warning : This code will be executed on TestApi (https://test-api.switcheo.network/v2) / TestNetV2 (a195c1549e7da61b8da315765a790ac7e7633b82)
                UseTestApi = true,
                ApiCredentials = new ApiCredentials(privateKeyHex.ToSecureString()),
                KeyType = BlockchainType.Neo,
                AutoTimestamp = true,
                LogVerbosity = LogVerbosity.Debug,
                LogWriters = new List<TextWriter>() { Console.Out }
            });

            using (var client = new SwitcheoClient())
            {
                // Setting default contract to be used in methods that require a contract hash
                // This avoids having to re-specify the contract hash at each call
                var lastNeoContract = client.GetContractHash(BlockchainType.Neo, "v2");
                client.SetDefaultContract(lastNeoContract);

                // You can also retrieve entire list of contracts by calling "GetContractsList"
                var contractsList = client.GetContractsList();

                /* Public */
                var serverTime = client.GetServerTime();
                var pairs = client.GetPairs();
                var candlesticks = client.GetCandlesticks("SWTH_NEO", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow, CandlestickInterval.ThirtyMinutes);
                var prices24hours = client.Get24HPrices();
                var lastPrices = client.GetLastPrices();
                var offers = client.GetOffers(BlockchainType.Neo, "GAS_NEO");
                var trades = client.GetTrades("GAS_NEO", 20, DateTime.UtcNow.AddHours(-6), DateTime.UtcNow);

                /* Private */
                var myBalances = client.GetMyBalances();
                // var someAddressBalances = client.GetBalances("<TARGET_ADDRESS>");

                // Open an order...
                var orderCreation = client.CreateOrder("SWTH_NEO", BlockchainType.Neo, OrderSide.Buy, 0.0006m, 1660, true, OrderType.Limit);
                if (orderCreation.Success)
                {
                    var orderExecution = client.ExecuteOrder(orderCreation.Data);
                    if (orderExecution.Success)
                    {
                        Console.WriteLine($"You're order '{orderExecution.Data.Id}' was successfully broadcasted !");

                        // Close an order...
                        var orderCancellationCreation = client.CreateCancellation(orderCreation.Data.Id);
                        if (orderCancellationCreation.Success)
                        {
                            var orderCancellationExec = client.ExecuteCancellation(orderCancellationCreation.Data.Id,
                                orderCancellationCreation.Data.Transaction);
                            if (orderCancellationExec.Success)
                                Console.WriteLine($"You're order cancellation '{orderCancellationExec.Data.Id}' was successfully broadcasted !");
                            else
                                Console.WriteLine($"Error while broadcasting your order cancellation : {orderCancellationExec.Error.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Error while broadcasting your order : {orderExecution.Error.Message}");
                    }
                }

                var myOrders = client.GetMyOrders();
                // var someAddressOrders = client.GetOrders("<TARGET_ADDRESS>");

                /* Deposit & Withdrawal */
                var depositCreation = client.CreateDeposit(BlockchainType.Neo, SupportedAsset.Neo, 1);
                if (depositCreation.Success)
                {
                    var depositExecution = client.ExecuteDeposit(depositCreation.Data.Id, depositCreation.Data.Transaction);
                    if (depositExecution.Success)
                        Console.WriteLine("You're deposit of 1 NEO was successfully executed ! It'll appear soon in your contract balance.");
                    else
                        Console.WriteLine($"Error while executing deposit : {depositExecution.Error.Message}");
                }
                else
                {
                    Console.WriteLine($"Error while creating deposit : {depositCreation.Error.Message}");
                }

                // Waiting till deposit shown in contract balance (in order to withdrawal)...
                int thirtySecondsAsMs = 30 * 1000;
                Thread.Sleep(thirtySecondsAsMs);

                var withdrawalCreation = client.CreateWithdrawal(BlockchainType.Neo, SupportedAsset.Neo, 1);
                if (withdrawalCreation.Success)
                {
                    var withdrawalExecution = client.ExecuteWithdrawal(withdrawalCreation.Data.Id);
                    if (withdrawalExecution.Success)
                        Console.WriteLine("You're withdrawal of 1 NEO was successfully executed ! It'll appear soon in your wallet balance.");
                    else
                        Console.WriteLine($"Error while executing withdrawal : {withdrawalExecution.Error.Message}");
                }
                else
                {
                    Console.WriteLine($"Error while creating withdrawal : {withdrawalCreation.Error.Message}");
                }
            }

            Console.ReadLine();
        }
    }
}
