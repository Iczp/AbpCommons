# IczpNet.AbpCommons

An abp module that provides standard tree structure entity implement.



## Usage

Add `DependsOn(typeof(AbpCommonsXxxModule))` attribute to configure the module dependencies. 

1. Application

```C#
[DependsOn(typeof(AbpCommonsApplicationModule))]
```
2. Application.Contracts
```C#
[DependsOn(typeof(AbpCommonsApplicationContractsModule))]
```
3. Domain.Shared
```C#
[DependsOn(typeof(AbpCommonsDomainSharedModule))]
```
4. EntityFrameworkCore
```C#
[DependsOn(typeof(AbpCommonsEntityFrameworkCoreModule))]
```
5. HttpApi
```C#
[DependsOn(typeof(AbpCommonsHttpApiModule))]
```
6. HttpApi.Client
```C#
[DependsOn(typeof(AbpCommonsHttpApiClientModule))]
```
7. Installer
```C#
[DependsOn(typeof(AbpCommonsInstallerModule))]
```
8. MongoDb
```C#
[DependsOn(typeof(AbpCommonsMongoDbModule))]
```

## RepositoryUrl

https://github.com/Iczp/AbpCommons.git

## PackageProjectUrl

https://github.com/Iczp/AbpCommons.git
