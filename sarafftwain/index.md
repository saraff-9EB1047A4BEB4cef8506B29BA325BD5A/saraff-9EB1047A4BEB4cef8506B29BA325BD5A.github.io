---
layout: default
title: Saraff.Twain.NET
#permalink: /about/
---
# Saraff.Twain.NET
Saraff.Twain.NET is the skillful scanning component which allows you to control work of flatbed scanner, web and digital camera and any other TWAIN device from .NET environment. You can use this library in your programs written in any programming languages compatible with .NET technology.
## Features:
* TWAIN specification 1.x / 2.x compatible
* Programming environments: .NET Framework 2.0 or higher, WPF 3.5 or higher
* Full support for x86 and x64 platforms
* This is a fully-managed .NET library to guarantee the fast working in .NET Framework
* Acquire images from scanners, web or digital cameras and any other TWAIN device
* Data source enumeration and selection
* Control the work of Automatic Document Feeder (ADF) while scanning
* Supports Native, Buffered Memory, Disk File and Memory File image transfer mode
* Set up images acquisition parameters (pixel type, resolution, page size, image layout rectangle, brightness, contrast, etc)
* Retrieve the extended image information from scanner (barcode, patch code info, etc)
* Control advanced capabilities of TWAIN devices (rotation of images, scaling of images, filters of images, paper handling, the patch code detection and etc)
* Save acquired images as BMP, JPEG, PNG, GIF, TIFF files
## Saraff.Twain.NET was tested and has examples of use for:
* Microsoft Visual C# 2005 / 2008 / 2010 / 2015
* Microsoft Visual Basic - VB.NET 2005 / 2008 / 2010 / 2015
## Supported frameworks: 
* **CLR v2.0**: .net 2.0 / 3.0 / 3.5
* **CLR v4.0**: .net 4.0 / 4.5 / 4.6
## System requirements: 
* .NET Framework
## Supported platforms:
* Windows 2000 / XP / 2003 / Vista / 2008 / 7 / 8 / 10, 32-bit / 64-bit
* Linux (was tested on Lubuntu 14.04 LTS x86_32 & Lubuntu 16.04 LTS x86_32)

[NuGet package](https://www.nuget.org/packages/Saraff.Twain/) is available.

##  Samples:
* [[Saraff.Twain.NET CS Samples | cs]] **(UPDATED. 04.06.2016)**[...](https://goo.gl/info/42Y0gh)
* [[Saraff.Twain.NET Extensions Samples (LINQ to TWAIN) | cs-extensions]] **(NEW. 29.06.2017)**[...](https://goo.gl/info/7KJS6h)
* [[Saraff.Twain.NET Vb.net Samples | vb]] **(UPDATED. 05.06.2016)**[...](https://goo.gl/info/V8vX10)
* [[Saraff.Twain.NET WPF Samples | wpf]] **(UPDATED. 22.10.2016)**[...](https://goo.gl/info/sP3vL1)
* [[Saraff.Twain.NET Multithreading Samples | threading]] **(NEW. 04.12.2016)**[...](https://goo.gl/info/kqywGn)
* [[Saraff.Twain.NET Outproc Samples | outproc]] **(UPDATED. 22.10.2016)**[...](https://goo.gl/info/tBmvjF)
* [[Saraff.Twain.NET Service Samples | service]] **(UPDATED. 05.05.2017)**[...](https://goo.gl/info/OOcy6H)
* [[Saraff.Twain.NET Web Samples | web]] **(UPDATED. 05.06.2016)**[...](https://goo.gl/info/eQk54c)
* [[Saraff.Twain.NET Silverlight Samples | silverlight]] **(UPDATED. 05.05.2017)**[...](https://goo.gl/info/zIlqqy)
* [[Saraff.Twain.NET UWP Samples (Universal Windows) | uwp]] **(UPDATED. 05.05.2017)**[...](https://goo.gl/info/Ssx4AZ)
* [[Saraff.Twain.NET HTML Samples | html]] **(UPDATED. 05.05.2017)**[...](https://goo.gl/info/EgJ4cY)
* [[Saraff.Twain.NET Dependency Injection Samples | di]] **(NEW. 21.08.2017)**[...](https://goo.gl/info/1iGKug)

![]({% link content/sample2.jpg%})

**Figure 1 - Saraff.Twain.Sample2 on Windows 7**

# TWAIN Specification 
Saraff.Twain.NET implements [TWAIN 2.4 Specification](http://www.twain.org/specification/). Explanation of [specification](http://www.twain.org/specification/) is not the purpose of this project. Here will be explained features of the implementation of TWAIN specification.

# Contents
* Introduction
	* [Elements of TWAIN](Elements-of-TWAIN)
	* [Installation of the Data Source Manager](Installation-of-the-Data-Source-Manager)
	* [Samples](Samples)
* [Minimal implementation](Minimal-implementation)
* Advanced Implementation
	* Capabilities
		* [Strongly typed accessing to a capabilities](Strongly-typed-accessing-to-a-capabilities)
		* [General (not strongly typed) accessing to a capabilities](General-(not-strongly-typed)-accessing-to-a-capabilities)
			* [Capability Containers](Capability-Containers)
			* [Custom capability](Custom-capability)
			* [String values](String-values)
	* [Image Layout](Image-Layout)
	* [Alternative User Interfaces](Alternative-User-Interfaces)
	* [Options for Transferring Data](Options-for-Transferring-Data)
		* [Native Mode Transfer](Native-Mode-Transfer)
			* [Using a IStreamProvider for caching data of acquired image](Using-a-IStreamProvider-for-caching-data-of-acquired-image)
			* [Using a IImageHandler for processing a acquired image data from unmanaged memory to a stream](Using-a-IImageHandler)
			* [Using a IImageFactory<T> for processing acquired a image data from a stream to a image](Using-a-IImageFactory)
		* [Disk File Mode Transfer](Disk-File-Mode-Transfer)
		* [Buffered Memory Mode Transfer](Buffered-Memory-Mode-Transfer)
		* [Buffered Memory Mode Transfer With File Format](Buffered-Memory-Mode-Transfer-With-File-Format)
	* [Cancel scanning](Cancel-scanning)
	* [Color Information for an Image](Color-Information-for-an-Image)
	* [Image Information and Extended Image Information](Image-Information-and-Extended-Image-Information)
* [Use TWAIN 2.x](Use-TWAIN-2.x)

_If you notice an error, please let me know about it._
