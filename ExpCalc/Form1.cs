using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace ExpCalc
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                var proc = new Calculate(textBox1.Text);                                
                label10.Text = proc.polish;
                label7.Text = proc.result;
                textBox1.Focus();
            }
            else
            {
                MessageBox.Show("Введите выражение", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Focus();
            label7.Text = "";
            label10.Text = "";
        }

        private void файлToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void helpMenu_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Программа 'Калькулятор выражений'\nПринцип работы: полученное выражение приводится к обратной Польской записи, после чего вычисляется значение по алгоритму Дейкстры. \n\nВыполнил Кириловский Дмитрий\nДля Лаборатории Касперского", "Справка", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F1)
            {
                helpMenu.PerformClick();
                return true;
            }
            if (keyData == Keys.Enter)
            {
                button1.PerformClick();
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void exitMenu_Click(object sender, EventArgs e)
        {
            var confirmation = MessageBox.Show("Выйти?", "Завершение работы программы", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirmation == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }
    }

    public class Calculate
    {
        public string expression;
        public string polish;
        public List<string> polishRecord = new List<string>();
        public string result;

        public Calculate() { }

        public void preChange()
        {
            expression = expression.Replace("w", "");
            expression = expression.Replace("x", "");
            expression = expression.Replace("y", "");
            expression = expression.Replace("z", "");
            expression = expression.Replace("cos", "w");
            expression = expression.Replace("sin", "x");
            expression = expression.Replace("ctg", "y");
            expression = expression.Replace("tg", "z");
            int k = expression.IndexOf("-");
            int l = -1;
            int cl;
            while (k > l)
            {
                if (k == 0 || expression.Substring(k - 1, 1) == "(")
                {
                    expression = expression.Substring(0, k) + "(0" + expression.Substring(k);
                    k += 3;
                    if (expression[k] == '(')
                    {
                        cl = k + minusExp(expression.Substring(k + 1)) + 1;
                    }
                    else if (Regex.IsMatch(expression[k].ToString(), "[w-z]"))
                    {
                        cl = k + minusExp(expression.Substring(k + 2)) + 2;
                    }
                    else
                    {
                        cl = k + minusNum(expression.Substring(k));
                    }
                    expression = expression.Substring(0, cl) + ")" + expression.Substring(cl);
                }
                l = k;
                k += expression.Substring(k+1).IndexOf("-") + 1;
            }
        }

        public int minusExp(string str)
        {
            int open = 0, close = 0, i = 0;
            while (close != open + 1 && i < str.Length)
            {
                if (str[i] == '(')
                    open += 1;
                else if (str[i] == ')')
                    close += 1;
                i += 1;
            }
            return i;
        }

        public int minusNum(string str)
        {
            Regex num = new Regex("[0-9\\.]");
            int i = 0;
            while (i < str.Length && num.IsMatch(str[i].ToString()))
            {
                i += 1;
            }
            return i;
        }

        public Calculate(string data)
        {
            expression = data;
            polish = "";
            result = "";
            preChange();
            if (exprValid())
            {
                preTreatment();
                calculation();                
            }
        }

        public void stackToString()
        {
            int i = 0;
            string temp = "";
            while (i < polishRecord.Count())
            {
                temp += polishRecord.ElementAt(i)+":";
                i += 1;
            }
            temp = temp.Replace("w", "cos");
            temp = temp.Replace("x", "sin");
            temp = temp.Replace("y", "ctg");
            temp = temp.Replace("z", "tg");
            polish = temp.Substring(0, temp.Length - 1);
        }

        public void calculation()
        {
            Stack<string> valStack = new Stack<string>();
            if (polishRecord.Count() > 0)
            {
                stackToString();
                string first, temp;
                Regex num = new Regex("[\\.0-9]");
                Regex trig = new Regex("[wxyz]");                
                while (polishRecord.Count() > 0)
                {
                    first = polishRecord.First();
                    polishRecord.RemoveAt(0);
                    if (num.IsMatch(first))
                    {
                        valStack.Push(first);
                    }
                    else
                    {
                        if (valStack.Count() > 1 && !(trig.IsMatch(first)))
                        {
                            temp = ariphCalculation(first, valStack.ElementAt(1).Replace(".", ","), valStack.ElementAt(0).Replace(".", ","));
                            valStack.Pop();
                            valStack.Pop();
                            valStack.Push(temp.Replace(",", "."));
                        }
                        else if (valStack.Count() > 0)
                        {
                            if (trig.IsMatch(first))
                            {
                                temp = trigCalculation(first, valStack.First().Replace(".", ","));
                                valStack.Pop();
                                valStack.Push(temp.Replace(",", "."));
                            }
                            else
                            {
                                MessageBox.Show("Неверно записана операция\nВыводимое значение так же неверно", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                break;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Неверно записана операция\nВыводимое значение так же неверно", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        }
                    }
                }
                double res = 0;
                try
                {
                    res = Convert.ToDouble(valStack.Pop().Replace(".", ","));
                }
                catch (InvalidOperationException)
                {
                    MessageBox.Show("Отсутствуют операнды", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); 
                }
                if (valStack.Count() > 0)
                    MessageBox.Show("Неверно записана операция\nВыводимое значение так же неверно", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                result = res.ToString("F2").Replace(",", ".");
            }
            else
            {
                MessageBox.Show("Неверно записана операция\nНечего вычислять", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public string ariphCalculation(string operation, string operand1, string operand2)
        {
            double num1 = Convert.ToDouble(operand1);
            double num2 = Convert.ToDouble(operand2);
            double res = 0;
            switch (operation)
            {
                case "+":
                    res = num1 + num2;
                    break;
                case "-":
                    res = num1 - num2;
                    break;
                case "*":
                    res = num1 * num2;
                    break;
                case "/":
                    res = num1 / num2;
                    break;
                case "^":
                    res = Math.Pow(num1, num2);
                    break;
                default:
                    res = 0;
                    MessageBox.Show("Неверная операция", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
            return res.ToString();            
        }

        public string trigCalculation(string operation, string operand)
        {
            double num = Convert.ToDouble(operand);
            double res = 0;
            switch (operation)
            {
                case "w":
                    res = Math.Cos(num);
                    break;
                case "x":
                    res = Math.Sin(num);
                    break;
                case "y":
                    res = 1 / Math.Tan(num);
                    break;
                case "z":
                    res = Math.Tan(num);
                    break;
            }
            return res.ToString();
        }

        public void preTreatment()
        {
            Stack<string> operStack = new Stack<string>();      // стэк операторов
            string temp = expression, sub = "";            
            Regex oper = new Regex("[^0-9\\.]");
            int n = 1, len;
            len = expression.Length;
            while (n <= len)                                     // пока не просмотрена вся строка
            {
                sub = temp.Substring(n - 1, 1);
                while (!(oper.IsMatch(sub)) && n < len && !(oper.IsMatch(temp.Substring(n, 1))))
                {
                    n += 1;
                    sub += temp.Substring(n - 1, 1);
                }
                n += 1;
                addToOutput(operStack, sub, oper);
            }
            while (operStack.Count() > 0)
            {
                polishRecord.Add(operStack.First());
                operStack.Pop();
            }
        }

        public void addToOutput(Stack<string> operStack, string sub, Regex oper)
        {
            int curOperPriority, lastOperPriority;
            string lastOper = "";
                                                                // регулярки для проверки: число или операция
            Regex num = new Regex("[\\.0-9]");
                                                                // регулярки для назначения приоритета операции
            Regex high = new Regex("[\\^]");
            Regex middle = new Regex("[/\\*wxyz]");            
            Regex bracket = new Regex("[()]");
            if (num.IsMatch(sub))                               // число или операция
            {
                polishRecord.Add(sub);
            }
            else                                                // если операция, то 
            {                                                   // если закрывающая скобка - вытаскиваем из стека все до открывающей скобки
                if (sub == ")")
                {
                    lastOper = operStack.First();
                    while (lastOper != "(")
                    {
                        polishRecord.Add(lastOper);
                        operStack.Pop();
                        lastOper = operStack.First();
                    }
                    operStack.Pop();
                    if (operStack.Count() > 0)
                    {
                        Regex trig = new Regex("[wxyz]");
                        if (trig.IsMatch(operStack.First()))
                        {
                            polishRecord.Add(operStack.First());
                            operStack.Pop();
                        }
                    }
                }
                else if (sub == "(")
                {
                    operStack.Push(sub);
                }
                else
                {                                               // выяснить приоритет текущей операции
                    curOperPriority = 0;
                    lastOperPriority = 0;
                    if (high.IsMatch(sub))
                        curOperPriority = 4;
                    else if (middle.IsMatch(sub))
                        curOperPriority = 3;
                    else curOperPriority = 2;
                    if (operStack.Count > 0)
                    {                                           // выяснить приоритет верхней операции в стеке (если есть)
                        lastOper = operStack.First();
                        if (high.IsMatch(lastOper))
                            lastOperPriority = 4;
                        else if (middle.IsMatch(lastOper))
                            lastOperPriority = 3;
                        else if (bracket.IsMatch(lastOper))
                            lastOperPriority = 1;
                        else lastOperPriority = 2;
                    }
                    while (lastOperPriority > curOperPriority)  // если приоритет операции в стеке выше, то все верхние операции в стеке с большим приоритетом записываем в выходную строку, удаляя их из стека
                    {
                        polishRecord.Add(lastOper);
                        operStack.Pop();
                        if (operStack.Count() > 0)
                        {
                            lastOper = operStack.First();
                            if (high.IsMatch(lastOper))
                                lastOperPriority = 4;
                            else if (middle.IsMatch(lastOper))
                                lastOperPriority = 3;
                            else if (bracket.IsMatch(lastOper))
                                lastOperPriority = 1;
                            else lastOperPriority = 2;
                        }
                        else lastOperPriority = 0;
                    }
                    operStack.Push(sub);
                }
            }
        }

        public bool exprValid()
        {
            int n1 = 0, n2 = 0, ln;
            string temp = expression;
            Regex forbidden = new Regex("[^0-9w-z\\(\\)\\+\\-\\*\\/\\^\\.]");
            if (forbidden.IsMatch(temp))
            {
                MessageBox.Show("Введенное выражение содержит недопустимые символы", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            ln = temp.IndexOf("(");
            while (ln >= 0)
            {
                temp = temp.Substring(ln + 1);
                ln = temp.IndexOf("(");
                n1 += 1;
            }
            temp = expression;
            ln = temp.IndexOf(")");
            while (ln >= 0)
            {
                temp = temp.Substring(ln + 1);
                ln = temp.IndexOf(")");
                n2 += 1;
            }
            if (n1 > n2)
            {
                MessageBox.Show("Лишняя(ие) открывающая скобка", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            else if ( n1 < n2 )
            {
                MessageBox.Show("Лишняя(ие) закрывающая скобка", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            temp = expression;
            int k = temp.IndexOf("w");
            while (k >= 0)
            {
                if (temp[k+1] != '(')
                {
                    MessageBox.Show("Выражение, над которым будет выполняется тригонометрическая функция, должно быть в скобках", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                temp = temp.Substring(k + 1);
                k = temp.IndexOf("w");
            }
            temp = expression;
            k = temp.IndexOf("x");
            while (k >= 0)
            {
                if (temp[k + 1] != '(')
                {
                    MessageBox.Show("Выражение, над которым будет выполняется тригонометрическая функция, должно быть в скобках", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                temp = temp.Substring(k + 1);
                k = temp.IndexOf("x");
            }
            temp = expression;
            k = temp.IndexOf("y");
            while (k >= 0)
            {
                if (temp[k + 1] != '(')
                {
                    MessageBox.Show("Выражение, над которым будет выполняется тригонометрическая функция, должно быть в скобках", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                temp = temp.Substring(k + 1);
                k = temp.IndexOf("y");
            }
            temp = expression;
            k = temp.IndexOf("z");
            while (k >= 0)
            {
                if (temp[k + 1] != '(')
                {
                    MessageBox.Show("Выражение, над которым будет выполняется тригонометрическая функция, должно быть в скобках", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                temp = temp.Substring(k + 1);
                k = temp.IndexOf("z");
            }
            return true;
        }
    }
}
