using CamScan.Router;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CamScan.Class;
using WpfSystem = System.Windows;
using CamScan.Services;
using CamScan.Pages;
using System.IO;
using WIA;
using System.Runtime.CompilerServices;
using System.Xml;
using PdfSharp.Drawing;
using System.Windows.Controls.Primitives;
using PdfSharp.Charting;
using System.Reflection;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.VisualBasic.ApplicationServices;
using static PdfSharp.Capabilities.Features;
using System.Reflection.Metadata;

namespace CamScan
{
    /// <summary>
    /// Interação lógica para Scanner.xam
    /// </summary>
    public partial class Scanner : Page
    {
        private int quantityScanned {  get; set; }
        private string driverType {  get; set; }

        
        private List<string> ScannedImages = new List<string>();
        private readonly XMLConnect xmlConnect = new XMLConnect();
        private int IndexOfListImagesScanned = 0;
        private string? DriverType { get; set; }
        private string? FolderDocumentoCliente { get; set; }
        private string? FolderConfissaodeDivida { get; set; }
        private string? FolderDespesas { get; set; }

        private string? FolderOutros { get; set; }

        private string? NameFranquia { get; set; }
        private string ChoiceOptionPath { get; set; }
        WiaScanner wiaScanner = new WiaScanner();
        
        
        public Scanner()
        {
            InitializeComponent();
            Loaded += RdBtn_DocCliente_Checked;
            InputError.Visibility = Visibility.Hidden;

            Loaded += OnLoaded;
        }
        private void SettingsFranquias()
        {
            try
            {
                var config = xmlConnect.LoadConfigurations();
                if (config != null && config.Franquias.Any())
                {
                    var ConfigFranquias = config.Franquias.Last();
                    NameFranquia = ConfigFranquias.Franquia;
                }
            }
            catch (Exception ex)
            {
                WpfSystem.MessageBox.Show(ex.Message);
            }
        }
        private ConfigScanner? SettingsDriverType()
        {
            try
            {
                var config = xmlConnect.LoadConfigurations();
                if (config != null && config.Scanner.Any())
                {
                    var scannerType = config.Scanner.Last();
                    return scannerType;
                }
                return null;
            }
            catch (Exception ex)
            {
                WpfSystem.MessageBox.Show(ex.Message);
                return null;
            }
        }

        private ConfigScanner? CallXML()
        {
            try
            {
                var scannerType = SettingsDriverType();
                return scannerType;
            }
            catch (Exception ex)
            {
                WpfSystem.MessageBox.Show(ex.Message);
                return null;
            }
        }

        public void OnLoaded(object sender, RoutedEventArgs e)
        {
            
            RdBtn_DocCliente.IsChecked = true;

            try
            {
                SettingsFranquias();
                var xml = CallXML();
                if(xml.ConfigDriver != null)
                {
                    //CARREGA CONEXÃO COM SCANNER DE PROTOCOLO WIA
                    driverType = xml.ConfigDriver.Type;
                    FolderConfissaodeDivida = xml.FolderConfissaoDivida;
                    FolderDocumentoCliente = xml.FolderDocumentoCliente;
                    FolderDespesas = xml.FolderDespesas;
                    FolderOutros = xml.FolderOutros;
                    if(driverType == "WIA")
                    {
                        try
                        {
                            Device device = wiaScanner.ConnectScan(xml.ConfigDriver.Name);
                            if (device != null)
                            {
                                wiaScanner.SelectedScan = device.Items[1];
                            }
                        }
                        catch (Exception ex)
                        {
                            WpfSystem.MessageBox.Show($"{ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WpfSystem.MessageBox.Show(ex.Message);
            }
        }


        //WIA PROCESS SHOW IMAGE IN TEMPLATE SCANNER
        private void AddRoundRadioButtonsToGrid(int numberOfRadioButtons)
        {
            if(numberOfRadioButtons >= 1)
            {
                SelectImageGrid.ColumnDefinitions.Clear();
                for (int i = 0; i < numberOfRadioButtons; i++)
                {
                    SelectImageGrid.ColumnDefinitions.Add(new ColumnDefinition());
                }

                for (int i = 0; i < numberOfRadioButtons; i++)
                {
                    WpfSystem.Controls.RadioButton radioButton = new WpfSystem.Controls.RadioButton
                    {
                        Width = 15,
                        Height = 15,
                        Margin = new Thickness(2),
                        VerticalAlignment = WpfSystem.VerticalAlignment.Center,
                        HorizontalAlignment = WpfSystem.HorizontalAlignment.Center,
                        GroupName = "SelecImageRadioButtonsGroup",
                        Cursor = WpfSystem.Input.Cursors.Hand,
                        Tag = i

                    };
                    radioButton.Checked += RadioButton_Checked;

                    Grid.SetColumn(radioButton, i);
                    SelectImageGrid.Children.Add(radioButton);
                }
            }
            else
            {
                SelectImageGrid.Children.Clear();
            }
            
        }

        private void ShowImageScannedSelect(int index)
        {
            if(index >=0 && index < ScannedImages.Count)
            {
                var image = ScannedImages[index];
                using(Bitmap bitmap = new Bitmap(image))
                {
                    PictureScanner.Source = ConvertBitmapToBitmapImage(bitmap);
                }
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            WpfSystem.Controls.RadioButton? clickedRadioButton = sender as WpfSystem.Controls.RadioButton;
            if (clickedRadioButton != null)
            {
                IndexOfListImagesScanned = (int)clickedRadioButton.Tag;
                ShowImageScannedSelect(IndexOfListImagesScanned);
            }
        }


        private void AddImageInList(string tempFile)
        {
            try
            {
                if (tempFile != null)
                {
                    Dispatcher.Invoke(() =>
                    {
                        ScannedImages.Add(tempFile);

                        AddRoundRadioButtonsToGrid(ScannedImages.Count);

                        // Selecionar o RadioButton correspondente ao novo item
                        var lastRadioButton = SelectImageGrid.Children
                            .OfType<WpfSystem.Controls.RadioButton>()
                            .LastOrDefault();

                        if (lastRadioButton != null)
                        {
                            lastRadioButton.IsChecked = true;
                        }

                        if (ScannedImages.Count > 0 && RdBtn_ConfissaoDivida.IsChecked == true)
                        {
                            Escanear.Text = "Adicionar";
                        }
                    });
                    
                    
                }
            }
            catch (Exception ex)
            {
                WpfSystem.MessageBox.Show($"{ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //TWAIN AND WIA PROCESS CONEVERT BITMAP TO BITMAP IMAGE
        private BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }


        
        //AREA OF USE STATIC BUTTONS
        private void Navigator_Click(object sender, RoutedEventArgs e)
        {
            var clickButton = e.OriginalSource as Navigator;
            NavigationService.Navigate(clickButton?.NavUri);
        }

        private void RdBtn_DocCliente_Checked(object sender, RoutedEventArgs e)
        {
            InputLabel.Content = "Código do Cliente:";
            InputText.Text = "";
        }

        private void RdBtn_ConfissaoDivida_Checked(object sender, RoutedEventArgs e)
        {
            InputLabel.Content = "Data das confissões:";
            DateTime dataAtual = DateTime.Today;
            string DataFormatada = dataAtual.ToString("dd-MM-yyyy"); 
            string data = $"CDF{NameFranquia} - " + DataFormatada;
            InputText.Text = data;
        }

        private void RdBtn_Despesas_Checked(object sender, RoutedEventArgs e)
        {
            InputLabel.Content = "Despesas:";
            InputText.Text = "";
        }

        private void RdBtn_Outros_Checked(object sender, RoutedEventArgs e)
        {
            InputLabel.Content = "Outros:";
            InputText.Text = "";
        }


        //AREA OF USE ACTION BUTTONS
        private async void Escanear_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            //UTILIZA SCANNER DO TIPO WIA
            if (driverType == "WIA")
            {
                try
                {
                    Dispatcher.Invoke(() => { loadingControl.Visibility = Visibility.Visible; });
                    await Task.Run(() =>
                    {
                        ScanDocument();
                    });
                }
                finally
                {
                    Dispatcher.Invoke(() =>
                    {
                        loadingControl.Visibility = Visibility.Collapsed;
                    });
                    Btn_Cancel.Visibility = Visibility.Visible;
                } 
            }
            else
            {
                new Error("Escaner não configurado, contate a TI!, Error:(_twain.AppproductName = null)");
            }
        }

        private void ScanDocument()
        {
            try
            {
                wiaScanner.Scan();
                if (wiaScanner._imageFile != null)
                {
                    string tempFolderPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "ScamCam");

                    if (!Directory.Exists(tempFolderPath))
                    {
                        Directory.CreateDirectory(tempFolderPath);
                    }
                    string tempFilePath = System.IO.Path.Combine(tempFolderPath, Guid.NewGuid().ToString() + ".bmp");
                    wiaScanner._imageFile.SaveFile(tempFilePath);

                    AddImageInList(tempFilePath);
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    WpfSystem.MessageBox.Show($"Erro nas configurações do scanner WIA {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                });
                
            }
        }

        //Cancelar Escaneamento

        private void Scanned_Close()
        {
            Btn_Cancel.Visibility = Visibility.Hidden;
            PictureScanner.Source = null;
            Escanear.Text = "Escanear";
            ScannedImages.Clear();
            AddRoundRadioButtonsToGrid(ScannedImages.Count);
        }

        private void Salvar_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (ScannedImages.Count > 0)
            {
                if (RdBtn_DocCliente.IsChecked == true)
                {
                    if (FolderDocumentoCliente == null || FolderDocumentoCliente == "")
                    {
                        WpfSystem.MessageBox.Show("Pasta de Documento de Cliente não configurada", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    string CodigoCliente = InputText.Text;
                    if (CodigoCliente == null || CodigoCliente.Length == 0 || CodigoCliente == "")
                    {
                        WpfSystem.MessageBox.Show("Digite um codigo de cliente antes te salvar!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    
                    try
                    {
                        SaveFolderScan saveFolder = new SaveFolderScan();
                        saveFolder.SaveDocumentoCliente(ScannedImages, FolderDocumentoCliente, CodigoCliente);
                        Scanned_Close();
                        InputText.Text = "";

                    }
                    catch (Exception ex)
                    {
                        WpfSystem.MessageBox.Show($"Erro ao salvar a imagem de cliente: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else if(RdBtn_ConfissaoDivida.IsChecked == true)
                {
                    if (FolderConfissaodeDivida == null || FolderConfissaodeDivida == "")
                    {
                        WpfSystem.MessageBox.Show("Pasta de Confissão de Divida não configurada", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    string dataConfissao = InputText.Text;
                    if (dataConfissao == null || dataConfissao.Length == 0 || dataConfissao == "")
                    {
                        WpfSystem.MessageBox.Show("Digite a data no padrão estabelecido para salvar a imagem!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    try
                    {
                        SaveFolderScan saveFolder = new SaveFolderScan();
                        saveFolder.SaveConfissaoDivida(ScannedImages, FolderConfissaodeDivida, dataConfissao);
                        Scanned_Close();
                    }
                    catch(Exception ex)
                    {
                        WpfSystem.MessageBox.Show($"Erro ao salvar a imagem da confissão: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else if(RdBtn_Despesas.IsChecked == true)
                {
                    if(FolderDespesas == null || FolderDespesas == "")
                    {
                        WpfSystem.MessageBox.Show("Pasta de Despesas não configurada", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    string despesas = InputText.Text;
                    if(despesas == null || despesas.Length == 0 || despesas == "")
                    {
                        WpfSystem.MessageBox.Show("Digite um nome para o arquivo de Despesas antes te salvar!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    try
                    {
                        SaveFolderScan saveFolder = new SaveFolderScan();
                        saveFolder.SaveDespesas(ScannedImages, FolderDespesas, despesas);
                        Scanned_Close();
                    }
                    catch (Exception ex)
                    {
                        WpfSystem.MessageBox.Show($"Erro ao salvar a imagem da confissão: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else if (RdBtn_Outros.IsChecked == true)
                {
                    if (FolderOutros == null || FolderOutros == "")
                    {
                        WpfSystem.MessageBox.Show("Pasta de Outros não configurada", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    string outros = InputText.Text;
                    if (outros == null || outros.Length == 0 || outros == "")
                    {
                        WpfSystem.MessageBox.Show("Digite um nome para o arquivo de Outros antes te salvar!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    try
                    {
                        SaveFolderScan saveFolder = new SaveFolderScan();
                        saveFolder.SaveOutros(ScannedImages, FolderOutros, outros);
                        Scanned_Close();
                    }
                    catch (Exception ex)
                    {
                        WpfSystem.MessageBox.Show($"Erro ao salvar a imagem da confissão: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                WpfSystem.MessageBox.Show("Escaneie uma imagem antes de salvar", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancelar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Scanned_Close();
        }

        private void Btn_Cancel_MouseMove(object sender, WpfSystem.Input.MouseEventArgs e)
        {
            var bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.UriSource = new Uri("pack://application:,,,/Images/Scanner/Cancel_Red.png");
            bitmap.EndInit();

            Btn_Cancel.Source = bitmap;
        }

        private void Btn_Cancel_MouseLeave(object sender, WpfSystem.Input.MouseEventArgs e)
        {
            var bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.UriSource = new Uri("pack://application:,,,/Images/Scanner/Cancel_Gray.png");
            bitmap.EndInit();

            Btn_Cancel.Source = bitmap;
        }

        private void Btn_Cancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(IndexOfListImagesScanned > 0)
            {
                ScannedImages.RemoveAt(IndexOfListImagesScanned);
                AddRoundRadioButtonsToGrid(ScannedImages.Count());
            }
        }
    }
}
