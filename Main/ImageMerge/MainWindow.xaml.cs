using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace ImageMerge
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void buttonOpenImages_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Multiselect = true;
			ofd.Filter = "PNG images (*.png)|*.png";

			if (ofd.ShowDialog() == true)
			{
				foreach (var file in ofd.FileNames)
				{
					listBoxImages.Items.Add(file);
				}
			}
		}

		private void buttonCreate_Click(object sender, RoutedEventArgs e)
		{
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Filter = "PNG images (*.png)|*.png";

			if (sfd.ShowDialog() == true)
			{
				string outFile = sfd.FileName;
				DateTime start = DateTime.Now;

				FileStream csvFile;
				StreamWriter csvWriter = null;
				if (checkBoxCsv.IsChecked == true)
				{
					csvFile = File.OpenWrite(outFile.Replace(".png", ".csv"));
					csvWriter = new StreamWriter(csvFile);
				}

				try
				{
					int columns = Math.Min(listBoxImages.Items.Count, Convert.ToInt32(textBoxColumns.Text));
					int rows = (int)Math.Ceiling(listBoxImages.Items.Count / (double)columns);

					int offset = Convert.ToInt32(textBoxOffset.Text);

					var collage = new Bitmap(columns * offset, rows * offset);
					var graphics = Graphics.FromImage(collage);

					int index = 0;
					for (int row = 0; row < rows; ++row)
					{
						for (int column = 0; column < columns; ++column)
						{
							if (index == listBoxImages.Items.Count)
							{
								collage.Save(outFile, ImageFormat.Png);
								TimeSpan time = DateTime.Now - start;

								if (checkBoxCsv.IsChecked == true)
									csvWriter.Close();

								MessageBox.Show("Image created in " + time.TotalMilliseconds + "ms.", "Image created");

								return;
							}

							string imagePath = listBoxImages.Items[index++].ToString();
							var image = Bitmap.FromFile(imagePath);

							graphics.DrawImageUnscaled(image, offset * column, offset * row);

							if (checkBoxCsv.IsChecked == true)
								csvWriter.WriteLine(String.Format("{0},{1},{2}", imagePath.Replace(textBoxBasePath.Text, String.Empty), offset * column, offset * row));
						}
					}
				}
				catch (Exception exp)
				{
					MessageBox.Show(exp.Message, "Error");
				}
			}
		}

		private void buttonClear_Click(object sender, RoutedEventArgs e)
		{
			listBoxImages.Items.Clear();
		}
	}
}
