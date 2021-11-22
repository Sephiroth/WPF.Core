using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace Common
{
    public class BleUtils : IDisposable
    {
        public ConcurrentDictionary<string, Windows.Devices.Bluetooth.BluetoothLEDevice> DeviceList { get; private set; }
        /// <summary>
        /// 搜索蓝牙设备对象
        /// </summary>
        private BluetoothLEAdvertisementWatcher watcher;
        private BleCore bleCore;
        private bool disposedValue;

        public BleUtils()
        {
            DeviceList = new ConcurrentDictionary<string, BluetoothLEDevice>();
            bleCore = new BleCore();
            bleCore.MessAgeLog += BleCore_MessAgeLog;
            bleCore.DeviceScan += BleCore_DeviceScan;
            bleCore.DeviceFindService += BleCore_DeviceFindService;
            bleCore.ReceiveNotification += BleCore_ReceiveNotification;
            bleCore.DeviceConnectionStatus += BleCore_DeviceConnectionStatus;
        }

        public string[] GetDiscoverName()
        {
            return DeviceList.Keys.ToArray();
        }

        /// <summary>
        /// 检查蓝牙是否存在
        /// </summary>
        /// <param name="name">蓝牙名前缀</param>
        /// <param name="end">蓝牙名以SN结尾</param>
        /// <param name="scanSeconds"></param>
        /// <returns></returns>
        public async Task<(bool, string)> CheckBleName(string name, string end, int scanSeconds = 10)
        {
            //bool exist = false;
            //int delay = 3000;
            //int times = scanSeconds / 3;
            //for (int i = 0; i < times; i++)
            //{
            //    await Task.Delay(delay);
            //    exist = DeviceList.ContainsKey(name);
            //    if (exist) { break; }
            //}
            await Task.Delay(scanSeconds * 1000);
            List<string> names = DeviceList.Keys.ToList();
            string devName = names.FirstOrDefault(s => s.StartsWith(name) && s.EndsWith(end));
            return (string.IsNullOrEmpty(devName) == false, devName);
        }

        public void StartScan()
        {
            bleCore.StartBleDeviceWatcher();
        }

        public void StopScan()
        {
            bleCore.StopBleDeviceWatcher();
        }

        /// <summary>
        /// 日志消息
        /// </summary>
        private void BleCore_MessAgeLog(MsgType type, string message, byte[] data)
        {
            //RunAsync(() =>
            //{
            //    this.listboxMessage.Items.Add(DateTime.Now.ToString("HH:mm:ss.fff", DateTimeFormatInfo.InvariantInfo) + ": " + message);
            //    this.listboxMessage.SelectedIndex = this.listboxMessage.Items.Count - 1;
            //});
        }

        /// <summary>
        /// 搜索蓝牙设备列表
        /// </summary>
        private void BleCore_DeviceScan(Windows.Devices.Bluetooth.BluetoothLEDevice bleDev)
        {
            bool exist = DeviceList.TryGetValue(bleDev.Name, out Windows.Devices.Bluetooth.BluetoothLEDevice outDev);
            if (exist) { return; }
            bool addRs = DeviceList.TryAdd(bleDev.Name, bleDev);
        }

        private void BleCore_DeviceFindService(DeviceService deviceService)
        {
            //deviceServices.Add(deviceService);
            //listViewServer.Items.Add(new ListViewItem(deviceService.gattDeviceService.Uuid.ToString())
            //{
            //    Tag = deviceService
            //});
        }

        private void BleCore_ReceiveNotification(GattCharacteristic sender, byte[] data)
        {
            //textBox_Recv.AppendText("\r\n" + sender.Uuid.ToString() + "\r\n");
            //string str = System.Text.Encoding.UTF8.GetString(data);
            //textBox_Recv.AppendText(str);
        }

        public void BleCore_DeviceConnectionStatus(BluetoothConnectionStatus status)
        {
            //if (status == BluetoothConnectionStatus.Disconnected)
            //{
            //    deviceServices.Clear();
            //    writeCharacteristic.Clear();
            //    notifyCharacteristics.Clear();
            //}
        }

        private void connectBle()
        {
            ////count = 0;
            ////this.bleCore.StopBleDeviceWatcher();
            //if (listviewBleDevice.SelectedItems.Count != 0)
            //{
            //    var deviceItem = listviewBleDevice.SelectedItems[0];
            //    BluetoothLEDevice device = (BluetoothLEDevice)deviceItem.Tag;
            //    if (device != null)
            //    {
            //        bleCore.CurrentDevice?.Dispose();
            //        deviceServices.Clear();
            //        notifyCharacteristics.Clear();
            //        writeCharacteristic.Clear();
            //        comboBoxWirteChart.Items.Clear();
            //        listViewServer.Items.Clear();
            //        listViewChar.Items.Clear();
            //        listViewCmd.Items.Clear();
            //        bleCore.StartMatching(device);
            //    }
            //    else
            //    {
            //        MessageBox.Show("没有发现此蓝牙，请重新搜索.");
            //        //this.btnServer.Enabled = false;
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("请选择连接的蓝牙.");
            //}
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    bleCore.Dispose();
                }
                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~BleUtils()
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
