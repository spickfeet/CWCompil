using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace CWCompil.State
{
    public class StartState : IState
    {
        public void Enter(StateMachine sm)
        {
            if(sm.CurrentTokenIndex >= sm.Tokens.Count)
            {
                ErrorNeutralizer(sm);
                return;
            }
            if (sm.Tokens[sm.CurrentTokenIndex] == "Console")
            {
                sm.CountDel = 0;
                sm.State = new ConsoleState();
            }
            else if (string.IsNullOrWhiteSpace(sm.Tokens[sm.CurrentTokenIndex]))
            {
                return;
            }
            else 
            {
                ErrorNeutralizer(sm);
            }
        }
        private void NeutralizerAddOrChangeError(StateMachine sm, string[] tokens)
        {
            for (int i = 0; i < tokens.Length; i++)
            {
                if (sm.CountDel > tokens.Length)
                {
                    sm.CountDel = tokens.Length;
                }
                if (sm.CountDel != 0)
                {
                    sm.ErrorsData[sm.ErrorsData.Count - sm.CountDel].Text = sm.ErrorsData[sm.ErrorsData.Count - sm.CountDel].Text.Replace("Отбрасывается", $"Заменяется на \"{tokens[i]}\"");
                    sm.CountDel--;
                }
                else
                {
                    sm.ErrorsData.Add(new(sm.Line, sm.GetIndexOfCurrentToken() + (sm.CurrentTokenIndex < sm.Tokens.Count ? sm.Offsets[sm.CurrentTokenIndex] : 0), $" Ожидается \"{tokens[i]}\" перед \"{sm.Tokens[sm.CurrentTokenIndex]}\""));
                }
            }
        }
        private void ErrorNeutralizer(StateMachine sm)
        {
            if (sm.CurrentTokenIndex >= sm.Tokens.Count)
            {
                sm.IsStopped = true;
                return;
            }
            if (sm.Tokens[sm.CurrentTokenIndex] == ".")
            {
                string[] tokens = { "Console" };
                NeutralizerAddOrChangeError(sm, tokens);
                sm.State = new ConsoleState();
                sm.State.Enter(sm);
                return;
            }
            if (sm.Tokens[sm.CurrentTokenIndex] == "ReadLine")
            {
                string[] tokens = { "Console", "." };
                NeutralizerAddOrChangeError(sm, tokens);
                sm.State = new PointState();
                sm.State.Enter(sm);
                return;
            }
            sm.ErrorsData.Add(new(sm.Line, sm.GetIndexOfCurrentToken() + (sm.CurrentTokenIndex < sm.Tokens.Count ? sm.Offsets[sm.CurrentTokenIndex] : 0), $"\"{sm.Tokens[sm.CurrentTokenIndex]}\" не является ожидаемым. (Отбрасывается)"));
            sm.CountDel++;
        }
    }
}
