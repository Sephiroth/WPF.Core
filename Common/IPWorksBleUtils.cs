using nsoftware.IPWorksBLE;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Common
{
    public class IPWorksBleUtils : IDisposable
    {
        private IContainer _container;
        private Bleclient _ble;
        private Bleclient.OnAdvertisementHandler _OnAdvertisementHandler;
        private Bleclient.OnDiscoveredHandler _OnDiscoveredHandler;

        public IPWorksBleUtils()
        {
            Init();
        }

        private void Init()
        {
            _container = new Container();
            _ble = new Bleclient(_container);
            _OnAdvertisementHandler = new Bleclient.OnAdvertisementHandler(BleOnAdvertisement);
            _ble.OnAdvertisement += _OnAdvertisementHandler;
            _OnDiscoveredHandler = new Bleclient.OnDiscoveredHandler(BleOnDiscoveredHandler);
            _ble.OnDiscovered += _OnDiscoveredHandler;
        }

        public void StartScan()
        {
            _ble.StartScanning(string.Empty);
        }

        private void BleOnAdvertisement(object sender, BleclientAdvertisementEventArgs e)
        {
            string info = $"Name:{e.Name};ServerId:{e.ServerId};ServiceUuids:{e.ServiceUuids};";
        }

        private void BleOnDiscoveredHandler(object sender, BleclientDiscoveredEventArgs e)
        {
            string info = $"ServiceId:{e.ServiceId};Uuid:{e.Uuid};ServiceUuids:{e.ServiceId};";
        }

        public void Dispose()
        {
            _ble.StopScanning();
            _ble.Dispose();
            _container.Dispose();
        }

    }
}