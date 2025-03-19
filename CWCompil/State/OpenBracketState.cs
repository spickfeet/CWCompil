﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCompil.State
{
    class OpenBracketState : IState
    {
        public void Enter(StateMachine sm)
        {
            if (sm.CurrentTokenIndex >= sm.Tokens.Count)
            {
                sm.ErrorsText += $"Строка: {sm.Line}. Ошибка: Не хватает \")\"!\n";
                sm.State = new CloseBracketState();
                return;
            }
            if (sm.Tokens[sm.CurrentTokenIndex] == ")")
            {
                sm.State = new CloseBracketState();
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

        private void ErrorNeutralizer(StateMachine sm)
        {
            if (sm.Tokens[sm.CurrentTokenIndex] == "Console")
            {
                sm.ErrorsText += $"Строка: {sm.Line}. Ошибка: Не хватает \")\"!\n" +
                    $"Строка: {sm.Line}. Ошибка: Не хватает \";\"!\n";
                sm.State = new ConsoleState();
            }
            else
            {
                sm.ErrorsText += $"Строка: {sm.Line}. Ошибка: \"{sm.Tokens[sm.CurrentTokenIndex]}\" не является ожидаемым. Ожидаемый терминал \"ReadLine\"!\n";
                sm.State = new CloseBracketState();
            }
        }
    }
}
