module Http

open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.AspNetCore.Mvc
open Microsoft.Azure.WebJobs.Extensions.SignalRService
open System.Threading.Tasks
open Microsoft.Azure.EventGrid.Models
open Microsoft.Azure.WebJobs.Extensions.EventGrid

[<FunctionName("hello")>]
let hello
    ([<HttpTrigger(AuthorizationLevel.Anonymous, "get")>] req : HttpRequest)
    : ActionResult =    

    new OkObjectResult("Hello, world!") :> ActionResult


[<FunctionName("negotiate")>]
let negotiate
    ([<HttpTrigger(AuthorizationLevel.Anonymous, "post")>] req: HttpRequest)
    ([<SignalRConnectionInfo(HubName = "chat")>] connectionInfo: SignalRConnectionInfo)
    : SignalRConnectionInfo =    

    connectionInfo


let sendMessage
    ([<HttpTrigger(AuthorizationLevel.Anonymous, "post")>] message: obj)
    ([<SignalR(HubName = "chat")>] chatHub : IAsyncCollector<SignalRMessage>)
    : Task =
     
    async {
        let signalRMessage = new SignalRMessage()
        signalRMessage.Target <- "messageReceived"
        signalRMessage.Arguments <- [| message |]

        do! chatHub.AddAsync(signalRMessage) |> Async.AwaitTask
    }
    |> Async.StartAsTask :> Task


[<FunctionName("eventgridevent")>]
let eventGridEvent
    ([<EventGridTrigger>] event : EventGridEvent)
    ([<SignalR(HubName = "chat")>] chathub : IAsyncCollector<SignalRMessage>)
    : Task =

    async {
        let signalRMessage = new SignalRMessage()
        signalRMessage.Target <- event.EventType
        signalRMessage.Arguments <- [| event.Data |]

        do! chathub.AddAsync(signalRMessage) |> Async.AwaitTask 
    }
    |> Async.StartAsTask :> Task