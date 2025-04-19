using System;
using System.Diagnostics;
using System.Threading;
using SharpDX.XInput;

class Program
{
    static Controller controller;
    static bool waitingForStart = false;
    static DateTime backPressedTime;
    static TimeSpan comboWindow = TimeSpan.FromSeconds(2);

    static void Main()
    {
        Console.WriteLine("Waiting for Xbox controller...");

        while (true)
        {
            if (controller == null || !controller.IsConnected)
            {
                controller = new Controller(UserIndex.One);

                if (!controller.IsConnected)
                {
                    Console.WriteLine("No controller connected...");
                    Thread.Sleep(1000);
                    continue;
                }

                Console.WriteLine("Controller connected");
            }

            try
            {
                var state = controller.GetState();
                var buttons = state.Gamepad.Buttons;

                bool back = (buttons & GamepadButtonFlags.Back) != 0;
                bool start = (buttons & GamepadButtonFlags.Start) != 0;

                Console.WriteLine($"Buttons: Back={back}, Start={start}, WaitingForStart={waitingForStart}");

                if (back && !waitingForStart)
                {
                    waitingForStart = true;
                    backPressedTime = DateTime.Now;
                    Console.WriteLine("Back pressed - 2 seconds to press Start...");
                }

                if (waitingForStart)
                {
                    if (start)
                    {
                        Console.WriteLine("Start pressed - launching program...");
                        StartProgram();
                        waitingForStart = false;
                    }
                    else if ((DateTime.Now - backPressedTime) > comboWindow)
                    {
                        waitingForStart = false;
                        Console.WriteLine("Timeout. Combination not detected.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading controller state: " + ex.Message);
                controller = null;
            }

            Thread.Sleep(100);
        }
    }

    static void StartProgram()
    {
        try
        {
            Process.Start(@"C:\Program Files\NVIDIA Corporation\NVIDIA App\CEF\NVIDIA App.exe");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error launching program: " + ex.Message);
        }
    }
}
