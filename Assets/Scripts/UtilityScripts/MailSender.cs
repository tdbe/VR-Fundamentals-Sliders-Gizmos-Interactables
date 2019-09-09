/***********************************************************************************
\file		MailSender.cs (version 1.0)
\brief		Makes it possible to send a mail from any script.
\long		To use to create a Variable of MailSender, and set the properties you want, then call the method sendEmail
\copyright Copyright 2019 Khora VR, LLC All Rights reserved.
************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public class MailSender {

	//==============================================================================
	// Fields
	//==============================================================================
	private string m_EmailAddress;
	private string m_EmailPassword;
	private List<string> m_MailRecipients = new List<string>();
	private string m_Subject;
	private string m_Message;
	private string m_Attachment;

	//==============================================================================
	// Mutator
	//==============================================================================
	public string EmailAddress { set { m_EmailAddress = value; } }
	public string EmailPassword { set { m_EmailPassword = value; } }
	public string AddMailRecipient { set { m_MailRecipients.Add(value); } }
	public string Subject { set { m_Subject = value; } }
	public string Message { set { m_Message = value; } }
	public string AttachmentPath { set { m_Attachment = value; } }

	//==============================================================================
	// Constructor
	//==============================================================================
	/// <summary>
	/// Email being sent from, the password and the recipient. Needs to be gmail for now
	/// The email should allow sending of email from unsecure apps - its a setting
	/// Sending emails from a phone requires the .api compatibility level to be .NET 2.0, and not .NET 2.0 subset
	/// </summary>
	public MailSender(string emailAddress, string emailPassword, string emailRecipient) {
		m_EmailAddress = emailAddress;
		m_EmailPassword = emailPassword;
		m_MailRecipients.Add(emailRecipient);
	}

	//==============================================================================
	// Methods
	//==============================================================================
	public void SendEmail() {
		if (m_MailRecipients.Count > 0) {
			MailMessage email = new MailMessage();
			email.From = new MailAddress(m_EmailAddress);
			for (int i = 0; i < m_MailRecipients.Count; i++) {
				email.To.Add(m_MailRecipients[i]);
			}
			email.Subject = m_Subject;
			email.Body = m_Message;
			if (m_Attachment.Length > 1) {
				System.Net.Mail.Attachment attachment;
				attachment = new System.Net.Mail.Attachment(m_Attachment);
				email.Attachments.Add(attachment);
			}

			SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
			smtpServer.Port = 587; //25
			smtpServer.Credentials = new System.Net.NetworkCredential(m_EmailAddress, m_EmailPassword) as ICredentialsByHost;
			smtpServer.EnableSsl = true;
			ServicePointManager.ServerCertificateValidationCallback =
				delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
			smtpServer.Send(email);
		}
	}

	public void ClearInfo() {
		m_EmailAddress = "";
		m_EmailPassword = "";
		m_MailRecipients.Clear();
		m_Subject = "";
		m_Message = "";
		m_Attachment = "";
	}
}