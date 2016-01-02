namespace Arche.Pages

open WebSharper
open WebSharper.UI.Next
open WebSharper.UI.Next.Html
open Arche.Webparts

module Weather =
    module Static =
        let html() =
            client <@ WeatherLocation.Client.page() @>