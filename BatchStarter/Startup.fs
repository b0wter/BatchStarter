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

type ErrorFile = {
    Name: string
    Error: string
}

type FileReadOptions<'a> =
    | Success of 'a list
    | NotFound of string
    | DeserializationError of ErrorFile

let fromFile (filename: string) : FileReadOptions<'a> =
    if filename |> File.Exists then
        let content = filename |> File.ReadAllText
        try
            let deserialized = (Newtonsoft.Json.JsonConvert.DeserializeObject<'a list>(content))
            deserialized |> Success
        with
            | :? Newtonsoft.Json.JsonException as ex ->
                { Name = filename; Error = ex.Message } |> DeserializationError
    else
        filename |> NotFound

let run (t: T) =
    let startInfo = ProcessStartInfo(t.Command, t.Arguments)
    let proc = Process.Start(startInfo)
    { t with Process = Some proc }