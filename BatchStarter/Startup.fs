module Startup

open System.IO
open System.Diagnostics
open System.Diagnostics
open System
open System.ComponentModel

type T = {
    Id: string
    Command: string
    Arguments: string
    PosX: int
    PosY: int
    Width: int
    Height: int
    Process: Process option
}

type ErrorFile = {
    Name: string
    Error: string
}

type ProcessRunResult =
    | Success of T
    | Invalid of (T * string)
    | Win32Error of (T * string)
    | PlatformNotSupported of T
    | AlreadyDisposed of T


type FileReadOptions<'a> =
    | Success of 'a list
    | NotFound of string
    | DeserializationError of ErrorFile

let fromFile (filename: string) : FileReadOptions<T> =
    if filename |> File.Exists then
        let content = filename |> File.ReadAllText
        try
            let deserialized = (Newtonsoft.Json.JsonConvert.DeserializeObject<T list>(content))
            deserialized |> List.mapi (fun i d -> { d with Id = (sprintf "FILE: %s; INDEX: %i" filename i) })
                         |> Success
        with
            | :? Newtonsoft.Json.JsonException as ex ->
                { Name = filename; Error = ex.Message } |> DeserializationError
    else
        filename |> NotFound

let run (t: T) : ProcessRunResult =
    let startInfo = ProcessStartInfo(t.Command, t.Arguments)
    
    try
        let proc = Process.Start(startInfo)
        ProcessRunResult.Success { t with Process = Some proc }
    with
        | :? ObjectDisposedException         -> t               |> AlreadyDisposed
        | :? InvalidOperationException as ex -> (t, ex.Message) |> Invalid
        | :? Win32Exception as ex            -> (t, ex.Message) |> Win32Error
        | :? PlatformNotSupportedException   -> t               |> PlatformNotSupported