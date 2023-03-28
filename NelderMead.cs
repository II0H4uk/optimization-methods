using System;
using System.Linq;
using System.Windows.Forms;

namespace optimization_methods
{
    class NelderMead
    {
        private const double alpha = 1;
        private const double beta = 0.5;
        private const double gamma = 2;
        private double[][] simplex;
        private double[] values;
        private Func<double[], double> func;
        private int dimensions;

        public NelderMead(Func<double[], double> inputFunc, int inputDimensions, double[] startState, int step = 1) //конструктор класса
        {
            dimensions = inputDimensions;   //размерность функции
            simplex = new double[dimensions + 1][]; //выделяем память для симплекса
            values = new double[dimensions + 1];    //выделяем память для значений функции
            func = inputFunc;   //записываем функцию

            for (int i = 0; i < dimensions + 1; i++)    //строим начальный симплекс
            {
                simplex[i] = new double[dimensions];
                for (int j = 0; j < dimensions; j++)
                {
                    simplex[i][j] = startState[j];
                    if (i == j + 1)
                        simplex[i][j] += step;
                }
                values[i] = func(simplex[i]);   //считаем значение функции в каждой точке
            }
        }

        public void Start() //начало метода Нелдера Мида
        {
            double[] centerX;
            double[] reflectedX;
            double reflectedVal;
            double[] extendedX;
            double extendedVal;
            double[] buf;
            double bufNum;
            double[] squeezeX;
            double squeezeVal;

            for (int i = 0; i < 1000; i++ ) //итерации алгоритма Нелдера Мида
            {
                Sort();  //Сортируем симплекс
                centerX = CalcCenter(); //центр тяжести
                reflectedX = CalcReflect(centerX);  //отражение точки xh
                reflectedVal = func(reflectedX);    //значение отражения

                if (reflectedVal < values[values.Length - 1])   //проверка на fr < fl
                {
                    extendedX = CalcExtension(centerX, reflectedX); //растяжение 
                    extendedVal = func(extendedX);

                    if (extendedVal < reflectedVal) //проверка на fe < fr
                        simplex[0] = extendedX;
                    else
                        simplex[0] = reflectedX;
                    continue;
                }

                if (values[values.Length - 1] < reflectedVal && reflectedVal < values[1])   //проверка на fl < fr < fg
                {
                    simplex[0] = reflectedX;    //записываем в xh точку xr
                    values[0] = reflectedVal;
                    continue;
                }

                if (values[1] < reflectedVal && reflectedVal < values[0])   //проверка на fg < fr < fh
                {
                    buf = simplex[0];   //меняем местами xr и xh
                    bufNum = values[0];
                    simplex[0] = reflectedX;
                    values[0] = reflectedVal;
                    reflectedX = buf;
                    reflectedVal = bufNum;
                }

                squeezeX = CalcSqueeze(centerX);    //сжатие
                squeezeVal = func(squeezeX);

                if (squeezeVal < values[0]) //проверка на fs < fh
                {
                    simplex[0] = squeezeX;  //присваиваем xh значение xs
                    values[0] = squeezeVal;
                    continue;
                }
                else
                    GlobalSqueeze();    //глобальное сжатие

            }
            MessageBox.Show("point: (" + simplex[simplex.Length - 1][0] + ", " + simplex[simplex.Length - 1][0] + ")\nvalue: " + values[values.Length - 1]);
        }

        void GlobalSqueeze()    //глобальное сжатие
        {
            for (int i = 0; i < simplex.Length - 1; i++)
                for (int j = 0; j < simplex[0].Length; j++)
                    simplex[i][j] = simplex[simplex.Length - 1][j] + (simplex[i][j] - simplex[simplex.Length - 1][j]) / 2;
        }

        double[] CalcSqueeze(double[] centerX)  //сжатие
        {
            double[] squeeze = new double[simplex[0].Length];

            for (int i = 0; i < simplex[0].Length; i++)
                squeeze[i] = beta * simplex[0][i] + (1 - beta) * centerX[i];

            return squeeze;
        }

        void Sort() //сортировка симплекса
        {
            double[] bufMas;
            double buf;

            for (int i = 0; i < values.Length; i++) //пузырьковая сортировка
                for (int j = i; j < values.Length; j++)
                    if (values[i] < values[j])
                    {
                        bufMas = simplex[i];    //меняем местами точки
                        simplex[i] = simplex[j];
                        simplex[j] = bufMas;

                        buf = values[i];    //меняем местами значение точек
                        values[i] = values[j];
                        values[j] = buf;
                    }
        }

        double[] CalcCenter()   //центр тяжести
        {
            double[] sum = new double[simplex[0].Length];

            for (int i = 1; i < simplex.Count(); i++)
                for (int j = 0; j < simplex[0].Length; j++)
                    sum[j] += simplex[i][j];

            for (int i = 0; i < sum.Length; i++)
                sum[i] /= (simplex.Length - 1);

            return sum;
        }

        double[] CalcReflect(double[] centerX)  //отражение
        {
            double[] reflection = new double[simplex[0].Length];

            for (int i = 0; i < simplex[0].Length; i++)
                reflection[i] = (1 + alpha) * centerX[i] - alpha * simplex[0][i];

            return reflection;
        }

        double[] CalcExtension(double[] centerX, double[] reflectedX)   //растяжение
        {
            double[] extention = new double[simplex[0].Length];

            for (int i = 0; i < simplex[0].Length; i++)
                extention[i] = (1 - gamma) * centerX[i] + gamma * reflectedX[i];

            return extention;
        }

        /*double CalcVariance()
        {
            double[] mean = new double[simplex[0].Length];
            double variance = 0;

            for (int i = 0; i < simplex.Length; i++)
                for (int j = 0; j < simplex[0].Length; j++)
                    mean[j] += simplex[i][j];

            for (int i = 0; i < simplex[0].Length; i++)
                mean[i] /= (simplex.Length - 1);

            for (int i = 0; i < simplex.Length; i++)
                for (int j = 0; j < simplex[0].Length; j++)
                    variance += Math.Pow(simplex[i][j] - mean[j], 2);

            variance /= (simplex.Length - 1);
            return variance;
        }*/
    }
}
