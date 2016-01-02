namespace Arche.Pages

open WebSharper
open WebSharper.UI.Next
open WebSharper.UI.Next.Html

module Home  =
    
    [<JavaScript>]
    module Client =
        open WebSharper.UI.Next.Client

        let page() = divAttr [ attr.``class`` "jumbotron" ] 
                             [ h1 [ text "Hi "; iAttr [ attr.``class`` "fa fa-smile-o" ] []; text "," ]
                               p  [ text "Arche is a simple app built in F# with WebSharper to demonstrate one way to structure a web app."
                                    br []
                                    text "This web app is explained in details on my blog post "; aAttr [ attr.href "#" ] [ text "link" ]; text "."
                                    br [] 
                                    text "The code can be found on "; aAttr [ attr.href "https://github.com/Kimserey/Arche" ] [ text "on github" ]; text "." ]
                               p  [ text "Get in touch on twitter "
                                    aAttr [ attr.href "https://twitter.com/Kimserey_Lam" ] [ text "@Kimserey_Lam" ]
                                    text "!" ] ]
