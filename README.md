# MunityClient
The MunityClient is a Blazor Frontend for the MUNityCore. It is a new take on the old Angular Version.

## About
This client should offer an interface for all tools and functions for the munity core.
> Note that this Software is still under development.

## General structure

This is a basic Balzor WebAssembly Application.

## Status
This application is under development and cannot be used at the moment. Please check later to see some progress.


| Name                | Status                              | Stage                                                            |
| ------------------- | ----------------------------------- | ---------------------------------------------------------------- |
| User Managment      | ![5%](https://progress-bar.dev/5) | Login and Registration done                                        |
| ConferenceManagment | ![0%](https://progress-bar.dev/1) | NOT STARTED                                                  |
| ResaEditor          | ![26%](https://progress-bar.dev/26) | First communication with Backend done |
| Speakerlist         | ![0%](https://progress-bar.dev/0) | NOT STARTED                                     |
| Simulation          | ![0%](https://progress-bar.dev/0)   | NOT STARTED                                                    |
| Administration      | ![0%](https://progress-bar.dev/0)   | NOT STARTED                                              |
| Dockerize           | ![0%](https://progress-bar.dev/0) | NOT STARTED       |

## Developing and Testing

You can develop without starting the backend. To do this you need to set the ids of your route to the name test.

For example to open a test Resolution navigate to

```[url]/resa/edit/test```

If you have the backend started you may need to set the target IP inside the Program.cs (API_URL)

## Setup Development Environment
Requiered: 
* Visual Studio Community Edtion 2019 or higher or Visual Studio Code https://visualstudio.microsoft.com/de/downloads/

To have an easier workflow with Visual Studio goto Tools (Extras) -> Command Prompt (Befehlszeile) -> Developer-Command-Prompt (Entwickler-EIngabeaufforderung)
and type in
```
cd MUNityClient
dotnet watch run
```

This will allow a Hot-Reload function


