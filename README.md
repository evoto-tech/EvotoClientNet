# EvotoClientNet
The .NET evoto desktop client.

This is the desktop application for _evoto_, a blockchain based voting system. The application relies on .NET Framework 4.5.

# Install
Download the [lastest release](https://github.com/evoto-tech/EvotoClientNet/releases) and follow the installer wizard.

# Build
The application should build without any external files or dependencies, in Visual Studio 2015 or 2017. To do so, first clone the repository and its recursive submodules:

    git clone https://github.com/evoto-tech/EvotoClientNet.git
    cd EvotoClientNet
    git submodule init --update --recursive

# Running
The application relies on an external Registrar API, hosted at https://api.evoto.tech. If the application is running in DEBUG mode, then it will be expecting a Registrar API to be reachable on `http://localhost:15893`.

# Submodules
The application relies on two submodules, both also written by [evoto-tech](https://github.com/evoto-tech):

## EvotoBlockchain
This is the common code repository for blockchain interractions for use by both the evoto client (this) and the Registrar API. This library also manages the [MultiChain](http://www.multichain.com) platform processes.

## MultiChainLib
Handler for communication with the MultiChain API via RPC. Forked from [PbjCloud/MultiChainLib](https://github.com/PbjCloud/MultiChainLib), with additional wrapper for new and missing commands, such as raw transactions.
