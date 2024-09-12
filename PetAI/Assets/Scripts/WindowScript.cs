using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class WindowScript : MonoBehaviour
{
    private struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cxTopHeight;
        public int cxBottomHeight;
    }

    // Get active window
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    // Allow mouse click through
    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    // Keep window on top
    [DllImport("user32.dll", SetLastError = true)]
    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y,
        int cx, int cy, uint uFlags);

    // Allow mouse interaction
    [DllImport ("user32.dll")]
    static extern int SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

    [DllImport("Dwmapi.dll")]
    private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

    const int GWL_EXSTYLE = -20;
    const uint WS_EX_LAYERED = 0x00080000;
    const uint WS_EX_TRANSPARENT = 0x00000020;
    static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    const uint LWA_COLORKEY = 0x00000001;

    private IntPtr hWnd; // Variable to store window handle

    private void Start()
    {
        // Allow application to run in background
        Application.runInBackground = true;

#if !UNITY_EDITOR // Only run if not on the Unity editor
        hWnd = GetActiveWindow(); // Set window handle

        MARGINS margins = new MARGINS { cxLeftWidth = -1 };

        // Enable window transparency
        DwmExtendFrameIntoClientArea(hWnd, ref margins);

        // Enable window mouse click through
        SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);

        // Enable window interaction
        //SetLayeredWindowAttributes(hWnd, 0, 0, LWA_COLORKEY);

        // Keep window on top
        SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, 0);
#endif
    }

    private void Update()
    {
        Collider2D hit = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        // If mouse is on top of player, allow click on object
        if (hit != null && hit.CompareTag("Player"))
            setClickthrough(false);
        else
            setClickthrough(true);
    }

    private void setClickthrough(bool clickthrough)
    {
        if (clickthrough)
            SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);
        else
            SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED);
    }
}
