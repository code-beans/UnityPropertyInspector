# UnityPropertyInspector

A Unity Extension to expose C# properties in the the Unity Inspector for MonoBehaviours

see [reference](http://wiki.unity3d.com/index.php/Expose_properties_in_inspector) to the original author

## Build & Install

You can install this extension in various ways depending on your personal preference.

### Source

Add the source files `\UnityPropertyInspector` to your `Assets\` folder

### UnityPackage

Use the appropriate \*.unitypackage from the [release section](https://github.com/code-beans/UnityPropertyInspector/releases)

### DLL

Copy the \*.dlls from the [release section](https://github.com/code-beans/UnityPropertyInspector/releases) into your `Assets\` folder.
Please note: You may need to compile your own .DLLs in case the precompiled versions do not match your required Unity version.

### Build

You may want to build your own .DLLs from source to declutter your code base.    

*Visual Studio*

* Check out the repository
* Set the correct path for the `UnityEngine.dll` and `UnityEditor.dll` in the project references to your installed Unity version
(usually installed under `C:\Program Files\Unity\Editor\Data\Managed\`)
* Set the target framework in the solution properties to the appropriate Unity target (i.e. either Unity .NET 3.5 subset or Unity .NET 3.5 full)
* Clean solution
* Build

### Documentation

The Unity Editor exposes public fields in the inspector window, but does not handle properties.
Some developers circumvent this limitation by polling changes in the "`Update()`-loop". The result is poorly maintainable and unperformant code.

Don't:

```csharp
public class MyMonoBehaviour : MonoBehaviour {

  public int health;

  //called per frame
  void Update() {
    SetHealthbar(health);
  }
}
```

Do:

```csharp
public class MyMonoBehaviour : ExposedMonoBehaviour {

  [SerializeField,HideInInspector] private int _health; //auto properties are not serialized by unity! Use fields instead

  [ExposeProperty]
  public int Health {
    get {
      return _health;
    }
    set {
      _health = value;
      SetHealthbar(value); //called only once
    }
  }
}
```
