open System.Runtime.InteropServices
open System
open Startup

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

let handleDeserializationErrors (xs: FileReadOptions<Startup.T>) : Startup.T list =
    match xs with
    | NotFound f ->
        do printfn "The file '%s' could not be found." f
        [ ]
    | DeserializationError de ->
        do printfn "The file '%s' could not be deserialized because: %s" de.Name de.Error
        [ ]
    | Success items ->
        items

let handleRunErrors (xs: ProcessRunResult) : Startup.T option =
    match xs with
    | ProcessRunResult.Success x -> Some x
    | Invalid x -> 
        do printfn "The process with the id '%s' could not be started because no file name was specified or the process is misconfigured (very unlikely!)." (x.Id) 
        None
    | Win32Error x -> 
        do printfn "The process with the id '%s' could not be started because a Win32 error occured. There is no additional information available." (x.Id) 
        None
    | AlreadyDisposed x -> 
        do printfn "The process with the id '%s' could was disposed before it could be started. This is a programming error, you cannot fix it." (x.Id) 
        None
    | PlatformNotSupported x -> 
        do printfn "This platform is not supported."
        None


[<EntryPoint>]
let main argv = 

    let arranger (s: Startup.T) : unit =
        do moveAndResizeWindow (s.Width) (s.Height) (s.PosX) (s.PosY) (s.Process.Value.MainWindowHandle) |> ignore

    let handles = argv 
                  |> List.ofArray
                  |> List.map (Startup.fromFile) 
                  |> List.map handleDeserializationErrors
                  |> List.collect id  // list list -> list
                  |> List.map (Startup.run)
                  |> List.map handleRunErrors
                  |> List.choose id

    // Sleep to make sure the windows actually exist.
    do System.Threading.Thread.Sleep(200)
    do handles |> List.iter arranger

    0 // return an integer exit code
