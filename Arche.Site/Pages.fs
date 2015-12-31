namespace Arche.Pages

open WebSharper
open WebSharper.UI.Next
open Arche
open Arche.Common.Domain

module All =
    let pages = [
        { Title   = "Home"
          Route   = Route.Create [ "" ]
          Content = Home.Static.html() 
          DisplayOption = DisplayOption.PageWithNav
          AccessOption  = AccessOption.Other }
    ]
