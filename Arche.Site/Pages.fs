namespace Arche.Pages

open WebSharper
open WebSharper.UI.Next
open WebSharper.UI.Next.Html
open Arche
open Arche.Webparts
open Arche.Common.Domain

module All =
    let pages = [
        { Title   = "Home"
          Route   = Route.Create [ "" ]
          Content = client <@ Home.Client.page() @>
          DisplayOption = DisplayOption.PageWithMenu
          AccessOption  = AccessOption.Other }

        { Title   = "Map"
          Route   = Route.Create [ "map" ]
          Content = client <@ Map.Client.webpart() @>
          DisplayOption = DisplayOption.PageWithMenu
          AccessOption  = AccessOption.Menu "Map" }
        
        { Title   = "Weather"
          Route   = Route.Create [ "weather" ]
          Content = 
            divAttr [ attr.style "max-width: 600px; margin: auto;" ] [ client <@ Weather.Client.webpart() @> ]
          DisplayOption = DisplayOption.PageWithMenu
          AccessOption  = AccessOption.Menu "Weather" }
    ]
