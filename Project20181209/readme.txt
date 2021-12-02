Change port number:
right click Project20181209 ¡ú properties ¡ú Debug ¡ú App URL

Update launch URL:

C:\Users\HJJ\Documents\Visual Studio 2017\Project20181209\.vs\config

1. delete applicationhost.config
2. Exit the ISS and VS2017
3. run VS2017 as admin and add the sentence below in the applicationhost.config
4. Clean and rebuild the project

<bindings>
    <binding protocol="http" bindingInformation="*:50000:localhost" />
    <binding protocol="http" bindingInformation="*:50000:127.0.0.1" />
    <binding protocol="http" bindingInformation="*:50000:192.168.1.215" />
    <binding protocol="http" bindingInformation="*:50000:192.168.43.150" />
</bindings>

Search "Firewall" at the start menu
Select Advanced settings
Inbound Rules

set 192.168.1.215:50000 can be accessed
name: Deaf Project