using ShowingAds.CoreLibrary.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Timers;

namespace ShowingAds.DevicesService.BusinessLayer
{
    public class RequestsFilter : Singleton<RequestsFilter>
    {
        private object _syncLock { get; }
        private Dictionary<IPAddress, DeviceRequests> _requests { get; set; }

        private RequestsFilter()
        {
            _syncLock = new object();
            _requests = new Dictionary<IPAddress, DeviceRequests>();
        }

        public bool IsBannedDevice(IPAddress iPAddress)
        {
            lock (_syncLock)
            {
                if (_requests.ContainsKey(iPAddress) == false)
                {
                    var deviceRequests = new DeviceRequests(iPAddress);
                    deviceRequests.BanTimeIsOver += RemoveIpAddress;
                    _requests.Add(iPAddress, deviceRequests);
                }
                var request = _requests[iPAddress];
                return request.IPAddressIsBanned();
            }
        }

        private void RemoveIpAddress(IPAddress obj)
        {
            lock (_syncLock)
            {
                _requests.Remove(obj);
            }
        }

        private class DeviceRequests
        {
            private static readonly int _banTimeMinutes = 5;
            private static readonly byte _maxRequestsPerSecond = 2;

            public event Action<IPAddress> BanTimeIsOver;

            private bool _isBanned { get; set; }
            private byte _requestsPerSecond { get; set; }
            private Timer _timerBan { get; set; }

            public DeviceRequests(IPAddress iPAddress)
            {
                if (iPAddress == null)
                    throw new ArgumentNullException(nameof(iPAddress));
                _requestsPerSecond = 0;
                _isBanned = false;
                _timerBan = new Timer();
                _timerBan.AutoReset = false;
                _timerBan.Interval = TimeSpan.FromSeconds(1).TotalMilliseconds;
                _timerBan.Elapsed += (s, e) => BanTimeIsOver?.Invoke(iPAddress);
                _timerBan.Start();
            }

            public bool IPAddressIsBanned()
            {
                if (_isBanned == false)
                {
                    _requestsPerSecond++;
                    if (_requestsPerSecond > _maxRequestsPerSecond)
                    {
                        _timerBan.Interval = TimeSpan.FromMinutes(_banTimeMinutes).TotalMilliseconds;
                        _isBanned = true;
                    }
                }
                return _isBanned;
            }
        }
    }
}
