# Example applications using Azure

This repository contains several example project created to learn basics of working with Azure.

## Configuration files

Connection strings to Azure were removed from configuration files (`appsettings.json`, `appsettings.Development.json`, `config.json`).
To run applications properly, create your own Azure services and add their connection strings to configuration files.
Some connections strings are already filled with values connecting to local Azurite instance.