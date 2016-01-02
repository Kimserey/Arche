namespace Arche.Modules

open WebSharper
open WebSharper.UI.Next

module LocationPicker =

    [<JavaScript>]
    module Client =
        open WebSharper.UI.Next.Html
        open WebSharper.UI.Next.Client

        let page() =
            let rvCity = Var.Create "London"
            form [ divAttr [ attr.``class`` "form-group" ]
                           [ labelAttr [ attr.``class`` "control-label" ] [ text "Enter a city:" ]
                             Doc.Input [ attr.``class`` "form-control"
                                         attr.placeholder "Type here..." ] rvCity ] ]
            , rvCity.View