﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCompil.State
{
    class PointState : IState
    {
        public void Enter(StateMachine sm)
        {
            if (sm.CurrentTokenIndex >= sm.Tokens.Count)
            {
                ErrorNeutralizer(sm);
                return;
            }
            if (sm.Tokens[sm.CurrentTokenIndex] == "ReadLine")
            {
                sm.CountDel = 0;
                sm.State = new ReadLineState();
            }
            else if (sm.Tokens[sm.CurrentTokenIndex] == "\n" || sm.Tokens[sm.CurrentTokenIndex] == "\t" ||
                sm.Tokens[sm.CurrentTokenIndex] == "\r" || sm.Tokens[sm.CurrentTokenIndex] == " ")
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
                    sm.ErrorsData.Add(new(sm.Line, sm.GetIndexOfCurrentToken(), sm.CurrentTokenIndex < sm.Tokens.Count ? $"Ожидается \"{tokens[i]}\" перед \"{sm.Tokens[sm.CurrentTokenIndex]}\"" :
                         $"В конце ожидается \"{tokens[i]}\""));
                }
            }
        }
        private void ErrorNeutralizer(StateMachine sm)
        {
            if (sm.CurrentTokenIndex >= sm.Tokens.Count)
            {
                string[] tokens = { "ReadLine", "(", ")", ";" };
                NeutralizerAddOrChangeError(sm, tokens);
                sm.State = new StartState();
                return;
            }
            if (sm.Tokens[sm.CurrentTokenIndex] == "Console")
            {
                string[] tokens = { "ReadLine", "(", ")", ";" };
                NeutralizerAddOrChangeError(sm, tokens);
                sm.State = new StartState();
                sm.State.Enter(sm);
                return;
            }
            if (sm.Tokens[sm.CurrentTokenIndex] == "(")
            {
                string[] tokens = { "ReadLine" };
                NeutralizerAddOrChangeError(sm, tokens);
                sm.State = new ReadLineState();
                sm.State.Enter(sm);
                return;
            }
            if (sm.Tokens[sm.CurrentTokenIndex] == ")")
            {
                string[] tokens = { "ReadLine", "(" };
                NeutralizerAddOrChangeError(sm, tokens);
                sm.State = new OpenBracketState();
                sm.State.Enter(sm);
                return;
            }
            if (sm.Tokens[sm.CurrentTokenIndex] == ";")
            {
                string[] tokens = { "ReadLine", "(", ")" };
                NeutralizerAddOrChangeError(sm, tokens);
                sm.State = new CloseBracketState();
                sm.State.Enter(sm);
                return;
            }
            sm.ErrorsData.Add(new(sm.Line, sm.GetIndexOfCurrentToken(), $"\"{sm.Tokens[sm.CurrentTokenIndex]}\" не является ожидаемым. (Отбрасывается)"));
            sm.CountDel++;
        }
    }
}
