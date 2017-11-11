using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Saraff.Twain.DS.BitmapSource;
using Saraff.Twain.DS.Capabilities;

namespace Saraff.Twain.DS.Screen {

    [Guid("185C9C7B-E6CA-4D50-8D8F-318809689A25")]
    [DeviceProperties(
        96f, //ICAP_PHYSICALWIDTH (DevicePropertiesAttribute or DataSource.OnCapabilityValueNeeded(CapabilityEventArgs))
        96f, //ICAP_PHYSICALHEIGHT (DevicePropertiesAttribute or DataSource.OnCapabilityValueNeeded(CapabilityEventArgs))
        96f, //ICAP_XNATIVERESOLUTION (DevicePropertiesAttribute or DataSource.OnCapabilityValueNeeded(CapabilityEventArgs))
        96f)] //ICAP_YNATIVERESOLUTION (DevicePropertiesAttribute or DataSource.OnCapabilityValueNeeded(CapabilityEventArgs))
    [ImageLayout(0f,0f,1.04167f,1.04167f)]
    [PlanarChunky(TwPC.Chunky,DefaultValue = TwPC.Chunky)] //ICAP_PLANARCHUNKY (PlanarChunkyAttribute or DataSource.OnCapabilityValueNeeded(CapabilityEventArgs))
    [PixelType(TwPixelType.RGB,DefaultValue = TwPixelType.RGB)] //ICAP_PIXELTYPE (PixelTypeAttribute or DataSource.OnCapabilityValueNeeded(CapabilityEventArgs))
    [XferMech(File = true)/*ICAP_XFERMECH*/, MemXferBufferSize(64*1024U /*64K*/)/*TW_SETUPMEMXFER.Preferred on DG_CONTROL / DAT_SETUPMEMXFER / MSG_GET operation*/]
    public sealed class SampleDataSource:BitmapDataSource {

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleDataSource"/> class.
        /// </summary>
        public SampleDataSource() {
            this[TwCap.XResolution].Value=new float[] { 96f };
            this[TwCap.XResolution].Value=(DefaultValue<float>)96f;
            this[TwCap.YResolution].Value=new float[] { 96f };
            this[TwCap.YResolution].Value=(DefaultValue<float>)96f;
        }

        /// <summary>
        /// If showUI is <c>true</c>, the Source should display its user interface and wait for
        /// the user to initiate an acquisition. If showUI is <c>false</c>,the Source should
        /// immediately begin acquiring data based on its current configuration (a device that requires the
        /// user to push a button on the device,such as a hand-scanner,will be “armed” by this operation and
        /// will assert MSG_XFERREADY as soon as the Source has data ready for transfer). The Source should
        /// fail any attempt to set a capability value (TWRC_FAILURE / TWCC_SEQERROR) until it returns to
        /// State 4 (unless an exception approval exists via a CAP_EXTENDEDCAPS agreement).
        /// Note: If the application has set showUI or CAP_INDICATORS to <c>true</c>, then the Source is
        /// responsible for presenting the user with appropriate progress indicators regarding the
        /// acquisition and transfer process. If showUI is set to <c>true</c>, CAP_INDICATORS is ignored
        /// and progress and errors are always shown.
        /// Note: It is strongly recommended that all Sources support being enabled without their User
        /// Interface if the application requests (showUI = <c>false</c>). But if your
        /// Source cannot be used without its User Interface, it should enable showing the Source
        /// User Interface (just as if showUI = <c>true</c>) but return TWRC_CHECKSTATUS. All Sources,
        /// however, must support the CAP_UICONTROLLABLE. This capability reports whether or
        /// not a Source allows showUI = <c>false</c>. An application can use this capability to know
        /// whether the Source-supplied user interface can be suppressed before it is displayed.
        /// </summary>
        /// <param name="showUI"><c>true</c> if DS should bring up its UI.</param>
        /// <param name="modalUI"><c>true</c> if the DS's UI is modal.</param>
        /// <param name="hwnd">For windows only - Application window handle.</param>
        protected override void OnEnableDS(bool showUI,bool modalUI,IntPtr hwnd) {
            this.OnXferReady();

            base.OnEnableDS(showUI,modalUI,hwnd);
        }

        /// <summary>
        /// Acquire bitmap image.
        /// </summary>
        /// <returns>
        /// The bitmap image.
        /// </returns>
        protected override Bitmap Acquire() {
            // return new image 100x100
            this._SetImageInfo();
            var _image = new Bitmap(100,100,PixelFormat.Format24bppRgb);
            _image.SetResolution(
                (float)this[TwCap.XResolution].Value,
                (float)this[TwCap.YResolution].Value);
            return _image;
        }

        private void _SetImageInfo() {
            this.XferEnvironment.ImageInfo=new ImageInfo {
                BitsPerPixel=24,
                BitsPerSample=new short[] { 8,8,8 },
                Compression=TwCompression.None,
                ImageLength=100,
                ImageWidth=100,
                PixelType=TwPixelType.RGB,
                Planar=false,
                XResolution=96f,
                YResolution=96f
            };
        }
    }
}
