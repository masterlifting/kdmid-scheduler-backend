module Eas.Api

open Infrastructure.DSL
open Infrastructure.Domain.Errors
open Eas.Domain.Internal.Core

module Get =

    let getSupportedEmbassies () =
        Set
        <| [ Russian <| Serbia Belgrade
             Russian <| Bosnia Sarajevo
             Russian <| Hungary Budapest
             Russian <| Montenegro Podgorica
             Russian <| Albania Tirana ]

    let initGetUserEmbassyRequests storage =
        fun user embassy ct ->
            Persistence.Repository.createStorage storage
            |> Result.map Persistence.Repository.Get.initGetUserEmbassyRequests
            |> ResultAsync.wrap (fun get -> get user embassy ct)
            |> ResultAsync.mapError Infrastructure

    let initGetEmbassyRequests storageOpt =
        fun embassy ct ->
            storageOpt
            |> Persistence.Repository.createStorage
            |> Result.map Persistence.Repository.Get.initGetEmbassyRequests
            |> ResultAsync.wrap (fun get -> get embassy ct)
            |> ResultAsync.mapError Infrastructure

    let initGetEmbassyResponse storageOpt =
        fun (request: Request) ct ->
            storageOpt
            |> Persistence.Repository.createStorage
            |> Result.mapError Infrastructure
            |> ResultAsync.wrap (fun storage ->
                match request.Embassy with
                | Russian _ -> Core.Russian.getEmbassyResponse request storage ct
                | _ -> async { return Error <| Logical(NotSupported $"{request.Embassy} for initGetEmbassyResponse") })

module Set =

    let initSetUserEmbassyRequest storageOpt =
        fun user request ct ->
            storageOpt
            |> Persistence.Repository.createStorage
            |> Result.map Persistence.Repository.Set.initSetUserEmbassyRequest
            |> ResultAsync.wrap (fun set -> set user request ct)
            |> ResultAsync.mapError Infrastructure

    let initSetEmbassyResponse storageOpt =
        fun response ct ->
            storageOpt
            |> Persistence.Repository.createStorage
            |> Result.map Persistence.Repository.Set.initSetEmbassyResponse
            |> ResultAsync.wrap (fun set -> set response ct)
            |> ResultAsync.mapError Infrastructure