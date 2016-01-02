namespace Arche.Common

open WebSharper
open WebSharper.UI.Next
open WebSharper.UI.Next.Html

[<JavaScript>]
module Bootstrap =
    
    let panel title content =
        divAttr [ attr.``class`` "panel panel-primary" ] 
                [ divAttr [ attr.``class`` "panel-heading" ]
                          [ divAttr [ attr.``class`` "panel-title" ]
                                    [ text title ] ]
                  divAttr [ attr.``class`` "panel-body" ] content ]