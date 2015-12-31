namespace Arche.Common

open WebSharper
open WebSharper.UI.Next
open System
open System.Text.RegularExpressions

module Domain =
    
    type Page = {
        Title: string
        DisplayOption: DisplayOption
        AccessOption: AccessOption
        Route: Route
        Content: Doc
    } 
    and DisplayOption = PageWithMenu | FullPage
    and AccessOption = Menu  of string | Other
    and Route = Route of string list
        with 
            static member Create (path: string list) = 
                Route (path |> List.map(fun route -> Regex.Replace(route.ToLowerInvariant(), "[^a-z0-9]+", "-")))
            member x.Group =
                let (Route path) = x
                if List.isEmpty path then ""
                else path |> List.head
             member x.Value = 
                x.ToString()
             override x.ToString() =
                let (Route path) = x
                match path with
                | []
                | [ "" ] -> "/"
                | _ -> path |> String.concat "/"

