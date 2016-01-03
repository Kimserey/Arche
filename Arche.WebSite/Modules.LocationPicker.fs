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

            formAttr [ attr.``class`` "form-horizontal" ]
                     [ divAttr [ attr.``class`` "form-group" ]
                               [ labelAttr [ attr.``class`` "col-lg-2 control-label" ] [ text "Enter a city:" ]
                                 divAttr   [ attr.``class`` "col-lg-10" ]
                                           [ Doc.Input [ attr.``class`` "form-control"
                                                         attr.placeholder "Type here..." ] rvCity ] ] ]
            , rvCity.View