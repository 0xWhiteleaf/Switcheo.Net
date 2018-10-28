using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Interfaces;
using CryptoExchange.Net.Objects;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Switcheo.Net.Helpers;
using Switcheo.Net.Objects;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Switcheo.Net.UnitTests
{
    [TestFixture()]
    public class SwitcheoClientTests
    {
        private const string SamplePrivateKey = "05f6324d0fafc12843ac80901dde59e24b02fd5f5ac46b45bf951c8dfa93c6b2";
        private const string SampleAddress = "fb17fd554c1c63fbc282c5ef03ccaf9f098d2af7";

        private const string SampleContractHash = "eed0d2e14b0027f5f30ade45f2b23dc57dd54ad2";

        public static SwitcheoToken[] SampleTokensList = new SwitcheoToken[]
        {
            new SwitcheoToken() { Symbol = "NEO", Id = "c56f33fc6ecfcd0c225c4ab356fee59390af8560be0e930faebe74a6daff7c9b", Precision = 8 },
            new SwitcheoToken() { Symbol = "GAS", Id = "602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7", Precision = 8 },
            new SwitcheoToken() { Symbol = "SWTH", Id = "ab38352559b8b203bde5fddfa0b07d8b2525e132", Precision = 8 },
            new SwitcheoToken() { Symbol = "RHT", Id = "2328008e6f6c7bd157a342e789389eb034d9cbc4", Precision = 0 },
            new SwitcheoToken() { Symbol = "ETH", Id = "0x0000000000000000000000000000000000000000", Precision = 18 }
        };

        [TestCase]
        public void GetCandlesticks_Should_RespondWithCandlesticksArray()
        {
            // arrange
            var candles = new[]
            {
                new SwitcheoCandlestick()
                {
                    Time = DatesHelper.FromTimestamp(1531215240),
                    Open = 0.00049408m,
                    Close = 0.00049238m,
                    High = 0.000497m,
                    Low = 0.00048919m,
                    Volume = 110169445,
                    QuoteVolume = 222900002152
                },
                new SwitcheoCandlestick()
                {
                    Time = DatesHelper.FromTimestamp(1531219800),
                    Open = 0.00050366m,
                    Close = 0.00049408m,
                    High = 0.00050366m,
                    Low = 0.00049408m,
                    Volume = 102398958,
                    QuoteVolume = 205800003323
                },
            };

            var client = PrepareClient(JsonConvert.SerializeObject(candles));

            // act
            var result = client.GetCandlesticks("SWTH_NEO", DateTime.Now.AddDays(-1), DateTime.Now, CandlestickInterval.ThirtyMinutes);

            // assert
            Assert.AreEqual(true, result.Success);
            Assert.IsTrue(Compare.PublicInstancePropertiesEqual(candles[0], result.Data[0]));
            Assert.IsTrue(Compare.PublicInstancePropertiesEqual(candles[1], result.Data[1]));
        }

        [TestCase]
        public void Get24HPrices_Should_RespondWith25HPricesArray()
        {
            // arrange
            var last24HoursSummary = new[]
            {
                new Switcheo24hPrice()
                {
                    Pair = "SWTH_NEO",
                    Open = 0.00047221m,
                    Close = 0.00049769m,
                    High = 1.2m,
                    Low = 0.0004563m,
                    Volume = 11897236125,
                    QuoteVolume = 19927606119564
                },
                new Switcheo24hPrice()
                {
                    Pair = "GAS_NEO",
                    Open = 0,
                    Close = 0,
                    High = 0,
                    Low = 0,
                    Volume = 0,
                    QuoteVolume = 0
                },
            };

            var client = PrepareClient(JsonConvert.SerializeObject(last24HoursSummary));

            // act
            var result = client.Get24HPrices();

            // assert
            Assert.AreEqual(true, result.Success);
            Assert.IsTrue(Compare.PublicInstancePropertiesEqual(last24HoursSummary[0], result.Data[0]));
            Assert.IsTrue(Compare.PublicInstancePropertiesEqual(last24HoursSummary[1], result.Data[1]));
        }

        [TestCase]
        public void GetLastPrices_Should_RespondWithLastPricesArray()
        {
            // arrange
            var expected = new SwitcheoPricesList()
            {
                Prices = new SwitcheoPrice[]
                {
                    new SwitcheoPrice() { Pair = "GAS_NEO", Price = 0.1m },
                    new SwitcheoPrice() { Pair = "SWTH_NEO", Price = 0.00050369m },
                    new SwitcheoPrice() { Pair = "SWTH_GAS", Price = 0.06052445m }
                }
            };

            var client = PrepareClient(JsonConvert.SerializeObject(expected));

            // act
            var result = client.GetLastPrices(new string[] { "GAS", "SWTH" });

            // assert
            Assert.AreEqual(true, result.Success);
            Assert.IsTrue(Compare.PublicInstancePropertiesEqual(expected.Prices[0], result.Data.Prices[0]));
            Assert.IsTrue(Compare.PublicInstancePropertiesEqual(expected.Prices[1], result.Data.Prices[1]));
            Assert.IsTrue(Compare.PublicInstancePropertiesEqual(expected.Prices[2], result.Data.Prices[2]));
        }

        [TestCase]
        public void GetOffers_Should_RespondWithOffersArray()
        {
            // arrange
            var offers = new[]
            {
                new SwitcheoOffer()
                {
                    Id = new Guid("b3a91e19-3726-4d09-8488-7c22eca76fc0"),
                    OfferAsset = SampleTokensList[2],
                    WantAsset = SampleTokensList[0],
                    AvailableAmount = SwitcheoHelpers.FromAssetAmount("2550000013", SampleTokensList[2].Precision),
                    OfferAmount = SwitcheoHelpers.FromAssetAmount("4000000000", SampleTokensList[2].Precision),
                    WantAmount = SwitcheoHelpers.FromAssetAmount("320000000", SampleTokensList[0].Precision)
                }
            };

            var client = PrepareClient(JsonConvert.SerializeObject(offers));

            // act
            var result = client.GetOffers(BlockchainType.Neo, "SWTH_NEO");

            // assert
            Assert.AreEqual(true, result.Success);
            Assert.IsTrue(Compare.PublicInstancePropertiesEqual(offers[0], result.Data[0]));
        }

        [TestCase]
        public void GetTrades_Should_RespondWithTradesArray()
        {
            // arrange
            var trades = new[]
            {
                new SwitcheoTrade()
                {
                    Id = new Guid("712a5019-3a23-463e-b0e1-80e9f0ad4f91"),
                    RawFillAmount = SwitcheoHelpers.FromAssetAmount("9122032316", SampleTokensList[2].Precision).ToString(),
                    RawTakeAmount = SwitcheoHelpers.FromAssetAmount("20921746", SampleTokensList[0].Precision).ToString(),
                    EventTime = DateTime.Now,
                    IsBuy = false
                },
                new SwitcheoTrade()
                {
                    Id = new Guid("5d7e42a2-a8f3-40a9-bce5-7304921ff691"),
                    RawFillAmount = SwitcheoHelpers.FromAssetAmount("280477933", SampleTokensList[0].Precision).ToString(),
                    RawTakeAmount = SwitcheoHelpers.FromAssetAmount("4207169", SampleTokensList[2].Precision).ToString(),
                    EventTime = DateTime.Now.AddMinutes(1),
                    IsBuy = true
                }
            };

            var client = PrepareClient(JsonConvert.SerializeObject(trades));

            // act
            var result = client.GetTrades("SWTH_NEO");

            // assert
            Assert.AreEqual(true, result.Success);
            Assert.IsTrue(Compare.PublicInstancePropertiesEqual(trades[0], result.Data[0]));
            Assert.IsTrue(Compare.PublicInstancePropertiesEqual(trades[1], result.Data[1]));
        }

        [TestCase]
        public void GetPairs_Should_RespondWithPairsArray()
        {
            // arrange
            var pairs = new string[]
            {
                "GAS_NEO",
                "SWTH_NEO"
            };

            var client = PrepareClient(JsonConvert.SerializeObject(pairs));

            // act
            var result = client.GetPairs();

            // assert
            Assert.AreEqual(true, result.Success);
            Assert.AreEqual(pairs[0], result.Data[0]);
            Assert.AreEqual(pairs[1], result.Data[1]);
        }

        [TestCase]
        public void GetTokens_Should_RespondWithTokensArray()
        {
            // arrange
            var tokensList = new SwitcheoTokensList()
            {
                Tokens = SampleTokensList
            };

            var client = PrepareClient(JsonConvert.SerializeObject(tokensList));

            // act
            var result = client.GetTokens();

            // assert
            Assert.AreEqual(true, result.Success);
            Assert.IsTrue(Compare.PublicInstancePropertiesEqual(tokensList.Tokens[0], result.Data.Tokens[0]));
            Assert.IsTrue(Compare.PublicInstancePropertiesEqual(tokensList.Tokens[1], result.Data.Tokens[1]));
            Assert.IsTrue(Compare.PublicInstancePropertiesEqual(tokensList.Tokens[2], result.Data.Tokens[2]));
        }

        [TestCase]
        public void GetContractsList_Should_RespondWithContractsArray()
        {
            // arrange
            var expected = new SwitcheoContractsList()
            {
                NeoContracts = new SwitcheoContract[]
                {
                    new SwitcheoContract() { Version = "V1", Hash = "0ec5712e0f7c63e4b0fea31029a28cea5e9d551f" },
                    new SwitcheoContract() { Version = "V1_5", Hash = "c41d8b0c30252ce7e8b6d95e9ce13fdd68d2a5a8" },
                    new SwitcheoContract() { Version = "V2", Hash = "48756743d524af03aa75729e911651ffd3cbe7d8" }
                }
            };

            var client = PrepareClient(JsonConvert.SerializeObject(expected));

            // act
            var result = client.GetContracts();

            // assert
            Assert.AreEqual(true, result.Success);
            Assert.IsTrue(Compare.PublicInstancePropertiesEqual(expected.NeoContracts[0], result.Data.NeoContracts[0]));
            Assert.IsTrue(Compare.PublicInstancePropertiesEqual(expected.NeoContracts[1], result.Data.NeoContracts[1]));
            Assert.IsTrue(Compare.PublicInstancePropertiesEqual(expected.NeoContracts[2], result.Data.NeoContracts[2]));
        }

        [TestCase]
        public void GetMyOrders_Should_RespondWithUserOrdersArray()
        {
            // arrange
            var orders = new SwitcheoOrder[]
            {
                new SwitcheoOrder()
                {
                    Id = new Guid("c415f943-bea8-4dbf-82e3-8460c559d8b7"),
                    Blockchain = BlockchainType.Neo,
                    ContractHash = SampleContractHash,
                    Address = SampleAddress,
                    Side = OrderSide.Buy,
                    OfferAsset = SampleTokensList[0],
                    WantAsset = SampleTokensList[1],
                    OfferAmount = SwitcheoHelpers.FromAssetAmount("100000000", SampleTokensList[0].Precision),
                    WantAmount = SwitcheoHelpers.FromAssetAmount("20000000", SampleTokensList[1].Precision),
                    UseNativeToken = true,
                    DepositTransaction = null,
                    CreatedAt = DateTime.Now,
                    Status = OrderStatus.Processed,
                    Fills = new SwitcheoFill[0],
                    Makes = new SwitcheoMake[0]
                }
            };

            var client = PrepareClient(JsonConvert.SerializeObject(orders), true);

            // act
            var result = client.GetMyOrders();

            // assert
            Assert.AreEqual(true, result.Success);
            Assert.IsTrue(Compare.PublicInstancePropertiesEqual(orders[0], result.Data[0], "Fills", "Makes"));
        }

        [TestCase]
        public void GetMyBalances_Should_RespondWithUserBalancesArray()
        {
            // arrange
            var balances = new SwitcheoBalancesList()
            {
                Confirming = new SwitcheoAssetConfirming[]
                {
                    new SwitcheoAssetConfirming()
                    {
                        Asset = SampleTokensList[1],
                        Events = new SwitcheoAssetConfirming.SwitcheoConfirmingEvent[]
                        {
                            new SwitcheoAssetConfirming.SwitcheoConfirmingEvent()
                            {
                                Type = EventType.Withdrawal,
                                Asset = SampleTokensList[1],
                                Amount = SwitcheoHelpers.FromAssetAmount("-100000000", SampleTokensList[1].Precision),
                                TransactionHash = null,
                                CreatedAt = DateTime.Now
                            }
                        }
                    }
                },
                Confirmed = new SwitcheoAssetBalance[]
                {
                    new SwitcheoAssetBalance() { Asset = SampleTokensList[1], Amount = SwitcheoHelpers.FromAssetAmount("47320000000",
                        SampleTokensList[1].Precision) },
                    new SwitcheoAssetBalance() { Asset = SampleTokensList[2], Amount = SwitcheoHelpers.FromAssetAmount("421549852102",
                        SampleTokensList[2].Precision) },
                    new SwitcheoAssetBalance() { Asset = SampleTokensList[0], Amount = SwitcheoHelpers.FromAssetAmount("50269113921",
                        SampleTokensList[0].Precision) }
                },
                Locked = new SwitcheoAssetBalance[]
                {
                    new SwitcheoAssetBalance() { Asset = SampleTokensList[1], Amount = SwitcheoHelpers.FromAssetAmount("500000000",
                        SampleTokensList[1].Precision) },
                    new SwitcheoAssetBalance() { Asset = SampleTokensList[0], Amount = SwitcheoHelpers.FromAssetAmount("1564605000",
                        SampleTokensList[0].Precision) }
                }
            };

            var client = PrepareClient(JsonConvert.SerializeObject(balances), true);

            // act
            var result = client.GetMyBalances();

            // assert
            Assert.AreEqual(true, result.Success);

            Assert.IsTrue(Compare.PublicInstancePropertiesEqual(balances.Confirming[0].Asset, result.Data.Confirming[0].Asset));
            Assert.IsTrue(Compare.PublicInstancePropertiesEqual(balances.Confirming[0].Events[0], result.Data.Confirming[0].Events[0]));

            Assert.IsTrue(Compare.PublicInstancePropertiesEqual(balances.Confirmed[0], result.Data.Confirmed[0]));
            Assert.IsTrue(Compare.PublicInstancePropertiesEqual(balances.Confirmed[1], result.Data.Confirmed[1]));
            Assert.IsTrue(Compare.PublicInstancePropertiesEqual(balances.Confirmed[2], result.Data.Confirmed[2]));

            Assert.IsTrue(Compare.PublicInstancePropertiesEqual(balances.Locked[0], result.Data.Locked[0]));
            Assert.IsTrue(Compare.PublicInstancePropertiesEqual(balances.Locked[1], result.Data.Locked[1]));
        }

        [TestCase]
        public void CreateDeposit_Should_RespondWithDepositCreationResult()
        {
            // arrange
            var expected = new SwitcheoDepositCreationResult()
            {
                Id = new Guid("ad5a6e05-d992-47c0-9f52-79ca173dccd1"),
                Transaction = new SwitcheoTransaction()
                {
                    Hash = "a53f26b21f2ede25909e8ac3bf094ac3318e31d5cd654bde4ba734478f1368b2",
                    Sha256 = "caa6727a795ef2dba75c7ea143b6020ffc0d2d4164d7be9639ebb9a1bbd7a6d1",
                    Type = 209,
                    Version = 1,
                    Attributes = new SwitcheoTransactionAttribute[]
                    {
                        new SwitcheoTransactionAttribute() { Usage = 32, Data = "f72a8d099fafcc03efc582c2fb631c4c55fd17fb" },
                    },
                    Inputs = new SwitcheoTransactionInput[]
                    {
                        new SwitcheoTransactionInput() { PrevHash = "f58adbee441670501487665b1a2cf9175994987921e35c58fae23ff218f1b29b", PrevIndex = 0 },
                        new SwitcheoTransactionInput() { PrevHash = "07162927e72c67d1e0993393e715aa3e9032f6ebe164f71fae9327126d542659", PrevIndex = 45 }
                    },
                    Outputs = new SwitcheoTransactionOutput[]
                    {
                        new SwitcheoTransactionOutput()
                        {
                            Asset = SampleTokensList[1],
                            ScriptHash = "e707714512577b42f9a011f8b870625429f93573",
                            Value = 1e-08m
                        }
                    },
                    Script = "0800e1f505000000001432e125258b7db0a0dffde5bd03b2b859253538ab145f8e3fcb095b55f53c44a1cab6e9c1a0da67cf8753c1076465706f73697467d24ad57dc53db2f245de0af3f527004be1d2d0ee",
                    Gas = 0
                },
                ScriptParams = new SwitcheoScriptParams()
                {
                    ScriptHash = SampleContractHash,
                    Operation = OperationType.Deposit
                }
            };

            var client = PrepareClient(JsonConvert.SerializeObject(expected), true);

            // act
            var result = client.CreateDeposit(BlockchainType.Neo, "SWTH", 10);

            // assert
            Assert.AreEqual(true, result.Success);
            Assert.IsTrue(Compare.PublicInstancePropertiesEqual(expected, result.Data, "Attributes", "Inputs", "Outputs"));

            Assert.IsTrue(Compare.PublicInstancePropertiesEqual(expected.Transaction.Attributes[0], result.Data.Transaction.Attributes[0]));

            Assert.IsTrue(Compare.PublicInstancePropertiesEqual(expected.Transaction.Inputs[0], result.Data.Transaction.Inputs[0]));
            Assert.IsTrue(Compare.PublicInstancePropertiesEqual(expected.Transaction.Inputs[1], result.Data.Transaction.Inputs[1]));

            Assert.IsTrue(Compare.PublicInstancePropertiesEqual(expected.Transaction.Outputs[0], result.Data.Transaction.Outputs[0]));
        }

        [TestCase]
        public void CreateWithdrawal_Should_RespondWithWithdrawalResult()
        {
            // arrange
            var expected = new SwitcheoWithdrawalCreationResult()
            {
                Id = new Guid("e0f56e23-2e11-4848-b749-a147c872cbe6"),
            };

            var client = PrepareClient(JsonConvert.SerializeObject(expected), true);

            // act
            var result = client.CreateWithdrawal(BlockchainType.Neo, "SWTH", 10);

            // assert
            Assert.AreEqual(true, result.Success);
            Assert.IsTrue(Compare.PublicInstancePropertiesEqual(expected, result.Data));
        }

        [TestCase]
        public void CreateOrder_Should_RespondWithOrder()
        {
            // arrange
            var expected = new SwitcheoOrder()
            {
                Id = new Guid("cfd3805c-50e1-4786-a81f-a60ffba33434"),
                Blockchain = BlockchainType.Neo,
                ContractHash = SampleContractHash,
                Address = SampleAddress,
                Side = OrderSide.Buy,
                OfferAsset = SampleTokensList[0],
                WantAsset = SampleTokensList[2],
                OfferAmount = SwitcheoHelpers.FromAssetAmount("2050000", SampleTokensList[0].Precision),
                WantAmount = SwitcheoHelpers.FromAssetAmount("2050000000", SampleTokensList[2].Precision),
                UseNativeToken = true,
                DepositTransaction = null,
                CreatedAt = DateTime.Now,
                Status = OrderStatus.Pending,
                Fills = new SwitcheoFill[]
                {
                    new SwitcheoFill()
                    {
                        Id = new Guid("2eaa3621-0e7e-4b3d-9c8c-454427f20949"),
                        OfferHash = "bb70a40e8465596bf63dbddf9862a009246e3ca27a4cf5140d70f01bdd107277",
                        OfferAsset = SampleTokensList[0],
                        WantAsset = SampleTokensList[2],
                        FillAmount = SwitcheoHelpers.FromAssetAmount("1031498", SampleTokensList[0].Precision),
                        WantAmount = SwitcheoHelpers.FromAssetAmount("2050000000", SampleTokensList[2].Precision),
                        FilledAmount = 0,
                        FeeAsset = SampleTokensList[2],
                        Price = 0.00050317m,
                        Transaction = null,
                        Status = FillStatus.Pending,
                        CreatedAt = DateTime.Now,
                        TransactionHash = "97ad8c0af68d22304e7f2d09d04f3beed29a845fe57de53444fff1507b752b99"
                    }
                }
            };

            var client = PrepareClient(JsonConvert.SerializeObject(expected), true);

            // act
            var result = client.CreateOrder("SWTH_NEO", BlockchainType.Neo, OrderSide.Buy, 0.00100000m,
                SwitcheoHelpers.FromAssetAmount("2050000000", SampleTokensList[0].Precision), true, OrderType.Limit);

            // assert
            Assert.AreEqual(true, result.Success);
            Assert.IsTrue(Compare.PublicInstancePropertiesEqual(expected, result.Data, "Fills", "Makes"));
            Assert.IsTrue(Compare.PublicInstancePropertiesEqual(expected.Fills[0], result.Data.Fills[0]));
        }

        [TestCase]
        public void CreateCancellation_Should_RespondWithCancellationCreationResult()
        {
            // arrange
            var expected = new SwitcheoCancellationCreationResult()
            {
                Id = new Guid("e4cf0472-59e3-4887-b2d3-720b98da0de6"),
                Transaction = new SwitcheoTransaction()
                {
                    Hash = "a53f26b21f2ede25909e8ac3bf094ac3318e31d5cd654bde4ba734478f1368b2",
                    Sha256 = "caa6727a795ef2dba75c7ea143b6020ffc0d2d4164d7be9639ebb9a1bbd7a6d1",
                    Type = 209,
                    Version = 1,
                    Attributes = new SwitcheoTransactionAttribute[]
                    {
                        new SwitcheoTransactionAttribute() { Usage = 32, Data = "f72a8d099fafcc03efc582c2fb631c4c55fd17fb" },
                    },
                    Inputs = new SwitcheoTransactionInput[]
                    {
                        new SwitcheoTransactionInput() { PrevHash = "f58adbee441670501487665b1a2cf9175994987921e35c58fae23ff218f1b29b", PrevIndex = 0 },
                        new SwitcheoTransactionInput() { PrevHash = "07162927e72c67d1e0993393e715aa3e9032f6ebe164f71fae9327126d542659", PrevIndex = 45 }
                    },
                    Outputs = new SwitcheoTransactionOutput[]
                    {
                        new SwitcheoTransactionOutput()
                        {
                            Asset = SampleTokensList[1],
                            ScriptHash = "e707714512577b42f9a011f8b870625429f93573",
                            Value = 1e-08m
                        }
                    },
                    Script = "0800e1f505000000001432e125258b7db0a0dffde5bd03b2b859253538ab145f8e3fcb095b55f53c44a1cab6e9c1a0da67cf8753c1076465706f73697467d24ad57dc53db2f245de0af3f527004be1d2d0ee",
                    Gas = 0
                },
                ScriptParams = new SwitcheoScriptParams()
                {
                    ScriptHash = SampleContractHash,
                    Operation = OperationType.CancelOffer
                }
            };

            var client = PrepareClient(JsonConvert.SerializeObject(expected), true);

            // act
            var result = client.CreateCancellation(new Guid("e4cf0472-59e3-4887-b2d3-720b98da0de6"));

            // assert
            Assert.AreEqual(true, result.Success);
            Assert.IsTrue(Compare.PublicInstancePropertiesEqual(expected, result.Data, "Attributes", "Inputs", "Outputs"));

            Assert.IsTrue(Compare.PublicInstancePropertiesEqual(expected.Transaction.Attributes[0], result.Data.Transaction.Attributes[0]));

            Assert.IsTrue(Compare.PublicInstancePropertiesEqual(expected.Transaction.Inputs[0], result.Data.Transaction.Inputs[0]));
            Assert.IsTrue(Compare.PublicInstancePropertiesEqual(expected.Transaction.Inputs[1], result.Data.Transaction.Inputs[1]));

            Assert.IsTrue(Compare.PublicInstancePropertiesEqual(expected.Transaction.Outputs[0], result.Data.Transaction.Outputs[0]));
        }

        [TestCase()]
        public void ReceivingSwitcheoError_Should_ReturnSwitcheoErrorAndNotSuccess()
        {
            // arrange
            var client = PrepareExceptionClient(JsonConvert.SerializeObject(new ArgumentError("TestMessage")), "503 error", 503);

            // act
            var result = client.GetServerTime();

            // assert
            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.Error);
            Assert.IsTrue(result.Error.Message.Contains("503 error"));
        }

        private SwitcheoClient PrepareClient(string responseData, bool credentials = false)
        {
            var expectedBytes = Encoding.UTF8.GetBytes(responseData);
            var responseStream = new MemoryStream();
            responseStream.Write(expectedBytes, 0, expectedBytes.Length);
            responseStream.Seek(0, SeekOrigin.Begin);

            var response = new Mock<IResponse>();
            response.Setup(c => c.GetResponseStream()).Returns(responseStream);

            var request = new Mock<IRequest>();
            request.Setup(c => c.Headers).Returns(new WebHeaderCollection());
            request.Setup(c => c.GetResponse()).Returns(Task.FromResult(response.Object));
            request.Setup(c => c.GetRequestStream()).Returns(Task.FromResult((Stream)new MemoryStream()));

            var tokensListExpectedBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new SwitcheoTokensList() { Tokens = SampleTokensList }));
            var tokensListResponseStream = new MemoryStream();
            tokensListResponseStream.Write(tokensListExpectedBytes, 0, tokensListExpectedBytes.Length);
            tokensListResponseStream.Seek(0, SeekOrigin.Begin);

            var tokensListResponse = new Mock<IResponse>();
            tokensListResponse.Setup(c => c.GetResponseStream()).Returns(tokensListResponseStream);

            var tokenListRequest = new Mock<IRequest>();
            tokenListRequest.Setup(c => c.Headers).Returns(new WebHeaderCollection());
            tokenListRequest.Setup(c => c.GetResponse()).Returns(Task.FromResult(tokensListResponse.Object));
            tokenListRequest.Setup(c => c.GetRequestStream()).Returns(Task.FromResult((Stream)new MemoryStream()));

            var factory = new Mock<IRequestFactory>();
            factory.Setup(c => c.Create(It.Is<string>(s => s.Contains("tokens"))))
                .Returns(tokenListRequest.Object);
            factory.Setup(c => c.Create(It.Is<string>(s => !s.Contains("tokens"))))
                .Returns(request.Object);

            SwitcheoClient client = credentials ? new SwitcheoClient(new SwitcheoClientOptions()
            {
                ApiCredentials = new ApiCredentials(new PrivateKey(SamplePrivateKey.ToSecureString())),
                KeyType = BlockchainType.Neo,
                DefaultContractHash = SampleContractHash
            }) : new SwitcheoClient(new SwitcheoClientOptions()
            {
                DefaultContractHash = SampleContractHash
            });

            client.RequestFactory = factory.Object;
            return client;
        }

        private SwitcheoClient PrepareExceptionClient(string responseData, string exceptionMessage, int statusCode, bool credentials = false)
        {
            var expectedBytes = Encoding.UTF8.GetBytes(responseData);
            var responseStream = new MemoryStream();
            responseStream.Write(expectedBytes, 0, expectedBytes.Length);
            responseStream.Seek(0, SeekOrigin.Begin);

            var we = new WebException();
            var r = new HttpWebResponse();
            var re = new HttpResponseMessage();

            typeof(HttpResponseMessage).GetField("_statusCode", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(re, (HttpStatusCode)statusCode);
            typeof(HttpWebResponse).GetField("_httpResponseMessage", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(r, re);
            typeof(WebException).GetField("_message", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(we, exceptionMessage);
            typeof(WebException).GetField("_response", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(we, r);

            var response = new Mock<IResponse>();
            response.Setup(c => c.GetResponseStream()).Throws(we);

            var request = new Mock<IRequest>();
            request.Setup(c => c.Headers).Returns(new WebHeaderCollection());
            request.Setup(c => c.GetResponse()).Returns(Task.FromResult(response.Object));

            var factory = new Mock<IRequestFactory>();
            factory.Setup(c => c.Create(It.IsAny<string>()))
                .Returns(request.Object);

            SwitcheoClient client = credentials ? new SwitcheoClient(
                new SwitcheoClientOptions()
                {
                    ApiCredentials = new ApiCredentials(new PrivateKey(SamplePrivateKey.ToSecureString())),
                    KeyType = BlockchainType.Neo
                }) : new SwitcheoClient();

            client.RequestFactory = factory.Object;
            return client;
        }
    }
}
