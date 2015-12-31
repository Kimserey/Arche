namespace Arche.Shell

open WebSharper
open WebSharper.Sitelets

open WebSharper
open Arche.Common
open Arche.Pages

module Main =
    
    let mkPage (page: Domain.Page) =
        Sitelet.Content page.Route.Value page.Route.Value (fun _ -> Content.Text "Hello world")

    let main() =
        All.pages
        |> List.map mkPage
        |> Sitelet.Sum