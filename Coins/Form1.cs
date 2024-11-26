using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ImageProcess2;

namespace Coins
{
    public partial class Form1 : Form
    {
        Bitmap image, processed;
        List<List<Point>> coins;
        bool[,] visited_coins;

        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            processed = (Bitmap)image.Clone();
            BitmapFilter.Threshold(ref processed, 200);
            pictureBox1.Image = processed;

            CountCoins(processed);
        }

        private void CountCoins(Bitmap mp)
        {
            coins = new List<List<Point>>();
            visited_coins = new bool[processed.Width, processed.Height];

            int coins_count = 0;
            int coins_total = 0;

            for (int x = 0; x < processed.Width; x++)
            {
                for (int y = 0; y < processed.Height; y++)
                {
                    Color pixel = processed.GetPixel(x, y);

                    if (pixel.R == 0 && !visited_coins[x, y])
                    {
                        List<Point> coin;
                        int size;

                        (coin, size) = GetCoin(x, y);

                        if (size < 20) continue;

                        coins.Add(coin);
                        coins_count++;
                        int coin_worth = GetValue(size);
                        coins_total += coin_worth;
                    }
                }
            }

            totalValue.Text = (coins_total / 100) + "." + (coins_total % 100);
        }

        private (List<Point>, int) GetCoin(int x, int y)
        {
            List<Point> points = new List<Point>();
            Bitmap image = (Bitmap)processed.Clone();

            int size = 0;

            Queue<Point> q = new Queue<Point>();
            q.Enqueue(new Point(x, y));

            while (q.Count > 0)
            {
                Point curr = q.Dequeue();
                points.Add(curr);
                int px = curr.X;
                int py = curr.Y;

                if (visited_coins[px, py])
                {
                    continue;
                }

                size++;

                visited_coins[px, py] = true;

                Color pixel = image.GetPixel(px, py);

                if (px - 1 >= 0 && pixel.R == 0 && !visited_coins[px - 1, py])
                {
                    q.Enqueue(new Point(px - 1, py));
                }

                if (px + 1 < image.Width && pixel.R == 0 && !visited_coins[px + 1, py])
                {
                    q.Enqueue(new Point(px + 1, py));
                }

                if (py - 1 >= 0 && pixel.R == 0 && !visited_coins[px, py - 1])
                {
                    q.Enqueue(new Point(px, py - 1));
                }

                if (py + 1 < image.Height && pixel.R == 0 && !visited_coins[px, py + 1])
                {
                    q.Enqueue(new Point(px, py + 1));
                }
            }

            return (points, size);
        }

        private int GetValue(int size)
        {
            if (size > 8000)
            {
                return 500;
            }

            if (size > 6000)
            {
                return 100;
            }

            if (size > 4000)
            {
                return 25;
            }

            if (size > 3500)
            {
                return 10;
            }
            
            return 5;
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            image = new Bitmap(openFileDialog1.FileName);
            pictureBox1.Image = image;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }
    }
}
