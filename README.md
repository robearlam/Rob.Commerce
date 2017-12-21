# Rob.Commerce
A series of plugins for Sitecore Experience Commerce

## Prerequisites

* Built against SxP 9.0.0
* Built against XC 9.0-LA

This is built against the software versions above, you should follow the installation instructions for both of these and ensure the installation is successfully running.

Before installing these plugins, it is **strongly** recommended to take a backup on both your Engine and Website installation folders so you can rollback if required.

## Installation
Follow the steps below to deploy the plugins contained in this solution.

### Copy Libraries
As Commerce 9.0 is still in LA, there is no nuget feed for the references. For this reason you need to copy all of the DLL's from your Website/bin folder into the lib folder at the root of this solution.

### Deploy Engine
Build & Deploy the Project.Commerce module to a location on disk, then overwrite all instances of the Engine you have installed with this new version

### Deploy Website
Delpoy all of the Website projects for each module to the Website folder of your XP instance, currently the list of projects you need to deploy are:
* Feature.Compare.Website

### Deploy TDS Items
Deploy TDS items.

## Notes

This is a POC and as such there are some hardcoded values which I've not had chance to move to be populated from data templates. Most of these are just text values, however the one that might cause issue is that the _View Compare_ link is currently hardcoded to go to _/compare_ for the URL.