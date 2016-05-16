
namespace Zelda.Editor.Workspaces.Scripts
{
    using System.Windows;
    using Atom.Scripting;

    /// <summary>
    /// Interaction logic for ScriptEditorWindow.xaml
    /// </summary>
    public partial class ScriptEditorWindow : Window
    {
        public string Code
        {
            get
            {
                return this.codeBox.Text;
            }

            set
            {
                this.codeBox.Text = value;
            }
        }

        public ScriptEditorWindow()
        {
            InitializeComponent();
        }

        private void OnSaveButtonClicked( object sender, RoutedEventArgs e )
        {
            this.DialogResult = true;
        }

        private void OnCancelButtonClicked( object sender, RoutedEventArgs e )
        {
            this.DialogResult = false;
        }
    }
}
