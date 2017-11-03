---
layout: default
title: Saraff.Twain.NET
---
# Saraff.Twain.NET Extensions Samples (LINQ to TWAIN)
Download [Saraff.Twain.Extensions.zip](https://goo.gl/7KJS6h) Simple, Advanced and Console samples

![]({% link sarafftwain/samples/images/cs1.jpg %})

**Figure 1 - Saraff.Twain.Sample1 on Windows 10**

![]({% link sarafftwain/samples/images/cs2.jpg %})

**Figure 2 - Saraff.Twain.Sample2 on Windows 7**

![]({% link sarafftwain/samples/images/cs3.jpg %})

**Figure 3 - Saraff.Twain.Sample3 on Windows 10**

```c#
// Get array of a Data Sources and add range it to a ComboBox
this.dsComboBox.Items.AddRange(
    this.Dsm.DataSources
        .Select(x => x.Identity.Name)
        .ToArray())
```

```c#
// Get values of a X Resolution
var _resolutions = this.Dsm
    .DataSources.ElementAtOrDefault(this.dsComboBox.SelectedIndex)?
    .GetCapability<float>(TwCap.XResolution).Values;

// Create array a ToolStripItem items and add range it to a DropDownButton
this.resolutionToolStripDropDownButton.DropDownItems.AddRange(
    _resolutions?
        .Where(x => _resolutions.Count()<20 ? true : x.Value%100==0)
        .Select(x => new ToolStripMenuItem(
            string.Format("{0:N0} dpi",x.Value),
            null,
            this._ToolStripItemSelectedHandler) {Tag=x.Value} as ToolStripItem)
        .ToArray());
```

```c#
// Acquire image using a Natives transfer mechanism
this.Dsm.DataSources.ElementAtOrDefault(this.dsComboBox.SelectedIndex)?.NativeTransfer(
    x => {
        this.pictureBox1.Image?.Dispose();
        this.pictureBox1.Image=x.Image;
    },
    null,
    null,
    x => this._ToLog(x.Exception));
```

```c#
// Acquire image
this.Dsm.DataSources.ElementAtOrDefault(_dsIndex)? // get a Data Source
    .GetCapability<float>(TwCap.XResolution).Set(x => 300f).DataSource // set a X Resolution to 300 dpi
    .GetCapability<float>(TwCap.YResolution).Set(x => 300f).DataSource // set a Y Resolution to 300 dpi
    .GetCapability<TwPixelType>(TwCap.IPixelType).Set(x => TwPixelType.RGB).DataSource // set a Pixel Type to a RGB
    .NativeTransfer( // acquire image using a Natives transfer mechanism
        x=> {
            using(var _image = x.Image) { // get instance of image
                _image.Save(Path.GetTempFileName()); // save image to temporary file
            }
        });
```
