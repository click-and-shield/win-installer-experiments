# A launcher for Java applications designed to use the console as user interface 

## Description

This project offers a dedicated launcher for Java applications, packaged as a standalone console executable. 
Upon execution, it seamlessly launches the Java application while displaying a console window during its runtime.

## Configuration

This application uses a JSON file as its configuration source.  
The JSON file serves as a dictionary, with its keys described in the table below.

| **Key**      | **Description**                                                                                                                                                                                                                                                           |
|--------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| JavaHomePath | The path to the directory that contains the JRE.                                                                                                                                                                                                                          |
| ClassPaths   | Specifies a list of directories, JAR files, and ZIP archives to search for class files.                                                                                                                                                                                   |
| ModulesPaths | Specifies where to find application modules (cf. key "`ModulesPaths`") with a list of path elements. The elements of a module path can be a file path to a module or a directory containing modules. Each module is either a modular JAR or an exploded-module directory. |
| Modules      | Specifies the root modules to resolve in addition to the initial module (ex. "`javafx.base`", "`javafx.controls`"...).                                                                                                                                                    |
| MainClass    | The name of the main class.                                                                                                                                                                                                                                               |

> Paths can be specified as either absolute or relative. When a relative path is provided, it is resolved in relation to the directory where the Java launcher resides.
> For example, if the path to the Java launcher is "`C:\path\to\launcher.exe`".
> Then the path "`jdk-23`" will be resolved into "`C:\path\to\jdk-23`".

## Example of configuration files

### Example of configuration file with absolute paths

```json
{
  "JavaHomePath": "C:\\Users\\denis\\Documents\\java\\jdk-23",
  "ModulesPaths": [ "C:\\Users\\denis\\.m2\\repository\\org\\openjfx\\javafx-base\\23\\javafx-base-23-win.jar",
    "C:\\Users\\denis\\.m2\\repository\\org\\openjfx\\javafx-base\\23\\javafx-base-23.jar",
    "C:\\Users\\denis\\.m2\\repository\\org\\openjfx\\javafx-controls\\23\\javafx-controls-23-win.jar",
    "C:\\Users\\denis\\.m2\\repository\\org\\openjfx\\javafx-controls\\23\\javafx-controls-23.jar",
    "C:\\Users\\denis\\.m2\\repository\\org\\openjfx\\javafx-graphics\\23\\javafx-graphics-23-win.jar",
    "C:\\Users\\denis\\.m2\\repository\\org\\openjfx\\javafx-graphics\\23\\javafx-graphics-23.jar" ],
  "Modules": [ "javafx.base",
    "javafx.controls",
    "javafx.graphics" ],
  "ClassPaths": [ "C:\\Users\\denis\\Documents\\github\\shadow\\target\\classes",
    "C:\\Users\\denis\\.m2\\repository\\org\\jetbrains\\annotations\\26.0.1\\annotations-26.0.1.jar" ],
  "MainClass": "org.shadow.skriva.Main"
}
```

### Example of configuration file with relative paths

Assuming that all files required for the application to run are stored under the 
application's directory, as described below:

```
- config.json
- java-launcher-gui-win.exe
+ classes\
  ...
+ jdk-23\
  ...
+ modules\
    - javafx-base-23-win.jar
    - javafx-base-23.jar
    - javafx-controls-23-win.jar
    - javafx-controls-23.jar
    - javafx-graphics-23-win.jar
    - javafx-graphics-23.jar
```

```json
{
  "JavaHomePath": "jdk-23",
  "ModulesPaths": [ "modules\\javafx-base-23-win.jar",
    "modules\\javafx-base-23.jar",
    "modules\\javafx-controls-23-win.jar",
    "modules\\javafx-controls-23.jar",
    "modules\\javafx-graphics-23-win.jar",
    "modules\\javafx-graphics-23.jar" ],
  "Modules": [ "javafx.base",
    "javafx.controls",
    "javafx.graphics" ],
  "ClassPaths": [ "classes", "classes\\annotations-26.0.1.jar" ],
  "MainClass": "org.shadow.skriva.Main"
```

> See [these examples](tests).

## Links

* OpenJDK: [https://jdk.java.net/23/](https://jdk.java.net/23/)
* JavaFX: [https://jdk.java.net/javafx23/](https://jdk.java.net/javafx23/)
