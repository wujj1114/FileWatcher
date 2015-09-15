using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net.Mime;
using System.IO;

namespace FileWatcher
{
    public class Mail
    {
        public void Send(string mailServerName, string uid, string pwd, string from, string fromName, string to, string toName, string subject, string body, List<string> attachmentpath)
        {
            Encoding code = Encoding.GetEncoding("UTF-8");
            MailAddress mf = new MailAddress(from, fromName, code);
            MailAddress mt = new MailAddress(to, toName, code);

            MailMessage message = new MailMessage(mf, mt);
            message.SubjectEncoding = code;
            message.Subject = subject;
            message.BodyEncoding = code;
            message.IsBodyHtml = true;
            message.Body = "<html><head><meta http-equiv=Content-Type content=\"text/html; charset=UTF-8\"></head><body>" + body + "</body></html>";
            //message.Body = body;

            if (attachmentpath != null)
            {
                foreach (string path in attachmentpath)
                {
                    Attachment data = new Attachment(path, MediaTypeNames.Application.Octet);
                    // Add time stamp information for the file.
                    ContentDisposition disposition = data.ContentDisposition;
                    disposition.CreationDate = File.GetCreationTime(path);
                    disposition.ModificationDate = File.GetLastWriteTime(path);
                    disposition.ReadDate = File.GetLastAccessTime(path);

                    string displayName = Path.GetFileName(path);
                    displayName = displayName.Substring(displayName.IndexOf('_') + 1);
                    data.Name = displayName;
                    data.NameEncoding = code;
                    message.Attachments.Add(data);
                }
            }
            SmtpClient mailClient = new SmtpClient(mailServerName);
            mailClient.UseDefaultCredentials = false;
            mailClient.Credentials = new System.Net.NetworkCredential(uid, pwd);
            try
            {
                mailClient.Send(message);
            }
            catch (Exception ex)
            {
                throw new Exception("Mail.Send:" + ex.Message);
            }
        }
    }
}
