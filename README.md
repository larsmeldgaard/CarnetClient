# CarnetClient
C# client to show how to connect to the Volkswagen Carnet service

I own an E-Up and want to get the information possible from the service.
I need the trip-statistics and want to export this to a spreadsheet so I can keep a long-term stat onthe cars performance.
I also want to be able to get easy access to heating and charging state. The goal is to show the stats in a Graphana like dashboard.

The library is based on the excellent work from https://github.com/wez3/volkswagen-carnet-client
I have ported the calls to C# and tried to cut to the bone so that only the most neccesary headers and api-calls have been made.

## Installation

Modify program.cs with your username and password from the Carnet service.

## Usage

Call the EManager to get the output for your car.
Call StartClimate to start the Heating of your car.

## Todo

Working on the trip statistics - need help.
Proper arguments to start the service
