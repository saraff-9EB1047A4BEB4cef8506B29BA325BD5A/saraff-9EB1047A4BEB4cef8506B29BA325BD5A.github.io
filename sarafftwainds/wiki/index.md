# Saraff.Twain.DS
Saraff.Twain.DS is the skillful class library which allows you to design drivers (a Data Source) of flatbed scanner, web and digital camera and any other TWAIN device from .NET environment. You can use this library in your programs written in any programming languages compatible with .NET technology. 
# TWAIN Specification 
Saraff.Twain.DS implements [TWAIN 2.3 Specification](http://twain.org/scanner-application-developers/specification-and-tools.html). Explanation of [specification](http://twain.org/scanner-application-developers/specification-and-tools.html) is not the purpose of this project. Here will be explained features of the implementation of TWAIN specification.
# Contents
* Introduction
	* [Elements of TWAIN](./w0100.md)
	* [Installation of the Data Source Manager](./w0200.md)
* The Saraff.Twain.DS
	* [The structure of a Source](./w1100.md)
	* [The framework of a Source](./w1200.md)
	* The metadata (attributes) of a Source
		* [DataSourceService Category](./w1310.md)
		* [DataSource Category](./w1320.md)
		* [ImageDataSourceService Category](./w1330.md)
		* [XferEnvironment Category](./w1340.md)
		* [DataSourceCapability Category](./w1350.md)
		* [Capability Category](./w1360.md)
	* Capabilities
		* [The list of implemented capabilities](./w1410.md)
		* [Adding new capabilities](./w1420.md)
* [Minimal Implementation](./w2000.md)
* [Minimal Implementation with a Saraff.Twain.DS.BitmapSource](./w3000.md)
* Advanced Implementation
	* Processing a TWAIN operations (Triplets)
		* [OnProcessRequest](./w4110.md)
	* Capabilities
		* [OnCapabilityChanged](./w4210.md)
		* [OnCapabilityValueNeeded](./w4220.md)
	* Image Layout
		* [DefaultImageLayout](./w4310.md)
		* [CurrentImageLayout](./w4320.md)
	* UI
		* [OnEnableDS](./w4410.md)
		* [OnProcessEvent](./w4420.md)
		* [OnDisableDS](./w4430.md)
		* [OnCloseDSReq](./w4440)
	* The transfer is ready
		* [OnXferReady](./w4510.md)
	* Transferring
		* [OnImageNativeXfer](./w4610.md)
		* [OnImageMemXfer](./w4620.md)
		* [OnSetupFileXfer](./w4630.md)
		* [OnImageFileXfer](./w4640.md)
	* The end of the transfer
		* [OnEndXfer](./w4710.md)
		* [OnResetXfer](./w4720.md)
* Installation of the Saraff.Twain.DS
	* [Tree of directories](./w5100.md)
	* [How create a Windows Installer package (msi)](./w5200.md)

_If you notice an error, please let me know about it._
