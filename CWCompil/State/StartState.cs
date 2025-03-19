using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWCompil.State
{
    public class StartState : IState
    {
        public void Enter(StateMachine sm)
        {
            if(sm.CurrentTokenIndex >= sm.Tokens.Count)
            {
                sm.IsStopped = true;
                return;
            }
            if (sm.Tokens[sm.CurrentTokenIndex] == "Console")
            {
                sm.State = new ConsoleState();
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
            sm.ErrorsText += $"Строка: {sm.Line}. Ошибка: \"{sm.Tokens[sm.CurrentTokenIndex]}\" не является ожидаемым. Ожидаемый терминал \"Console\"!\n";
            sm.State = new ConsoleState();
        }
    }
}
