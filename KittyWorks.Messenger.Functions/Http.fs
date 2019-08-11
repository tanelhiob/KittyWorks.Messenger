module Http

open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs
open Microsoft.Extensions.Logging
open Microsoft.Azure.WebJobs.Extensions.Http
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc

[<FunctionName("hello")>]
let hello
    ([<HttpTrigger(AuthorizationLevel.Anonymous, "get")>] req : HttpRequest)
    (logger: ILogger) : Task<ActionResult> =
    async {
        return new OkObjectResult("Hello, world!") :> ActionResult;
    }
    |> Async.StartAsTask