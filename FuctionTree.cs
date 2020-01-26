using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    class FunctionTree
    {
        top Tree;
        class top//класс вершины дерева
        {
            public string value;//значение дерева (либо знак, либо переменная, либо константа)
            public top p;//родитель вершины
            public top l;//левый предок вершины
            public top r;//правый редок вершины
        }
        public FunctionTree(string Function)//конструктор дерева
        {
            Tree = new top();//создаем корень дерева
            Tree.p = null;
            CreateFuncTree(Tree, Function);//запускаем создание дерева
        }
        void CreateFuncTree(top CurTop, string Function)//функция создания дерева
        {
            if (InParentheses(Function))//проверяем нужно ли опустить скобки
            {
                CreateFuncTree(CurTop, Function.Substring(1, Function.Length - 2));//вызываем ту же функцию для того же выражения без скобок
                return;
            }
            if (SearchOperations(CurTop, Function, '+', '-'))
                return;
            if (SearchOperations(CurTop, Function, '*', '/'))
                return;
            if (IsTrigFunction(CurTop, Function, "sin"))
                return;
            if (IsTrigFunction(CurTop, Function, "cos"))
                return;
            if (IsTrigFunction(CurTop, Function, "tg"))
                return;
            int Const = 0;
            if (Function.Length>2 && (Function.Substring(0,2)=="x^" || Function.Substring(0, 2) == "y^") && 
                int.TryParse(Function.Substring(2), out Const) && Const>0) //если слева переменная, справа положительная целая константа
            {
                CurTop.value = "^";
                CurTop.l = new top() { p = CurTop };
                CreateFuncTree(CurTop.l, Function.Substring(0,1));
                CurTop.r = new top() { p = CurTop };
                CreateFuncTree(CurTop.r, Function.Substring(2));
                return;
            }
            double d = 0;
            if (Function=="x" || Function=="y" || Double.TryParse(Function, out d))//если выражение - это только переменная
            {
                CurTop.value = Function;//кладем в значение текущей вершины переменную
                return;
            }
            throw new Exception();//иначе функция была некорректная
        }
        bool InParentheses(string Function)//функция проверяет - в скобках ли выражение
        {
            if (Function.Length < 3 || Function[0]!='(' || Function[Function.Length-1]!=')')
                return false;//если выражение длиной меньше трех и нет скобок по бокам - выражение не в скобках
            int Check =0;
            for(int i=1; i<Function.Length-1; i++)//идем сос второго знака, заканчиваем на предпоследнем
            {
                if(Function[i]=='(')
                    Check++;
                else if (Function[i] == ')')
                {
                    Check--;
                    if (Check < 0)//если для закрывающей скобки не нашлась открывающая => открывающая была в начале и мы ее пропустили
                        break;
                }
            }
            if (Check == 0)//если мы прошли весь цикл
                return true;
            else
                return false;
        }
        private bool SearchOperations(top CurTop, string Function, char FirstOp, char SecondOp)
        {
            int check = 0;
            for (int i = Function.Length - 1; i >= 0; i--)//идем с конца
            {
                if (Function[i] == '(')
                    check++;
                else if (Function[i] == ')')
                    check--;
                else if ((Function[i] == FirstOp || Function[i] == SecondOp) && check == 0)//если нет незакрытых открывающихся скобок и найден знак
                {
                    CurTop.value = Function[i].ToString();//в значение текущей вершины кладем этот знак
                    CurTop.l = new top() { p = CurTop };//создаем левое поддерево для этого знака
                    string LeftOperand;
                    if (Function[i] == '-' && Function.Substring(0, i) == "")//обрабатываем ситуацию унарного минуса
                        LeftOperand = "0";//-х=0-х
                    else
                        LeftOperand = Function.Substring(0, i);//иначе берем подстроку строки функцию слева от знака и создаем эту функцию в левом поддереве
                    CreateFuncTree(CurTop.l, LeftOperand);
                    CurTop.r = new top() { p = CurTop };//аналагично для правого поддерева
                    CreateFuncTree(CurTop.r, Function.Substring(i + 1));
                    if(Function[i] == '/' && int.TryParse(CurTop.r.value, out int a) && a==0)//обрабатываем деление на 0
                        throw new Exception();
                    return true;
                }
            }
            return false; 
        }
        private bool IsTrigFunction(top CurTop, string Function, string TrigFunction)
        {

            if (Function.Length>= TrigFunction.Length + 3 && Function.Substring(0, TrigFunction.Length+1) == $"{TrigFunction}(" && Function[Function.Length - 1] == ')')//если выражение представляет из себя триг.функ(...)
            {
                CurTop.value = TrigFunction;//в значение текущей вершины кладем знак триг функции
                CurTop.l = new top() { p = CurTop };//значение внутри скобок строим в левом поодере текущей вершины
                CreateFuncTree(CurTop.l, Function.Substring(TrigFunction.Length+1, Function.Length- TrigFunction.Length-2));
                return true;
            }
            return false;

        }
        public double GetValue(double x, double y)//функция нужно, чтобы вызывать ее изве, иначе нельзя осуществить рекуосию
        {
            return GetValuePr(x, y, Tree);
        }
        double GetValuePr(double x, double y, top CurTop)
        {
            if (CurTop.value == "+")//если найден знак операции рекурсивно вычисляем значение функции в левом и правом поддеревьях текущей вершины, к полученным числам применяем знак операции
                return GetValuePr(x, y, CurTop.l) + GetValuePr(x, y, CurTop.r);
            if (CurTop.value == "-")
                return GetValuePr(x, y, CurTop.l) - GetValuePr(x, y, CurTop.r);
            if (CurTop.value == "*")
                return GetValuePr(x, y, CurTop.l) * GetValuePr(x, y, CurTop.r);
            if (CurTop.value == "/")
            {
                double denominator = GetValuePr(x, y, CurTop.r);//для деления отдельно считаем знаменатель
                if (denominator == 0)//превращаем ноль в маленькое число
                    denominator = 1e-6;
                return GetValuePr(x, y, CurTop.l) / denominator;
            }
            if (CurTop.value == "^")
            {
                int degree = int.Parse(CurTop.r.value);
                double Var;
                if (CurTop.l.value == "x")
                    Var = x;
                else
                    Var = y;
                double res = 1;
                while(degree--!=0)
                {
                    res*= Var;
                }
                return res;
            }
            if (CurTop.value == "sin")
                return Math.Sin(GetValuePr(x, y, CurTop.l));
            if (CurTop.value == "cos")
                return Math.Cos(GetValuePr(x, y, CurTop.l));
            if (CurTop.value == "tg")
                return Math.Tan(GetValuePr(x, y, CurTop.l));
            if (CurTop.value == "x")//если переменная - возвращаем значение выбранное значение переемнной
                return x;
            if (CurTop.value == "y")
                return y;
            return Double.Parse(CurTop.value);//иначе в вершине лежит число - возвращаем число
        }
    }
}
