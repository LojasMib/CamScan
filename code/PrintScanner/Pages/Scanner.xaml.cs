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
using CamScan.Components;
using Saraff.Twain;
using System.Security.Cryptography;

namespace CamScan
{
    /// <summary>
    /// Interação lógica para Scanner.xam
    /// </summary>
    public partial class Scanner : Page
    {
        private string? DataConfissao { get; set; }
        private const string AcessKey = "SENHA";

        private Twain32 _twain;

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

        public void ScannerDeviceConnectWIA(string deviceName)
        {
            Device device = wiaScanner.ConnectScan(deviceName);
            if (device != null)
            {
                wiaScanner.SelectedScan = device.Items[1];
            }
        }

        public void ScannerDeviceConnectTWAIN(string deviceName)
        {
            var mainWindow = WpfSystem.Application.Current.MainWindow;
            var windowInteropHelper = new System.Windows.Interop.WindowInteropHelper(mainWindow);
            var win32Window = new Win32Window(windowInteropHelper.Handle);
            _twain = new Twain32()
            {
                Parent = win32Window,
                ShowUI = false,
            };

            _twain.AcquireCompleted += _twain_AcquireCompleted;


            if (!_twain.OpenDSM())
            {
                throw new Exception("Não foi possível abrir o DSM.");
            }

            int sourcesCount = _twain.SourcesCount;
            int matchingIndex = -1;
            if (sourcesCount > 0)
            {
                for (int i = 0; i < sourcesCount; i++)
                {
                    string sourceName = _twain.GetSourceProductName(i);

                    if (sourceName.Equals(deviceName, StringComparison.OrdinalIgnoreCase))
                    {
                        matchingIndex = i;
                        break;
                    }
                }
            }

            if (matchingIndex != -1)
            {
                _twain.CloseDataSource();
                _twain.SourceIndex = matchingIndex;
                _twain.OpenDataSource();
                _twain.SetCap(TwCap.IPixelType, TwPixelType.RGB);
                _twain.SetCap(TwCap.XResolution, 300); // Resolução X em DPI
                _twain.SetCap(TwCap.YResolution, 300); // Resolução Y em DPI

                _twain.SetCap(TwCap.Brightness, 0); // Brilho padrão
                _twain.SetCap(TwCap.Contrast, 0);   // Contraste padrão

                _twain.SetCap(TwCap.AutomaticColorEnabled, true); // Habilita detecção automática de cores
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
                    
                    driverType = xml.ConfigDriver.Type;
                    FolderConfissaodeDivida = xml.FolderConfissaoDivida;
                    FolderDocumentoCliente = xml.FolderDocumentoCliente;
                    FolderDespesas = xml.FolderDespesas;
                    FolderOutros = xml.FolderOutros;

                    //CARREGA CONEXÃO COM SCANNER DE PROTOCOLO WIA
                    if (driverType == "WIA")
                    {
                        try
                        {
                            ScannerDeviceConnectWIA(xml.ConfigDriver.Name);
                        }
                        catch (Exception ex)
                        {
                            WpfSystem.MessageBox.Show($"{ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    //CARREGA CONEXÃO COM SCANNER DE PROTOCOLO TWAIN
                    else if (driverType == "TWAIN")
                    {
                        try
                        {
                            ScannerDeviceConnectTWAIN(xml.ConfigDriver.Name);
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

        //TWAIN PROCESS SAVE IMAGE IN TEMPLATE
        private void _twain_AcquireCompleted(object sender, EventArgs e)
        {
            try
            {
                if (_twain.ImageCount > 0)
                {
                    using (var image = _twain.GetImage(0))
                    {
                        string tempFolderPath = CreateTempFolder();
                        string tempFilePath = System.IO.Path.Combine(tempFolderPath, Guid.NewGuid().ToString() + ".bmp");


                        var bitmap = new Bitmap(image);
                        BitmapImage bitmapImage = ConvertBitmapToBitmapImage(new Bitmap(bitmap));
                        bitmap.Save(tempFilePath);

                        AddImageInList(tempFilePath);
                        PictureScanner.Source = bitmapImage;
                        DisableButtonsNotChecked();
                        CursorPointerSet();
                    }

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


        //RESTRICTIONS OF INPUT DOC CLIENTE
        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            // Permite apenas números
            e.Handled = !IsTextNumeric(e.Text) && !( e.Text == "F" || e.Text == "V");
        }

        private bool IsTextNumeric(string text)
        {
            // Verifica se o texto é numérico
            return int.TryParse(text, out _);
        }


        //AREA OF USE STATIC BUTTONS
        private void Navigator_Click(object sender, RoutedEventArgs e)
        {
            var clickButton = e.OriginalSource as Navigator;
            NavigationService.Navigate(clickButton?.NavUri);
        }

        private void RdBtn_DocCliente_Checked(object sender, RoutedEventArgs e)
        {
            InputText.Visibility = Visibility.Visible;
            InputDate.Visibility = Visibility.Hidden;
            Btn_Lock.Visibility = Visibility.Hidden;
            InputLabel.Content = "Código do Cliente:";
            InputText.Text = "";
            InputText.IsReadOnly = false;
            InputText.PreviewTextInput += TextBox_PreviewTextInput;
        }

        private void RdBtn_ConfissaoDivida_Checked(object sender, RoutedEventArgs e)
        {
            InputLabel.Content = "Data das confissões:";
            InputText.Visibility = Visibility.Hidden;
            InputDate.Visibility = Visibility.Visible;
            Btn_Lock.Visibility = Visibility.Visible;

            InputDate.IsEnabled = false;
            DateTime dataAtual = DateTime.Today;
            InputDate.SelectedDate = dataAtual;
            string DataFormatada = InputDate.SelectedDate.Value.ToString("dd-MM-yyyy"); 
            DataConfissao = $"CDF{NameFranquia} - {DataFormatada}";
            
        }

        private void RdBtn_Despesas_Checked(object sender, RoutedEventArgs e)
        {
            InputText.PreviewTextInput -= TextBox_PreviewTextInput;
            InputText.Visibility = Visibility.Visible;
            InputDate.Visibility = Visibility.Hidden;
            Btn_Lock.Visibility = Visibility.Hidden;
            InputLabel.Content = "Despesas:";
            InputText.Text = "";
            InputText.IsReadOnly = false;
            
        }

        private void RdBtn_Outros_Checked(object sender, RoutedEventArgs e)
        {
            InputText.PreviewTextInput -= TextBox_PreviewTextInput;
            InputText.Visibility = Visibility.Visible;
            InputDate.Visibility = Visibility.Hidden;
            Btn_Lock.Visibility = Visibility.Hidden;
            InputLabel.Content = "Outros:";
            InputText.Text = "";
            InputText.IsReadOnly = false;
            
        }


        private void VisibilityBtn_Cancel()
        {
            if (ScannedImages.Count > 1)
            {
                Btn_Cancel.Visibility = Visibility.Visible;
            }
        }

        //AREA OF USE ACTION BUTTONS
        private async void Escanear_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
            
            if (ScannedImages.Count > 0 && (RdBtn_DocCliente.IsChecked == true || RdBtn_Despesas.IsChecked == true || RdBtn_Outros.IsChecked == true))
            {
                return;
            }
            if (IndexOfListImagesScanned >= 2)
            {
                WpfSystem.MessageBox.Show("A quantidade de imagens por documento não pode ser maior que 8", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            //UTILIZA SCANNER DO TIPO WIA
            if (driverType == "WIA")
            {
                try
                {
                    Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                    Dispatcher.Invoke(() => { loadingControl.Visibility = Visibility.Visible; });
                    await Task.Run(() =>
                    {
                        ScanDocument();
                    });
                }
                catch
                {
                    new Error("Erro no escaneamento, contate a TI!, Error:( utilização scanner Wia )");
                }

                finally
                {
                    Dispatcher.Invoke(() =>
                    {
                        loadingControl.Visibility = Visibility.Collapsed;
                        VisibilityBtn_Cancel();
                    });
                    InputText.PreviewTextInput -= TextBox_PreviewTextInput;
                    Mouse.OverrideCursor = null;
                } 
            }
            
            //UTILIZA SCANNER DO TIPO TWAIN
            else if (driverType == "TWAIN")
            {
                try
                {
                    Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                    Dispatcher.Invoke(() => { loadingControl.Visibility = Visibility.Visible; });

                    _twain.Acquire();
                }
                catch
                {
                    new Error("Erro no escaneamento, contate a TI!, Error:( utilização scanner Twain )");
                }

                finally
                {
                    Dispatcher.Invoke(() =>
                    {
                        loadingControl.Visibility = Visibility.Collapsed;
                        Btn_Cancel.Visibility = Visibility.Visible;
                    });
                    
                    Mouse.OverrideCursor = null;
                }
            }

            else
            {
                new Error("Driver do Scanner não configurado, contate a TI!");
            }
        }

        private void ScanDocument()
        {
            try
            {
                wiaScanner.Scan();
                if (wiaScanner._imageFile != null)
                {
                    string tempFolderPath = CreateTempFolder();
                    string tempFilePath = System.IO.Path.Combine(tempFolderPath, Guid.NewGuid().ToString() + ".bmp");
                    wiaScanner._imageFile.SaveFile(tempFilePath);

                    AddImageInList(tempFilePath);
                    DisableButtonsNotChecked();
                    CursorPointerSet();
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




        //DISABLE BUTTONS TYPE CHECKBOX NOT CHECKED
        private void DisableButtonsNotChecked()
        {
            Dispatcher.Invoke(() =>
            {
                if (RdBtn_DocCliente.IsChecked == false)
                {
                    RdBtn_DocCliente.Cursor = System.Windows.Input.Cursors.No;
                    Escanear.Cursor = System.Windows.Input.Cursors.No;
                    RdBtn_DocCliente.IsEnabled = false;
                    Escanear.IsEnabled = false;
                }
                if (RdBtn_Despesas.IsChecked == false)
                {
                    RdBtn_Despesas.Cursor = System.Windows.Input.Cursors.No;
                    Escanear.Cursor = System.Windows.Input.Cursors.No;
                    RdBtn_Despesas.IsEnabled = false;
                    Escanear.IsEnabled = false;
                }
                if (RdBtn_Outros.IsChecked == false)
                {
                    RdBtn_Outros.Cursor = System.Windows.Input.Cursors.No;
                    Escanear.Cursor = System.Windows.Input.Cursors.No;
                    RdBtn_Outros.IsEnabled = false;
                    Escanear.IsEnabled = false;
                    
                }
                if (RdBtn_ConfissaoDivida.IsChecked == false)
                {
                    RdBtn_ConfissaoDivida.Cursor = System.Windows.Input.Cursors.No;
                    Escanear.Cursor = System.Windows.Input.Cursors.No;
                    RdBtn_ConfissaoDivida.IsEnabled = false;
                    Escanear.IsEnabled = false;
                    Escanear.Background = new SolidColorBrush((WpfSystem.Media.Color)WpfSystem.Media.ColorConverter.ConvertFromString("#D3D3D3"));
                }
            });
            
        }

        private void CursorPointerSet()
        {
            Dispatcher.Invoke(() =>
            {
                if (RdBtn_ConfissaoDivida.IsChecked == true)
                {
                    Escanear.IsEnabled = true;
                    Escanear.Cursor = System.Windows.Input.Cursors.Hand;
                }
            });

        }


        private void EnableButtonsTypeCheckBox()
        {
            RdBtn_DocCliente.IsEnabled = true;
            RdBtn_DocCliente.Cursor = System.Windows.Input.Cursors.Hand;

            RdBtn_ConfissaoDivida.IsEnabled = true;
            RdBtn_ConfissaoDivida.Cursor = System.Windows.Input.Cursors.Hand;

            RdBtn_Despesas.IsEnabled = true;
            RdBtn_Despesas.Cursor = System.Windows.Input.Cursors.Hand;

            RdBtn_Outros.IsEnabled = true;
            RdBtn_Outros.Cursor = System.Windows.Input.Cursors.Hand;

            Escanear.IsEnabled = true;
            Escanear.Cursor = System.Windows.Input.Cursors.Hand;

            InputText.Text = "";
            Escanear.Background = new SolidColorBrush((WpfSystem.Media.Color)WpfSystem.Media.ColorConverter.ConvertFromString("#F69D0D"));
        }

        private string CreateTempFolder()
        {
            string tempFolderPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "ScamCam");

            if (!Directory.Exists(tempFolderPath))
            {
                Directory.CreateDirectory(tempFolderPath);
            }

            return tempFolderPath;
        }

        private void DeleteTempFolder()
        {
            string tempFolderPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "ScamCam");
            if (Directory.Exists(tempFolderPath))
            {
                Directory.Delete(tempFolderPath, true);
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

        private string? Select_File_Type()
        {
            ChoiceExportFile choiceExportFile = new ChoiceExportFile();
            Window parentWindow = Window.GetWindow(this);
            choiceExportFile.ShowDialog();
            if (choiceExportFile.TypeFile == "PDF")
            {
                return "PDF";
            }
            else if (choiceExportFile.TypeFile == "JPG")
            {
                return "JPG";
            }
            else
            {
                return null;
            }

        }

        private async void Sucess_Message()
        {
            MessageSucess.Visibility = Visibility.Visible;
            await Task.Delay(3000);// 1000 => 1 segundo
            MessageSucess.Visibility = Visibility.Hidden;
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
                        EnableButtonsTypeCheckBox();
                        DeleteTempFolder();
                        Sucess_Message();

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
                    if (DataConfissao == null || DataConfissao.Length == 0 || DataConfissao == "")
                    {
                        WpfSystem.MessageBox.Show("Digite a data no padrão estabelecido para salvar a imagem!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    try
                    {
                        SaveFolderScan saveFolder = new SaveFolderScan();
                        saveFolder.SaveConfissaoDivida(ScannedImages, DataConfissao, FolderConfissaodeDivida);
                        Scanned_Close();
                        EnableButtonsTypeCheckBox();
                        DeleteTempFolder();
                        Sucess_Message();
                    }
                    catch(Exception ex)
                    {
                        WpfSystem.MessageBox.Show($"Erro ao salvar a imagem da Confissão: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        string typeFile = Select_File_Type();
                        if (typeFile == null)
                        {
                            throw new Exception("O tipo de arquivo não foi selecionado.");
                        }
                        
                        saveFolder.SaveDespesas(ScannedImages, FolderDespesas, despesas, typeFile);
                        Scanned_Close();
                        EnableButtonsTypeCheckBox();
                        DeleteTempFolder();
                        Sucess_Message();
                    }
                    catch (Exception ex)
                    {
                        WpfSystem.MessageBox.Show($"Erro ao salvar a imagem da Despesas: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        string typeFile = Select_File_Type();
                        if (typeFile == null)
                        {
                            throw new Exception("O tipo de arquivo não foi selecionado.");
                        }
                        SaveFolderScan saveFolder = new SaveFolderScan();
                        saveFolder.SaveOutros(ScannedImages, FolderOutros, outros, typeFile);
                        Scanned_Close();
                        EnableButtonsTypeCheckBox();
                        DeleteTempFolder();
                        Sucess_Message();
                    }
                    catch (Exception ex)
                    {
                        WpfSystem.MessageBox.Show($"Erro ao salvar a imagem da Outros: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
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
            EnableButtonsTypeCheckBox();
            DeleteTempFolder();
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
            if(ScannedImages.Any() && IndexOfListImagesScanned >= 0)
            {
                ScannedImages.RemoveAt(IndexOfListImagesScanned);
                AddRoundRadioButtonsToGrid(ScannedImages.Count());
            }
        }

        private void Btn_Lock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            KeyAcess keyAcess = new KeyAcess(AcessKey);
            Window parentWindow = Window.GetWindow(this);

            if (parentWindow != null)
            {
                keyAcess.Owner = parentWindow;
            }
            keyAcess.ShowDialog();
            if (keyAcess.FreeAcess == true)
            {
                InputDate.IsEnabled = true;
                keyAcess.Close();
            }
        }
    }
}
