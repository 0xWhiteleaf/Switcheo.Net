using CryptoExchange.Net;
using NeoModules.Core;
using NeoModules.KeyPairs;
using NeoModules.NEP6.Models;
using Switcheo.Net.Objects;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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

        public static KeyValuePair<string, string> GetPublicKeyAndAddress(SecureString privateKey, BlockchainType keyType)
        {
            KeyValuePair<string, string> publicKeyAndAddress = new KeyValuePair<string, string>();

            switch (keyType)
            {
                case BlockchainType.Neo:
                    var keyPair = new KeyPair(privateKey.GetString().HexToBytes());
                    string publicKey = keyPair.PublicKey.ToString();
                    string scriptHash = Helper.CreateSignatureRedeemScript(keyPair.PublicKey).ToScriptHash().ToString();
                    string address = scriptHash.Substring(2, scriptHash.Length - 2);

                    publicKeyAndAddress = new KeyValuePair<string, string>(publicKey, address);
                    break;

                case BlockchainType.Qtum:
                    throw new NotImplementedException();

                case BlockchainType.Ethereum:
                    throw new NotImplementedException();
            }

            return publicKeyAndAddress;
        }
    }
}
