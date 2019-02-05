using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Tesseract;

namespace FindByColumnAndRow
{
    public static class TableSearch
    {

        /// <summary>
        ///     This gathers all the cells but only reads the text of a requested cell.
        /// </summary>
        /// <param name="origImage">the original image</param>
        /// <param name="findRow">the row to find</param>
        /// <param name="findCol">the column to find</param>
        public static Tuple<Rectangle, string> GetRectangleAndText(Image origImage, int findCol, int findRow)
        {
            try
            {
                var newColumnsList = GetCellsAndColumns(origImage);

                // mark the requested column and row and set the ROI to it
                var rowTb = findRow;
                var colTb = findCol;
                Rectangle rectangle;
                Bitmap roiImage;

                // find the requested cell by row and column
                using (var image = new Image<Bgr, byte>((Bitmap)origImage))
                {
                    rectangle = new Rectangle();
                    if (rowTb > 0 && colTb > 0)
                    {
                        var target = newColumnsList[colTb - 1];
                        rectangle = new Rectangle(target.X, target.YList[rowTb - 1],
                            target.Widths[rowTb - 1], target.Heights[rowTb - 1]);
                        image.ROI = rectangle;
                    }

                    //this is the region of interest
                    roiImage = image.ToBitmap();
                }

                //this is the returned text

                return new Tuple<Rectangle, string>(rectangle, ReadText(roiImage));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        /// <summary>
        ///     This gathers all the cells and reads the text in every available cell.
        /// </summary>
        /// <param name="origImage">the original image</param>
        public static List<Tuple<Rectangle, string>> GetAllRectanglesAndText(Image origImage)
        {
            try
            {
                var newColumnsList = GetCellsAndColumns(origImage);
                var list = new List<Tuple<Rectangle, string>>();

                using (var image = new Image<Bgr, byte>((Bitmap)origImage))
                {
                    foreach (var col in newColumnsList)
                        for (var i = 0; i < col.YList.Count; i++)
                        {
                            var rectangle = new Rectangle(col.X, col.YList[i],
                                col.Widths[i], col.Heights[i]);
                            image.ROI = rectangle;
                            var text = ReadText(image.ToBitmap());
                            list.Add(new Tuple<Rectangle, string>(rectangle, text));
                        }
                }

                return list;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// This iterates over an image of an extracted table to locate all the cells and columns.
        /// </summary>
        /// <param name="origImage">the original image</param>
        /// <returns></returns>
        private static List<PointColumn> GetCellsAndColumns(Image origImage)
        {
            try
            {
                //list to hold column and cells
                var columnsList = new List<Point>();
                var cellsList = new List<Point>();
                List<PointColumn> newColumnsList;
                using (var bitmap = GetTable(origImage))
                {
                    //find middle then look for last white line
                    var middleHeight = bitmap.Height / 2;

                    var lastX = 0;
                    // find the probable end of the table
                    for (var i = 0; i < bitmap.Width; i++)
                    {
                        var colorPix = bitmap.GetPixel(i, middleHeight);
                        if (colorPix.ToArgb() == Color.White.ToArgb()) lastX = i;
                    }

                    // find the starting position
                    var startX = 20;
                    var startY = 20;
                    var toggle = false;
                    while (true)
                    {
                        if (bitmap.GetPixel(startX, startY).ToArgb() == Color.Black.ToArgb()) break;

                        if (toggle == false)
                        {
                            startX++;
                            toggle = true;
                        }
                        else
                        {
                            startY++;
                            toggle = false;
                        }
                    }

                    // create new bitmap to draw lines on
                    var newBmp = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format32bppArgb);

                    columnsList.Add(new Point(startX, startY));
                    //prepare the image for drawing
                    using (var g = Graphics.FromImage(newBmp))
                    {
                        g.DrawImage(bitmap, 0, 0);
                    }

                    // id there a new row or column
                    var newRow = false;
                    var newCol = false;

                    // scan horizontally for points
                    for (var x = startX; x < lastX; x++)
                    {
                        // scan vertically to find lines, each line is another cell in this column
                        for (var y = startY; y < bitmap.Height; y++)
                            // if it is a line add it to the cells list as set new row to true
                            if (bitmap.GetPixel(x, y).ToArgb() == Color.White.ToArgb() && newRow == false)
                            {
                                cellsList.Add(new Point(x, y - 5));

                                newRow = true;
                            }
                            // keep looping until a black pixel is found the set newRow to true to look for another cell
                            else if (bitmap.GetPixel(x, y).ToArgb() == Color.Black.ToArgb() && newRow)
                            {
                                newRow = false;
                            }

                        // get the number of columns
                        for (var newX = x; newX < lastX; newX++)
                            // if it is a white pixel add the column to the list and set new column to true
                            if (bitmap.GetPixel(newX, startY).ToArgb() == Color.White.ToArgb() && newCol == false)
                            {
                                columnsList.Add(new Point(newX, startY));

                                newCol = true;
                            }
                            // keep looping through until a black pixel is reached then set newCol to true to look for white pixels again
                            else if (bitmap.GetPixel(newX, startY).ToArgb() == Color.Black.ToArgb() && newCol)
                            {
                                x = newX;
                                newCol = false;
                                break;
                            }
                    }

                    //only have distinct columns
                    columnsList = columnsList.Distinct().ToList();

                    // remove cells that have very close Y's as it will be an error with reading the image
                    var tmpList = new List<Point>();
                    for (var i = 0; i + 1 < cellsList.Count; i++)
                        if (cellsList[i + 1].Y - cellsList[i].Y > 10)
                            tmpList.Add(cellsList[i]);
                        else if (cellsList[i + 1].X != cellsList[i].X) tmpList.Add(cellsList[i]);

                    cellsList = tmpList;

                    // make a list of columns with their respective cells
                    var columns = new List<PointColumn>();
                    foreach (var col in columnsList)
                    {
                        var column = new PointColumn(col.X);
                        foreach (var cell in cellsList)
                            if (Math.Abs(cell.X - column.X) < 10)
                                column.YList.Add(cell.Y);

                        columns.Add(column);
                    }

                    // turn each point into the ROI of a cell
                    newColumnsList = new List<PointColumn>();

                    // for each column find the bounding box of the cell
                    foreach (var col in columns)
                    {
                        var pc = new PointColumn();
                        foreach (var cell in col.YList)
                        {
                            var point = new Point(col.X, cell);
                            var x = point.X;
                            var y = point.Y;
                            var left = 0;
                            var top = 0;
                            var width = 0;
                            var height = 0;
                            var foundX = false;
                            var findingRectangle = false;
                            var findWidth = false;
                            // goes to the top left point, records it, then finds the width by going right and the height by going down
                            while (true)
                                if (!findingRectangle)
                                {
                                    if (!foundX)
                                    {
                                        if (bitmap.GetPixel(x, y).ToArgb() != Color.White.ToArgb())
                                        {
                                            x--;
                                        }
                                        else
                                        {
                                            x++;
                                            left = x;
                                            foundX = true;
                                        }
                                    }
                                    else
                                    {
                                        if (y >= bitmap.Height || y < 0)
                                        {
                                            pc = null;
                                            break;
                                        }

                                        if (bitmap.GetPixel(left, y).ToArgb() != Color.White.ToArgb())
                                        {
                                            y--;
                                        }
                                        else
                                        {
                                            y++;
                                            top = y;
                                            findingRectangle = true;
                                            x = left;
                                            y = top;
                                        }
                                    }
                                }
                                else
                                {
                                    if (!findWidth)
                                    {
                                        if (x >= bitmap.Width || x < 0)
                                        {
                                            pc = null;
                                            break;
                                        }

                                        if (bitmap.GetPixel(x, y).ToArgb() != Color.White.ToArgb())
                                        {
                                            x++;
                                            width++;
                                        }
                                        else
                                        {
                                            x--;
                                            width--;
                                            findWidth = true;
                                        }
                                    }
                                    else
                                    {
                                        if (bitmap.GetPixel(x, y).ToArgb() != Color.White.ToArgb())
                                        {
                                            y++;
                                            height++;
                                        }
                                        else
                                        {
                                            height--;
                                            if (pc != null)
                                            {
                                                pc.X = left;
                                                pc.YList.Add(top);
                                                pc.Widths.Add(width);
                                                pc.Heights.Add(height);
                                            }

                                            break;
                                        }
                                    }
                                }
                        }

                        newColumnsList.Add(pc);
                    }
                }

                // return all values that are not null
                newColumnsList = newColumnsList.FindAll(i => i != null);
                //if any points are negative delete them
                var filteredList = new List<PointColumn>();
                foreach (var col in newColumnsList)
                {
                    if (col.X < 0) continue;
                    var pointColumn = new PointColumn(col.X);
                    for (var i = 0; i < col.YList.Count; i++)
                        if (col.YList[i] >= 0 && col.Widths[i] >= 0 && col.Heights[i] >= 0)
                        {
                            pointColumn.YList.Add(col.YList[i]);
                            pointColumn.Widths.Add(col.Widths[i]);
                            pointColumn.Heights.Add(col.Heights[i]);
                        }

                    filteredList.Add(pointColumn);
                }

                newColumnsList = filteredList;

                return newColumnsList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        /// <summary>
        ///    Sets the resolution of the image, processes the image and gets the table.
        /// </summary>
        /// <param name="image">the original image</param>
        private static Bitmap GetTable(Image image)
        {
            try
            {
                Image<Gray, byte> bw;
                // set the resolution of the image
                using (var bitmap = new Bitmap(image))
                {
                    bitmap.SetResolution(300, 300);

                    // pre process the image
                    bw = PreProcessImage(bitmap);
                }

                // get the table
                using (var mask = ExtractTable(bw))
                {
                    return RemoveNoiseFromTable(mask);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        /// <summary>
        ///     Inverts the image and thresholds the image so it is ready for table extraction.
        /// </summary>
        /// <param name="image">The selected image</param>
        /// <returns></returns>
        private static Image<Gray, byte> PreProcessImage(Bitmap image)
        {
            // make image gray and resize
            Image<Gray, byte> bw;

            // invert and threshold the image
            using (var src = new Image<Gray, byte>(image))
            {
                using (var grayI = src.CopyBlank())
                {
                    CvInvoke.BitwiseNot(src, grayI);
                    bw = grayI.CopyBlank();
                    CvInvoke.AdaptiveThreshold(grayI, bw, 255, AdaptiveThresholdType.MeanC, ThresholdType.Binary, 19, -2);
                }
            }

            return bw;
        }

        /// <summary>
        ///     Find the horizontal and vertical strucutres of a table and combine them. This allows you to find columns and cells.
        ///     http://answers.opencv.org/question/63847/how-to-extract-tables-from-an-image/
        /// </summary>
        /// <param name="blackWhite"> PreProcessedImage</param>
        /// <returns></returns>
        private static Image<Gray, byte> ExtractTable(Image<Gray, byte> blackWhite)
        {
            const int scale = 15;
            Image<Gray, byte> mask;

            // filter the image so only horizontal lines are present
            using (var horizontal = blackWhite.Clone())
            {
                var horizontalSize = horizontal.Cols / scale;
                using (var horizontalStructure = CvInvoke.GetStructuringElement(ElementShape.Rectangle,
                    new Size(horizontalSize, 1), new Point(-1, -1)))
                {
                    CvInvoke.Erode(horizontal, horizontal, horizontalStructure, new Point(-1, -1), 1,
                        BorderType.Reflect101,
                        default(MCvScalar));
                    CvInvoke.Dilate(horizontal, horizontal, horizontalStructure, new Point(-1, -1), 1,
                        BorderType.Reflect101,
                        default(MCvScalar));
                }

                // filter the image so only vertical line are present
                using (var vertical = blackWhite.Clone())
                {
                    var verticalSize = vertical.Rows / scale;
                    using (var verticalStructure = CvInvoke.GetStructuringElement(ElementShape.Rectangle,
                        new Size(1, verticalSize), new Point(-1, -1)))
                    {
                        CvInvoke.Erode(vertical, vertical, verticalStructure, new Point(-1, -1), 1,
                            BorderType.Reflect101,
                            default(MCvScalar));
                        CvInvoke.Dilate(vertical, vertical, verticalStructure, new Point(-1, -1), 1,
                            BorderType.Reflect101,
                            default(MCvScalar));
                    }

                    // combine horizontal and vertical lines
                    mask = vertical + horizontal;
                }
            }

            return mask;
        }

        /// <summary>
        ///     Looks at the table structure and removes lines that don't belong in the table.
        /// </summary>
        /// <param name="mask"></param>
        /// <returns></returns>
        private static Bitmap RemoveNoiseFromTable(Image<Gray, byte> mask)
        {
            Bitmap newBmpX;
            List<LineSegment> pointListY;
            Bitmap newBmpY;
            using (var bitmap = mask.ToBitmap())
            {
                // List to hold points connecting lines on the x axis
                var pointListX = new List<LineSegment>();

                int counter;
                for (var y = 0; y < mask.Height; y++)
                    for (var x = 0; x < mask.Width; x++)
                    {
                        //get the pixel
                        var color = bitmap.GetPixel(x, y);
                        //if it is not a white pixel loop again again
                        if (color.ToArgb() != Color.White.ToArgb()) continue;
                        // if it is a white pixel create a new point
                        var point1 = new Point(x, y);
                        // continue along the x axis to find the last point
                        for (counter = x; counter < mask.Width; counter++)
                        {
                            // if it is white loop again otherwise break
                            if (bitmap.GetPixel(counter, y).ToArgb() == Color.White.ToArgb()) continue;
                            break;
                        }

                        // create a point at the end of the line
                        var point2 = new Point(counter, y);
                        // add it to the list of the points connecting a line
                        pointListX.Add(new LineSegment(point1.X, point1.Y, point2.X, point2.Y));
                        // set the x to the end of the current line so it can look for the next one
                        x = counter;
                    }

                // create a writable bitmap to draw the x lines on
                newBmpX = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format32bppArgb);

                using (var g = Graphics.FromImage(newBmpX))
                {
                    g.DrawImage(bitmap, 0, 0);
                }

                // for each line check if it is larger than 100 pixels, if it is continue, otherwise draw a black line over the white line
                foreach (var line in pointListX)
                {
                    var x1 = line.X1;
                    var y1 = line.Y1;
                    var x2 = line.X2;
                    if (x2 - x1 >= 100) continue;
                    for (var i = x1; i < x1 + (x2 - x1); i++) newBmpX.SetPixel(i, y1, Color.Black);
                }

                // List to hold points connecting lines on the y axis
                pointListY = new List<LineSegment>();
                for (var x = 0; x < mask.Width; x++)
                    for (var y = 0; y < mask.Height; y++)
                    {
                        // get the color of the pixel
                        var color = bitmap.GetPixel(x, y);
                        //if it is not white loop again
                        if (color.ToArgb() != Color.White.ToArgb()) continue;
                        // if it is white create a new point at the start of the line
                        var point1 = new Point(x, y);
                        // continue along the y axis until the end of the line is reached
                        for (counter = y; counter < mask.Height; counter++)
                            // if it is not a white pixel break
                            if (bitmap.GetPixel(x, counter).ToArgb() != Color.White.ToArgb())
                                break;

                        // create a new point at the end of the line
                        var point2 = new Point(x, counter);
                        // add it to the list of points connecting a line
                        pointListY.Add(new LineSegment(point1.X, point1.Y, point2.X, point2.Y));
                        y = counter;
                    }

                // create a writable image for the y lines
                newBmpY = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format32bppArgb);

                using (var g = Graphics.FromImage(newBmpY))
                {
                    g.DrawImage(bitmap, 0, 0);
                }
            }

            // for each line check if it is larger than 100 pixels, if it is continue otherwise draw over the line in black
            foreach (var points in pointListY)
            {
                var x1 = points.X1;
                var y1 = points.Y1;
                var y2 = points.Y2;
                if (y2 - y1 >= 100) continue;
                for (var i = y1; i < y1 + (y2 - y1); i++) newBmpY.SetPixel(x1, i, Color.Black);
            }

            // combine the images
            using (var widthPicture = new Image<Gray, byte>(newBmpX))
            {
                using (var heightPicture = new Image<Gray, byte>(newBmpY))
                {
                    return (widthPicture + heightPicture).ToBitmap();
                }
            }
        }

        /// <summary>
        ///     Processes the image by using an edge detection filter, thresholding it, and then subtracting one from the other. Then Tesseract is used on the image to find the text.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private static string ReadText(Bitmap image)
        {
            try
            {
                // make the image grey
                var gray = new Image<Gray, byte>(image);
                // resize the image
                gray = gray.Resize(5, Inter.Cubic);
                // use a laplace filter for edge detection
                var laplace = gray.Laplace(31);
                var threshold = laplace.CopyBlank();
                // threshold the image
                CvInvoke.Threshold(laplace, threshold, 80, 200, ThresholdType.Binary);
                // minus the threshold image by the edges in the laplace filter so it is easy to find words
                threshold = threshold - laplace;

                //---
                //var dialog = new SaveFileDialog
                //{
                //    Filter =
                //        "Bitmap Image (*.bmp)|*.bmp|GIF Image (*.gif)|*.gif|JPG Image (*.jpg)|*.jpg|PNG Image (*.png)|*.png|All files (*.*)|*.*"
                //};
                //if (dialog.ShowDialog() == DialogResult.OK)
                //{
                //    threshold.ToBitmap().Save(dialog.FileName, ImageFormat.Bmp);
                //}
                //---

                // use tesseract to find the words
                using (var engine = new TesseractEngine(@"./tessdata", "eng",
                    EngineMode.TesseractAndCube))
                {
                    using (var img = PixConverter.ToPix(threshold.ToBitmap()))
                    {
                        using (var page = engine.Process(img))
                        {
                            var text = page.GetText();
                            var textArray = text.Split('\n');
                            text = String.Join(" ", textArray);
                            return text;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        /// <summary>
        ///     hold the information about the first and last point on a line
        /// </summary>
        private class LineSegment
        {
            public LineSegment(int x1, int y1, int x2, int y2)
            {
                X1 = x1;
                Y1 = y1;
                X2 = x2;
                Y2 = y2;
            }

            public int X1 { get; }
            public int Y1 { get; }
            public int X2 { get; }
            public int Y2 { get; }
        }

        // holds a column, its cells and their respective sizes
        private class PointColumn
        {
            public PointColumn(int x)
            {
                X = x;
                YList = new List<int>();
                Widths = new List<int>();
                Heights = new List<int>();
            }

            public PointColumn()
            {
                YList = new List<int>();
                Widths = new List<int>();
                Heights = new List<int>();
            }

            public int X { get; set; }
            public List<int> YList { get; }
            public List<int> Widths { get; }
            public List<int> Heights { get; }
        }
    }
}
