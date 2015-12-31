namespace Arche.Pages

open WebSharper
open WebSharper.UI.Next
open WebSharper.UI.Next.Html

module Home  =
    
    [<JavaScript>]
    module private Client =
        open WebSharper.UI.Next.Client

        let page() = text "Home"

    module Static =

        let html() =
            client <@ Client.page() @>

