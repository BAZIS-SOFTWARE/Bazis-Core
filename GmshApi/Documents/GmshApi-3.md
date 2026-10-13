# GmshApi-3 — Жизненный цикл и CAD API

[Диаграмма](GmshApi-3.mdpuml). [Обзор проекта](GmshApi.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `Gmsh`

Пространство имён: `GmshApi`.

Исходники: [Gmsh.cs](../../GmshApi/Wrapper/Gmsh.cs).

```text
Logger : Logger [get, private set]
Model : Model [get, private set]
Option : Option [get, private set]
Plugin : Plugin [get, private set]
Clear() : void
Gmsh()
Initialize(int argc= 0, string[] argv= null, bool readConfigFiles= true, bool run= false) : void
IsInitialized() : int
Merge(string fileName) : void
Open(string fileName) : void
Write(string fileName) : void
finalize() : void
```

## `GmshKernel`

Пространство имён: `GmshApi`.

Исходники: [GmshKernel.cs](../../GmshApi/GmshKernel.cs).

```text
DLL : IntPtr [get, set]
GetFunction<T>(string functionName) : T
GetProcAddress(IntPtr hModule, string procedureName) : IntPtr
LoadLibrary(string dllToLoad) : IntPtr
```

## `InvisibleOptions`

Пространство имён: `GmshApi`.

Исходники: [InvisibleOptions.cs](../../GmshApi/Wrapper/Enums/PluginNameOptions/InvisibleOptions.cs).

```text
DeleteElements
ReverseElements
XMax
XMin
YMax
YMin
ZMax
ZMin
```

## `Logger`

Пространство имён: `GmshApi`.

Исходники: [Logger.cs](../../GmshApi/Wrapper/Logger.cs).

```text
Get() : string[]
GetLastError() : string
Start() : void
Stop() : void
Write(string message, LoggerLevel level= LoggerLevel.info) : void
```

## `LoggerLevel`

Пространство имён: `GmshApi`.

Исходники: [LoggerLevel.cs](../../GmshApi/Wrapper/Enums/LoggerLevel.cs).

```text
error
info
warning
```

## `Memory`

Пространство имён: `GmshApi`.

Исходники: [Memory.cs](../../GmshApi/Wrapper/Memory.cs).

```text
Free(IntPtr p) : void
GetArrayOfObjectsData<T>(IntPtr outArray, IntPtr outArrayLength, IntPtr inArray) : T[][] where T : struct
GetArrayOfString(IntPtr array, IntPtr length) : string[]
GetObjectsData<T>(IntPtr src, IntPtr eCount) : T[] where T : struct
GetString(IntPtr ptr) : string
Malloc(IntPtr n) : IntPtr
```
