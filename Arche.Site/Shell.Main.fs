namespace Arche.Shell

open WebSharper
open WebSharper.Sitelets

module Main =

    let main() = 
        Sitelet.Content "" "" (fun _ -> Content.Text "Hello world")

