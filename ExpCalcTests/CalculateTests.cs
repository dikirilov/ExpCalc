using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExpCalc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ExpCalc.Tests
{
    [TestClass()]
    public class CalculateTests
    {
        public string expression;
        public List<string> polishRecord = new List<string>();
        public double result;

        [TestMethod()]
        public void Calculate_IntegrationTest()
        {
            var proc = new Calculate("25*5");
            Assert.AreEqual("125.00", proc.result);
            proc = null;

            proc = new Calculate("-5*5");
            Assert.AreEqual("-25.00", proc.result);
            proc = null;

            proc = new Calculate("-2+10");
            Assert.AreEqual("8.00", proc.result);
            proc = null;

            proc = new Calculate("3+(-2+5)*3");
            Assert.AreEqual("12.00", proc.result);

        }

        [TestMethod()]
        public void stackToString()
        {
            var proc = new Calculate();
            proc.polishRecord.Add("+");
            proc.polishRecord.Add("-");
            proc.polishRecord.Add("24");
            proc.polishRecord.Add("w");
            proc.polishRecord.Add("z");
            proc.stackToString();
            Assert.AreEqual("+:-:24:cos:tg", proc.polish);
        }

        [TestMethod()]
        public void calculation_aClac_tCalc_stackToS_IntegrationTest()
        {
            var proc = new Calculate();
            proc.polishRecord.Add("3.92");
            proc.polishRecord.Add("3.08");
            proc.polishRecord.Add("+");
            proc.calculation();
            Assert.AreEqual("7.00", proc.result);
            Assert.AreEqual("3.92:3.08:+", proc.polish);
        }

        [TestMethod()]
        public void trigCalculation_Test()
        {
            string operation, operand, res;
            var proc = new Calculate();
            // косинус
            operation = "w";
            operand = "3,14";
            res = Math.Cos(Convert.ToDouble(operand)).ToString();
            Assert.AreEqual(res, proc.trigCalculation(operation, operand));
            // синус
            operation = "x";
            res = Math.Sin(Convert.ToDouble(operand)).ToString();
            Assert.AreEqual(res, proc.trigCalculation(operation, operand));
            // тангенс
            operation = "y";
            res = (1 / Math.Tan(Convert.ToDouble(operand))).ToString();
            Assert.AreEqual(res, proc.trigCalculation(operation, operand));
            // контангенс
            operation = "z";
            res = Math.Tan(Convert.ToDouble(operand)).ToString();
            Assert.AreEqual(res, proc.trigCalculation(operation, operand));
        }

        [TestMethod()]
        public void ariphCalculation_Test()
        {
            string operation, operand1, operand2, res;
            var proc = new Calculate();
            // сложение
            operation = "+";
            operand1 = "50";
            operand2 = "12.53";
            res = "62.53";
            Assert.AreEqual(res, proc.ariphCalculation(operation, operand1.Replace(".", ","), operand2.Replace(".", ",")).Replace(",", "."));
            // вычитание
            operation = "-";
            operand1 = "50";
            operand2 = "12.53";
            res = "37.47";
            Assert.AreEqual(res, proc.ariphCalculation(operation, operand1.Replace(".", ","), operand2.Replace(".", ",")).Replace(",", "."));
            // умножение
            operation = "*";
            operand1 = "6";
            operand2 = "2.5";
            res = "15";
            Assert.AreEqual(res, proc.ariphCalculation(operation, operand1.Replace(".", ","), operand2.Replace(".", ",")).Replace(",", "."));
            // деление 
            operation = "/";
            operand1 = "50";
            operand2 = "12.50";
            res = "4";
            Assert.AreEqual(res, proc.ariphCalculation(operation, operand1.Replace(".", ","), operand2.Replace(".", ",")).Replace(",", "."));
            // возведение в степень
            operation = "^";
            operand1 = "3";
            operand2 = "3";
            res = "27";
            Assert.AreEqual(res, proc.ariphCalculation(operation, operand1.Replace(".", ","), operand2.Replace(".", ",")).Replace(",", "."));
        }

        [TestMethod()]
        public void preTreatment_addToOutput_IntegrationTest()
        {
            bool k = true;
            int i = 0, n = polishRecord.Count();

            var proc = new Calculate();
            proc.expression = "w(x(3.14/2))";
            polishRecord.Add("3.14");
            polishRecord.Add("2");
            polishRecord.Add("x");
            polishRecord.Add("w");
            proc.preTreatment();
            while (i < n)
            {
                k = polishRecord.ElementAt(i) == proc.polishRecord.ElementAt(i);
                i += 1;
            }
            Assert.IsTrue(k);
            proc = null;
        }

        [TestMethod()]
        public void preChange_preTreatment_IntegrationTest()
        {
            bool k = true;
            int i = 0, n = polishRecord.Count();
            //
            var proc = new Calculate("cos(ctg(3.14/2))");
            polishRecord.Add("3.14");
            polishRecord.Add("2");
            polishRecord.Add("x");
            polishRecord.Add("w");
            while (i < n)
            {
                k = polishRecord.ElementAt(i) == proc.polishRecord.ElementAt(i);
                i += 1;
            }
            Assert.IsTrue(k);
            proc = null;

            //proc = new Calculate("");
        }

        [TestMethod()]
        public void addToOutput_Test()
        {
            bool k;
            int n, i;
            var proc = new Calculate();
            Regex oper = new Regex("[^a-vA-Z;\\=,?!@`~$%|0-9]()");
            string sub;
            Stack<string> input = new Stack<string>();
            Stack<string> output = new Stack<string>();
            // операции одинакового приоритета
            input.Push("+");
            output.Push("+");
            output.Push("+");
            sub = "+";
            proc.addToOutput(input, sub, oper);
            k = true;
            n = 2;
            i = 0;
            while (i < n)
            {
                k = input.ElementAt(i) == output.ElementAt(i);
                i += 1;
            }
            Assert.IsTrue(k);
            input.Clear();
            output.Clear();
            proc = null;
            // операция в стеке меньшего приоритета
            proc = new Calculate();
            input.Push("+");
            output.Push("+");
            output.Push("*");
            sub = "*";
            proc.addToOutput(input, sub, oper);
            k = true;
            n = 2;
            i = 0;
            while (i < n)
            {
                k = input.ElementAt(i) == output.ElementAt(i);
                i += 1;
            }
            Assert.IsTrue(k);
            input.Clear();
            output.Clear();
            proc = null;
            // операция в стеке бОльшего приоритета
            proc = new Calculate();
            input.Push("+");
            input.Push("*");
            input.Push("/");
            output.Push("+");
            output.Push("-");
            sub = "-";
            proc.addToOutput(input, sub, oper);
            k = true;
            n = 2;
            i = 0;
            while (i < n)
            {
                k = input.ElementAt(i) == output.ElementAt(i);
                i += 1;
            }
            Assert.IsTrue(k);
            input.Clear();
            output.Clear();
            proc = null;
            // закрывающая скобка
            proc = new Calculate();
            input.Push("*");
            input.Push("(");
            input.Push("+");
            input.Push("*");
            output.Push("*");
            sub = ")";
            proc.addToOutput(input, sub, oper);
            k = true;
            n = 1;
            i = 0;
            while (i < n)
            {
                k = input.ElementAt(i) == output.ElementAt(i);
                i += 1;
            }
            Assert.IsTrue(k);
        }

        [TestMethod()]
        public void preChange_Test()
        {
            // проверка на минус
            var proc = new Calculate("-5");
            Assert.AreEqual("(0-5)", proc.expression);
            proc = null;

            proc = new Calculate("-2+10");
            Assert.AreEqual("(0-2)+10", proc.expression);
            proc = null;

            proc = new Calculate("-(23+2*3-4*(-2+5))");
            Assert.AreEqual("(0-(23+2*3-4*((0-2)+5)))", proc.expression);
            proc = null;

            proc = new Calculate("3+(-5)");
            Assert.AreEqual("3+((0-5))", proc.expression);
            proc = null;

            proc = new Calculate("-2+4-43+(-3-5)-2+(-2)");
            Assert.AreEqual("(0-2)+4-43+((0-3)-5)-2+((0-2))", proc.expression);
            proc = null;
            // проверка на тригонометрические функции
            proc = new Calculate("cos(1)");
            Assert.AreEqual("w(1)", proc.expression);
            proc = null;

            proc = new Calculate("sin(1)");
            Assert.AreEqual("x(1)", proc.expression);
            proc = null;

            proc = new Calculate("ctg(1)");
            Assert.AreEqual("y(1)", proc.expression);
            proc = null;

            proc = new Calculate("tg(1)");
            Assert.AreEqual("z(1)", proc.expression);
            proc = null;

            proc = new Calculate("cos(tg(1))");
            Assert.AreEqual("w(z(1))", proc.expression);
            proc = null;
        }

        [TestMethod()]
        public void preTreatment_calculation_IntegrationgTest()
        {
            var proc = new Calculate();
            proc.expression = "w(3.14)";
            proc.preTreatment();
            proc.calculation();
            Assert.AreEqual("3.14:cos", proc.polish);
            Assert.AreEqual("-1.00", proc.result);
        }

        [TestMethod()]
        public void minusExpTest()
        {
            int k;
            var proc = new Calculate();
            k = proc.minusNum("235");
            Assert.AreEqual(3, k);

            k = proc.minusNum("234+12");
            Assert.AreEqual(3, k);

            k = proc.minusNum("21434.24538");
            Assert.AreEqual(11, k);

            k = proc.minusNum("21434.24538+32");
            Assert.AreEqual(11, k);

        }

        [TestMethod()]
        public void minusExpTest1()
        {
            int k;
            var proc = new Calculate();
            k = proc.minusExp("(234+3)-2)");
            Assert.AreEqual(10, k);

            k = proc.minusExp("(234+3)-2)+4");
            Assert.AreEqual(10, k);
        }
    }
}