using System;
using System.Collections;
using System.IO;
using System.Text;
using Hardware.Info;

static class PCM_Backend
{
    const int DATAPOINTS_PER_FILE = 10;
    static readonly IHardwareInfo hardwareInfo = new HardwareInfo();

    public static void Main()
    {
        DateTime lastLog = DateTime.Now;
        int dataPointCount = 0;
        float[] data = new float[DATAPOINTS_PER_FILE];

        while (true)
        {
            if (lastLog.AddSeconds(5) < DateTime.Now)
            {
                hardwareInfo.RefreshMemoryStatus();
                float memUsage = hardwareInfo.MemoryStatus.AvailablePhysical / (float)hardwareInfo.MemoryStatus.TotalPhysical;
                data[dataPointCount] = 1 - memUsage;

                Console.WriteLine(data[dataPointCount]);

                lastLog = DateTime.Now;
                dataPointCount++;

                if (dataPointCount >= DATAPOINTS_PER_FILE)
                {

                    int fileCount = Directory.GetFiles("../data/", "*.pcmd", SearchOption.TopDirectoryOnly).Length;

                    FileStream fstream = File.Open("../data/d" + fileCount.ToString() + ".pcmd", FileMode.Create);
                    BinaryWriter writer = new BinaryWriter(fstream, Encoding.UTF8, false);

                    for (int i = 0; i < DATAPOINTS_PER_FILE; i++)
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