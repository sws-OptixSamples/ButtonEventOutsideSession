#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.UI;
using FTOptix.HMIProject;
using FTOptix.NativeUI;
using FTOptix.Retentivity;
using FTOptix.CoreBase;
using FTOptix.Core;
using FTOptix.NetLogic;
using System.Linq;
#endregion

public class ButtonEventOutSession : BaseNetLogic
{
    private DelayedTask delayedTask;
    private int retryCount = 0;
    private const int MaxRetries = 10;
    private const int RetryDelayMs = 1;

    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
        ScheduleGetButton();
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
        delayedTask?.Dispose();
    }

    private void ScheduleGetButton()
    {
        delayedTask = new DelayedTask(GetButton, RetryDelayMs, LogicObject);
        delayedTask.Start();
    }

    private void GetButton()
    {
        try
        {
            // try to get the button, if it isn't there (yet) log and retry
            var button = Project.Current.Get("UI")
                    ?.FindNodesByType<NativeUIPresentationEngine>()
                    ?.FirstOrDefault()
                    ?.Get("Sessions")
                    ?.Children[0]
                    ?.Get("UIRoot")
                    ?.Get<NavigationPanel>("NavigationPanel1")
                    ?.CurrentPanel
                    is NodeId screenId && screenId != NodeId.Empty
                    ? InformationModel.Get(screenId)?.Get<Button>("Button2")
                    : null;

            if (button == null)
            {
                RetryOrLog("Button2 not found");
                return;
            }

            button.OnMouseClick += Button_MouseClick;
            Log.Info("Button found and subscribed successfully");
            delayedTask?.Dispose();
        }
        catch (Exception ex)
        {
            RetryOrLog($"Exception: {ex.Message}");
        }
    }

    private void RetryOrLog(string reason)
    {
        retryCount++;
        if (retryCount < MaxRetries)
        {
            Log.Info($"Retry {retryCount}/{MaxRetries}: {reason}");
            ScheduleGetButton();
        }
        else
        {
            Log.Error($"Failed to get button after {MaxRetries} retries: {reason}");
        }
    }

    private void Button_MouseClick(object sender, MouseClickEvent e)
    {
        Log.Info($"you changed color of a button from outside the session");
        var button = (Button)sender;
        button.BackgroundColor = RandomColor();
    }

    private Random random = new Random();
    private Color RandomColor()
    {
        byte r = (byte)random.Next(0, 256);
        byte g = (byte)random.Next(0, 256);
        byte b = (byte)random.Next(0, 256);
        byte a = 255; // Fully opaque
        return new Color(a, r, g, b);
    }
}
