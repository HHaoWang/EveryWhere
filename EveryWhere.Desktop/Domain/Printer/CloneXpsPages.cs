using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

namespace EveryWhere.Desktop.Domain.Printer;

/// <summary>
/// Creates a copy of XPS pages and outputs them to a new XPS file.
/// </summary>
/// <remarks>
/// Creates a new copy of each page so that the copied pages can be saved to the new XPS file.
/// If you simply added the pages to the new file an exception occurs when trying to save as the
/// pages are still attached to their original parent.
/// <example>
/// To copy pages from a source file (VB.Net);
/// <code>
///Sub CopyPagesFromXps(ByVal SourceXpsPath As String, ByVal StartPage As Integer, ByVal PageCount As Integer, ByVal OutputXpsPath As String)
/// Dim XpsPageCopier As CloneXpsPages
///
/// XpsPageCopier = New CloneXpsPages()
///
/// Using SrcXpsDoc As New XpsDocument(SourceXpsPath, FileAccess.Read)
/// Dim FixedDocSeq As Windows.Documents.FixedDocumentSequence = SrcXpsDoc.GetFixedDocumentSequence()
/// Dim FixedDoc As Windows.Documents.FixedDocument = FixedDocSeq.References(0).GetDocument(False)
///
/// ' Assumes you have passed in a document with at least 4 pages.
/// XpsPageCopier.Pages.Add(FixedDoc.Pages(1)) ' Copy page 2.
/// XpsPageCopier.Pages.Add(FixedDoc.Pages(3)) ' Copy page 4.
/// End Using
///
/// XpsPageCopier.SavePath = OutputXpsPath
/// XpsPageCopier.Save()
/// XpsPageCopier = Nothing
///
///End Sub
/// </code>
/// </example>
/// </remarks>
public class CloneXpsPages
{

    #region " Properties "

    /// <summary>
    /// 文件保存路径，需要在调用Save方法前设置好
    /// </summary>
    public string SavePath { get; set; }

    /// <summary>
    /// Collection to add PageContent instances to.
    /// </summary>
    public List<PageContent> Pages = new();

    #endregion

    #region " Methods "

    /// <summary>
    /// Save XPS document to disk with pages from the Pages collection.
    /// </summary>
    public void Save()
    {

        if (string.IsNullOrEmpty(SavePath))
        {
            throw new ArgumentException("SavePath has not been specified");
        }

        using XpsDocument xpsOutputDoc = new XpsDocument(SavePath, FileAccess.ReadWrite);
        FixedDocumentSequence fixedDocSequence = new FixedDocumentSequence();

        DocumentReference docRef = new DocumentReference();
        CopyPages(docRef, Pages);

        FixedDocument fixedDoc = docRef.GetDocument(true)!;
        fixedDocSequence.References.Add(docRef);

        XpsDocumentWriter xpsDocWriter = XpsDocument.CreateXpsDocumentWriter(xpsOutputDoc);
        xpsDocWriter.Write(fixedDocSequence);
        xpsOutputDoc.Close();
    }

    #endregion

    /// <summary>
    /// Copy pages from the Pages collection to the passed DocumentReference instance.
    /// By copying in this manner we unlink the parent object of the source page so that
    /// the new copy can be attached to the new XPS file.
    /// </summary>
    /// <param name="docRef">Document reference to attach the pages to.</param>
    /// <param name="pagesToCopy">A list of pages to attach to the passed document reference.</param>
    /// <remarks>If pages are copied directly between the XPS files then exceptions occur as the source
    /// page parent will need detaching first. Hence the need for this method.</remarks>
    private void CopyPages(DocumentReference docRef, List<PageContent> pagesToCopy)
    {
        FixedDocument fixedDoc = new FixedDocument();
        docRef.SetDocument(fixedDoc);
        foreach (PageContent pageItem in pagesToCopy)
        {
            PageContent pageContent = new()
            {
                Source = pageItem.Source
            };
            (pageContent as IUriContext).BaseUri = ((IUriContext)pageItem).BaseUri;
            pageContent.GetPageRoot(true);
            fixedDoc.Pages.Add(pageContent);
        }
    }
}
