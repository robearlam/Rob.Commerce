# Rob.Commerce
A series of plugins for Sitecore Experience Commerce

## Prerequisites

* Built against XC9.0.3

This is built against the software versions above, you should follow the installation instructions for both of these and ensure the installation is successfully running.

## Installation
Follow the steps below to deploy the plugins contained in this solution.

### Configuration
Edit the _gulp-config.js_ & _z.RobStorefront.DevSettings.config_ to ensure that the various parameters are correct for your system. 

### ES6
The gulp scripts are written using ES6, which isn't supported OOTB in VS2017, follow the fix here to enable that: https://github.com/madskristensen/NpmTaskRunner/issues/47

### Deploy
Excucute the _default_ task to build and deploy the engine to all of your roles, and all of website projects.

### Deploy Unicorn Items
Deploy Unicorn items.

## Credits
The following modules were based on code provided by:
- Foundation.PluginEnhancements - [Kerry Havas](https://github.com/kerryhavas)
- Feature.Order.ServiceBus - Mike Shae
- Feature.Payments - [Sitecore.HabitatHome.Commerce](https://github.com/Sitecore/Sitecore.HabitatHome.Commerce)