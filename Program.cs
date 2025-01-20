using System.Drawing;
using System.Runtime.InteropServices;

[DllImport("user32.dll")]
static extern void mouse_event(uint dwFlags, uint dX, uint dY, uint dwData, IntPtr dwExtraInfo);

[DllImport("user32.dll")]
static extern short GetAsyncKeyState(int vKey);

[DllImport("User32.Dll")]
static extern long SetCursorPos(int x, int y);

const uint LEFTDOWN = 0x02;
const uint LEFTUP = 0x04;
const int HOTKEY = 0xC0; // ` key
const int CONFIGURATION = 0x31; // 1 key

bool enableClicker = false;
int clickInterval = 300;
var configured = false;
List<Point> clickPoints = new List<Point>();

while (true)
{
    if (GetAsyncKeyState(HOTKEY) <0)
    {
        configured = false;
        enableClicker = !enableClicker;
        Console.WriteLine($"Clicker On: {enableClicker}");
        Thread.Sleep(500);
    }

    if (GetAsyncKeyState(CONFIGURATION) < 0)
    {
        if (!configured) // reset
        {
            clickPoints = new List<Point>();
        }

        configured = true;
        enableClicker = false;
        var point = GetCursorPosition();
        if (!clickPoints.Any(x => x.X == point.X && x.Y == point.Y))
        {
            clickPoints.Add(point);
            Console.WriteLine($"{clickPoints.Count} - Point Saved X:{point.X} Y:{point.Y}");
        }
    }

    if (enableClicker)
    {
        foreach(var point in clickPoints)
        {
            MouseClick(point);
            Thread.Sleep(1000);
        }
        Thread.Sleep(3000);
    }
}

void MouseClick(Point p)
{
    Console.WriteLine($"Trying to click X:{p.X} Y:{p.Y}");

    SetCursorPos(p.X, p.Y);
    mouse_event(LEFTDOWN, 0, 0,0,IntPtr.Zero);
    mouse_event(LEFTUP, 0, 0, 0,IntPtr.Zero);

    var pointClicked = GetCursorPosition();
    Console.WriteLine($"Actually clicked X:{pointClicked.X} Y:{pointClicked.Y}");
}

[DllImport("user32.dll")]
static extern bool GetCursorPos(out POINT lpPoint);

static Point GetCursorPosition()
{
    POINT lpPoint;
    GetCursorPos(out lpPoint);
    // NOTE: If you need error handling
    // bool success = GetCursorPos(out lpPoint);
    // if (!success)

    return lpPoint;
}

[StructLayout(LayoutKind.Sequential)]
public struct POINT
{
    public int X;
    public int Y;

    public static implicit operator Point(POINT point)
    {
        return new Point(point.X, point.Y);
    }
}