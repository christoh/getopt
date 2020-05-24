GetOpt
------

This library serves the purpose to parse options in a command line. It works as getopt in POSIX and also supports the GNU extensions. Options starting with "-" or "--" can occur at any argument position. "--" itself causes that all subsequent arguments are treated as non-option arguments.

Each option can have a short form (single character) starting with "-" and/or a long form (multiple characters) starting with "--".


Requirements
------------

This library compiles to .NET Standard 2.0. The units tests require .NET Core 2.0.

### Minimum versions of target frameworks
.NET Framework 4.6.1 (recommended 4.7.2)  
.NET Core 2.0  
Mono 5.4  
Xamarin.iOS 10.14  
Xamarin.Mac 3.8  
Xamarin.Android 8.0  
UWP 10.0.16299 (Windows 10 1709)  
Unity 2018.1

Questions, Comments, Patches
----------------------------

Please use Github. E-Mails will not be answered.


Initial Development
-------------------

GetOpt .NET is currently in initial development (version < 1.0.0) and the API may change