
using Raylib_cs;
using System;
using System.Numerics;
using System.Collections;
using System.IO;
using System.Text;


static class PCM_Frontend
{
    const int DATA_PER_HOUR = 60;
    const int WIDTH = 1600;
    const int HEIGHT = 900;
    const int BLOCK_WIDTH = 1;

    static Color graphColor = new Color(0, 255, 0, 255);
    static Color fillGraphColor = new Color(0, 255, 0, 55);
    static Color bgColor = new Color(41, 43, 44, 255);
    static Color textColor = new Color(230, 230, 230, 255);
    static Color buttonColor = new Color(65, 72, 96, 255);
    static Color gridColor = new Color(255, 255, 0, 105);

    public static void Main()
    {
        Raylib.InitWindow(WIDTH, HEIGHT, "PC-Metrics");

        float[] data = LoadData();
        float[] dataView = data;
        int dataViewLenth = data.Length;

        string[] buttonPresets = {"reload", "all", "1h", "6h", "12h", "24h"};
        Button[] btns = SpawnButtons(buttonPresets, 5);

        while (!Raylib.WindowShouldClose())
        {
            if (Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT))
            {
                Button btn = ButtonClicked(btns, Raylib.GetMousePosition());
                if (btn != null)
                {
                    switch (btn.Text)
                    {
                    case "reload":
                        if (dataViewLenth == data.Length)
                        {
                            data = LoadData();
                            dataViewLenth = data.Length;
                        }
                        break;
                    case "all":
                        dataViewLenth = data.Length;
                        break;
                    case "1h":
                        dataViewLenth = DATA_PER_HOUR;
                        break;
                    case "6h":
                        dataViewLenth = DATA_PER_HOUR * 6;
                        break;
                    case "12h":
                        dataViewLenth = DATA_PER_HOUR * 12;
                        break;
                    case "24h":
                        dataViewLenth = DATA_PER_HOUR * 24;
                        break;
                    default:
                        break;
                    }

                    dataViewLenth = (int)MathF.Min(data.Length, dataViewLenth);
                    dataView = new float[dataViewLenth];
                    int j = 0;
                    for (int i = data.Length - dataViewLenth; i < data.Length; i++, j++)
                    {
                        dataView[j] = data[i];
                    }
                }
            }

            Raylib.BeginDrawing();

            Raylib.ClearBackground(bgColor);


            int blockCount = WIDTH / BLOCK_WIDTH;
            int prevx = 0;
            int prevy = 0;
            for (int i = 0; i < blockCount; i++)
            {
                int dataPoint = (int)(((float)i/blockCount) * dataView.Length);
                int blockHeight = (int)(dataView[dataPoint] * HEIGHT);

                Raylib.DrawRectangle(i * BLOCK_WIDTH, HEIGHT- blockHeight, BLOCK_WIDTH, blockHeight, fillGraphColor);
                Raylib.DrawLine(prevx, prevy, i * BLOCK_WIDTH, HEIGHT - blockHeight, graphColor);
                prevx = i * BLOCK_WIDTH;
                prevy = HEIGHT - blockHeight;
                
            }

            DrawGrid(3);

            for (int i = 0; i < btns.Length; i++)
            {
			    btns[i].Draw();
            }
            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }

    public static float[] LoadData()
    {
        int fileCount = Directory.GetFiles("../data/", "*.pcmd", SearchOption.TopDirectoryOnly).Length;
        float[] data = new float[fileCount * DATA_PER_HOUR];
        for (int i = 0; i < fileCount; i++)
        {
            ReadDataFile(i).CopyTo(data, i * DATA_PER_HOUR);
        }

     
        return data;
    }

    public static float[] ReadDataFile(int index)
    {
        float[] ret = new float[DATA_PER_HOUR];
        string path = "../data/h" + index.ToString() + ".pcmd";

        FileStream fstream = File.Open(path, FileMode.Open);
        BinaryReader reader = new BinaryReader(fstream, Encoding.UTF8, false);
        
        for (int i = 0; i < DATA_PER_HOUR; i++)
        { 
            ret[i] = reader.ReadSingle();
        } 
        
        reader.Close();

        return ret;
    }

    public static Button ButtonClicked(Button[] btns, Vector2 mousePos)
    {
        for (int i = 0; i < btns.Length; i++)
        {
            if (mousePos.X > btns[i].X && mousePos.X < btns[i].X + btns[i].Width && 
                mousePos.Y > btns[i].Y && mousePos.Y < btns[i].Y + btns[i].Height)
            {
                return btns[i];
            }
        }
        return Button.btn_null;
    }

    public static Button[] SpawnButtons(string[] buttonPresets, int y)
    {
        Button[] btns = new Button[buttonPresets.Length];
        int offset = 5;
        int i = 0;
        foreach (string str in buttonPresets)
        {
            btns[i] = new Button(str, offset, y, 20, textColor, buttonColor);
            offset += btns[i].Width + 5;
            i++;
        }
        return btns;
    }

    public static void DrawGrid(int lines)
    {
        lines++;
        int yFraction = HEIGHT / lines;
        for (int y = 0; y < lines; y++)
        {
            Raylib.DrawLine(0, yFraction * y, WIDTH, yFraction * y, gridColor);
            string percText = (100 - (float)y / lines * 100).ToString() + "%";
            Raylib.DrawText(percText, 5, yFraction * y, 20, textColor);
        }

        int xFraction = WIDTH / lines;
        for (int x = 0; x < lines; x++)
        {
            Raylib.DrawLine(xFraction * x, 0, xFraction * x, HEIGHT, gridColor);
        }
        return;
    }
}

public class Button
{
	public string Text;
	public int X;
	public int Y;
	public int Height;
	public int Width;
	public Color BGColor;
	public Color FGColor;

	public int TextSize;

    public static Button btn_null = null; 

	public Button(string text, int x, int y, int size, Color fg, Color bg)
	{
		Text = text;
		X = x;
		Y = y;
		FGColor = fg;
		BGColor = bg;
        TextSize = size;
		Height = TextSize + 10;
		Width = Raylib.MeasureText(Text, TextSize) + 10;
		
	}
	public void Draw()
	{
		Raylib.DrawRectangle(X, Y, Width, Height, BGColor);
		Raylib.DrawText(Text, X + 5, Y + 5, TextSize, FGColor);
	}
}