using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Torsion.Apps.IconConverter.Utils;

namespace Torsion.Apps.IconConverter.ViewModels;

internal class IconViewModel : BaseViewModel
{
    private static readonly int[] IconSizes = { 16, 32, 48, 64, 128, 256 };
    private System.Windows.Media.Color selectedColor = System.Windows.Media.Colors.White;
    private bool fillBackground = false;

    public bool FillBackground
    {
        get
        {
            return fillBackground;
        }
        set
        {
            fillBackground = value;
            OnPropertyChanged(nameof(FillBackground));
        }
    }
    public System.Windows.Media.Color SelectedColor
    {
        get
        {
            return selectedColor;
        }
        set
        {
            selectedColor = value;
            ColorText = $"({value.R}, {value.G}, {value.B})";
            OnPropertyChanged(nameof(SelectedColor));
            OnPropertyChanged(nameof(FillColor));
            OnPropertyChanged(nameof(ColorText));
        }
    }
    public string ColorText { get; set; }
    public System.Windows.Media.SolidColorBrush FillColor => new System.Windows.Media.SolidColorBrush(SelectedColor);

    public IconViewModel(Window win)
    {
        Win = win;
        WindowTitle = "Image Converter";
        OkCMD = new RelayCommand(OnOk);
        PickColorCMD = new RelayCommand(OnPickColor);
        CloseCMD = new RelayCommand(OnClose);
    }

    public ICommand OkCMD { get; set; }
    public ICommand PickColorCMD { get; set; }
    public ICommand CloseCMD { get; set; }
    private void OnOk()
    {
        try
        {
            Microsoft.Win32.OpenFileDialog openDialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Select PNG File",
                Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg",
                CheckFileExists = true,
                Multiselect = false
            };
            if(openDialog.ShowDialog() != true)
            {
                return;
            }
            string inputPath = openDialog.FileName;
            string defaultOutputName = System.IO.Path.GetFileNameWithoutExtension(inputPath) + ".ico";
            Microsoft.Win32.SaveFileDialog saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Save ICO File",
                Filter = "Icon Files (*.ico)|*.ico",
                FileName = defaultOutputName,
                AddExtension = true,
                DefaultExt = ".ico",
                OverwritePrompt = true
            };
            if(saveDialog.ShowDialog() != true)
            {
                return;
            }
            Convert(inputPath, saveDialog.FileName);
            System.Windows.MessageBox.Show(Win, $"ICO file created:\n{saveDialog.FileName}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch(Exception ex)
        {
            System.Windows.MessageBox.Show(Win, ex.Message, "Conversion Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    private void OnPickColor()
    {
        using ColorDialog dialog = new System.Windows.Forms.ColorDialog
        {
            AllowFullOpen = true,
            FullOpen = true,
            SolidColorOnly = false,
            Color = System.Drawing.Color.FromArgb(
              SelectedColor.A,
              SelectedColor.R,
              SelectedColor.G,
              SelectedColor.B)
        };

        if(dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            SelectedColor = System.Windows.Media.Color.FromArgb(
                dialog.Color.A,
                dialog.Color.R,
                dialog.Color.G,
                dialog.Color.B);
        }
    }
    private void OnClose()
    {
        Win.Close();
    }
    public void Convert(string imagePath, string icoPath)
    {
        string ext = Path.GetExtension(imagePath).ToLowerInvariant();
        using Bitmap originalRaw = new Bitmap(imagePath);
        using Bitmap original = Ensure32bpp(originalRaw);
        using MemoryStream ms = new MemoryStream();
        using BinaryWriter bw = new BinaryWriter(ms);
        bw.Write((ushort)0);
        bw.Write((ushort)1);
        bw.Write((ushort)IconSizes.Length);
        (int size, MemoryStream stream)[] imageStreams = IconSizes
            .Select(size =>
            {
                Bitmap resized = ResizeBitmap(original, size, size);
                MemoryStream stream = new MemoryStream();
                resized.Save(stream, ImageFormat.Png);
                stream.Position = 0;
                resized.Dispose();
                return (size, stream);
            })
            .ToArray();
        int imageOffset = 6 + (16 * imageStreams.Length);
        foreach((int size, MemoryStream? stream) in imageStreams)
        {
            bw.Write((byte)(size >= 256 ? 0 : size));
            bw.Write((byte)(size >= 256 ? 0 : size));
            bw.Write((byte)0);
            bw.Write((byte)0);
            bw.Write((ushort)1);
            bw.Write((ushort)32);
            bw.Write((uint)stream.Length);
            bw.Write((uint)imageOffset);
            imageOffset += (int)stream.Length;
        }
        foreach((int _, MemoryStream? stream) in imageStreams)
        {
            stream.CopyTo(ms);
            stream.Dispose();
        }
        File.WriteAllBytes(icoPath, ms.ToArray());
    }
    private static Bitmap ResizeBitmap(Bitmap source, int width, int height)
    {
        Bitmap result = new Bitmap(width, height);
        result.SetResolution(source.HorizontalResolution, source.VerticalResolution);
        using Graphics graphics = Graphics.FromImage(result);
        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        graphics.DrawImage(source, 0, 0, width, height);
        return result;
    }
    private Bitmap Ensure32bpp(Bitmap source)
    {
        Bitmap clone = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppArgb);
        using Graphics g = Graphics.FromImage(clone);
        if(FillBackground)
        {
            g.Clear(System.Drawing.Color.FromArgb(
              SelectedColor.A,
              SelectedColor.R,
              SelectedColor.G,
              SelectedColor.B));
        }
        g.DrawImage(source, 0, 0, source.Width, source.Height);
        return clone;
    }
}