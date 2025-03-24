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
        public StateMachine()
        {
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
        public void Start(string text)
        {
            CountDel = 0;
            CurrentTokenIndex = 0;
            Tokens.Clear();
            IsStopped = false;
            ErrorsData.Clear();
            Line = 1;

            //string pattern = @"Console|ReadLine|((?!(ReadLine|Console))[^\s\.();])+|\.|\(|\)|;|\s";
            string pattern = @"Console|ReadLine|((?!(ReadLine|Console))[^\.();])+|\.|\(|\)|;";
            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches(text);

            for (int i = 0; i < matches.Count; i++)
            {
                Tokens.Add(matches[i].Value);
            }
            for (; IsStopped == false; CurrentTokenIndex++) 
            {
                State.Enter(this);
                if (CurrentTokenIndex < Tokens .Count && Tokens[CurrentTokenIndex] == "\n")
                {
                    Line++;
                }
            }
        }
    }
}
