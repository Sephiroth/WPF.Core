using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class BluetoothHelper : IDisposable
    {
        private bool disposedValue;
        private BluetoothClient client;
        private IReadOnlyCollection<BluetoothDeviceInfo> devices;
        private BluetoothDeviceInfo curDevice;

        public BluetoothHelper()
        {
            client = new BluetoothClient();
        }

        public void InTheHandBluetoothLE()
        {
            devices = client.DiscoverDevices();
            curDevice = devices.First();
            if (!curDevice.Connected)
            {
                client.Connect(curDevice.DeviceAddress, Guid.NewGuid());
            }
            string name = curDevice.DeviceName;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    if (client != null)
                    {
                        if (client.Connected) { client.Close(); }
                        client?.Dispose();
                    }
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~BluetoothHelper()
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
    }
}