using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;

namespace Bongo.Areas.TimetableArea.Infrastructure
{
    public static class TimetableControlHelpers
    {

        public static PdfBorders GetBorders(int latest, int i, int j)
        {
            return new PdfBorders
            {
                Bottom = i == latest - 1 ? PdfPens.Black : PdfPens.Transparent,
                Left = j == -1 ? PdfPens.Black : PdfPens.Transparent,
                Right = j == 4 ? PdfPens.Black : PdfPens.Transparent,
                Top = PdfPens.Transparent

            };
        }
        public static Color GetBackColor(int i)
        {
            return i % 2 == 0 ? new PdfColor(ColorTranslator.FromHtml("#dee2e6")) : Syncfusion.Drawing.Color.White;
        }
        public static PdfFont GetFont()
        {
            // Specify the font settings
            PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Regular);

            // Set the text color
            return font;
        }
    }
}
