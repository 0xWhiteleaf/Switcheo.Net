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

        public static string ToUnsecureString(this SecureString privateKey)
        {
            if (privateKey == null)
                throw new Exception("privateKey can't be null or empty");

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(privateKey);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
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
            return Wallet.GetPrivateKeyFromWif(privateKeyWif.ToUnsecureString()).ToHexString().ToSecureString();
        }

        public static SecureString ConvertToWif(SecureString privateKey)
        {
            return new KeyPair(privateKey.ToUnsecureString().HexToBytes()).Export().ToSecureString();
        }

        public static KeyValuePair<string, string> GetPublicKeyAndAddress(SecureString privateKey, BlockchainType keyType)
        {
            KeyValuePair<string, string> publicKeyAndAddress = new KeyValuePair<string, string>();

            switch (keyType)
            {
                case BlockchainType.Neo:
                    var keyPair = new KeyPair(privateKey.ToUnsecureString().HexToBytes());
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

        #region Neo

        // Forked from NeonJS : https://github.com/CityOfZion/neon-js/blob/cdd6a1aa6303c858943f66f9c8f296acf7c9dcbc/src/wallet/verify.js
        public static bool IsNep2(SecureString encryptedPrivateKey)
        {
            try
            {
                if (encryptedPrivateKey.Length != 58) return false;
                string hexStr = BitConverter.ToString(Base58.Decode(encryptedPrivateKey.ToUnsecureString()));
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

        // Forked from NeoModules : https://github.com/CityOfZion/NeoModules/blob/69c5582513674eb232eb147182b10e468d26138c/src/NeoModules.NEP6/Helpers/Utils.cs
        public static byte[] TranscodeSignatureToConcat(byte[] derSignature, int outputLength)
        {
            if (derSignature.Length < 8 || derSignature[0] != 48) throw new Exception("Invalid ECDSA signature format");

            int offset;
            if (derSignature[1] > 0)
                offset = 2;
            else if (derSignature[1] == 0x81)
                offset = 3;
            else
                throw new Exception("Invalid ECDSA signature format");

            var rLength = derSignature[offset + 1];

            int i = rLength;
            while (i > 0
                   && derSignature[offset + 2 + rLength - i] == 0)
                i--;

            var sLength = derSignature[offset + 2 + rLength + 1];

            int j = sLength;
            while (j > 0
                   && derSignature[offset + 2 + rLength + 2 + sLength - j] == 0)
                j--;

            var rawLen = Math.Max(i, j);
            rawLen = Math.Max(rawLen, outputLength / 2);

            if ((derSignature[offset - 1] & 0xff) != derSignature.Length - offset
                || (derSignature[offset - 1] & 0xff) != 2 + rLength + 2 + sLength
                || derSignature[offset] != 2
                || derSignature[offset + 2 + rLength] != 2)
                throw new Exception("Invalid ECDSA signature format");

            var concatSignature = new byte[2 * rawLen];

            Array.Copy(derSignature, offset + 2 + rLength - i, concatSignature, rawLen - i, i);
            Array.Copy(derSignature, offset + 2 + rLength + 2 + sLength - j, concatSignature, 2 * rawLen - j, j);

            return concatSignature;
        }

        #endregion
    }
}
