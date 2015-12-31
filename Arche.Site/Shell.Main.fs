namespace Arche.Shell

open WebSharper
open WebSharper.Sitelets

open WebSharper
open WebSharper.UI.Next
open WebSharper.UI.Next.Html
open Arche.Common
open Arche.Pages

module Main =
    
    let mkPage (page: Domain.Page) =
        Sitelet.Content page.Route.Value page.Route.Value (fun _ -> Content.Page(Title = page.Title, Body = [ page.Content ]))

    let main() =
        All.pages
        |> List.map mkPage
        |> Sitelet.Sum