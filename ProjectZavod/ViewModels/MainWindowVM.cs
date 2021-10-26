using netDxf;
using netDxf.Entities;
using netDxf.Header;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace ProjectZavod.ViewModels
{
    public class MainWindowVM//: INotifyPropertyChanged
    {
        public string[] Files;
        public class CommandHandler : ICommand
        {
            private Action _action;
            private Func<bool> _canExecute;

            /// <summary>
            /// Creates instance of the command handler
            /// </summary>
            /// <param name="action">Action to be executed by the command</param>
            /// <param name="canExecute">A bolean property to containing current permissions to execute the command</param>
            public CommandHandler(Action action, Func<bool> canExecute)
            {
                _action = action;
                _canExecute = canExecute;
            }

            /// <summary>
            /// Wires CanExecuteChanged event 
            /// </summary>
            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            /// <summary>
            /// Forcess checking if execute is allowed
            /// </summary>
            /// <param name="parameter"></param>
            /// <returns></returns>
            public bool CanExecute(object parameter)
            {
                return _canExecute.Invoke();
            }

            public void Execute(object parameter)
            {
                _action();
            }
        }

        public MainWindowVM()
        {

        }

        //public event PropertyChangedEventHandler PropertyChanged;


        private ICommand _startProgrammButton;
        public ICommand StartProgrammButton
        {
            get
            {
                return _startProgrammButton ?? (_startProgrammButton = new CommandHandler(() => MakeCut(), () => CanExecute));
            }
        }

        private ICommand _browseFolderButton;
        public ICommand BrowseFolderButton
        {
            get
            {
                return _browseFolderButton ?? (_browseFolderButton = new CommandHandler(() => BrowseFolder(), () => CanExecute));
            }
        }


        public bool CanExecute
        {
            get
            {
                // check if executing is allowed, i.e., validate, check if a process is running, etc. 
                return true;
            }
        }

        public void MakeCut()
        {
            if (Files == null)
            {
                throw new Exception("не указан путь к папке");
            }

            for(int i=0; i< Files.Length; i++)
            {
                string file = string.Format("createdFile, {0}, .dxf", i);
                DxfDocument ourFile = DxfDocument.Load(Files[i]);
                if (ourFile == null)
                {
                    throw new Exception(Files[i] + " File is not loaded, incorrect format");
                }

                Line entity = new Line(new Vector2(5, 5), new Vector2(10, 5));
                ourFile.AddEntity(entity);
                ourFile.Save(file);

                DxfVersion dxfVersion = DxfDocument.CheckDxfFileVersion(file, out _);
                if (dxfVersion < DxfVersion.AutoCad2000)
                    throw new Exception("you are using an old AutoCad Version");
            }
        }

        public void BrowseFolder()
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            DialogResult dialogResult = folderBrowser.ShowDialog();
            if (!string.IsNullOrWhiteSpace(folderBrowser.SelectedPath))
            {
                Files = Directory.GetFiles(folderBrowser.SelectedPath);
            }
        }
    }
}
