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
                ErrorNeutralizer(sm);
                return;
            }
            if (sm.Tokens[sm.CurrentTokenIndex] == "Console")
            {
                sm.CountDel = 0;
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
                    sm.ErrorsData.Add(new(sm.Line, sm.GetIndexOfCurrentToken(), $" Ожидается \"{tokens[i]}\" перед \"{sm.Tokens[sm.CurrentTokenIndex]}"));
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
            //if (sm.Tokens[sm.CurrentTokenIndex] == "(")
            //{
            //    string[] tokens = { "Console", ".", "ReadLine" };
            //    for (int i = 0; i < tokens.Length; i++)
            //    {
            //        if (sm.CountDel > 3)
            //        {
            //            sm.CountDel = 3;
            //        }
            //        if (sm.CountDel != 0)
            //        {
            //            sm.ErrorsData[sm.ErrorsData.Count - sm.CountDel].Text = sm.ErrorsData[sm.ErrorsData.Count - sm.CountDel].Text.Replace("Отбрасывается", $"Заменяется на \"{tokens[i]}\"");
            //            sm.CountDel--;
            //        }
            //        else
            //        {
            //            sm.ErrorsData.Add(new(sm.Line, sm.GetIndexOfCurrentToken(), $" Ожидается \"{tokens[i]}\" перед \"{sm.Tokens[sm.CurrentTokenIndex]}"));
            //        }
            //    }
            //    sm.State = new ReadLineState();
            //    sm.State.Enter(sm);
            //    return;
            //}
            //if (sm.Tokens[sm.CurrentTokenIndex] == ")")
            //{
            //    string[] tokens = { "Console", ".", "ReadLine", "(" };
            //    for (int i = 0; i < tokens.Length; i++)
            //    {
            //        if (sm.CountDel > 4)
            //        {
            //            sm.CountDel = 4;
            //        }
            //        if (sm.CountDel != 0)
            //        {
            //            sm.ErrorsData[sm.ErrorsData.Count - sm.CountDel].Text = sm.ErrorsData[sm.ErrorsData.Count - sm.CountDel].Text.Replace("Отбрасывается", $"Заменяется на \"{tokens[i]}\"");
            //            sm.CountDel--;
            //        }
            //        else
            //        {
            //            sm.ErrorsData.Add(new(sm.Line, sm.GetIndexOfCurrentToken(), $" Ожидается \"{tokens[i]}\" перед \"{sm.Tokens[sm.CurrentTokenIndex]}"));
            //        }
            //    }
            //    sm
            //    sm.State = new OpenBracketState();
            //    sm.State.Enter(sm);
            //    sm.ErrorsText += $"Строка: {sm.Line}. Ошибка: Ожидается \"Console\" перед \"{sm.Tokens[sm.CurrentTokenIndex]}\"\n" +
            //        $"Строка: {sm.Line}. Ошибка: Ожидается \".\" перед \"{sm.Tokens[sm.CurrentTokenIndex]}\"\n" +
            //        $"Строка: {sm.Line}. Ошибка: Ожидается \"ReadLine\" перед \"{sm.Tokens[sm.CurrentTokenIndex]}\"\n" +
            //        $"Строка: {sm.Line}. Ошибка: Ожидается \"(\" перед \"{sm.Tokens[sm.CurrentTokenIndex]}\"\n";
            //    return;
            //}
            //if (sm.Tokens[sm.CurrentTokenIndex] == ";")
            //{
            //    sm.State = new CloseBracketState();
            //    sm.State.Enter(sm);
            //    sm.ErrorsText += $"Строка: {sm.Line}. Ошибка: Ожидается \"Console\" перед \"{sm.Tokens[sm.CurrentTokenIndex]}\"\n" +
            //        $"Строка: {sm.Line}. Ошибка: Ожидается \".\" перед \"{sm.Tokens[sm.CurrentTokenIndex]}\"\n" +
            //        $"Строка: {sm.Line}. Ошибка: Ожидается \"ReadLine\" перед \"{sm.Tokens[sm.CurrentTokenIndex]}\"\n" +
            //        $"Строка: {sm.Line}. Ошибка: Ожидается \"(\" перед \"{sm.Tokens[sm.CurrentTokenIndex]}\"\n" +
            //        $"Строка: {sm.Line}. Ошибка: Ожидается \")\" перед \"{sm.Tokens[sm.CurrentTokenIndex]}\"\n";
            //    return;
            //}
            sm.ErrorsData.Add(new(sm.Line, sm.GetIndexOfCurrentToken(), $"\"{sm.Tokens[sm.CurrentTokenIndex]}\" не является ожидаемым. (Отбрасывается)"));
            sm.CountDel++;
        }
    }
}
