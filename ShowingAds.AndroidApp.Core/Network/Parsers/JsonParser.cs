using Newtonsoft.Json;
using ShowingAds.AndroidApp.Core.Network.Parsers.Interfaces;
using ShowingAds.AndroidApp.Core.Network.WebClientCommands.Filters;
using ShowingAds.CoreLibrary.Models.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShowingAds.AndroidApp.Core.Network.Parsers
{
    public class JsonParser : IParser
    {
        public event Action<BaseFilter> LogotypesParsed;
        public event Action<string, TimeSpan> TickerParsed;
        public event Action<TimeSpan> RebootTimeParsed;
        public event Action<BaseFilter> ContentsParsed;
        public event Action<BaseFilter> AdvertisingParsed;
        public event Action<BaseFilter> AdvertisingIntervalsParsed;
        public event Action<BaseFilter> AdvertisingOrdersParsed;

        public void Parse(string json)
        {
            if (json == string.Empty)
            {
                LogotypesParsed?.Invoke(new LogotypeFilter(Guid.Empty, Guid.Empty));
                TickerParsed?.Invoke(string.Empty, TimeSpan.Zero);
                RebootTimeParsed?.Invoke(TimeSpan.Zero);
                ContentsParse(new List<ContentJson>());
                AdvertisingParse(new List<ClientChannelJson>());
                AdvertisingOrdersParse(new List<OrderJson>());
            }
            else
            {
                var response = JsonConvert.DeserializeObject<ChannelJson>(json);
                LogotypesParsed?.Invoke(new LogotypeFilter(response.LogoLeft, response.LogoRight));
                TickerParsed?.Invoke(response.Ticker, response.TickerInterval);
                RebootTimeParsed?.Invoke(response.ReloadTime);
                ContentsParse(response.Contents.OrderBy(x => x.Id).ToList());
                AdvertisingParse(response.Clients);
                AdvertisingOrdersParse(response.Orders);
            }
        }

        private void ContentsParse(List<ContentJson> contents)
        {
            var validVideos = new List<(Guid, VideoJson)>();
            foreach (var content in contents)
                foreach (var video in content.Videos)
                    validVideos.Add((content.Id, video));
            ContentsParsed?.Invoke(new VideoFilter(validVideos));
        }

        private void AdvertisingParse(List<ClientChannelJson> clients)
        {
            var validVideos = new List<(Guid, VideoJson)>();
            var validIntervals = new List<(Guid, TimeSpan)>();
            foreach (var client in clients)
            {
                foreach (var video in client.Videos)
                    validVideos.Add((client.Id, video));
                validIntervals.Add((client.Id, client.Interval));
            }
            AdvertisingParsed?.Invoke(new VideoFilter(validVideos));
            AdvertisingIntervalsParsed?.Invoke(new ClientIntervalFilter(validIntervals));
        }

        private void AdvertisingOrdersParse(List<OrderJson> orders)
        {
            var validOrders = new List<(Guid, DateTime)>();
            foreach (var order in orders)
                validOrders.Add((order.Id, order.OrderField));
            AdvertisingOrdersParsed?.Invoke(new ClientOrderFilter(validOrders));
        }
    }
}
