using CryptoExchange.Net;
using NeoModules.Core;
using NeoModules.KeyPairs;
using NeoModules.NEP6.Models;
using Switcheo.Net.Objects;
using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Cryptography;
using System.Threading;
using Helper = NeoModules.KeyPairs.Helper;

namespace Switcheo.Net.Helpers
{
    public static class WalletsHelper
    {
        private static readonly ThreadLocal<SHA256> _sha256 = new ThreadLocal<SHA256>(SHA256.Create);

        public static SecureString ToSecureString(this string privateKey)
        {
            if (string.IsNullOrEmpty(privateKey))
                throw new Exception("privateKey can't be null or empty");

            unsafe
            {
                fixed (char* privateKeyChars = privateKey)
                {
                    var securePassword = new SecureString(privateKeyChars, privateKey.Length);
                    securePassword.MakeReadOnly();
                    return securePassword;
                }
            }
        }

        public static bool IsHex(IEnumerable<char> chars)
        {
            bool isHex;
            foreach (var c in chars)
            {
                isHex = ((c >= '0' && c <= '9') ||
                         (c >= 'a' && c <= 'f') ||
                         (c >= 'A' && c <= 'F'));

                if (!isHex)
                    return false;
            }
            return true;
        }

        public static SecureString ConvertToHex(SecureString privateKeyWif)
        {
            return Wallet.GetPrivateKeyFromWif(privateKeyWif.GetString()).ToHexString().ToSecureString();
        }

        public static SecureString ConvertToWif(SecureString privateKey)
        {
            return new KeyPair(privateKey.GetString().HexToBytes()).Export().ToSecureString();
        }

        public static WalletInformations GetWalletInformations(SecureString privateKey, BlockchainType keyType)
        {
            WalletInformations walletInformations = new WalletInformations();

            switch (keyType)
            {
                case BlockchainType.Neo:
                    var keyPair = new KeyPair(privateKey.GetString().HexToBytes());

                    string publicKey = keyPair.PublicKey.ToString();
                    UInt160 scriptHash = Helper.CreateSignatureRedeemScript(keyPair.PublicKey).ToScriptHash();

                    // This is a basic NEO address
                    string address = scriptHash.ToAddress();
                    // This is a derivative of script hash (required by Switcheo)
                    string fixedAddress = scriptHash.ToString().RemoveZeroX();

                    walletInformations = new WalletInformations()
                    {
                        Wif = keyPair.Export().ToSecureString(),
                        PublicKey = publicKey,
                        ScriptHash = scriptHash.ToString(),
                        Address = address,
                        FixedAddress = fixedAddress
                    };
                    break;

                case BlockchainType.Qtum:
                    throw new NotImplementedException();

                case BlockchainType.Ethereum:
                    throw new NotImplementedException();
            }

            return walletInformations;
        }

        public class WalletInformations
        {
            public SecureString Wif { get; set; }
            public string PublicKey { get; set; }
            public string ScriptHash { get; set; }
            public string Address { get; set; }
            public string FixedAddress { get; set; }
        }

        #region Neo	

        // Forked from NeonJS : https://github.com/CityOfZion/neon-js/blob/cdd6a1aa6303c858943f66f9c8f296acf7c9dcbc/src/wallet/verify.js	
        public static bool IsNep2(SecureString privateKey)
        {
            try
            {
                if (privateKey.Length != 58) return false;
                string hexStr = Base58.Decode(privateKey.GetString()).ToHexString();
                if (string.IsNullOrEmpty(hexStr)) return false;
                if (hexStr.Length != 86) return false;
                if (hexStr.Substring(0, 2) != "01") return false;
                if (hexStr.Substring(2, 2) != "42") return false;
                if (hexStr.Substring(4, 2) != "e0") return false;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Forked from NeoLux : https://github.com/CityOfZion/neo-lux/blob/4a1ec92a5392cdf7b2b410205812096e862fc52c/Neo.Lux/Cryptography/KeyPair.cs
        public static byte[] GetScriptHashFromAddress(this string address)
        {
            var temp = address.Base58CheckDecode();
            temp = temp.SubArray(1, 20);
            return temp;
        }

        #endregion
    }
}
