using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using folder = System.Windows.Forms;
using Crypto.Stenografie;

namespace Crypto.Gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private OpenFileDialog browseVenster = new OpenFileDialog();
        private String _imagePath;

        public MainWindow()
        {
            InitializeComponent();
        }


        #region Stenografie
        private void browseImageButton_Click(object sender, RoutedEventArgs e)
        {
            browseVenster.Filter = "Image Files (*.jpeg; *.png; *.bmp)| *.jpg; *.png; *.bmp";
            if (browseVenster.ShowDialog() == true)
            {
                string padFoto = browseVenster.FileName;
                //FileNameLabel.Content = selectedFileName;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(padFoto);
                bitmap.EndInit();
                SelectedImage.Source = bitmap;
                // labelSelectedImage.Content = padFoto;
                _imagePath = padFoto;
            }
        }

        private void HideData(object sender, RoutedEventArgs e)
        {
            try
            {
                string text = RichTextBox.Text;

                // string padFoto = labelSelectedImage.Content.ToString();
                Bitmap bitmap = new Bitmap(_imagePath);

                if (text.Equals(""))
                {
                    MessageBox.Show("The text you want to hide can't be empty", "Warning");
                    return;
                }

                if (EncrypedCheckBox.IsChecked == true)
                {
                    if (PasswordPassword.Password.Length < 6)
                    {
                        MessageBox.Show("Please enter a password with at least 6 characters", "Warning");
                        return;
                    }
                    else
                    {
                        text = StenografieCrypto.EncryptStringAes(text, PasswordPassword.Password);
                    }
                }

                bitmap = StenografieHelper.EmbedText(text, bitmap);

                MessageBox.Show("Your text was hidden in the image successfully!", "Done");
                MaakLeeg();

                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Filter = "Png Image|*.png|Bitmap Image|*.bmp";

                if (saveDialog.ShowDialog() == true)
                {
                    switch (saveDialog.FilterIndex)
                    {
                        case 0:
                        {
                            bitmap.Save(saveDialog.FileName, ImageFormat.Png);
                        }
                            break;
                        case 1:
                        {
                            bitmap.Save(saveDialog.FileName, ImageFormat.Bmp);
                        }
                            break;
                    }


                }
            }
            catch
            {
                MessageBox.Show("Something went wrong");
            }
            finally
            {
                MaakLeeg();
            }
        }

        /// <summary>
        /// Maar de invoervelden leeg
        /// </summary>
        private void MaakLeeg()
        {
            RichTextBox.Text = "";
            PasswordPassword.Password = "";
            EncrypedCheckBox.IsChecked = false;
        }
        #endregion

        private void discoverButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string padFoto = browseVenster.FileName;
                Bitmap bitmap = new Bitmap(padFoto);

                string extractedText = StenografieHelper.ExtractText(bitmap);

                if (EncrypedCheckBox.IsChecked == true)
                {
                    try
                    {
                        extractedText = StenografieCrypto.DecryptStringAes(extractedText, PasswordPassword.Password);
                    }
                    catch
                    {
                        MessageBox.Show("Wrong password", "Error");

                        return;
                    }
                }
                RichTextBox.Text = extractedText;
            }
            catch
            {
                MessageBox.Show("Something went wrong");
            }
        }
    }
    
}
