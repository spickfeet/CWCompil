using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CWCompil.State
{
    public class StateMachine
    {
        public bool IsStopped {  get; set; }
        public IState State { get; set; }
        public List<string> Tokens { get; set; }
        public int CurrentTokenIndex { get; set; }
        public List<ErrorData> ErrorsData {  get; set; }
        public int CountDel { get; set; }
        public int Line {  get; set; }
        public List<int> Offsets { get; set; }
        private Dictionary<int, int> _offsetsByIndexDict;
        public StateMachine()
        {
            _offsetsByIndexDict = new();
            Offsets = new();
            ErrorsData = new();
            Tokens = new List<string>();
            State = new StartState();
        }
        public int GetIndexOfCurrentToken()
        {
            int index = 0;
            for (int i = 0; i < CurrentTokenIndex; i++) 
            {
                index += Tokens[i].Length;
            }
            return index;
        } 

        public string DelNonValidSymbols(string text)
        {
            _offsetsByIndexDict = new Dictionary<int, int>();
            int line = 1;
            int offset = 0;
            string nonValidSymbols = "!@#$%^&*\"№:?";
            StringBuilder nonValidText = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                if (nonValidSymbols.Contains(text[i]))
                {
                    offset++;
                    if (i + 1 != text.Length && nonValidSymbols.Contains(text[i + 1]))
                    {
                        nonValidText.Append(text[i]);
                    }
                    else
                    {
                        nonValidText.Append(text[i]);
                        ErrorsData.Add(new(line, i + 1 - nonValidText.Length, $"\"{nonValidText}\" не является ожидаемым. (Отбрасывается)"));

                        _offsetsByIndexDict.Add(i - offset, offset);
                        nonValidText.Clear();
                    }
                    if(text[i] == '\n')
                    {
                        line++;
                    }
                }
            }
            return Regex.Replace(text, $"[{nonValidSymbols}]", "");
        }



        public void Start(string text)
        {
            CountDel = 0;
            CurrentTokenIndex = 0;
            Tokens.Clear();
            IsStopped = false;
            ErrorsData.Clear();
            Line = 1;
            //-----------------------------Удаление некорректных символов-----------------------------
            //text = DelNonValidSymbols(text);
            //----------------------------------------------------------------------------------------

            //string pattern = @"Console|ReadLine|((?!(ReadLine|Console))[^\s\.();])+|\.|\(|\)|;|\s";
            string pattern = @"Console|ReadLine|((?!(ReadLine|Console))[^\.();])+|\.|\(|\)|;";
            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches(text);
            
            for (int i = 0; i < matches.Count; i++)
            {
                Tokens.Add(matches[i].Value);

                //-----------------------------Удаление некорректных символов-----------------------------

                //CurrentTokenIndex++;
                Offsets.Add(0);
                //foreach (int kay in _offsetsByIndexDict.Keys) 
                //{
                //    if (GetIndexOfCurrentToken() < kay)
                //    {
                //        break;
                //    }
                //    else
                //    {
                //        Offsets[i] = _offsetsByIndexDict[kay];
                //    }
                //}

                //--------------------------------------------------------------------------------------------------------------------
            }
            Line = 1;
            CurrentTokenIndex = 0;
            for (; IsStopped == false; CurrentTokenIndex++) 
            {
                State.Enter(this);
                if (CurrentTokenIndex < Tokens.Count && Tokens[CurrentTokenIndex].Contains("\n"))
                {
                    Line += Tokens[CurrentTokenIndex].Count(x => x == '\n');
                }
            }
        }
    }
}
