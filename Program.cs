using System;

namespace optimization_methods
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Func<double[], double> rosenbrock = x => Math.Pow(1 - x[0], 2) + 100 * Math.Pow(x[1] - x[0] * x[0], 2); //задание входной функции (функция Розенброка)
            NelderMead nelderMead = new NelderMead(rosenbrock, 2, new double[] { 0, 0}); //создание объекта класса (ввод входных параметров)
            nelderMead.Start();  //запуск метода Нелдера Мида

            /*Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());*/
        } 
    }
}

