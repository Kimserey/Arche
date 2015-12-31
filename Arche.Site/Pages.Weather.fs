namespace Arche.Pages

open WebSharper
open WebSharper.UI.Next
open WebSharper.UI.Next.Html

module Weather  =
    
    [<JavaScript>]
    module private Client =
        open WebSharper.UI.Next.Client

        let page() = text "Hello world"

    module Static =

        let html() =
            client <@ Client.page() @>