using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace optimization_methods
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void DrawContourLines()
        {
            // Создаем объект Graphics для PictureBox
            Graphics g = pictureBox1.CreateGraphics();

            // Очищаем PictureBox
            g.Clear(Color.White);

            // Определяем диапазон значений x и y
            double xmin = -1;
            double xmax = 1;
            double ymin = -1;
            double ymax = 1;

            // Определяем размер PictureBox
            int width = pictureBox1.Width;
            int height = pictureBox1.Height;

            // Определяем шаг сетки
            double dx = (xmax - xmin) / (width - 1);
            double dy = (ymax - ymin) / (height - 1);

            // Определяем функцию двух переменных
            double Func(double x, double y)
            {
                return x * x + y * y;
            }

            // Определяем количество линий уровня и их значения
            int numContours = 10;
            double[] contourValues = new double[numContours];
            for (int i = 0; i < numContours; i++)
            {
                contourValues[i] = (i + 1) * (xmax - xmin) / numContours;
            }

            // Рисуем линии уровня
            for (int i = 0; i < numContours; i++)
            {
                double contourValue = contourValues[i];

                // Определяем контурную линию
                List<PointF> contourLine = new List<PointF>();
                for (double y = ymin; y <= ymax; y += dy)
                {
                    PointF[] points = new PointF[width];
                    int numPoints = 0;
                    for (double x = xmin; x <= xmax; x += dx)
                    {
                        double z = Func(x, y);
                        if (Math.Abs(z - contourValue) < dx / 2)
                        {
                            points[numPoints] = new PointF((float)((x - xmin) / (xmax - xmin) * width), (float)((ymax - y) / (ymax - ymin) * height));
                            numPoints++;
                        }
                    }
                    if (numPoints > 1)
                    {
                        Array.Resize(ref points, numPoints);
                        contourLine.AddRange(points);
                    }
                }
                if (contourLine.Count > 1)
                {
                    g.DrawLines(Pens.Black, contourLine.ToArray());
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DrawContourLines();
        }
    }
}
