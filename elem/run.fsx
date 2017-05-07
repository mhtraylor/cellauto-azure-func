#r "System.Net.Http"
#r "Newtonsoft.Json"
#r "FifteenBelow.Json"

open System.Net
open System.Net.Http
open Newtonsoft.Json
open FifteenBelow.Json

let converters = [ OptionCoverter () :> JsonConverter ]
let serializer = JsonSerializerSettings (
    ContractResolver = CamelCasePropertyNamesContractResolver(),
    Converters = converters,
    Formatting = Formatting.Indented,
    NullValueHandling = NullValueHandling.Ignore
)
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
        log.Info(sprintf "Request JSON: %s" json)
        try
            let elemReq = JsonConvert.DeserializeObject<ElemRequest>(json, serializer)
            log.Info(sprintf "Request serialized: %A" elemReq)
            return req.CreateResponse(HttpStatusCode.OK, { message = "A-OK" })
        with _ ->
            return req.CreateResponse(HttpStatusCode.BadRequest)
    } |> Async.StartAsTask