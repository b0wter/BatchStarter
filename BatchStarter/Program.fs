open System.Runtime.InteropServices
open System

let SWP_NOSIZE = (uint32)0x0001
let SWP_NOZORDER = (uint32)0x0004

[<DllImport("user32.dll", SetLastError = true)>]
 extern IntPtr FindWindow(string lpClassName, string lpWindowName)

 [<DllImport("user32.dll", SetLastError = true)>]
 extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, UInt32 uFlags)

 let moveAndResizeWindow (newWidth: int) (newHeight: int) (newX: int) (newY: int) (hWnd: IntPtr) : bool =
    let operators = if newWidth = 0 && newHeight = 0 then SWP_NOZORDER ||| SWP_NOSIZE else SWP_NOZORDER
    if hWnd = IntPtr.Zero then
        false
    else
        SetWindowPos(hWnd, IntPtr.Zero, newX, newY, newWidth, newHeight, operators) |> ignore
        true

let moveWindow = moveAndResizeWindow 0 0

[<EntryPoint>]
let main argv = 

    let arranger (s: Startup.T) : unit =
        do moveAndResizeWindow (s.Width) (s.Height) (s.PosX) (s.PosY) (s.Process.Value.MainWindowHandle) |> ignore

    let handles = argv 
                  |> List.ofArray
                  |> List.map (Startup.fromFile) 
                  |> List.choose id   // filtert die Nones heraus
                  |> List.collect id  // macht aus list list nur noch list
                  |> List.map (Startup.run)

    // Sleep to make sure the windows actually exist.
    do System.Threading.Thread.Sleep(200)
    do handles |> List.iter arranger

    0 // return an integer exit code
