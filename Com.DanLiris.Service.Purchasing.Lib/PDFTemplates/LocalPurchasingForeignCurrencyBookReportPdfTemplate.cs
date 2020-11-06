﻿using Com.DanLiris.Service.Purchasing.Lib.ViewModels.UnitReceiptNote;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Com.DanLiris.Service.Purchasing.Lib.PDFTemplates
{
    public static class LocalPurchasingForeignCurrencyBookReportPdfTemplate
    {
        private static readonly Font _headerFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 18);
        private static readonly Font _subHeaderFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 16);
        private static readonly Font _normalFont = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 9);
        private static readonly Font _smallFont = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
        private static readonly Font _smallerFont = FontFactory.GetFont(BaseFont.HELVETICA, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 7);
        private static readonly Font _normalBoldFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 9);
        private static readonly Font _smallBoldFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 8);
        private static readonly Font _smallerBoldFont = FontFactory.GetFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED, 7);

        public static MemoryStream Generate(LocalPurchasingBookReportViewModel viewModel, int timezoneOffset, DateTime? dateFrom, DateTime? dateTo)
        {
            var d1 = dateFrom.GetValueOrDefault().ToUniversalTime();
            var d2 = (dateTo.HasValue ? dateTo.Value : DateTime.Now).ToUniversalTime();

            var document = new Document(PageSize.A4.Rotate(), 5, 5, 25, 25);
            var stream = new MemoryStream();
            PdfWriter.GetInstance(document, stream);
            document.Open();

            SetHeader(document, d1, d2, timezoneOffset);

            SetReportTable(document, viewModel, timezoneOffset);

            //document.Add(new Paragraph("\n"));

            //SetCategoryCurrencySummaryTable(document, viewModel.CategorySummaries, viewModel.CategorySummaryTotal, viewModel.CurrencySummaries);

            //SetCategoryTable(document, viewModel.CategorySummaries, viewModel.CategorySummaryTotal);

            //SetCurrencyTable(document, viewModel.CurrencySummaries);

            //SetFooter(document);

            document.Close();
            byte[] byteInfo = stream.ToArray();
            stream.Write(byteInfo, 0, byteInfo.Length);
            stream.Position = 0;

            return stream;
        }

        //private static void SetCategoryCurrencySummaryTable(Document document, List<Summary> categorySummaries, decimal categorySummaryTotal, List<Summary> currencySummaries)
        //{
        //    var table = new PdfPTable(3)
        //    {
        //        WidthPercentage = 95,

        //    };

        //    var widths = new List<float>() { 6f, 1f, 3f };
        //    table.SetWidths(widths.ToArray());

        //    table.AddCell(GetCategorySummaryTable(categorySummaries, categorySummaryTotal));
        //    table.AddCell(new PdfPCell() { Border = Rectangle.NO_BORDER });
        //    table.AddCell(GetCurrencySummaryTable(currencySummaries));

        //    document.Add(table);
        //}

        private static PdfPCell GetCurrencySummaryTable(List<Summary> currencySummaries)
        {
            var table = new PdfPTable(3)
            {
                WidthPercentage = 100
            };

            var widths = new List<float>() { 1f, 1f, 1f };
            table.SetWidths(widths.ToArray());

            // set header
            var cell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            cell.Phrase = new Phrase("Mata Uang", _smallerFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Total", _smallerFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Total (IDR)", _smallerFont);
            table.AddCell(cell);

            foreach (var currency in currencySummaries)
            {
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Phrase = new Phrase(currency.CurrencyCode, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Phrase = new Phrase(string.Format("{0:n}", currency.SubTotal), _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Phrase = new Phrase(string.Format("{0:n}", currency.SubTotalCurrency), _smallerFont);
                table.AddCell(cell);
            }

            return new PdfPCell(table) { Border = Rectangle.NO_BORDER };
        }

        private static PdfPCell GetCategorySummaryTable(List<Summary> categorySummaries, decimal categorySummaryTotal)
        {
            var table = new PdfPTable(2)
            {
                WidthPercentage = 100
            };

            var widths = new List<float>() { 1f, 2f };
            table.SetWidths(widths.ToArray());

            // set header
            var cell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            cell.Phrase = new Phrase("Kategori", _smallerFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Total (IDR)", _smallerFont);
            table.AddCell(cell);

            foreach (var categorySummary in categorySummaries)
            {
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Phrase = new Phrase(categorySummary.Category, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Phrase = new Phrase(string.Format("{0:n}", categorySummary.SubTotal), _smallerFont);
                table.AddCell(cell);
            }

            cell.Phrase = new Phrase("", _smallerFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase(string.Format("{0:n}", categorySummaryTotal), _smallerFont);
            table.AddCell(cell);

            return new PdfPCell(table) { Border = Rectangle.NO_BORDER };
        }

        private static PdfPCell GetUnitSummaryTable(Dictionary<string, decimal> unitSummaries)
        {
            var table = new PdfPTable(2)
            {
                WidthPercentage = 100
            };

            var widths = new List<float>() { 1f, 2f };
            table.SetWidths(widths.ToArray());

            // set header
            var cell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            cell.Phrase = new Phrase("Unit", _smallerFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Total (IDR)", _smallerFont);
            table.AddCell(cell);

            decimal totalSummary = 0;
            foreach (var unitSummary in unitSummaries)
            {
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Phrase = new Phrase(unitSummary.Key, _smallerFont);
                table.AddCell(cell);

                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Phrase = new Phrase(string.Format("{0:n}", unitSummary.Value), _smallerFont);
                table.AddCell(cell);

                totalSummary += unitSummary.Value;
            }

            cell.Phrase = new Phrase("", _smallerFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase(string.Format("{0:n}", totalSummary), _smallerFont);
            table.AddCell(cell);

            return new PdfPCell(table) { Border = Rectangle.NO_BORDER };
        }

        //private static void SetFooter(Document document)
        //{

        //}

        //private static void SetCurrencyTable(Document document, List<Summary> currencySummaries)
        //{

        //}

        //private static void SetCategoryTable(Document document, List<Summary> categorySummaries, decimal categorySummaryTotal)
        //{

        //}

        private static void SetReportTable(Document document, LocalPurchasingBookReportViewModel viewModel, int timezoneOffset)

        {
            var table = new PdfPTable(17)
            {
                WidthPercentage = 95
            };

            var widths = new List<float>();
            for (var i = 0; i < 17; i++)
            {
                if (i == 9 || i == 8)
                {
                    widths.Add(1f);
                    continue;
                }

                if (i == 1 || i == 2)
                {
                    widths.Add(3f);
                    continue;
                }

                widths.Add(2f);
            }
            table.SetWidths(widths.ToArray());

            SetReportTableHeader(table);

            var listCategoryReports = viewModel.Reports.GroupBy(x => x.CategoryCode).ToList();

            var cell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cellAlignRight = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var categoryCell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var totalCell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var totalDPPCell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var totalUnitCell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var summaryUnit = new Dictionary<string, decimal>();

            foreach (var cat in listCategoryReports)
            {
                var categoryName = cat.Select(x => x.CategoryName).FirstOrDefault();
                categoryCell.Phrase = new Phrase(categoryName, _smallBoldFont);
                categoryCell.Colspan = 17;
                table.AddCell(categoryCell);

                decimal total = 0;
                decimal totalDPP = 0;
                decimal totalPPN = 0;
                decimal totalPPH = 0;

                var totalUnit = new Dictionary<string, decimal>();

                foreach (var data in cat)
                {
                    cell.Phrase = new Phrase(data.ReceiptDate.AddHours(timezoneOffset).ToString("yyyy-dd-MM"), _smallerFont);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(data.Remark, _smallerFont);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(data.SupplierName, _smallerFont);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(data.URNNo, _smallerFont);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(data.InvoiceNo, _smallerFont);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(data.VATNo, _smallerFont);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(data.UPONo, _smallerFont);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(data.CategoryCode + " - " + data.CategoryName, _smallerFont);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(data.UnitName, _smallerFont);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(data.CurrencyCode, _smallerFont);
                    table.AddCell(cell);

                    cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", data.Quantity), _smallerFont);
                    table.AddCell(cellAlignRight);

                    cell.Phrase = new Phrase(data.Uom, _smallerFont);
                    table.AddCell(cell);

                    cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", data.DPP), _smallerFont);
                    table.AddCell(cellAlignRight);

                    cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", data.DPPCurrency), _smallerFont);
                    table.AddCell(cellAlignRight);

                    cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", data.VAT * data.CurrencyRate), _smallerFont);
                    table.AddCell(cellAlignRight);

                    cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", data.IncomeTax * data.CurrencyRate), _smallerFont);
                    table.AddCell(cellAlignRight);
                    table.AddCell(cellAlignRight);

                    if (totalUnit.ContainsKey(data.UnitName))
                        totalUnit[data.UnitName] += data.Total;
                    else
                        totalUnit.Add(data.UnitName, data.Total);

                    totalDPP += data.DPP;
                    totalPPN += data.VAT;
                    totalPPH += data.IncomeTax;
                    total += data.Total;
                }

                //var cellGrandTotal = new PdfPCell()
                //{
                //    HorizontalAlignment = Element.ALIGN_RIGHT,
                //    VerticalAlignment = Element.ALIGN_CENTER,
                //    Colspan = 16
                //};

                //cellGrandTotal.Phrase = new Phrase("Grand Total", _smallerBoldFont);
                //table.AddCell(cellGrandTotal);

                //cellGrandTotal.Phrase = new Phrase(string.Format("{0:n}", grandTotal), _smallerBoldFont);
                //table.AddCell(cellGrandTotal);

                totalCell.Phrase = new Phrase("Total  ", _smallBoldFont);
                totalCell.Colspan = 12;
                table.AddCell(totalCell);

                totalDPPCell.Phrase = new Phrase(string.Format("{0:n}", totalDPP), _smallerFont);
                totalDPPCell.Colspan = 2;
                table.AddCell(totalDPPCell);

                cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", totalPPN), _smallerFont);
                table.AddCell(cellAlignRight);

                cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", totalPPH), _smallerFont);
                table.AddCell(cellAlignRight);

                cellAlignRight.Phrase = new Phrase(string.Format("{0:n}", total), _smallerBoldFont);
                table.AddCell(cellAlignRight);

                if (totalUnit.Count() > 0)
                    foreach (var v in totalUnit)
                    {
                        totalCell.Phrase = new Phrase($"{v.Key}  ", _smallBoldFont);
                        totalCell.Colspan = 12;
                        table.AddCell(totalCell);

                        totalUnitCell.Phrase = new Phrase(string.Format("{0:n}", v.Value), _smallFont);
                        totalUnitCell.Colspan = 5;
                        table.AddCell(totalUnitCell);

                        if (summaryUnit.ContainsKey(v.Key))
                            summaryUnit[v.Key] += v.Value;
                        else
                            summaryUnit.Add(v.Key, v.Value);
                    }
            }

            document.Add(table);

            document.Add(new Paragraph("\n"));

            var summaryTable = new PdfPTable(5)
            {
                WidthPercentage = 95,

            };

            var widthSummaryTable = new List<float>() { 2f, 1f, 2f, 1f, 4f };
            summaryTable.SetWidths(widthSummaryTable.ToArray());

            summaryTable.AddCell(GetCategorySummaryTable(viewModel.CategorySummaries, viewModel.CategorySummaryTotal));
            summaryTable.AddCell(new PdfPCell() { Border = Rectangle.NO_BORDER });
            summaryTable.AddCell(GetUnitSummaryTable(summaryUnit));
            summaryTable.AddCell(new PdfPCell() { Border = Rectangle.NO_BORDER });
            summaryTable.AddCell(GetCurrencySummaryTable(viewModel.CurrencySummaries));

            document.Add(summaryTable);
        }

        private static void SetReportTableHeader(PdfPTable table)
        {
            var cell = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER
            };

            var cellColspan4 = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER,
                Colspan = 4
            };

            var cellRowspan2 = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_CENTER,
                Rowspan = 2
            };

            cellRowspan2.Phrase = new Phrase("Tanggal", _smallerFont);
            table.AddCell(cellRowspan2);

            cellRowspan2.Phrase = new Phrase("Keterangan", _smallerFont);
            table.AddCell(cellRowspan2);

            cellRowspan2.Phrase = new Phrase("Supplier", _smallerFont);
            table.AddCell(cellRowspan2);

            cellRowspan2.Phrase = new Phrase("No. Bon Penerimaan", _smallerFont);
            table.AddCell(cellRowspan2);

            cellRowspan2.Phrase = new Phrase("No. Inv.", _smallerFont);
            table.AddCell(cellRowspan2);

            cellRowspan2.Phrase = new Phrase("No. Faktur Pajak", _smallerFont);
            table.AddCell(cellRowspan2);

            cellRowspan2.Phrase = new Phrase("No. SPB/NI", _smallerFont);
            table.AddCell(cellRowspan2);

            cellRowspan2.Phrase = new Phrase("Kategori", _smallerFont);
            table.AddCell(cellRowspan2);

            cellRowspan2.Phrase = new Phrase("Unit", _smallerFont);
            table.AddCell(cellRowspan2);

            cellRowspan2.Phrase = new Phrase("Mata Uang", _smallerFont);
            table.AddCell(cellRowspan2);

            cellRowspan2.Phrase = new Phrase("Kuantiti", _smallerFont);
            table.AddCell(cellRowspan2);

            cellRowspan2.Phrase = new Phrase("Satuan", _smallerFont);
            table.AddCell(cellRowspan2);

            cellColspan4.Phrase = new Phrase("Pembelian", _smallerFont);
            table.AddCell(cellColspan4);

            cellRowspan2.Phrase = new Phrase("Total", _smallerFont);
            table.AddCell(cellRowspan2);

            cell.Phrase = new Phrase("DPP", _smallerFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("DPP RUPIAH", _smallerFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("PPN IDR", _smallerFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("PPh", _smallerFont);
            table.AddCell(cell);
        }

        private static void SetHeader(Document document, DateTime dateFrom, DateTime dateTo, int timezoneOffset)
        {
            var table = new PdfPTable(1)
            {
                WidthPercentage = 95
            };
            var cell = new PdfPCell()
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                Phrase = new Phrase("PT DAN LIRIS", _headerFont)
            };
            table.AddCell(cell);

            cell.Phrase = new Phrase("BUKU PEMBELIAN LOKAL VALAS", _headerFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase($"Dari {dateFrom.AddHours(timezoneOffset):yyyy-dd-MM} Sampai {dateTo.AddHours(timezoneOffset):yyyy-dd-MM}", _subHeaderFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("", _headerFont);
            table.AddCell(cell);

            document.Add(table);
        }
    }
}
