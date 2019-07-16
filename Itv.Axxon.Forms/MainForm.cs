using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Itv.Axxon.Model;
using Itv.Axxon.Model.Figures;
using Itv.Axxon.Model.Parsing;
using Point = System.Drawing.Point;

namespace Itv.Axxon.Forms
{
    public partial class MainForm : Form
    {
        private const int MaxSize = 1000;

        private readonly Image _image = new Bitmap(MaxSize, MaxSize);

        private Color _color = Color.Green;

        private List<Triangle> _sortedFigures = new List<Triangle>();

        private int _levelsCount = 1;

        private string _title;

        public MainForm()
        {
            InitializeComponent();

            this.Shown += (a, e) => _title = this.Text;

            openFileDialog1.InitialDirectory = Directory.GetCurrentDirectory();
        }

        private void openToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (var stream = openFileDialog1.OpenFile())
                {
                    try
                    {
                        var triangles = TriangleParser.ParseTriangles(stream);

                        ProcessFigures(triangles);
                    }
                    catch (FormatException fe)
                    {
                        MessageBox.Show(fe.Message, @"Ошибка формата", MessageBoxButtons.OK);
                        return;
                    }
                    catch (FiguresIntersectingException fie)
                    {
                        MessageBox.Show(fie.Message, @"Ошибка", MessageBoxButtons.OK);

                        levelsCountTextBox.Text = @"Error";
                        _levelsCount = 1;
                        _sortedFigures = new List<Triangle>();
                    }
                }

                this.Text = $@"{_title} - {openFileDialog1.FileName}";

                RedrawImageAndRefresh();
            }
        }

        private void ProcessFigures(IEnumerable<Triangle> triangles)
        {
            var nestingHierarchy = new NestingHierarchy(triangles);

            _levelsCount = nestingHierarchy.GetHeight() + 1;
            levelsCountTextBox.Text = _levelsCount.ToString();

            _sortedFigures = nestingHierarchy.GetSorted().ToList();
        }

        private void RedrawImageAndRefresh()
        {
            var graphics = Graphics.FromImage(_image);

            // трансформируем Graphics так, чтобы начало координат находилось в левом нижнем углу и ось Y была направлена вверх
            graphics.TranslateTransform(0, MaxSize);
            graphics.ScaleTransform(1, -1);

            // оттенки реализованы с помощью прозрачности цвета закрашивания
            // чем больше уровней вложенности, тем меньше шаг изменения темноты оттенка
            var alpha = 0xFF / _levelsCount;
            var stepColor = Color.FromArgb((int)alpha, _color);

            graphics.Clear(stepColor);

            // рисуем фигуры последовательно полупрозрачным цветом,
            // т.к. они отсортированы в порядке от самых внешних до самых вложенных.
            // накладываясь вложенные будут темнее
            var solidBrush = new SolidBrush(stepColor);
            foreach (var triangle in _sortedFigures)
            {
                graphics.FillPolygon(solidBrush,
                    triangle.Points.Select(x => x.ToPointF()).ToArray());
            }

            Refresh();
        }

        private void colorPictureBox_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(_color);
        }

        private void pickColorButton_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                _color = colorDialog1.Color;
                RedrawImageAndRefresh();
            }
        }

        private void imagePanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(_image, new Point(0, 0));
        }
    }
}
