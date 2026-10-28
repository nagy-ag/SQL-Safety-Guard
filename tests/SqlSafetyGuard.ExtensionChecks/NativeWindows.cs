using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

public static class ExtensionCheckWindows
{
    public delegate bool Callback(IntPtr handle, IntPtr parameter);
    [DllImport("user32.dll")] private static extern bool EnumWindows(Callback callback, IntPtr parameter);
    [DllImport("user32.dll")] private static extern bool EnumChildWindows(IntPtr parent, Callback callback, IntPtr parameter);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int GetClassName(IntPtr handle, StringBuilder text, int count);
    public static IntPtr[] Children(IntPtr parent)
    {
        var handles = new List<IntPtr>();
        EnumChildWindows(parent, (handle, parameter) => { handles.Add(handle); return true; }, IntPtr.Zero);
        return handles.ToArray();
    }
    public static string ClassName(IntPtr handle)
    {
        var text = new StringBuilder(256);
        GetClassName(handle, text, text.Capacity);
        return text.ToString();
    }
    [DllImport("user32.dll")] private static extern uint GetWindowThreadProcessId(IntPtr handle, out uint processId);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int GetWindowText(IntPtr handle, StringBuilder text, int count);
    [DllImport("user32.dll")] private static extern bool ShowWindow(IntPtr handle, int command);
    [DllImport("user32.dll")] private static extern IntPtr SendMessage(IntPtr handle, uint message, IntPtr word, IntPtr value);
    public static void ClickButton(IntPtr handle, int processId)
    {
        uint owner;
        GetWindowThreadProcessId(handle, out owner);
        if (owner != processId) throw new InvalidOperationException("Button belongs to a different process.");
        SendMessage(handle, 0x00F5, IntPtr.Zero, IntPtr.Zero);
    }
    public static IntPtr[] ForProcess(int processId)
    {
        var handles = new List<IntPtr>();
        EnumWindows((handle, parameter) => {
            uint owner;
            GetWindowThreadProcessId(handle, out owner);
            if (owner == processId) handles.Add(handle);
            return true;
        }, IntPtr.Zero);
        return handles.ToArray();
    }
    public static string Title(IntPtr handle)
    {
        var text = new StringBuilder(1024);
        GetWindowText(handle, text, text.Capacity);
        return text.ToString();
    }
    public static void ShowWithoutActivation(IntPtr handle, int processId)
    {
        uint owner;
        GetWindowThreadProcessId(handle, out owner);
        if (owner != processId) throw new InvalidOperationException("Window belongs to a different process.");
        ShowWindow(handle, 4);
    }
}
