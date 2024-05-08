using DHDM;
using SheetsPersist;
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

namespace LiveVideoAnimationSandbox;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    LiveVideoEditor liveVideoEditor;
    public MainWindow()
    {
        InitializeComponent();
        liveVideoEditor = new LiveVideoEditor();
        VideoAnimationManager.Initialize(liveVideoEditor);
        GoogleSheets.RegisterDocumentID("Live Video Animation", "1_axdPNFXyWqGkdwGtLkmqHWdgTFLq4nMbacKHa0b5mQ");
        DmxLight.Enabled = false;
    }

    private void btnShowEditor_Click(object sender, RoutedEventArgs e)
    {
        new FrmLiveAnimationEditor().Show();
    }
}