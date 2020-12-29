using AssemblyBrowserLibrary;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace ApplicationWPF
{
    internal class ApplicationViewModel : INotifyPropertyChanged
    {
        private const string ERROR = "Ошибка при загрузке сборки.";

        private readonly AssemblyBrowser assemblyBrowser;
        private readonly FileDialogService fileDialogService;

        public ApplicationViewModel()
        {
            fileDialogService = new FileDialogService("DLL|*.dll");
            assemblyBrowser = new AssemblyBrowser();
        }

        private List<NodeView> nodes;
        public List<NodeView> Nodes
        {
            get => nodes;
            set
            {
                nodes = value;
                OnPropertyChanged();
            }
        }

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }


        public event PropertyChangedEventHandler PropertyChanged;

        // opening file
        private Command openCommand;

        // opening file
        public Command OpenCommand
        {
            get
            {
                return openCommand ??
                (openCommand = new Command(obj =>
                {
                    if (fileDialogService.OpenFileDialog())
                    {
                        try
                        {
                            assemblyBrowser.LoadAssemblyFromFile(fileDialogService.FilePath);
                            Node rootNode = assemblyBrowser.GetTree();
                            Nodes = RecursiveConvertNodeToViewNode(rootNode).Nodes;
                        }
                        catch (LoadException e)
                        {
                            fileDialogService.ShowErrorMessage(e.Message, ERROR);
                        }
                    }
                }
                ));
            }
        }

        // conversion from model to view
        private NodeView RecursiveConvertNodeToViewNode(Node node)
        {
            NodeView assemblyNodeView = new NodeView();
            assemblyNodeView.TextRepresentation = node.TextRepresentation;
            if (node.GetNodes() != null)
            {
                assemblyNodeView.Nodes = new List<NodeView>();
                foreach (Node nestedNode in node.GetNodes())
                {
                    assemblyNodeView.Nodes.Add(RecursiveConvertNodeToViewNode(nestedNode));
                }
            }
            return assemblyNodeView;
        }

        // closing window
        private Command closeWindowCommand;

        // closing window
        public Command CloseWindowCommand
        {
            get
            {
                return closeWindowCommand ??
                (closeWindowCommand = new Command(obj =>
                {
                    (obj as Window).Close();
                }
                ));
            }
        }

        // about program
        private Command programCommand;

        // about program
        public Command ProgramCommand
        {
            get
            {
                return programCommand ??
                (programCommand = new Command(obj =>
                {
                    MessageBox.Show("Лабораторная работа №3 по предмету СПП.", "О програме", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                ));
            }
        }

        // about developer
        private Command developerCommand;

        // about developer
        public Command DeveloperCommand
        {
            get
            {
                return developerCommand ??
                (developerCommand = new Command(obj =>
                {
                    MessageBox.Show("Программа была разработана Ильиной Александрой, студенткой группы 851001.", 
                        "О разработчике", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                ));
            }
        }
    }
}
