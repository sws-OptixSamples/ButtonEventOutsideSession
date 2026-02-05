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
#endregion

public class ButtonEventInSession : BaseNetLogic
{
    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started

        // get the button
        var button = Owner.Get<Button>("Button1");

        // subscribe to the MouseClick event
        button.OnMouseClick += Button_MouseClick;
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }

    private void Button_MouseClick(object sender, MouseClickEvent e)
    {
        Log.Info($"color changed from netlogic within button session");
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
