using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using C1.Win.C1Tile;

namespace Image_Gallery_GrapeCity
{
    public partial class ImageGallery : Form
    {
        // Declaring instance and variables -
        DataFetcher datafetch = new DataFetcher(); 
        List<ImageItem> imagesList;
        C1.C1Pdf.C1PdfDocument imagePdfDocument = new C1.C1Pdf.C1PdfDocument();
        int checkedItems = 0;

        public ImageGallery()
        {
            InitialFetch();
            InitializeComponent();
        }
        // Initial data is fetched from sampleData.json
        private async void InitialFetch()
        {
            imagesList = await datafetch.GetImageData("");
            AddTiles(imagesList);
        }
        //Event handler for panel1 Split container
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Rectangle r = _searchBox.Bounds; 
            r.Inflate(3, 3); 
            Pen p = new Pen(Color.LightGray); 
            e.Graphics.DrawRectangle(p, r);


        }
        // Event handler for Search Click
        private async void _search_Click(object sender, EventArgs e)
        {
            checkedItems = 0;
            _exportImage.Visible = false;
            _exportLocalImage.Visible = false;
            statusStrip1.Visible = true;
            imagesList = await datafetch.GetImageData(_searchBox.Text); 
            AddTiles(imagesList); 
            statusStrip1.Visible = false;

        }
        // Event handler for Add Tiles in Base64 format
        private void AddTiles(List<ImageItem> imageList)
        {
            _imageTileControl.Groups[0].Tiles.Clear();

            foreach (var imageitem in imageList)
            {
                Tile tile = new Tile();
                tile.HorizontalSize = 2; 
                tile.VerticalSize = 2;

                _imageTileControl.Groups[0].Tiles.Add(tile);

                Image img = Image.FromStream(new MemoryStream(imageitem.Base64));

                Template tl = new Template(); 
                ImageElement ie = new ImageElement(); 
                ie.ImageLayout = ForeImageLayout.Stretch; 
                tl.Elements.Add(ie); 
                tile.Template = tl; 
                tile.Image = img;

            }

        }

        // Function definition of Image to PDF Conversion
        private void ConvertToPdf(List<Image> images)
        {
            RectangleF rect = imagePdfDocument.PageRectangle;
            bool firstPage = true;
            foreach (var selectedimg in images)
            {
                if (!firstPage)
                {
                    imagePdfDocument.NewPage();
                }
                firstPage = false;

                rect.Inflate(-72, -72);
                imagePdfDocument.DrawImage(selectedimg, rect);

            }
        }
        // Event handler for Clicking Export to PDF  
        private void _exportImage_Click_1(object sender, EventArgs e)
            {
            List<Image> images = new List<Image>();
            foreach (Tile tile in _imageTileControl.Groups[0].Tiles)
            {
                if (tile.Checked)
                {
                    images.Add(tile.Image);
                }
            }
            ConvertToPdf(images);
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.DefaultExt = "pdf";
            saveFile.Filter = "PDF files (*.pdf)|*.pdf*";
            saveFile.Title = "Save an PDF File";
            if (saveFile.ShowDialog() == DialogResult.OK)
            {

                imagePdfDocument.Save(saveFile.FileName);
            }
        }


        // Event handler for Formatting PDF from Images
        private void _exportImage_Paint(object sender, PaintEventArgs e)
        {
            Rectangle r = new Rectangle(_exportImage.Location.X,
                _exportImage.Location.Y, _exportImage.Width, _exportImage.Height);             
            r.X -= 29; 
            r.Y -= 3; 
            r.Width--; 
            r.Height--; 
            Pen p = new Pen(Color.LightGray); 
            e.Graphics.DrawRectangle(p, r); 
            e.Graphics.DrawLine(p, new Point(0, 43), new Point(this.Width, 43));
        }

        // Event handler for Incrementing Checked items to keep a count of images to convert
        // into PDF
        private void _imageTileControl_TileChecked(object sender, C1.Win.C1Tile.TileEventArgs e)
        {
            checkedItems++; 
            _exportImage.Visible = true;
            _exportLocalImage.Visible = true;
        }

        // Event handler for Decrementing UnChecked items to keep a count of images to convert
        // into PDF
        private void _imageTileControl_TileUnchecked(object sender, C1.Win.C1Tile.TileEventArgs e)
        {
            checkedItems--; 
            _exportImage.Visible = checkedItems > 0;
            _exportLocalImage.Visible = checkedItems > 0;
        }

        // Event handler for Designing Format of Tiles 
        private void _imageTileControl_Paint(object sender, PaintEventArgs e)
        {
            Pen p = new Pen(Color.LightGray); 
            e.Graphics.DrawLine(p, 0, 43, 800, 43);
        }

        // Event handler for Clicking Export To Local System (converting in PNG format) 
        private void _exportLocalImage_Click(object sender, EventArgs e)
        {
            List<Image> images = new List<Image>();
            
            foreach (Tile tile in _imageTileControl.Groups[0].Tiles)
            {
                if (tile.Checked)
                {
                    // images.Add(tile.Image);
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                    saveFileDialog1.Filter = "PNG | *.png";
                    saveFileDialog1.Title = "Save an Image File";
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {

                        tile.Image.Save(saveFileDialog1.FileName);
                    }
                }
            }
        
        }
    }
}
