using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

[assembly: XmlnsDefinition("http://schemas.microsoft.com/winfx/2006/xaml/presentation",
    "NeuroSpeech.XamlToPDF.Xaml")]

namespace NeuroSpeech.XamlToPDF.Xaml
{
    public class InlineDisposable : IDisposable
    {
        private Action d;

        public InlineDisposable(Action disposeAction)
        {
            d = disposeAction;
        }

        public void Dispose()
        {
            d();
        }
    }

    [ContentProperty("Base64Source")]
    public class InlineImage : Image 
    {
        //#region DependencyProperty 'Width'

        ///// <summary>
        ///// Gets or sets the width.
        ///// </summary>
        //public double Width
        //{
        //    get { return (double)GetValue(WidthProperty); }
        //    set { SetValue(WidthProperty, value); }
        //}

        ///// <summary>
        ///// Registers a dependency property to get or set the width
        ///// </summary>
        //public static readonly DependencyProperty WidthProperty =
        //    DependencyProperty.Register("Width", typeof(double),
        //    typeof(InlineImage),
        //    new FrameworkPropertyMetadata(Double.NaN));

        //#endregion

        //#region DependencyProperty 'Height'

        ///// <summary>
        ///// Gets or sets the height.
        ///// </summary>
        //public double Height
        //{
        //    get { return (double)GetValue(HeightProperty); }
        //    set { SetValue(HeightProperty, value); }
        //}

        ///// <summary>
        ///// Registers a dependency property to get or set the height
        ///// </summary>
        //public static readonly DependencyProperty HeightProperty =
        //    DependencyProperty.Register("Height", typeof(double),
        //    typeof(InlineImage),
        //    new FrameworkPropertyMetadata(Double.NaN));

        //#endregion

        //#region DependencyProperty 'MaxHeight'

        ///// <summary>
        ///// Gets or sets the height.
        ///// </summary>
        //public double MaxHeight
        //{
        //    get { return (double)GetValue(MaxHeightProperty); }
        //    set { SetValue(MaxHeightProperty, value); }
        //}

        ///// <summary>
        ///// Registers a dependency property to get or set the height
        ///// </summary>
        //public static readonly DependencyProperty MaxHeightProperty =
        //    DependencyProperty.Register("MaxHeight", typeof(double),
        //    typeof(InlineImage),
        //    new FrameworkPropertyMetadata(Double.NaN));

        //#endregion

        //#region DependencyProperty 'MaxWidth'

        ///// <summary>
        ///// Gets or sets the height.
        ///// </summary>
        //public double MaxWidth
        //{
        //    get { return (double)GetValue(MaxWidthProperty); }
        //    set { SetValue(MaxWidthProperty, value); }
        //}

        ///// <summary>
        ///// Registers a dependency property to get or set the height
        ///// </summary>
        //public static readonly DependencyProperty MaxWidthProperty =
        //    DependencyProperty.Register("MaxWidth", typeof(double),
        //    typeof(InlineImage),
        //    new FrameworkPropertyMetadata(Double.NaN));

        //#endregion


        //#region DependencyProperty 'Stretch'

        ///// <summary>
        ///// Gets or sets the stretch behavior.
        ///// </summary>
        //public Stretch Stretch
        //{
        //    get { return (Stretch)GetValue(StretchProperty); }
        //    set { SetValue(StretchProperty, value); }
        //}

        ///// <summary>
        ///// Registers a dependency property to get or set the stretch behavior
        ///// </summary>
        //public static readonly DependencyProperty StretchProperty =
        //    DependencyProperty.Register("Stretch", typeof(Stretch),
        //    typeof(InlineImage),
        //    new FrameworkPropertyMetadata(Stretch.Uniform));

        //#endregion

        //#region DependencyProperty 'StretchDirection'

        ///// <summary>
        ///// Gets or sets the stretch direction.
        ///// </summary>
        //public StretchDirection StretchDirection
        //{
        //    get { return (StretchDirection)GetValue(StretchDirectionProperty); }
        //    set { SetValue(StretchDirectionProperty, value); }
        //}

        ///// <summary>
        ///// Registers a dependency property to get or set the stretch direction
        ///// </summary>
        //public static readonly DependencyProperty StretchDirectionProperty =
        //    DependencyProperty.Register("StretchDirection", typeof(StretchDirection),
        //    typeof(InlineImage),
        //    new FrameworkPropertyMetadata(StretchDirection.Both));

        //#endregion

        #region DependencyProperty 'Base64Source'

        /// <summary>
        /// Gets or sets the base64 source.
        /// </summary>
        public string Base64Source
        {
            get { return (string)GetValue(Base64SourceProperty); }
            set { SetValue(Base64SourceProperty, value); }
        }

        /// <summary>
        /// Registers a dependency property to get or set the base64 source
        /// </summary>
        public static readonly DependencyProperty Base64SourceProperty =
            DependencyProperty.Register("Base64Source", typeof(string), typeof(InlineImage),
            new FrameworkPropertyMetadata(null, OnBase64SourceChanged));

        #endregion

        #region Private Members

        private static void OnBase64SourceChanged(DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            var inlineImage = (InlineImage)sender;

            string val = inlineImage.Base64Source;
            if (string.IsNullOrWhiteSpace(val))
                return;

            var stream = new MemoryStream(Convert.FromBase64String(inlineImage.Base64Source));

            
            var bitmapImage = new BitmapImage();
            try
            {
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }
            catch {
                //FlowDocumentPackage.toDispose.Add(new InlineDisposable(() =>
                //{
                //    stream.Dispose();
                //}));
                stream = new MemoryStream(XResources.EmptyImageData);
                bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }
            inlineImage.Source = bitmapImage;

            //FlowDocumentPackage.toDispose.Add(new InlineDisposable(() =>
            //{
            //    inlineImage.Source = null;
            //    stream.Dispose();
            //}));

            //var image = new Image
            //{
            //    Source = bitmapImage,
            //    Stretch = inlineImage.Stretch,
            //    StretchDirection = inlineImage.StretchDirection,
            //};

            //if (!double.IsNaN(inlineImage.Width))
            //{
            //    image.Width = inlineImage.Width;
            //}

            //if (!double.IsNaN(inlineImage.Height))
            //{
            //    image.Height = inlineImage.Height;
            //}

            //if (!double.IsNaN(inlineImage.MaxHeight)) {
            //    image.MaxHeight = inlineImage.MaxHeight;
            //}

            //if (!double.IsNaN(inlineImage.MaxWidth)) {
            //    image.MaxWidth = inlineImage.MaxWidth;
            //}

            //inlineImage.Child = image;
        }

        #endregion

    }
}

