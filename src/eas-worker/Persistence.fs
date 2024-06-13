module internal Eas.Worker.Persistence

open Infrastructure
open Infrastructure.Domain.Errors

[<Literal>]
let private sectionName = "Worker"

let private getTasksGraph handlersGraph configuration =
    async {
        return
            match Configuration.getSection<Worker.Domain.External.Task> configuration sectionName with
            | None ->
                Error
                <| Persistence $"Section '%s{sectionName}' was not found in the configuration."
            | Some graph -> Worker.Mapper.buildCoreGraph graph handlersGraph |> Result.mapError Persistence
    }

let getTaskNode handlersGraph configuration =
    fun taskName ->
        async {
            match! getTasksGraph handlersGraph configuration with
            | Error error -> return Error error
            | Ok graph ->
                return
                    match Dsl.Graph.findNode taskName graph with
                    | Some node -> Ok node
                    | None ->
                        Error
                        <| Persistence
                            $"Task '{taskName}' was not found in the section '{sectionName}' of the configuration."
        }