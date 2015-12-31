namespace Arche.Shell

open WebSharper
open WebSharper.Sitelets
open WebSharper.UI.Next
open WebSharper.UI.Next.Html
open Arche
open Arche.Common
open Arche.Common.Domain
open Arche.Pages

module Main =
    
    let mkContent ctx curr =
        match curr.DisplayOption with
        | DisplayOption.FullPage ->
            curr.Content
            
        | DisplayOption.PageWithMenu ->
            let routes = 
                All.pages |> List.choose (fun p -> 
                    match p.AccessOption with
                    | AccessOption.Menu title -> Some (title, ctx.Link p.Title)
                    | AccessOption.Other      -> None)

            curr.Content 
            |> Menu.Static.embed (ctx.Link "Home") curr.Title routes

    let mkPage page =
        Sitelet.Content
        <| page.Route.Value 
        <| page.Title 
        <| fun ctx -> Content.Page (Title = page.Title, Body = [ mkContent ctx page ])

    let main() =
        All.pages
        |> List.map mkPage 
        |> Sitelet.Sum