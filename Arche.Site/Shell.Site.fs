namespace Arche.Site

open WebSharper.Html.Server
open WebSharper
open WebSharper.Sitelets

module Resources =
    open WebSharper.Resources

    module Bootstrap =
        type Css() =
            inherit BaseResource("https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap.min.css")
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

    open global.Owin
    open Microsoft.Owin.Hosting
    open Microsoft.Owin.StaticFiles
    open Microsoft.Owin.FileSystems
    open WebSharper.Owin

    [<EntryPoint>]
    let Main args =
        let rootDirectory, url =
            match args with
            | [| rootDirectory; url |] -> rootDirectory, url
            | [| url |] -> "..", url
            | [| |] -> "..", "http://localhost:9000/"
            | _ -> eprintfn "Usage: Arche.Site ROOT_DIRECTORY URL"; exit 1
        use server = WebApp.Start(url, fun appB ->
            appB.UseStaticFiles(
                    StaticFileOptions(
                        FileSystem = PhysicalFileSystem(rootDirectory)))
                .UseSitelet(rootDirectory, Arche.Shell.Main.main())
            |> ignore)
        stdout.WriteLine("Serving {0}", url)
        stdin.ReadLine() |> ignore
        0
