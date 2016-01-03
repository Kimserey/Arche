namespace Arche.Site

open WebSharper
open WebSharper.Sitelets

module Resources =
    open WebSharper.Resources

    module Bootstrap =
        type Css() =
            inherit BaseResource("https://bootswatch.com/paper/bootstrap.min.css")
        type Js() =
            inherit BaseResource("https://maxcdn.bootstrapcdn.com/bootstrap/3.3.5/js/bootstrap.min.js")

    module FontAwesome =
        type Css() =
            inherit BaseResource("https://maxcdn.bootstrapcdn.com/font-awesome/4.5.0/css/font-awesome.min.css")

    [<assembly:Require(typeof<Bootstrap.Css>);
      assembly:Require(typeof<Bootstrap.Js>);
      assembly:Require(typeof<FontAwesome.Css>)>]
    do()

module Site =

    [<Website>]
    let Main =
        Arche.Shell.Main.main()