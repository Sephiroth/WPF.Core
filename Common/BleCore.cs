using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Foundation;
using Windows.Security.Cryptography;

namespace Common
{
    internal class BleCore
    {
        private static readonly object _lock = new object();

        private bool asyncLock = false;

        /// <summary>
        /// 搜索蓝牙设备对象
        /// </summary>
        private BluetoothLEAdvertisementWatcher watcher;

        /// <summary>
        /// 当前连接的蓝牙设备
        /// </summary>
        public BluetoothLEDevice CurrentDevice { get; set; }

        /// <summary>
        /// 特性通知类型通知启用
        /// </summary>
        private const GattClientCharacteristicConfigurationDescriptorValue CHARACTERISTIC_NOTIFICATION_TYPE = GattClientCharacteristicConfigurationDescriptorValue.Notify;

        /// <summary>
        /// 存储检测到的设备
        /// </summary>
        private List<BluetoothLEDevice> DeviceList = new List<BluetoothLEDevice>();

        /// <summary>
        /// 搜索蓝牙设备委托
        /// </summary>
        public delegate void DeviceScanEvent(BluetoothLEDevice bluetoothLEDevice);

        /// <summary>
        /// 搜索蓝牙事件
        /// </summary>
        public event DeviceScanEvent DeviceScan;

        /// <summary>
        /// 提示信息委托
        /// </summary>
        public delegate void MessAgeLogEvent(MsgType type, string message, byte[] data = null);

        /// <summary>
        /// 提示信息事件
        /// </summary>
        public event MessAgeLogEvent MessAgeLog;

        /// <summary>
        /// 接收通知委托
        /// </summary>
        public delegate void ReceiveNotificationEvent(GattCharacteristic sender, byte[] data);

        /// <summary>
        /// 接收通知事件
        /// </summary>
        public event ReceiveNotificationEvent ReceiveNotification;

        /// <summary>
        /// 获取服务委托
        /// </summary>
        public delegate void DeviceFindServiceEvent(DeviceService deviceService);

        /// <summary>
        /// 获取服务事件
        /// </summary>
        public event DeviceFindServiceEvent DeviceFindService;


        /// <summary>
        /// 蓝牙状态委托
        /// </summary>
        public delegate void DeviceConnectionStatusEvent(BluetoothConnectionStatus status);

        /// <summary>
        /// 蓝牙状态事件
        /// </summary>
        public event DeviceConnectionStatusEvent DeviceConnectionStatus;


        /// <summary>
        /// 当前连接的蓝牙Mac
        /// </summary>
        private string CurrentDeviceMAC { get; set; }

        public BleCore()
        {

        }

        /// <summary>
        /// 搜索蓝牙设备
        /// </summary>
        public void StartBleDeviceWatcher()
        {
            watcher = new BluetoothLEAdvertisementWatcher();

            watcher.ScanningMode = BluetoothLEScanningMode.Active;

            // only activate the watcher when we're recieving values >= -80
            watcher.SignalStrengthFilter.InRangeThresholdInDBm = -80;

            // stop watching if the value drops below -90 (user walked away)
            watcher.SignalStrengthFilter.OutOfRangeThresholdInDBm = -90;

            // register callback for when we see an advertisements
            watcher.Received += OnAdvertisementReceived;

            // wait 5 seconds to make sure the device is really out of range
            watcher.SignalStrengthFilter.OutOfRangeTimeout = TimeSpan.FromMilliseconds(5000);
            watcher.SignalStrengthFilter.SamplingInterval = TimeSpan.FromMilliseconds(2000);

            // starting watching for advertisements
            lock (_lock)
                this.DeviceList.Clear();
            watcher.Start();
            string msg = "开始搜索蓝牙设备...";

            //this.MessAgeLog(MsgType.NotifyTxt, msg);
        }

        /// <summary>
        /// 停止搜索蓝牙
        /// </summary>
        public void StopBleDeviceWatcher()
        {
            this.watcher.Stop();
        }

        private void OnAdvertisementReceived(BluetoothLEAdvertisementWatcher watcher, BluetoothLEAdvertisementReceivedEventArgs eventArgs)
        {
            //this.MessAgeChanged(MsgType.NotifyTxt, "发现设备FR_NAME:"+ eventArgs.Advertisement.LocalName + "BT_ADDR: " + eventArgs.BluetoothAddress);
            BluetoothLEDevice.FromBluetoothAddressAsync(eventArgs.BluetoothAddress).Completed = async (asyncInfo, asyncStatus) =>
            {
                if (asyncStatus == AsyncStatus.Completed)
                {
                    if (asyncInfo.GetResults() != null)
                    {
                        BluetoothLEDevice currentDevice = asyncInfo.GetResults();

                        Boolean contain = false;
                        lock (_lock)
                        {
                            foreach (BluetoothLEDevice device in DeviceList)//过滤重复的设备
                            {
                                if (device.BluetoothAddress == currentDevice.BluetoothAddress)
                                {
                                    contain = true;
                                }
                            }
                        }
                        if (!contain)
                        {
                            byte[] _Bytes1 = BitConverter.GetBytes(currentDevice.BluetoothAddress);
                            Array.Reverse(_Bytes1);
                            lock (_lock)
                                this.DeviceList.Add(currentDevice);
                            string str;
                            if (currentDevice.DeviceInformation.Name != "")
                            {
                                str = "发现设备：" + currentDevice.DeviceInformation.Name + " - " + BitConverter.ToString(_Bytes1, 2, 6).Replace('-', ':').ToUpper();
                            }
                            else
                            {
                                str = "发现设备：" + BitConverter.ToString(_Bytes1, 2, 6).Replace('-', ':').ToUpper();
                            }
                            this.MessAgeLog(MsgType.NotifyTxt, str);
                            this.DeviceScan(currentDevice);
                        }
                    }

                }
            };
        }


        /// <summary>
        /// 匹配
        /// </summary>
        /// <param name="Device"></param>
        public void StartMatching(BluetoothLEDevice Device)
        {
            this.CurrentDevice?.Dispose();
            this.CurrentDevice = Device;
            this.Connect();
            FindService();
        }

        /// <summary>
        /// 获取蓝牙服务
        /// </summary>
        public async void FindService()
        {
            this.MessAgeLog(MsgType.NotifyTxt, "开始获取服务列表");
            this.CurrentDevice.GetGattServicesAsync().Completed = async (asyncInfo, asyncStatus) =>
            {
                if (asyncStatus == AsyncStatus.Completed)
                {
                    var services = asyncInfo.GetResults().Services;
                    //this.MessAgeChanged(MsgType.NotifyTxt, "GattServices size=" + services.Count);
                    foreach (GattDeviceService ser in services)
                    {
                        FindCharacteristic(ser);
                    }
                }
            };
        }

        /// <summary>
        /// 获取特性
        /// </summary>
        public async void FindCharacteristic(GattDeviceService gattDeviceService)
        {
            IAsyncOperation<GattCharacteristicsResult> result = gattDeviceService.GetCharacteristicsAsync();
            result.Completed = async (asyncInfo, asyncStatus) =>
            {
                if (asyncStatus == AsyncStatus.Completed)
                {
                    var characters = asyncInfo.GetResults().Characteristics;
                    List<GattCharacteristic> characteristics = new List<GattCharacteristic>(characters.Count);
                    foreach (GattCharacteristic characteristic in characters)
                    {
                        characteristics.Add(characteristic);
                        this.MessAgeLog(MsgType.NotifyTxt, "服务UUID:" + gattDeviceService.Uuid.ToString() + " --- 特征UUID:" + characteristic.Uuid.ToString());
                    }

                    DeviceService deviceService = new DeviceService();
                    deviceService.gattDeviceService = gattDeviceService;
                    deviceService.gattCharacteristic = characteristics;
                    this.DeviceFindService(deviceService);
                }
            };
        }

        /// <summary>
        /// 获取操作
        /// </summary>
        /// <returns></returns>
        public async Task SetNotify(GattCharacteristic gattCharacteristic)
        {
            GattCharacteristic CurrentNotifyCharacteristic;
            if ((gattCharacteristic.CharacteristicProperties & GattCharacteristicProperties.Notify) != 0)
            {
                CurrentNotifyCharacteristic = gattCharacteristic;
                CurrentNotifyCharacteristic.ProtectionLevel = GattProtectionLevel.Plain;
                CurrentNotifyCharacteristic.ValueChanged += Characteristic_ValueChanged;
                await this.EnableNotifications(CurrentNotifyCharacteristic);
            }
        }

        /// <summary>
        /// 连接蓝牙
        /// </summary>
        /// <returns></returns>
        private async Task Connect()
        {
            byte[] _Bytes1 = BitConverter.GetBytes(this.CurrentDevice.BluetoothAddress);
            Array.Reverse(_Bytes1);
            this.CurrentDeviceMAC = BitConverter.ToString(_Bytes1, 2, 6).Replace('-', ':').ToLower();

            string msg = "正在连接设备:" + this.CurrentDeviceMAC.ToUpper() + " ...";
            this.MessAgeLog(MsgType.NotifyTxt, msg);
            this.CurrentDevice.ConnectionStatusChanged += this.CurrentDevice_ConnectionStatusChanged;
        }

        /// <summary>
        /// 搜索到的蓝牙设备
        /// </summary>
        /// <returns></returns>
        private async Task Matching(string Id)
        {
            try
            {
                BluetoothLEDevice.FromIdAsync(Id).Completed = async (asyncInfo, asyncStatus) =>
                {
                    if (asyncStatus == AsyncStatus.Completed)
                    {
                        BluetoothLEDevice bleDevice = asyncInfo.GetResults();
                        lock (_lock)
                            this.DeviceList.Add(bleDevice);
                        this.DeviceScan(bleDevice);
                        this.CurrentDevice = bleDevice;
                        FindService();
                    }
                };
            }
            catch (Exception e)
            {
                string msg = "没有发现设备" + e.ToString();
                this.MessAgeLog(MsgType.NotifyTxt, msg);
                this.StartBleDeviceWatcher();
            }
        }

        /// <summary>
        /// 主动断开连接
        /// </summary>
        /// <returns></returns>
        public void Dispose()
        {
            CurrentDeviceMAC = null;
            CurrentDevice?.Dispose();
            CurrentDevice = null;
            MessAgeLog(MsgType.NotifyTxt, "主动断开连接");
        }

        private void CurrentDevice_ConnectionStatusChanged(BluetoothLEDevice sender, object args)
        {
            this.DeviceConnectionStatus(sender.ConnectionStatus);
            if (sender.ConnectionStatus == BluetoothConnectionStatus.Disconnected && CurrentDeviceMAC != null)
            {
                string msg = "设备已断开,自动重连...";
                MessAgeLog(MsgType.NotifyTxt, msg);
                if (!asyncLock)
                {
                    asyncLock = true;
                    this.CurrentDevice.Dispose();
                    this.CurrentDevice = null;
                    SelectDeviceFromIdAsync(CurrentDeviceMAC);
                }
            }
            else
            {
                string msg = "设备已连接！";
                MessAgeLog(MsgType.NotifyTxt, msg);
            }
        }

        /// <summary>
        /// 按MAC地址直接组装设备ID查找设备
        /// </summary>
        public async Task SelectDeviceFromIdAsync(string MAC)
        {
            CurrentDeviceMAC = MAC;
            CurrentDevice = null;
            BluetoothAdapter.GetDefaultAsync().Completed = async (asyncInfo, asyncStatus) =>
            {
                if (asyncStatus == AsyncStatus.Completed)
                {
                    BluetoothAdapter mBluetoothAdapter = asyncInfo.GetResults();
                    byte[] _Bytes1 = BitConverter.GetBytes(mBluetoothAdapter.BluetoothAddress);//ulong转换为byte数组
                    Array.Reverse(_Bytes1);
                    string macAddress = BitConverter.ToString(_Bytes1, 2, 6).Replace('-', ':').ToLower();
                    string Id = "BluetoothLE#BluetoothLE" + macAddress + "-" + MAC;
                    await Matching(Id);
                }
            };
        }

        /// <summary>
        /// 设置特征对象为接收通知对象
        /// </summary>
        /// <param name="characteristic"></param>
        /// <returns></returns>
        public async Task EnableNotifications(GattCharacteristic characteristic)
        {
            if (CurrentDevice.ConnectionStatus != BluetoothConnectionStatus.Connected)
            {
                this.MessAgeLog(MsgType.NotifyTxt, "蓝牙未连接！");
                return;
            }

            characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(CHARACTERISTIC_NOTIFICATION_TYPE).Completed = async (asyncInfo, asyncStatus) =>
            {
                Console.WriteLine("asyncStatus:" + asyncStatus);
                if (asyncStatus == AsyncStatus.Completed)
                {
                    GattCommunicationStatus status = asyncInfo.GetResults();

                    asyncLock = false;
                    string msg = "Notify(" + characteristic.Uuid.ToString() + ")：" + status;
                    this.MessAgeLog(MsgType.NotifyTxt, msg);
                }
                else
                {
                    Console.WriteLine(asyncInfo.ErrorCode.ToString());
                }
            };
        }

        /// <summary>
        /// 接受到蓝牙数据
        /// </summary>
        private void Characteristic_ValueChanged(GattCharacteristic characteristic, GattValueChangedEventArgs args)
        {
            byte[] data;
            CryptographicBuffer.CopyToByteArray(args.CharacteristicValue, out data);
            string str = System.Text.Encoding.UTF8.GetString(data);
            this.ReceiveNotification(characteristic, data);

            this.MessAgeLog(MsgType.BleData, "收到数据(" + characteristic.Uuid.ToString() + ")：" + str);
        }

        /// <summary>
        /// 发送数据接口
        /// </summary>
        /// <returns></returns>
        public async Task Write(GattCharacteristic writeCharacteristic, byte[] data)
        {
            if (writeCharacteristic != null)
            {
                string str = "发送数据(" + writeCharacteristic.Uuid.ToString() + ")：" + BitConverter.ToString(data);
                this.MessAgeLog(MsgType.BleData, str, data);
                IAsyncOperation<GattCommunicationStatus> async = writeCharacteristic.WriteValueAsync(CryptographicBuffer.CreateFromByteArray(data), GattWriteOption.WriteWithResponse);
                //async.Completed = async (asyncInfo, asyncStatus) =>
                //{
                //    if (asyncStatus == AsyncStatus.Completed)
                //    {
                //        this.MessAgeLog(MsgType.BleData, "数据发送成功！");
                //    }
                //    else
                //    {
                //        this.MessAgeLog(MsgType.BleData, "数据发送失败：" + asyncInfo.ErrorCode.ToString());
                //    }
                //};
            }
        }
    }
    public enum MsgType
    {
        NotifyTxt,
        BleData,
        BleDevice
    }

    public class DeviceService
    {
        public GattDeviceService gattDeviceService;
        public List<GattCharacteristic> gattCharacteristic;
    }
}