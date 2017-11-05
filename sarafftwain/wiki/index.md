---
layout: default
title: Saraff.Twain.NET Wiki
---
# TWAIN Specification 
Saraff.Twain.NET implements [TWAIN 2.4 Specification](http://www.twain.org/specification/). Explanation of [specification](http://www.twain.org/specification/) is not the purpose of this project. Here will be explained features of the implementation of TWAIN specification.

# Contents
* Introduction
	* [Elements of TWAIN](./w0100.md)
	* [Installation of the Data Source Manager](./w0200.md)
	* [Samples](./w0300.md)
* [Minimal implementation](./w1000.md)
* Advanced Implementation
	* Capabilities
		* [Strongly typed accessing to a capabilities](./w2110.md)
		* [General (not strongly typed) accessing to a capabilities](./w2120.md)
			* [Capability Containers](./w2121.md)
			* [Custom capability](./w2122.md)
			* [String values](./w2123.md)
	* [Image Layout](./w2200.md)
	* [Alternative User Interfaces](./w2300.md)
	* [Options for Transferring Data](./w2400.md)
		* [Native Mode Transfer](./w2410.md)
			* [Using a IStreamProvider for caching data of acquired image](./w2411.md)
			* [Using a IImageHandler for processing a acquired image data from unmanaged memory to a stream](./w2412.md)
			* [Using a IImageFactory for processing acquired a image data from a stream to a image](./w2413.md)
		* [Disk File Mode Transfer](./w2420.md)
		* [Buffered Memory Mode Transfer](./w2430.md)
		* [Buffered Memory Mode Transfer With File Format](./w2440.md)
	* [Cancel scanning](./w2500.md)
	* [Color Information for an Image](./w2600.md)
	* [Image Information and Extended Image Information](./w2700.md)
* [Use TWAIN 2.x](./w3000.md)

_If you notice an error, please let me know about it._