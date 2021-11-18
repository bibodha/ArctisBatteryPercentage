using HidLibrary;

namespace ArctisBattery
{
    public class Arctis 
    {
        public int CheckBattery()
        {
            ushort vendorId = 0x1038;
            ushort productId = 0x12AD;

            var devices = HidDevices.Enumerate(vendorId, productId);
            if (devices.Any())
            {
                var device = devices.First(x => x.Capabilities.OutputReportByteLength > 0);
                var outData = new byte[device.Capabilities.OutputReportByteLength - 1];


                outData[0] = 0x06;
                outData[1] = 0x18;

                device.Write(outData);

                // Blocking read of report
                HidDeviceData InData;

                InData = device.Read();
                var text = InData.Data[2];
                _ = int.TryParse(text.ToString(), out var percentageValue);
                return percentageValue;
            }
            return -1;
        }
    }
}