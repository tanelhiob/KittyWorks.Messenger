open System
open Microsoft.AspNetCore.SignalR.Client
open FSharp.Data

let handle input = 
    let httpRequestBody = HttpRequestBody.TextRequest input
    Http.Request("https://kittyworks-messenger-functions.azurewebsites.net/api/sendMessage", body = httpRequestBody) |> ignore

let handleInput () =
    match Console.ReadLine() with
    | "exit" -> false
    | message -> handle message; true

[<EntryPoint>]
let main _ =

    let url = "https://kittyworks-messenger-functions.azurewebsites.net/api"
    let connection = (new HubConnectionBuilder()).WithUrl(url).Build()

    connection.On("messageReceived", fun message -> printfn "%A" message) |> ignore

    async {
        do! connection.StartAsync() |> Async.AwaitTask
    }
    |> Async.RunSynchronously

    while handleInput () do ignore()

    0
