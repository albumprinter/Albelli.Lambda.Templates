#r "paket:
nuget Fake.DotNet.Cli
nuget Fake.IO.FileSystem
nuget Fake.Core.Target
nuget Fake.Tools.GitVersion 
nuget FSharp.Core 4.7.0


github albumprinter/Fake.Extra
 //"
#load ".fake/build.fsx/intellisense.fsx"
#load ".fake/build.fsx/paket-files/albumprinter/Fake.Extra/GitVersionTool.fs"

open System.IO
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators
open Fake.Tools

open Newtonsoft.Json

open Albelli

    module ScriptVars =
        let artifacts = "./artifacts" |> Path.GetFullPath

        let version() = GitVersionTool.generateProperties().NuGetVersionV2

        let nugetKey() = "NUGET_API_KEY" |> Environment.environVarOrFail

        let nugetSource() = "https://www.nuget.org/api/v2/package"



Target.initEnvironment ()

Target.create "Trace" (fun _ ->
    Trace.log "Hello world"
    Trace.log <| ScriptVars.version()
)

Target.create "Clean" (fun _ ->
    !! "src/**/bin"
    ++ "src/**/obj"
    |> Shell.cleanDirs
)

Target.create "Build" (fun _ ->
    !! "src/**/*.*proj"
    |> Seq.iter (DotNet.build id)
)

Target.create "All" ignore

"Clean"
  ==> "Trace"
  //==> "Build"
  ==> "All"

Target.runOrDefault "All"
