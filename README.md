![MICROFOCUS LOGO](https://upload.wikimedia.org/wikipedia/commons/4/4e/MicroFocus_logo_blue.png)

# ALM Octane plugin for Microsoft Team Foundation Server CI                        

[![Codacy Badge](https://api.codacy.com/project/badge/Grade/fde28fd11a494839b50c2b49f2fd486a)](https://www.codacy.com/app/MicroFocus/octane-tfs-plugin?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=MicroFocus/octane-tfs-plugin&amp;utm_campaign=Badge_Grade)

Project status:
[![Build status](https://ci.appveyor.com/api/projects/status/uhl2cmsb7sngtkf5?svg=true)](https://ci.appveyor.com/project/OctaneCIPlugins/octane-tfs-plugin)

Latest release branch status:
[![Build status](https://ci.appveyor.com/api/projects/status/uhl2cmsb7sngtkf5/branch/master?svg=true)](https://ci.appveyor.com/project/OctaneCIPlugins/octane-tfs-plugin/branch/master)

##### This plugin integrates ALM Octane with TFS, enabling ALM Octane to display TFS build pipelines and track build and test run results.

## Relevant links
-	**Download the most recent LTS version of the plugin** at [Github release page](https://github.com/MicroFocus/octane-tfs-plugin/releases)
-	**Check the open issues (and add new issues)** at [Github issues](https://github.com/MicroFocus/octane-tfs-plugin/issues)

## Installation instructions

1. Download latest msi release setup from : [releases](https://github.com/MicroFocus/octane-tfs-plugin/releases)
2. Run setup.exe
3. Configure all relevant fields following the [Configure the setup](https://github.com/MicroFocus/octane-tfs-plugin#configure-the-setup) guide

**Pay attention to your TFS main and update version when downloading the setup. If the main and update version of your TFS does not match the version specified with the setup - the plugin may not work!**

### Configuring the TFS ci plugin to connect to ALM Octane
#### Before you configure the connection:
1. Ask the ALM Octane shared space admin for an API access Client ID and Client secret. The plugin uses these for authentication when
communicating with ALM Octane. The access keys must be assigned the CI/CD Integration role in all relevant workspaces. For details, see Set up API access for integration.
2. To enable the TFS server to communicate with ALM Octane, make sure the server can access the Internet. If your network requires a proxy to connect to the Internet, setup the required proxy configuration.

#### Configure the setup
During the msi setup
Enter the following information in the relevant fields:

**Installation folder**: C:\Program Files\Microsoft Team Foundation Server 15.0\Application Tier\TFSJobAgent\Plugins\

      This folder is provided by default and should be changed only if TFS installation path is different from the default

**Location**: http://myServer.myCompany.com:8081/ui/?p=1002
where 1002 is your shared space id and 8081 is the ALM Octane port service port

**Client ID/Secret**: Ask the ALM Octane shared space admin for an API access Client ID and Client secret. The plugin uses these for authentication when communicating with ALM Octane

**PAT (TFS Personal access token)**: The token should be configured by TFS admin (see [PAT](https://docs.microsoft.com/en-us/vsts/accounts/use-personal-access-tokens-to-authenticate) ). PAT minimal permission set should contain :
- Build (read and execute)
- Code (read)
- Project and team (read)
- Test management (read)

**After the connection is set up**, open ALM Octane, define a CI server and create pipelines.
For details, see Integrate with your CI server in the ALM Octane Help.

## Administer the plugin
There are several operations that can be performed to monitor plugin health from the browser.

Through the main configuration url (http://localhost:4567/) it is possible to perform the following :
- Configure plugin connection settings
- View logs
- Start/Stop plugin

**The urls are available only from the localhost address.**

| URI           | Description  |
| -------------         | -----:|
| http://localhost:4567/      |   Plugin configuration main page |
| http://localhost:4567/logs       | Plugin logs list  |
| http://localhost:4567/logs/last |    Last plugin logs |
| http://localhost:4567/config   |Configure plugin connection settings|

Plugin logs are located in C:\Users\Public\Documents\OctaneTfsPlugin\logs

## Limitations
- The plugin does not support commits to ALM Octane if using TFVC

## Contribute to the TFS plugin
- Contributions of code are always welcome!
- Follow the standard GIT workflow: Fork, Code, Commit, Push and start a Pull request
- Each pull request will be tested, pass static code analysis and code review results.
- All efforts will be made to expedite this process.

#### Guidelines
- Document your code – it enables others to continue the great work you did on the code and update it.

### Feel free to contact us on any question related to contributions - octane.ci.plugins@gmail.com

## Disclaimer
Certain versions of software accessible here may contain branding from Hewlett-Packard Company (now HP Inc.) and Hewlett Packard Enterprise Company.  As of September 1, 2017, the software is now offered by Micro Focus, a separately owned and operated company.  Any reference to the HP and Hewlett Packard Enterprise/HPE marks is historical in nature, and the HP and Hewlett Packard Enterprise/HPE marks are the property of their respective owners.

