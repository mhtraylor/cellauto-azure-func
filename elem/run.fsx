#r "System.Net.Http"
#r "NewtonSoft.Json"

open System.Net
open System.Net.Http
open Newtonsoft.Json

type ElemRequest = {
    code: int
    start: int array Option
}

type ElemResponse = {
    message: string
}

let Run (req: HttpRequestMessage, log: TraceWriter) =
    async {
        log.Info("Webhook was triggered!")
        let! json = req.Content.ReadAsStringAsync() |> Async.AwaitTask
        try
            let elemReq = JsonConvert.DeserializeObject<ElemRequest>(json)
            return req.CreateResponse(HttpStatusCode.OK, { message = "A-OK" })
        with _ ->
            return req.CreateResponse(HttpStatusCode.BadRequest)
    } |> Async.StartAsTask