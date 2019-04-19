module Legacy

open System
open System.Text
open System.Runtime.InteropServices

//
// Code that is no longer required but might prove useful in the future.
//

[<DllImport("user32.dll", CharSet=CharSet.Auto)>]
extern int GetWindowTextLength(IntPtr hWnd)

[<DllImport("user32.dll", CharSet=CharSet.Auto)>]
extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount)

[<DllImport("user32.dll", SetLastError = true)>]
extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle)

let windowName (window: IntPtr) : string =
    let stringBuilder = StringBuilder(1024)
    let textLength = GetWindowText(window, stringBuilder, 1024)
    stringBuilder.ToString()

let enumChildren (root: IntPtr) : IntPtr list =
    let rec run (current: IntPtr) (acc: IntPtr list) : IntPtr list =
        let zero = FindWindowEx(root, current, null, null)
        if zero = IntPtr.Zero then
            acc
        else
            run zero (zero :: acc)
    run IntPtr.Zero []
