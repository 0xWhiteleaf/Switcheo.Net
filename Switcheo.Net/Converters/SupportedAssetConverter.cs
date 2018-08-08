using CryptoExchange.Net.Converters;
using Switcheo.Net.Helpers;
using Switcheo.Net.Objects;
using System.Collections.Generic;
using System.Linq;

namespace Switcheo.Net.Converters
{
    public class SupportedAssetConverter : BaseConverter<SupportedAsset>
    {
        private bool IsAssetId;

        public SupportedAssetConverter() : this(false, true) { }
        public SupportedAssetConverter(bool isAssetId) : this(isAssetId, true) { }
        public SupportedAssetConverter(bool isAssetId, bool quotes) : base(quotes)
        {
            this.IsAssetId = isAssetId;
        }

        protected override Dictionary<SupportedAsset, string> Mapping => GetMapping();

        private Dictionary<SupportedAsset, string> GetMapping()
        {
            Dictionary<SupportedAsset, string> mapping = null;

            if (!this.IsAssetId)
            {
                mapping = new Dictionary<SupportedAsset, string>()
                {
                    { SupportedAsset.Neo, SupportedAsset.Neo.GetSymbol() },
                    { SupportedAsset.Gas, SupportedAsset.Gas.GetSymbol() },
                    { SupportedAsset.Swth, SupportedAsset.Swth.GetSymbol() },
                    { SupportedAsset.Acat, SupportedAsset.Acat.GetSymbol() },
                    { SupportedAsset.Aph, SupportedAsset.Aph.GetSymbol() },
                    { SupportedAsset.Ava, SupportedAsset.Ava.GetSymbol() },
                    { SupportedAsset.Cpx, SupportedAsset.Cpx.GetSymbol() },
                    { SupportedAsset.Dbc, SupportedAsset.Dbc.GetSymbol() },
                    { SupportedAsset.Efx, SupportedAsset.Efx.GetSymbol() },
                    { SupportedAsset.Gala, SupportedAsset.Gala.GetSymbol() },
                    { SupportedAsset.Lrn, SupportedAsset.Lrn.GetSymbol() },
                    { SupportedAsset.Mct, SupportedAsset.Mct.GetSymbol() },
                    { SupportedAsset.Nkn, SupportedAsset.Nkn.GetSymbol() },
                    { SupportedAsset.Obt, SupportedAsset.Obt.GetSymbol() },
                    { SupportedAsset.Ont, SupportedAsset.Ont.GetSymbol() },
                    { SupportedAsset.Pkc, SupportedAsset.Pkc.GetSymbol() },
                    { SupportedAsset.Rht, SupportedAsset.Rht.GetSymbol() },
                    { SupportedAsset.Rpx, SupportedAsset.Rpx.GetSymbol() },
                    { SupportedAsset.Tky, SupportedAsset.Tky.GetSymbol() },
                    { SupportedAsset.Tnc, SupportedAsset.Tnc.GetSymbol() },
                    { SupportedAsset.Toll, SupportedAsset.Toll.GetSymbol() },
                    { SupportedAsset.Qlc, SupportedAsset.Qlc.GetSymbol() },
                    { SupportedAsset.Soul, SupportedAsset.Soul.GetSymbol() },
                    { SupportedAsset.Swh, SupportedAsset.Swh.GetSymbol() }
                };
            }
            else
            {
                mapping = new Dictionary<SupportedAsset, string>()
                {
                    { SupportedAsset.Neo, SupportedAsset.Neo.GetId() },
                    { SupportedAsset.Gas, SupportedAsset.Gas.GetId() },
                    { SupportedAsset.Swth, SupportedAsset.Swth.GetId() },
                    { SupportedAsset.Acat, SupportedAsset.Acat.GetId() },
                    { SupportedAsset.Aph, SupportedAsset.Aph.GetId() },
                    { SupportedAsset.Ava, SupportedAsset.Ava.GetId() },
                    { SupportedAsset.Cpx, SupportedAsset.Cpx.GetId() },
                    { SupportedAsset.Dbc, SupportedAsset.Dbc.GetId() },
                    { SupportedAsset.Efx, SupportedAsset.Efx.GetId() },
                    { SupportedAsset.Gala, SupportedAsset.Gala.GetId() },
                    { SupportedAsset.Lrn, SupportedAsset.Lrn.GetId() },
                    { SupportedAsset.Mct, SupportedAsset.Mct.GetId() },
                    { SupportedAsset.Nkn, SupportedAsset.Nkn.GetId() },
                    { SupportedAsset.Obt, SupportedAsset.Obt.GetId() },
                    { SupportedAsset.Ont, SupportedAsset.Ont.GetId() },
                    { SupportedAsset.Pkc, SupportedAsset.Pkc.GetId() },
                    { SupportedAsset.Rht, SupportedAsset.Rht.GetId() },
                    { SupportedAsset.Rpx, SupportedAsset.Rpx.GetId() },
                    { SupportedAsset.Tky, SupportedAsset.Tky.GetId() },
                    { SupportedAsset.Tnc, SupportedAsset.Tnc.GetId() },
                    { SupportedAsset.Toll, SupportedAsset.Toll.GetId() },
                    { SupportedAsset.Qlc, SupportedAsset.Qlc.GetId() },
                    { SupportedAsset.Soul, SupportedAsset.Soul.GetId() },
                    { SupportedAsset.Swh, SupportedAsset.Swh.GetId() }
                };
            }

            return mapping;
        }

        public SupportedAsset GetSupportedAssetFromString(string value, bool isAssetId = false)
        {
            this.IsAssetId = isAssetId;

            var supportedAsset = SupportedAsset.Unknown;

            var pair = this.Mapping.FirstOrDefault(m => m.Value == value);
            if (!pair.IsDefault())
                supportedAsset = pair.Key;

            return supportedAsset;
        }
    }
}
