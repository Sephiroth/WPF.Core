using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common
{
    public class BluetoothUtils : IDisposable
    {
        private readonly BluetoothClient _client;
        /// <summary>
        /// 是否已开始持续扫描附件设备
        /// </summary>
        private bool _isStartScan;

        private bool disposedValue;

        public List<BluetoothDeviceInfo> BleList { get; private set; }

        public BluetoothUtils()
        {
            _client = new BluetoothClient();
            BleList = new List<BluetoothDeviceInfo>(30);
        }

        /// <summary>
        /// 查询附近所有蓝牙设备
        /// </summary>
        /// <param name="scanSeconds">扫描时间</param>
        /// <returns></returns>
        public List<BluetoothDeviceInfo> ScanDevice(int scanSeconds = 10)
        {
            //_client.InquiryLength = TimeSpan.FromSeconds(scanSeconds);
            List<BluetoothDeviceInfo> devs = _client.DiscoverDevices(255).ToList();
            int len = devs.Count;
            for (int i = 0; i < len; i++)
            {
                if (BleList.Any(s => s.DeviceAddress.ToUInt64() == devs[i].DeviceAddress.ToUInt64()) == false)
                {
                    BleList.Add(devs[i]);
                }
            }
            return BleList;
        }

        public void StartScan()
        {
            if (_isStartScan) { return; }
            _isStartScan = true;
            _ = Task.Factory.StartNew(async () =>
            {
                while (!disposedValue)
                {
                    _ = ScanDevice();
                    await Task.Delay(1000);
                }
            });

        }

        /// <summary>
        /// 检查附近是否存在name设备
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool CheckDevice(string name)
        {
            //List<BluetoothDeviceInfo> devs = ScanDevice();
            //if (devs.Count < 1) { return false; }
            return BleList.Any(s => s.DeviceName.Equals(name));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    _client.Dispose();
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~BluetoothUtils()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #region 单例
        private static readonly object _lock = new object();
        private static BluetoothUtils _instance;

        public static BluetoothUtils Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new BluetoothUtils();
                        _instance.StartScan();
                    }
                }
                return _instance;
            }
            private set { }
        }

        #endregion
    }
}