using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfSystem = System.Windows;
using Saraff.Twain;
using System.Windows.Interop;


namespace CamScan.Class
{
    public class TwainScanner
    {
        private Twain32 _twain;

        public TwainScanner()
        {
            _twain = new Twain32();

        }

        public void Connect_SaraffTwain()
        {
            try
            {
                var mainWindow = WpfSystem.Application.Current.MainWindow;
                var windowInteropHelper = new WindowInteropHelper(mainWindow);
                var win32Window = new Win32Window(windowInteropHelper.Handle);

                _twain.IsTwain2Enable = true;
                _twain.Parent = win32Window;
            }
            catch (Exception ex)
            {
                WpfSystem.MessageBox.Show($"Erro ao conectar com o TWAIN: {ex.Message}", "Erro", WpfSystem.MessageBoxButton.OK, WpfSystem.MessageBoxImage.Error);
            }
        }

        public List<string>? ListDevices()
        {
            try
            {
                _twain.CloseDataSource();
                _twain.OpenDSM();
                List<string> twainDevices = new List<string>();
                for (int i = 0; i < _twain.SourcesCount; i++)
                {
                    twainDevices.Add(_twain.GetSourceProductName(i));
                }
                _twain.CloseDataSource();
                _twain.CloseDSM();
                return twainDevices;
            }catch (Exception ex)
            {
                WpfSystem.MessageBox.Show($"Erro ao listar os devices TWAIN {ex.Message}", "Erro", WpfSystem.MessageBoxButton.OK, WpfSystem.MessageBoxImage.Error);
                return null;
            }
            
        }

    }
}
