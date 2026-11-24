---
# You don't need to edit this file, it's empty on purpose.
# Edit theme's home layout instead if you wanna make some changes
# See: https://jekyllrb.com/docs/themes/#overriding-theme-defaults
layout: default
title: SARAFF SOFTWARE
---
# [Saraff.Twain.NET]({% link sarafftwain/index.md %})
Saraff.Twain.NET is the Windows TWAIN component (`net462`, `net8.0-windows`) for WinForms and WPF applications. It supports TWAIN 1.x and 2.x sources, `Twain32`, image transfer and strong capability control.
# [Saraff.TwainX]({% link sarafftwainx/index.md %}) (.NET Standard 2.0)
Saraff.TwainX is the stream-based TWAIN 2.x component for `netstandard2.0`. It has no WinForms/WPF dependency and provides synchronous `Acquire()` and asynchronous `AcquireAsync()` acquisition.
- ## [Saraff.Twain.Extensions / Saraff.TwainX.Extensions]({% link sarafftwainext/index.md %})
Saraff.Twain.NET Extensions (LINQ to TWAIN).
- ## [Saraff.Twain.Aux](./sarafftwainaux/)
Provides outproc interaction with a Saraff.Twain.NET.

# [Saraff.Twain.DS]({% link sarafftwainds/index.md %})
Saraff.Twain.DS is the skillful class library which allows you to design drivers (a Data Source) of flatbed scanner, web and digital camera and any other TWAIN device from .NET environment. You can use this library in your programs written in any programming languages compatible with .NET technology.
- ## [Saraff.Twain.DS.BitmapSource](./sarafftwaindsbitmap/)
Simplifies the creation of a Data Source, if it returns a bitmap.
- ## [Saraff DirectX DS (Saraff.Twain.DS.DirectX)](./sarafftwaindsdx/)
TWAIN Data Source that capturing video from a USB web camera and making snapshots using it.

# [Saraff.IoC](./saraffioc/index.md)
Saraff.IoC is a lightweight, thread-safe dependency injection container for .NET (`net40` and `netstandard2.0`). It provides constructor, property and method injection, singleton/scoped/transient lifetimes, scopes and nested containers, interception, asynchronous initialization and a modern registration API while keeping the legacy `Bind`/`Load` API compatible.
# [Saraff.AxHost.NET](./saraffaxhost/index.md)
Saraff.AxHost.NET Provides hosting for custom components (.NET) in applications with unmanaged code.
# [Saraff.Tiff.NET](./sarafftiff/index.md)
Saraff.Tiff.NET is the skillful component which represents a writer and a reader that provides fast, non-cached, forward-only access to TIFF data.
# [Saraff.ThreadingModel.NET](./saraffthread/)
Saraff.ThreadingModel.NET is a single-threaded execution model for .NET: an STA dispatcher with a Win32 message pump, an interface proxy that marshals every call to that thread, and a DI-friendly message queue (`net462` and `net8.0-windows`).
# [Saraff.VisualFoxpro.NET](./saraffvfp/)
Hosting .NET Components in a Visual Foxpro.
