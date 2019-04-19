module Startup

open System.IO
open System.Diagnostics
open System.Diagnostics
open System

type T = {
    WindowName: string
    Command: string
    Arguments: string
    PosX: int
    PosY: int
    Width: int
    Height: int
    Process: Process option
}

let fromFile (filename: string) : T list option =
    if filename |> File.Exists then
        let content = filename |> File.ReadAllText
        Some (Newtonsoft.Json.JsonConvert.DeserializeObject<T list>(content))
    else
        None

let run (t: T) =
    let startInfo = ProcessStartInfo(t.Command, t.Arguments)
    let proc = Process.Start(startInfo)
    { t with Process = Some proc }