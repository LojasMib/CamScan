using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Atualizate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string repoOwner = "AmauryMagno";
        private static readonly string repoName = "CamScan";
        private static readonly string branchName = "Develop";

        private static readonly string token = "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";
        public MainWindow()
        {
            InitializeComponent();
        }

        public static async Task<string> GetRemoteVersionAsync()
        {
            using(HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.TryParseAdd("request");

                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var url = $"https://api.github.com/repos/{repoOwner}/{repoName}/contents/version.txt?ref={branchName}";

                try
                {
                    var response = await client.GetStringAsync(url);
                    return response.Trim();
                }
                catch(HttpRequestException e)
                {
                    Console.WriteLine("Erro ao acessar a API de atualização:" + e.Message);
                    return null;
                }
            }
        }
    }
}