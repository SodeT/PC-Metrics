using System;
using System.Collections;
using System.IO;
using System.Text;
using Hardware.Info;

static class PCM_Backend
{
    const int DATA_PER_HOUR = 600;
    const int TIME_BETWEEN_DATA = 3600 / DATA_PER_HOUR;
    static readonly IHardwareInfo hardwareInfo = new HardwareInfo();

    public static void Main()
    {
        DateTime lastLog = DateTime.Now;
        int dataPointCount = 0;
        float[] data = new float[DATA_PER_HOUR];

        while (true)
        {
            if (lastLog.AddSeconds(TIME_BETWEEN_DATA) < DateTime.Now)
            {
                hardwareInfo.RefreshMemoryStatus();
                float memUsage = hardwareInfo.MemoryStatus.AvailablePhysical / (float)hardwareInfo.MemoryStatus.TotalPhysical;
                data[dataPointCount] = 1 - memUsage;

                lastLog = DateTime.Now;
                dataPointCount++;

                if (dataPointCount >= DATA_PER_HOUR)
                {

                    int fileCount = Directory.GetFiles("../data/", "*.pcmd", SearchOption.TopDirectoryOnly).Length;

                    FileStream fstream = File.Open("../data/h" + fileCount.ToString() + ".pcmd", FileMode.Create);
                    BinaryWriter writer = new BinaryWriter(fstream, Encoding.UTF8, false);

                    for (int i = 0; i < DATA_PER_HOUR; i++)
                    {
                        writer.Write(data[i]);
                    }

                    writer.Close();
                    dataPointCount = 0;
                }
            } 
        }
    }
}