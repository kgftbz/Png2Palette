using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;

namespace MoO2_Png2Pallete
{
    class Program
    {
        static void Main(string[] args)
        {
            Bitmap _img;
            int _widRow, heiRow;
            Color _col;
            //Dictionary<Color, int> _usedColors = new Dictionary<Color, int>();  //hard to manage the sorting
            List<Color> _usedColors = new List<Color>(); //lazy method but makes sorting easier
            List<int> _usedAmount = new List<int>();
            Action<Color> _AddToColor = delegate (Color color)
            {
                for (int i = 0; i < _usedColors.Count; i++)
                {
                    if (_usedColors[i] == color)
                    {
                        _usedAmount[i]++;
                        return;
                    }
                }
                _usedColors.Add(color);
                _usedAmount.Add(1);
            };
            List<Color> _mostUsedColors = new List<Color>();
            int _prevLarger = 0;
            int _largerIndex = 0;



            Console.WriteLine("(.pal) Pallete file maker from png image, by kgftbz");

            if (args.Length == 0 || !args[0].ToLower().EndsWith(".png"))
            {
                Console.WriteLine("Drag and drop a PNG file on the EXE file.");
                Console.WriteLine("Press the Any key to close");
                Console.ReadKey();
                return;
            }


            //read the file
            _img = new Bitmap(args[0]);

            //for each Width and Height row
            for (_widRow = 0; _widRow < _img.Width; _widRow++)
            {
                for (heiRow = 0; heiRow < _img.Height; heiRow++)
                {
                    //get the pixel's color and add it or +1 to the list of most used colors
                    _col = _img.GetPixel(_widRow, heiRow);

                    _AddToColor(_col);
                }
            }

            //get the 256 most used colors into _mostUsedColors
            for (int i = 0; i < 256; i++)
            {
                //sort the colors into a new List
                if (_usedAmount.Count > (256 - i))
                {
                    for (int ii = 0; ii < _usedAmount.Count; ii++)
                    {
                        if (_usedAmount[ii] > _prevLarger)
                        {
                            _prevLarger = _usedAmount[ii];
                            _largerIndex = ii;
                        }
                    }
                    _mostUsedColors.Add(_usedColors[_largerIndex]);
                    _usedColors.RemoveAt(_largerIndex);
                    _usedAmount.RemoveAt(_largerIndex);
                    _prevLarger = 0;
                    _largerIndex = 0;
                }
                else
                {//add empty color for remaining pallete entries
                    _mostUsedColors.Add(Color.Black);
                }
            }

            //make a pallete with these 256 colors
            string _filename = Path.GetFileName(args[0]).ToLower().Replace(".png", ".pal");
            Byte[] _fileBytes = new byte[1048];
            Byte[] _headerBytes = new byte[]{0x52, 0x49, 0x46, 0x46, 0x10, 0x04, 0x00, 0x00, 0x50, 0x41, 0x4c, 0x20, 0x64, 0x61, 0x74, 0x61,
                                             0x04, 0x04, 0x00, 0x00, 0x00, 0x03, 0x00, 0x01};

            for (int i = 0; i < _headerBytes.Length; i++)
                _fileBytes[i] = _headerBytes[i];

            int _currentColor = 0;
            for (int i = _headerBytes.Length; i < 1048;)
            {
                _fileBytes[i++] = _mostUsedColors[_currentColor].R;
                _fileBytes[i++] = _mostUsedColors[_currentColor].G;
                _fileBytes[i++] = _mostUsedColors[_currentColor].B;
                _fileBytes[i++] = 0; //_mostUsedColors[_currentColor].A;
                _currentColor++;
            }

            File.WriteAllBytes(_filename, _fileBytes);

            Console.WriteLine("-- Completed");
            Console.WriteLine("Created file: " + _filename);
            Console.ReadKey();
        }
    }
}
