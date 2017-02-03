# EvlDaemon

**A cross-platform .NET Core API & daemon for the Envisalink TPI (DSC) module**

## How

1. Download and install the latest .NET Core SDK for your platform [here](https://www.microsoft.com/net/core).
2. Clone this repository and navigate the the EvlDaemonCore directory.
3. Copy the `config.example.json` file to `config.json` and edit to suit your needs.
4. Run `dotnet restore` and `dotnet run` in the EvlDaemonCore directory to run the program.

## Configuration

By default, the program will look for a `config.json` file in the same
directory as the program itself. You can specify an alternate location
for this file by passing the full path via the `--config` parameter.

The password, IP and port options can be specified on the command line
using the `--password`, `--ip` and `--port` options respectively.
Parameters passed on the command line will override the same parameters
found in the `config.json` file.

## Example usage

Specifying IP and port on the command line (password read from config.json file):
`dotnet run --ip=192.168.0.2 --port=4025`

Specifying location of config.json file:
`dotnet run --config=/home/mike/.evldaemon/config.json`
