# Rob.Commerce
A series of plugins for Sitecore Experience Commerce

## Prerequisites
* XC91 - Install a clean version of XC91, however before you run the install comment our the _"CreateDefaultTenantAndSite"_ task in the _Master_SingleServer.json_.

## Installation
Follow the steps below to deploy the plugins contained in this solution.

### Configuration
Edit the _gulp-config.js_ & _z.RobStorefront.DevSettings.config_ to ensure that the various parameters are correct for your system. 

### ES6
The gulp scripts are written using ES6, which isn't supported OOTB in VS2017, follow the fix here to enable that: https://github.com/madskristensen/NpmTaskRunner/issues/47

### Deploy
Excucute the _first-install_ task to build and deploy the engine to all of your roles, and all of website projects. This will also clean the OOTB configured environments and re-bootstrap and initialize the configuration contained in this solution. Note that the final step _initial-engine-setup_ may take a little time to complete.

### Deploy Unicorn Items
Deploy Unicorn items.
Perform a full publish.
Perform a full rebuild of indexes.

### Accessing the site
The storefront is configured to run against the url https://rob.storefront/. You will need to configure IIS & your hosts file manully.

### Further deployments
After your have initialized the engine using the steps above you can then just execute the _default_ task to deploy any code changes as required.

## Credits
The following modules were based on code provided by:
- Foundation.PluginEnhancements - [Kerry Havas](https://github.com/kerryhavas)
- Feature.Order.ServiceBus - Mike Shae
- Feature.Payments - [Sitecore.HabitatHome.Commerce](https://github.com/Sitecore/Sitecore.HabitatHome.Commerce)