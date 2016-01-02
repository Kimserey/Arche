namespace Arche.Common

open FSharp.Data

module Config =
    
    type T = JsonProvider<""" { "openWeather": { "apiKey" : "x" } } """>

    let value = 
        T.Load("config.json") 