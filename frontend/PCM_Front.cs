using Raylib_cs;
using System;
using System.Collections;
using System.IO;
using System.Text;


static class PCM_Frontend
{
    const int DATAPOINTS_PER_FILE = 10;
    const int WIDTH = 800;
    const int HEIGHT = 480;

    
    public static void Main()
    {
        Raylib.InitWindow(WIDTH, HEIGHT, "PC-Metrics");

        float[] data = LoadData();

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.BLACK);

            for (int i = 0; i < data.Length; i++)
            {
                int blockHeight = (int)(data[i] * HEIGHT);
                int blockWidht = WIDTH / data.Length;
                Raylib.DrawRectangle(i * blockWidht, HEIGHT- blockHeight, blockWidht, blockHeight, Color.WHITE);
            }

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }

    public static float[] LoadData()
    {
        int fileCount = Directory.GetFiles("../data/", "*.pcmd", SearchOption.TopDirectoryOnly).Length;
        float[] data = new float[fileCount * DATAPOINTS_PER_FILE];
        for (int i = 0; i < fileCount; i++)
        {
            ReadDataFile(i).CopyTo(data, i * DATAPOINTS_PER_FILE);
        }

        return data;
    }

    public static float[] ReadDataFile(int index)
    {
        float[] ret = new float[DATAPOINTS_PER_FILE];
        string path = "../data/d" + index.ToString() + ".pcmd";

        FileStream fstream = File.Open(path, FileMode.Open);
        BinaryReader reader = new BinaryReader(fstream, Encoding.UTF8, false);
        
        for (int i = 0; i < DATAPOINTS_PER_FILE; i++)
        { 
            ret[i] = reader.ReadSingle();
        } 
        
        reader.Close();

        return ret;
    }
}