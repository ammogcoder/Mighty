# Mighty [![NuGet](https://img.shields.io/nuget/v/Mighty.svg)](https://nuget.org/packages/Mighty)

[DOCUMENTATION](https://mightyorm.github.io/Mighty/)

A new small, happy, dynamic micro-ORM and general purpose .NET data access wrapper.

Based on and highly compatible with Massive, but now with:

* .NET Core 1.0, 1.1, 2.0, 3.0-preview
* Stored procedure suppport
* Parameter names and directions (where you need it; automatic parameter naming as in Massive still works as before)
* Transaction support
* Cursors (on Oracle and PostgreSQL; cursors are not designed to be passed out to client code on other databases)
* Multiple result sets
* Simultaneous access to more then one database provider

In addition to .NET Core, Mighty still runs on .NET Framework 4.5+; and on .NET Framework 4.0+ without async support.
