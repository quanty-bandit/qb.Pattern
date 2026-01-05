# qb.Pattern

## CONTENT

**MBSingleton<T>**
Monobehaviour singleton abstract class implementation with persistence and dupplicate instance behaviour action options.

> **Remark**
> there is a lot of discussion about the use of singletons versus data injection, and the consensus is that data injection is the preferred method whenever possible to minimize dependencies and avoid spaghetti code. 



**SOWithGUID**
Implementation of an abstract scriptable object class with automatic serialization of the GUID.

The serialized GUID allows a specific instance of an object to be identified at runtime. The GUID is used in particular by the qb.Events and qb.Datas packages to resolve reference issues that may exist on addressable scenes.



**SOSingleton<T>**
Custom Scriptable object singleton abstract class implementation.

> **Remark**
> Like ScriptableSingleton but with a different instantation way... 



## HOW TO INSTALL

Use the Unity package manager and the Install package from git url option.

- Install at first time,if you haven't already done so previously, the package <mark>[unity-package-manager-utilities](https://github.com/sandolkakos/unity-package-manager-utilities.git)</mark> from the following url: 
  [GitHub - sandolkakos/unity-package-manager-utilities: That package contains a utility that makes it possible to resolve Git Dependencies inside custom packages installed in your Unity project via UPM - Unity Package Manager.](https://github.com/sandolkakos/unity-package-manager-utilities.git)

- Next, install the package from the current package git URL. 
  
  All other dependencies of the package should be installed automatically.



## Dependencies

[GitHub - codewriter-packages/Tri-Inspector: Free inspector attributes for Unity [Custom Editor, Custom Inspector, Inspector Attributes, Attribute Extensions]](https://github.com/codewriter-packages/Tri-Inspector.git)




