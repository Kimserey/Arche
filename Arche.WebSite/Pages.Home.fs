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
                               p  [ text "Arche is a simple app which demonstrates one way to build web apps in F# with WebSharper."
                                    br []
                                    text "The structure is explained in details on my blog post "
                                    aAttr [ attr.href "http://kimsereyblog.blogspot.sg/2016/01/architecture-for-web-app-built-in-f-and.html" ] [ text "link" ]
                                    text " and the code can be found "; aAttr [ attr.href "https://github.com/Kimserey/Arche" ] [ text "on github" ]; text "." ]
                               p  [ text "Get in touch on twitter "
                                    aAttr [ attr.href "https://twitter.com/Kimserey_Lam" ] [ text "@Kimserey_Lam" ]
                                    text "!" ] ]
