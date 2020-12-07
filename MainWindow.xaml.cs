using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Stenografia
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private byte[] pixelByteArray;
        private int amountOfBYtesInBitmap;
        byte[] ascii;
        bool isEncrypted;
        double spaceOfFileToCode;
        double freeSpaceInFile;
        double spaceWichLeftInFile;



        int redBits = 1;
        int greenBits = 1;
        int blueBits = 1;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            CleanAfterEncryption();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {

                image1.Source = new BitmapImage(new Uri(openFileDialog.FileName));


                readBitmap();

            }



        }

        private void showColumnChart()
        {
            List<KeyValuePair<string, double>> valueList = new List<KeyValuePair<string, double>>();
            if (spaceWichLeftInFile > 0)
                valueList.Add(new KeyValuePair<string, double>("Pozostałe wolne miejsce:", spaceWichLeftInFile));
            else
                valueList.Add(new KeyValuePair<string, double>("Pozostałe wolne miejsce:", 0));
            // valueList.Add(new KeyValuePair<string, double>("Pojemność nośnika kB:", freeSpaceInFile));

            valueList.Add(new KeyValuePair<string, double>("Zajęte miejsce po zakodowanie", spaceOfFileToCode));

            //valueList.Add(new KeyValuePair<string, double>("QA", 30));
            // valueList.Add(new KeyValuePair<string, int>("Project Manager", 40));

            //Setting data for column chart
            //columnChart.DataContext = valueList;

            // Setting data for pie chart
            pieChart.DataContext = valueList;

            //Setting data for area chart
            //areaChart.DataContext = valueList;

            //Setting data for bar chart
            //.DataContext = valueList;

            //Setting data for line chart
            // lineChart.DataContext = valueList;
        }

        private void readBitmap() // wczytuje bitmape i tworzy tablice z bajtami kolorow
        {
            pixelByteArray = convertImageToByte(convertImageWpfToGDI(image1.Source));
            //ImageSource ims = image1.Source;
            //BitmapImage bitmapImage = (BitmapImage)ims;
            //int height = bitmapImage.PixelHeight;
            //int width = bitmapImage.PixelWidth;
            //int nStride = (bitmapImage.PixelWidth * bitmapImage.Format.BitsPerPixel + 7) / 8;
            //pixelByteArray = new byte[bitmapImage.PixelHeight * nStride];
            //bitmapImage.CopyPixels(pixelByteArray, nStride, 0);
            //bitmapFormat = bitmapImage.Format.BitsPerPixel;
            amountOfBYtesInBitmap = (int)((pixelByteArray.Length) / 4);
            isEncrypted = ifBitmapWasEncrypted(pixelByteArray);
            if (isEncrypted)
            {
                lIfBitmapWasEncrypted.Content = "Tak";
                lIfBitmapWasEncrypted.Foreground = System.Windows.Media.Brushes.Green;
            }
            else
            {
                lIfBitmapWasEncrypted.Content = "Nie";
                lIfBitmapWasEncrypted.Foreground = System.Windows.Media.Brushes.Red;
            }
            labelOriginalImageSize.Content = (((pixelByteArray.Length * 3) / 4) / 1000).ToString();


        }
        private System.Drawing.Image convertImageWpfToGDI(System.Windows.Media.ImageSource image)
        {
            MemoryStream ms = new MemoryStream();
            var encoder = new System.Windows.Media.Imaging.BmpBitmapEncoder();
            encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(image as System.Windows.Media.Imaging.BitmapSource));
            encoder.Save(ms);
            ms.Flush();
            return System.Drawing.Image.FromStream(ms);
        }
        public static byte[] convertImageToByte(System.Drawing.Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        private void BtnOpenFileBinnaryFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text|*.txt|All|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                // fileBytes = File.ReadAllBytes(openFileDialog.FileName);
                string textToEncrypt = System.IO.File.ReadAllText(openFileDialog.FileName);

                ascii = Encoding.ASCII.GetBytes(textToEncrypt);
                spaceOfFileToCode = (((double)(ascii.Length)) / 1000);
                labelAmountOfNeededSpace.Content = spaceOfFileToCode.ToString();
            }

            // showColumnChart();


            Int32.TryParse(numberOfBytesRed.Text, out redBits);

            Int32.TryParse(numberOfBytesGreen.Text, out greenBits);

            Int32.TryParse(numberOfBytesBlue.Text, out blueBits);
            if (amountOfBYtesInBitmap > 0)
            {
                freeSpaceInFile = ((((double)redBits * amountOfBYtesInBitmap + greenBits * amountOfBYtesInBitmap + blueBits * amountOfBYtesInBitmap) - 43) / 1000 / 8);
                labelBitCapacityInBitmap.Content = freeSpaceInFile.ToString("0.00");
                showColumnChart();

            }

            if (labelSpaceWichLeft != null)
            {

                spaceWichLeftInFile = (freeSpaceInFile - spaceOfFileToCode);
                labelSpaceWichLeft.Content = spaceWichLeftInFile.ToString("0.00");
                showColumnChart();
            }
        }



        private void encryptBinnaryFileInBitMap()
        {
            if (ascii == null)
            {
                if (tbMultiLine.Text == "")
                {
                    MessageBox.Show("Nie ma pliku ani wiadomosci\n do zakodowania!");
                    return;
                }
                ascii = Encoding.ASCII.GetBytes(tbMultiLine.Text);
            }
            if (pixelByteArray == null)
            {
                MessageBox.Show("Nie wybrano żadnej bitmapy\n do zakodowania!");
                return;
            }

            int numberOfbit = 0;
            byte[] bytesToEncrypt = new byte[ascii.Length];
            int byteToInt;
            byte[] numberOfElemntsInString = System.BitConverter.GetBytes(ascii.Length);

            pixelByteArray[54] = Convert.ToByte(69);
            pixelByteArray[55] = Convert.ToByte(69);
            pixelByteArray[56] = Convert.ToByte(69);
            pixelByteArray[57] = Convert.ToByte(blueBits);
            pixelByteArray[58] = Convert.ToByte(greenBits);
            pixelByteArray[59] = Convert.ToByte(redBits);


            pixelByteArray[60] = Convert.ToByte(numberOfElemntsInString[0]);
            pixelByteArray[61] = Convert.ToByte(numberOfElemntsInString[1]);
            pixelByteArray[62] = Convert.ToByte(numberOfElemntsInString[2]);
            pixelByteArray[63] = Convert.ToByte(numberOfElemntsInString[3]);

            //bytesToEncrypt[0] = Convert.ToByte(blueBits);
            //bytesToEncrypt[1] = Convert.ToByte(greenBits);
            //bytesToEncrypt[2] = Convert.ToByte(redBits);
            //Array.Copy(numberOfElemntsInString, 0, pixelByteArray, 60, 4);
            Array.Copy(ascii, 0, bytesToEncrypt, 0, ascii.Length);

            //int logOfBitesToWrite;
            //int asciLenght = ascii.Length;

            //var buffer = new MemoryStream(ascii);
            //int tempForBits = ascii[0];


            BitArray bitsToEncrypt = new BitArray(bytesToEncrypt);

            for (int i = 64, j = 2; i < pixelByteArray.Length && numberOfbit < bitsToEncrypt.Length; i++)
            {

                byteToInt = (int)pixelByteArray[i];

                //fullByteValue = 255;
                if (j == 0)
                {
                    ////byteToInt &= (256-(int)Math.Pow(2, blueBits));

                    ////while (bitesToWrite < 255) { 

                    ////    logOfBitesToWrite = (int)(Math.Log(bitesToWrite + 1, 2));
                    ////   // if ();

                    ////}
                    //if(z < ascii.Length)
                    //{
                    //    //if(blueBits==8)
                    //    //{
                    //    //    //TO DO
                    //    //}
                    //    if ((amountOfBitsInByte - blueBits) < 0)
                    //    {
                    //        //TO DO
                    //        z++;
                    //        tempForBits = ascii[z];
                    //    }
                    //    else
                    //    {

                    //        tempForBits <<= (amountOfBitsInByte - blueBits);
                    //        tempForBits >>= (amountOfBitsInByte - blueBits);
                    //    }
                    //}
                    BitArray blueBitsArray = new BitArray(System.BitConverter.GetBytes(pixelByteArray[i]));

                    for (int z = 0; z < blueBits && numberOfbit < bitsToEncrypt.Length; z++, numberOfbit++)
                        blueBitsArray[z] = bitsToEncrypt[numberOfbit];


                    byteToInt = convertBitArrayToInt(blueBitsArray);



                }
                else if (j == 1)
                {
                    //byteToInt &= (256-(int)Math.Pow(2, greenBits));
                    BitArray greenBitsArray = new BitArray(System.BitConverter.GetBytes(pixelByteArray[i]));

                    for (int z = 0; z < greenBits && numberOfbit < bitsToEncrypt.Length; z++, numberOfbit++)
                        greenBitsArray[z] = bitsToEncrypt[numberOfbit];

                    byteToInt = convertBitArrayToInt(greenBitsArray);


                }
                else if (j == 2)
                {
                    BitArray redBitsArray = new BitArray(System.BitConverter.GetBytes(pixelByteArray[i]));

                    for (int z = 0; z < redBits && numberOfbit < bitsToEncrypt.Length; z++, numberOfbit++)
                        redBitsArray[z] = bitsToEncrypt[numberOfbit];

                    //byteToInt &= (256-(int)Math.Pow(2, redBits));
                    byteToInt = convertBitArrayToInt(redBitsArray);

                }
                else
                {
                    j = 0;
                    continue;
                }



                pixelByteArray[i] = Convert.ToByte(byteToInt);
                j++;
            }

            Bitmap encryptedBitmap;
            var ms = new MemoryStream(pixelByteArray);


            encryptedBitmap = new Bitmap(ms);
            encryptedImage.Source = convertBitmapToImageSource(encryptedBitmap);
            //System.Drawing.Image x = (Bitmap)((new ImageConverter()).ConvertFrom(pixelByteArray));
            //var ms = new MemoryStream(pixelByteArray);
            //encryptedImage = x;
            //System.Drawing.Image test = System.Drawing.Image.FromStream(ms);
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "bmp files (*.bmp)|*.bmp";
            saveFileDialog.DefaultExt = "Test.bmp";
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    encryptedBitmap.Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                    labelEncryptedImageSize.Content = ((encryptedBitmap.Height * encryptedBitmap.Width * 4) / 1000).ToString();

                }
                catch
                {

                }


            }




        }

        private bool ifBitmapWasEncrypted(byte[] pixelByteArray)
        {
            if (pixelByteArray[54] == 69 && pixelByteArray[55] == 69 && pixelByteArray[56] == 69)
                return true;
            else
                return false;
        }

        private BitmapImage convertBitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private void EncryptButton_Click(object sender, RoutedEventArgs e)
        {
            if (spaceWichLeftInFile < 0)
                MessageBox.Show("Brak Miejsca!!!");
            else
                encryptBinnaryFileInBitMap();
        }
        private void RefreshAfterValueChangeInTextBoxes(object sender, RoutedEventArgs e)
        {



            if ((sender as TextBox).Name == "numberOfBytesRed")
                Int32.TryParse(numberOfBytesRed.Text, out redBits);
            if ((sender as TextBox).Name == "numberOfBytesGreen")
                Int32.TryParse(numberOfBytesGreen.Text, out greenBits);
            if ((sender as TextBox).Name == "numberOfBytesBlue")
                Int32.TryParse(numberOfBytesBlue.Text, out blueBits);
            if (amountOfBYtesInBitmap > 0)
            {
                freeSpaceInFile = ((((double)redBits * amountOfBYtesInBitmap + greenBits * amountOfBYtesInBitmap + blueBits * amountOfBYtesInBitmap) - 43) / 1000 / 8);
                labelBitCapacityInBitmap.Content = freeSpaceInFile.ToString("0.00");
                showColumnChart();

            }

            if (labelSpaceWichLeft != null)
            {

                spaceWichLeftInFile = (freeSpaceInFile - spaceOfFileToCode);
                labelSpaceWichLeft.Content = spaceWichLeftInFile.ToString("0.00");
                showColumnChart();
            }




        }

        private void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            if (pixelByteArray == null)
            {
                MessageBox.Show("Nie wybrano żadnej bitmapy\n do dekodowania!");
                return;
            }

            if (!isEncrypted)
                MessageBox.Show("Plik nie jest zaszyfrowany");
            else
            {
                //CleanAfterEncryption();
                short redBits = pixelByteArray[59];
                short greenBits = pixelByteArray[58];
                short blueBits = pixelByteArray[57];


                byte[] numberOfBytestoDecrypt = new byte[4];

                // Array.Copy(numberOfBytestoDecrypt,0,pixelByteArray,60,4);

                numberOfBytestoDecrypt[0] = pixelByteArray[60];
                numberOfBytestoDecrypt[1] = pixelByteArray[61];
                numberOfBytestoDecrypt[2] = pixelByteArray[62];
                numberOfBytestoDecrypt[3] = pixelByteArray[63];

                BitArray temporaryNumberOfBytes = new BitArray(numberOfBytestoDecrypt);

                int intNumberOfBytesToDecrypt = convertBitArrayToInt(temporaryNumberOfBytes);

                BitArray bitsToDecrypt = new BitArray(intNumberOfBytesToDecrypt * 8);


                int numberOfbit = 0;


                for (int i = 64, j = 2; i < pixelByteArray.Length && numberOfbit < bitsToDecrypt.Length; i++)
                {

                    //byteToInt = (int)pixelByteArray[i];

                    //fullByteValue = 255;
                    if (j == 0)
                    {
                        ////byteToInt &= (256-(int)Math.Pow(2, blueBits));

                        ////while (bitesToWrite < 255) { 

                        ////    logOfBitesToWrite = (int)(Math.Log(bitesToWrite + 1, 2));
                        ////   // if ();

                        ////}
                        //if(z < ascii.Length)
                        //{
                        //    //if(blueBits==8)
                        //    //{
                        //    //    //TO DO
                        //    //}
                        //    if ((amountOfBitsInByte - blueBits) < 0)
                        //    {
                        //        //TO DO
                        //        z++;
                        //        tempForBits = ascii[z];
                        //    }
                        //    else
                        //    {

                        //        tempForBits <<= (amountOfBitsInByte - blueBits);
                        //        tempForBits >>= (amountOfBitsInByte - blueBits);
                        //    }
                        //}
                        BitArray blueBitsArray = new BitArray(System.BitConverter.GetBytes(pixelByteArray[i]));

                        for (int z = 0; z < blueBits && numberOfbit < bitsToDecrypt.Length; z++, numberOfbit++)
                            bitsToDecrypt[numberOfbit] = blueBitsArray[z];






                    }
                    else if (j == 1)
                    {
                        //byteToInt &= (256-(int)Math.Pow(2, greenBits));
                        BitArray greenBitsArray = new BitArray(System.BitConverter.GetBytes(pixelByteArray[i]));

                        for (int z = 0; z < greenBits && numberOfbit < bitsToDecrypt.Length; z++, numberOfbit++)
                            bitsToDecrypt[numberOfbit] = greenBitsArray[z];




                    }
                    else if (j == 2)
                    {
                        BitArray redBitsArray = new BitArray(System.BitConverter.GetBytes(pixelByteArray[i]));

                        for (int z = 0; z < redBits && numberOfbit < bitsToDecrypt.Length; z++, numberOfbit++)
                            bitsToDecrypt[numberOfbit] = redBitsArray[z];

                        //byteToInt &= (256-(int)Math.Pow(2, redBits));


                    }
                    else
                    {
                        j = 0;
                        continue;
                    }




                    j++;
                }

                byte[] decryptedTextInByteArray = new byte[intNumberOfBytesToDecrypt];
                bitsToDecrypt.CopyTo(decryptedTextInByteArray, 0);
                var str = System.Text.Encoding.Default.GetString(decryptedTextInByteArray);
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "txt files (*.txt)|*.txt";
                //saveFileDialog.DefaultExt = "Test.bmp";

                if (saveFileDialog.ShowDialog() == true)
                {

                    File.WriteAllText(saveFileDialog.FileName, str);

                }
            }

        }
        private int convertBitArrayToInt(BitArray bitArray)
        {
            int resoult = 0;
            for (int i = 0; i < bitArray.Length; i++)
            {
                if (bitArray.Get(i))
                    resoult += (int)Math.Pow(2, i);
            }
            return resoult;
        }
        /*Metoda CleanAfterEncryption usuwa zbedne elementy w gui,
         * lub zeruje.
         */
        private void CleanAfterEncryption()
        {
            encryptedImage.Source = null;
            labelAmountOfNeededSpace.Content = "0";
            labelSpaceWichLeft.Content = "0";
            labelBitCapacityInBitmap.Content = "0";
            lIfBitmapWasEncrypted.Content = "NIE";
            lIfBitmapWasEncrypted.Foreground = System.Windows.Media.Brushes.Red;
            tbMultiLine.Text = "";

            pixelByteArray = null;

            amountOfBYtesInBitmap = 0;
            ascii = null;
            isEncrypted = false;
            spaceOfFileToCode = 0;
            freeSpaceInFile = 0;
            spaceWichLeftInFile = 0;
            showColumnChart();



        }
    }

}
