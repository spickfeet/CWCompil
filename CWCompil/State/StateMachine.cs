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
        public string ErrorsText { get; set; }
        public int Line {  get; set; }
        public void Start(string text)
        {
            IsStopped = false;
            State = new StartState();
            Tokens = new List<string>();
            ErrorsText = "";
            Line = 1;
            string pattern = @"[^\s\.();]+|\.|\(|\)|;|\s";
            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches(text);
            foreach (Match match in matches)
            {
                Tokens.Add(match.Value);
            }
            for (; IsStopped == false; CurrentTokenIndex++) 
            {
                if (CurrentTokenIndex < Tokens .Count && Tokens[CurrentTokenIndex] == "\n")
                {
                    Line++;
                }
                State.Enter(this);
            }
        }
    }
}
