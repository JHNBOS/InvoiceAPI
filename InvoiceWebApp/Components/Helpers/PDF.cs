﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using InvoiceWebApp.Components.DataContext;
using InvoiceWebApp.Components.Entities;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InvoiceWebApp.Components.Helpers
{
    public class PDF
    {
        private InvoiceContext _context;
        private Invoice invoice = null;
        private Debtor debtor = null;
        private List<InvoiceItem> invoiceItems = null;

        public PDF()
        {
            _context = new InvoiceContext();
        }

        private async Task GetData()
        {
            try
            {
                invoice = await _context.Invoices.FirstOrDefaultAsync(s => s.InvoiceNumber == id);
                debtor = await _context.Debtors.Include(i => i.Addresses).FirstOrDefaultAsync(s => s.Id == invoice.CustomerId);
                invoiceItems = await _context.InvoiceItems.Where(s => s.InvoiceNumber == invoice.InvoiceNumber).ToListAsync();
                debtor.Addresses.First().Address = await _context.Addresses.FirstOrDefaultAsync(q => q.PostalCode == debtor.Addresses.First().PostalCode && q.Number == debtor.Addresses.First().Number);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public async Task<byte[]> CreatePDF(string id)
        {
            await this.GetData();

            var invoiceNumber = invoice.InvoiceNumber.ToString();

            //Creating PDF document
            var doc = this.InitDocument();
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, ms);
            PdfContentByte cb = writer.DirectContent;

            doc.Open();

            //--------------------- CONTENT -----------------------------// // 2480 X 3508
            ColumnText ct = new ColumnText(cb);
            Phrase companyName = new Phrase("Invoice Panel");
            ct.SetSimpleColumn(companyName, 2340, 100, 2440, 200, 15, Element.ALIGN_LEFT);
            ct.Go();

            //--------------------- CONTENT -----------------------------//

            doc.Close();
            writer.Flush();

            return ms.ToArray();
        }

        private Document InitDocument()
        {
            Document doc = new Document(PageSize.A4);
            doc.AddTitle("Hello World example");
            doc.AddSubject(String.Format("This invoice belongs to {0}. {1}", debtor.FirstName[0], debtor.LastName));
            doc.AddKeywords("Invoice, Payment");
            doc.AddCreator("Invoice Panel");

            return doc;
        }
    }
}