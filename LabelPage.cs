using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace labelAndTrain
{
    public partial class LabelPage : Form
    {
        bool mouseDown;
        Point lastLocation;
        Point startPoint;
        Point endPoint;
        Rectangle cropRect = new Rectangle();
        FileInfo[] imageFiles;
        string saveFolder;
        List<Label> labels = new List<Label>();
        Label label = new Label();
        LabelRegion labelRegion = new LabelRegion();

        public LabelPage()
        {
            InitializeComponent();
        }



        private void buttonOpenDir_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {

                    //string[] files = Directory.GetFiles(fbd.SelectedPath);

                    DirectoryInfo obj = new DirectoryInfo(fbd.SelectedPath);
                    imageFiles = obj.GetFiles();

                    //dataGridViewFiles.DataSource = imageFiles;



                    for (int i = 0; i < imageFiles.Length; i++)
                    {
                        try
                        {
                            using (Image newImage = Image.FromFile(imageFiles[i].FullName))
                            { }

                            dataGridViewFiles.Rows.Add(imageFiles[i].Name);
                            dataGridViewFiles.Rows[i].Cells[1].Value = imageFiles[i].FullName;
                            Label label = new Label();
                            label.imagePath = imageFiles[i].Name;
                            labels.Add(new Label(label));


                        }
                        catch (Exception ex)
                        {

                            Console.WriteLine("Invalid image path {0}", ex.Message);
                        }

                    }
                    dataGridViewFiles.Rows[0].Selected = true;
                    pictureBoxLabel.Image = Image.FromFile(dataGridViewFiles.Rows[0].Cells[1].Value.ToString());

                    //for (int i = 1; i < dataGridViewFiles.Columns.Count; i++)
                    //{
                    //    dataGridViewFiles.Columns[i].Visible = false;
                    //}
                    System.Windows.Forms.MessageBox.Show("Files found: " + imageFiles.Length.ToString(), "Message");

                }
            }

        }


        void AddLabelToControl()
        {
            comboBoxRects.Items.Clear();
            int cellIndex = dataGridViewFiles.SelectedCells[0].RowIndex;

            foreach (Label label in labels)
            {
                if (label.imagePath == dataGridViewFiles.Rows[cellIndex].Cells[0].Value.ToString())
                {
                    foreach (LabelRegion labelRegion in label.labelRegions)
                    {
                        comboBoxRects.Items.Add(labelRegion.rect);
                    }
                }
            }
            if (comboBoxRects.Items.Count > 0)
            {
                comboBoxRects.SelectedIndex = 0;
            }
        }

        private void buttonPrev_Click(object sender, EventArgs e)
        {
            try
            {
                int cellIndex = dataGridViewFiles.SelectedCells[0].RowIndex;

                if (dataGridViewFiles.Rows.Count > 0 && imageFiles.Length > 0 &&
                    cellIndex >= 0)
                {
                    dataGridViewFiles.ClearSelection();
                    //dataGridViewFiles.Rows[cellIndex - 1].Cells[0].Selected = true;
                    Console.WriteLine("Index {0} Count {1} selected {2} imageFiles size {3}",
                        (cellIndex + imageFiles.Count() - 1) % imageFiles.Count(),
                        dataGridViewFiles.Rows.Count, cellIndex, imageFiles.Count());
                    dataGridViewFiles.Rows[(cellIndex + imageFiles.Count() - 1) % imageFiles.Count()].Cells[0].Selected = true;

                    cellIndex = dataGridViewFiles.SelectedCells[0].RowIndex;

                    Console.WriteLine("Image path {0}", dataGridViewFiles.Rows[cellIndex].Cells[1].Value);


                    pictureBoxLabel.Image = Image.FromFile(dataGridViewFiles.Rows[cellIndex].Cells[1].Value.ToString());
                    //labels.Add(new Label l);
                    AddLabelToControl();

                    label.labelRegions.Clear();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show("Invalid image path");
            }
            
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            //int cellIndex = dataGridViewFiles.SelectedCells[0].RowIndex;


            //if (dataGridViewFiles.Rows.Count > 0 && imageFiles.Length > 0 &&
            //    cellIndex <= imageFiles.Length)
            //{
            //    dataGridViewFiles.ClearSelection();
            //    dataGridViewFiles.Rows[cellIndex + 1].Cells[0].Selected = true;
            //    pictureBoxLabel.Image = Image.FromFile(imageFiles[cellIndex].FullName);
            //}

            //if (listViewFiles.Items.Count > 0 && imageFiles.Length > 0 &&
            //    listViewFiles.Items.IndexOf(listViewFiles.SelectedItems[0]) <= imageFiles.Length)
            //{
            //    listViewFiles.Items[listViewFiles.Items.IndexOf(listViewFiles.SelectedItems[0]) + 1].Selected = true;
            //    Console.WriteLine(listViewFiles.Items.IndexOf(listViewFiles.SelectedItems[0]));
            //    pictureBoxLabel.Image = Image.FromFile(imageFiles[listViewFiles.Items.IndexOf(listViewFiles.SelectedItems[0])].FullName);
            //}

            try
            {
                int cellIndex = dataGridViewFiles.SelectedCells[0].RowIndex;

                if (dataGridViewFiles.Rows.Count > 0 && imageFiles.Length > 0 &&
                    cellIndex <= imageFiles.Length)
                {
                    dataGridViewFiles.ClearSelection();
                    //dataGridViewFiles.Rows[cellIndex - 1].Cells[0].Selected = true;
                    Console.WriteLine("Index {0} Count {1} selected {2} imageFiles size {3}",
                        (cellIndex + imageFiles.Count() + 1) % imageFiles.Count(),
                        dataGridViewFiles.Rows.Count, cellIndex, imageFiles.Count());
                    dataGridViewFiles.Rows[(cellIndex + imageFiles.Count() + 1) % imageFiles.Count()].Cells[0].Selected = true;

                    cellIndex = dataGridViewFiles.SelectedCells[0].RowIndex;

                    Console.WriteLine("Image path {0}", dataGridViewFiles.Rows[cellIndex].Cells[1].Value);


                    pictureBoxLabel.Image = Image.FromFile(dataGridViewFiles.Rows[cellIndex].Cells[1].Value.ToString());

                    AddLabelToControl();

                    label.labelRegions.Clear();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                MessageBox.Show("Invalid image path");
            }

        }

        public Point GetImageCoordinates(Point mouseCoordinates, PictureBox pictureBox, Bitmap image)
        {
            Point point = new Point();
            switch (pictureBox.SizeMode)
            {
                case PictureBoxSizeMode.StretchImage:
                    point.X = (image.Width * mouseCoordinates.X / pictureBox.Width);
                    point.Y = (image.Height * mouseCoordinates.Y / pictureBox.Height);

                    break;
            }


            return point;
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            
            if (e.Button == MouseButtons.Left)
            {
                mouseDown = true;
                startPoint.X = e.X;
                startPoint.Y = e.Y;
            }
            
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            PictureBox pictureBox = (PictureBox) sender;
            if (mouseDown == true && e.Button == MouseButtons.Left)
            {
                endPoint.X = e.X;
                endPoint.Y = e.Y;
                pictureBox.Invalidate();
            }
        }


        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {

            mouseDown = false;
            DialogResult dialogResult = MessageBox.Show("Is cropped area correct ?", "Confirmation",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (dialogResult == DialogResult.Yes)
            {
                LabelRect labelRect = new LabelRect();
                labelRect.X = (float)100 * ((float)cropRect.X / (float)pictureBoxLabel.Image.Width) / 100;
                labelRect.Y = ((float)100 * ((float)cropRect.Y / (float)pictureBoxLabel.Image.Height)) / 100;
                labelRect.Width = ((float)100 * ((float)cropRect.Width / (float)pictureBoxLabel.Image.Height)) / 100;
                labelRect.Height = ((float)100 * ((float)cropRect.Height / (float)pictureBoxLabel.Image.Height)) / 100;

                labelRegion = new LabelRegion(labelRect, rnd.Next(0, 5), rc);

                label.labelRegions.Add(new LabelRegion(labelRegion));
                
                int cellIndex = dataGridViewFiles.SelectedCells[0].RowIndex;

                foreach (Label l in labels)
                {
                    if (l.imagePath == dataGridViewFiles.Rows[cellIndex].Cells[0].Value.ToString())
                    {
                        l.labelRegions.Add(new LabelRegion(labelRegion));
                    }
                }
            }

            startPoint = new Point(0, 0);
            endPoint = new Point(0, 0);
            pictureBoxLabel.Invalidate();


        }
        Random rnd = new Random();
        Rectangle rc = new Rectangle();

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            PictureBox pictureBox = (PictureBox)sender;

            Pen pen = new Pen(Color.Orange, 2);

            rc = new Rectangle(
                    Math.Min(startPoint.X, endPoint.X),
                    Math.Min(startPoint.Y, endPoint.Y),
                    Math.Abs(endPoint.X - startPoint.X),
                    Math.Abs(endPoint.Y - startPoint.Y));


            if (rc.Width > -1 && rc.Height > -1)
            {
                e.Graphics.DrawRectangle(pen, rc);
            }

            Point topLeft = GetImageCoordinates(new Point(rc.X, rc.Y), pictureBox, (Bitmap)pictureBox.Image);
            Point bottomRight = GetImageCoordinates(new Point(rc.X + rc.Width, rc.Y + rc.Height), pictureBox, (Bitmap)pictureBox.Image);

            cropRect = new Rectangle(topLeft, new Size(bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y));

            //if (cropRect.Width % 4 != 0)
            //    cropRect.Width += (4 - (cropRect.Width % 4));

            //if (cropRect.Height % 4 != 0)
            //    cropRect.Height += (4 - (cropRect.Height % 4));

            //Console.WriteLine(pictureBox.Image.Width);

            //Console.WriteLine("Pixel value before {0}", );


            //Console.WriteLine("X in percentage {0} Y in percentage {1}", (float)100 * ((float)topLeft.X/ (float)pictureBox.Image.Width) /100, ((float)100 * ((float)topLeft.Y / (float)pictureBox.Image.Height)) /100);
            //pen = new Pen(Color.Orange, 2);
            //e.Graphics.DrawRectangle(pen, rc);

            

        }

        private void LabelPage_Load(object sender, EventArgs e)
        {
            pictureBoxLabel.MouseDown += pictureBox_MouseDown;
            pictureBoxLabel.MouseMove += pictureBox_MouseMove;
            pictureBoxLabel.MouseUp += pictureBox_MouseUp;
            pictureBoxLabel.Paint += pictureBox_Paint;
            //pictureBoxLabel.MouseWheel += PictureBoxLabel_MouseWheel;
            saveFolder = @"C:\Label";
            for (int i = 0; i < 10; i++)
            {
                comboBoxClass.Items.Add(i);
            }
            
        }

        //private void PictureBoxLabel_MouseWheel(object sender, MouseEventArgs e)
        //{
        //    if (pictureBoxLabel.Image != null)
        //    {
        //        // Mouse Wheel is Moved
        //        if (e.Delta > 0)
        //        {
        //            //PictureBox Dimensions Are range in 15
        //            if (pictureBoxLabel.Width < (10 * this.Width) && (pictureBoxLabel.Height < (10 * this.Height)))
        //            {
        //                //Change pictureBox Size and Multiply Zoomfactor
        //                pictureBoxLabel.Width = (int)(pictureBoxLabel.Width * 2);
        //                pictureBoxLabel.Height = (int)(pictureBoxLabel.Height * 2);

                        


        //                pictureBoxLabel.Invalidate();
        //                //Move Picture box
        //                //pictureBoxLabel.Top = (int)(e.Y - 1.25 * (e.Y - pictureBoxLabel.Top));
        //                //pictureBoxLabel.Left = (int)(e.X - 1.25 * (e.X - pictureBoxLabel.Left));
        //            }
        //        }
        //        else if ((pictureBoxLabel.Width > panelPbZoom.Width) && (pictureBoxLabel.Height > panelPbZoom.Height))

        //        // {  //PictureBox Dimensions Are range in 15
        //        //if (pictureBoxLabel.Width > (CableImgPanel.Width) && (pictureBoxLabel.Height > (CableImgPanel.Height)))
        //        {
        //            //Change pictureBox Size and Multiply Zoomfactor
        //            pictureBoxLabel.Width = (int)(pictureBoxLabel.Width / 2);
        //            pictureBoxLabel.Height = (int)(pictureBoxLabel.Height / 2);

                    
        //            pictureBoxLabel.Invalidate();
        //            //Move Picture box
        //            //pictureBoxLabel.Top = (int)(e.Y - 0.80 * (e.Y - pictureBoxLabel.Top));
        //            //pictureBoxLabel.Left = (int)(e.X - 0.80 * (e.X - pictureBoxLabel.Left));
        //        }

        //        //}
        //    }
        //}

        private void buttonSave_Click(object sender, EventArgs e)
        {
            //int cellIndex = dataGridViewFiles.SelectedCells[0].RowIndex;
            //File.AppendAllText(string.Format(@"{0}\{1}.txt", saveFolder, imageFiles[cellIndex].FullName), labelRegion.ToText() + Environment.NewLine);
            if (dataGridViewFiles.Rows.Count > 0)
            {
                int cellIndex = dataGridViewFiles.SelectedCells[0].RowIndex;
                label.imagePath = imageFiles[cellIndex].FullName;
            }
            else
            {
                label.imagePath = "";

            }
            pictureBoxLabel.Invalidate();

        }

       

        private void buttonSaveDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            folderDlg.ShowNewFolderButton = true;
            // Show the FolderBrowserDialog.  
            DialogResult result = folderDlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                Console.WriteLine(folderDlg.SelectedPath);
            }
        }

        

        private void pictureBoxLabel_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                if (dataGridViewFiles.Rows.Count > 0)
                {

                    int cellIndex = dataGridViewFiles.SelectedCells[0].RowIndex;


                    foreach (Label label in labels)
                    {
                        if (label.imagePath == dataGridViewFiles.Rows[cellIndex].Cells[0].Value.ToString())
                        {
                            foreach (LabelRegion labelRegion in label.labelRegions)
                            {
                                if (comboBoxRects.Items.Count > 0 && comboBoxRects.SelectedItem == labelRegion.rect)
                                {
                                    e.Graphics.DrawRectangle(Pens.Green, labelRegion.imageCoord);

                                }
                                else
                                {
                                    e.Graphics.DrawRectangle(Pens.Blue, labelRegion.imageCoord);

                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            
            
        }

        private void comboBoxClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            int cellIndex = dataGridViewFiles.SelectedCells[0].RowIndex;

            foreach (Label label in labels)
            {
                if (label.imagePath == dataGridViewFiles.Rows[cellIndex].Cells[0].Value.ToString())
                {
                    foreach (LabelRegion labelRegion in label.labelRegions)
                    {
                        if (comboBoxRects.SelectedItem == labelRegion.rect)
                        {
                            labelRegion.classId = Convert.ToInt32(comboBoxClass.SelectedItem);
                            break;
                        }
                    }
                }
            }
        }

        private void comboBoxRects_SelectedIndexChanged(object sender, EventArgs e)
        {
            int cellIndex = dataGridViewFiles.SelectedCells[0].RowIndex;

            foreach (Label label in labels)
            {
                if (label.imagePath == dataGridViewFiles.Rows[cellIndex].Cells[0].Value.ToString())
                {
                    foreach (LabelRegion labelRegion in label.labelRegions)
                    {
                        if (comboBoxRects.SelectedItem == labelRegion.rect)
                        {
                            comboBoxClass.SelectedItem = labelRegion.classId;
                            break;
                        }
                    }
                }
            }
            pictureBoxLabel.Invalidate();
        }

        private void buttonDelRegion_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Delete selected label ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                int cellIndex = dataGridViewFiles.SelectedCells[0].RowIndex;

                foreach (Label label in labels)
                {
                    if (label.imagePath == dataGridViewFiles.Rows[cellIndex].Cells[0].Value.ToString())
                    {
                        foreach (LabelRegion labelRegion in label.labelRegions)
                        {
                            if (comboBoxRects.SelectedItem == labelRegion.rect)
                            {
                                int index = label.labelRegions.FindIndex(r => r.rect == comboBoxRects.SelectedItem);

                                label.labelRegions.RemoveAt(index);
                                AddLabelToControl();
                                break;

                            }
                        }
                    }
                }
            }
        }
    }
}
