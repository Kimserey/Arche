namespace Arche.Pages

open WebSharper
open WebSharper.UI.Next
open Arche.Common.Domain

module All =
    let pages = [
        { Title   = "Home"
          Route   = Route.Create [ "" ]
          Content = Doc.Empty
          DisplayOption = DisplayOption.PageWithNav
          AccessOption  = AccessOption.Other }
    ]
