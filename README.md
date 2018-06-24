# Rob.Commerce
A series of plugins for Sitecore Experience Commerce

## Prerequisites

* Built against XC9.0.1

This is built against the software versions above, you should follow the installation instructions for both of these and ensure the installation is successfully running.

Before installing these plugins, it is **strongly** recommended to take a backup on both your Engine and Website installation folders so you can rollback if required.

## Installation
Follow the steps below to deploy the plugins contained in this solution.

### Copy Libraries
There is no nuget feed for the SxA references. For this reason you need to copy all of the DLL's from your Websites bin folder into the lib folder at the root of this solution.

### Configuration
Edit the _gulp-config.js_ & _z.RobStorefront.DevSettings.config_ to ensure that the various parameters are correct for your system. 

### ES6
The gulp scripts are written using ES6, which isn't supported OOTB in VS2017, follow the fix here to enable that: https://github.com/madskristensen/NpmTaskRunner/issues/47

### Deploy
Excucute the _default_ task to build and deploy the engine to all of your roles, and all of website projects.

### Deploy TDS Items
Deploy TDS items.