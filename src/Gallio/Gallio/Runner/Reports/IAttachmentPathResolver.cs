namespace Gallio.Runner.Reports
{
    internal interface IAttachmentPathResolver {
        string GetAttachmentPath(string testStepId, string attachmentName, string mimeType);
    }
}
