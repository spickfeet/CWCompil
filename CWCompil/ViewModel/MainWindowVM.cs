using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CWCompil.Files;
using CWCompil.State;

namespace CWCompil.ViewModel
{
    public class MainWindowVM : BasicVM
    {
        private FileWorker _fileWorker;
        private string _textInput;
        private string _debugText;

        private bool _fileSaved;
        public string TextInput
        {
            get { return _textInput; }
            set 
            {
                Set(ref _textInput, value);
                _fileSaved = false;
            }
        }
        
        public string DebugText
        {
            get { return _debugText; }
            set
            {
                Set(ref _debugText, value);
            }
        }

        public Action<string> PathChanged;
        public MainWindowVM()
        {
            _fileSaved = true;
            _fileWorker = new();
            PathChanged += _fileWorker.OnPathChanged;
        }
        private bool TryChoosePath()
        {
            SaveFileDialog fileDialog = new SaveFileDialog()
            {
                AddExtension = true,
                Filter = "Text Files(*.txt)|*.txt|All(*.*)|*"
            };

            if (fileDialog.ShowDialog() == true)
            {
                string pathName = fileDialog.FileName;
                PathChanged?.Invoke(pathName);

                if (!File.Exists(pathName))
                    File.Create(pathName).Close();

                return true;
            }
            else { return false; }
        }

        private bool TryOpen()
        {
            OpenFileDialog fileDialog = new OpenFileDialog()
            {
                AddExtension = true,
                Filter = "Text Files(*.txt)|*.txt|All(*.*)|*"
            };

            if (fileDialog.ShowDialog() == true)
            {
                string pathName = fileDialog.FileName;
                PathChanged?.Invoke(pathName);

                return true;
            }
            else { return false; }
        }

        public void PromptSave() // Изменить название
        {
            while (_fileSaved == false)
            {
                MessageBoxResult result = MessageBox.Show(
                "У вас есть несохраненные изменения. Хотите сохранить файл перед выходом?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    Save();
                }
                else if (result == MessageBoxResult.No)
                {
                    return;
                }
            }
        }

        private void Open()
        {
            PromptSave();
            if (TryOpen() == false) return;
            TextInput = _fileWorker.GetData();
            _fileSaved = true;

        }

        private void Create()
        {
            PromptSave();
             TextInput = "";
            _fileWorker.OnPathChanged(null);
            _fileSaved = true;

        }

        private void Save()
        {
            if (string.IsNullOrEmpty(_fileWorker.PathName))
                if (TryChoosePath() == false) return;
            _fileWorker.Save(TextInput);
            _fileSaved = true;
        }

        private void SaveAs()
        {
            if (TryChoosePath() == false) return;
            _fileWorker.Save(TextInput);
            _fileSaved = true;
        }

        public ICommand CreateCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    Create();
                });
            }
        }

        public ICommand OpenCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    Open();
                });
            }
        }

        public ICommand SaveCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    Save();
                });
            }
        }

        public ICommand SaveAsCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    SaveAs();
                });
            }
        }
        public ICommand Start
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    StateMachine sm = new();
                    sm.Start(TextInput);
                    DebugText = "";
                    if (sm.ErrorsData.Count == 0)
                    {
                        DebugText = "Ошибок нет";
                    }
                    foreach(ErrorData errorData in sm.ErrorsData)
                    {
                        DebugText += $"Строка: {errorData.Line} Глобальный индекс: {errorData.GlobalIndex} Ошибка {errorData.Text} \n";
                    }
                });
            }
        }
    }
}
