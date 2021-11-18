using HidLibrary;

namespace ArctisBattery
{
    public class Arctis
    {
        public int CheckBattery()
        {
            ushort vendorId = 0x1038;
            List<ushort> productIds = new()
            { 
                0x12AD, 
                0x1260, 
                0x1252, 
                0x12B3, 
                0x12C2 
            };
            
            List<HidDevice> devices = new();

            productIds.ForEach(productId =>
            {
                devices.AddRange(HidDevices.Enumerate(vendorId, productId));
            });

            if (devices.Any())
            {
                var device = devices.First(x => x.Capabilities.OutputReportByteLength > 0);
                var outData = new byte[device.Capabilities.OutputReportByteLength - 1];


                outData[0] = 0x06;
                outData[1] = 0x18;

                device.Write(outData);

                // Blocking read of report
                HidDeviceData inData = device.Read();
                var text = inData.Data[2];
                _ = int.TryParse(text.ToString(), out var percentageValue);
                return percentageValue;
            }
            return -1;
        }
    }
}