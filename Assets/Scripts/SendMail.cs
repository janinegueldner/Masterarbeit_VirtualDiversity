using System;
using System.Net;
using System.Net.Mail;
using UnityEngine;

public static class Emailer
{
    public static void SendEmail()
    {
        try
        {
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587;
            string mailAddress = "nine04635@gmail.com";
            string smtpPass = "bqui gzqk mpym hdct"; // "uxznspwgimeehink";

            // Get the ToTextFile component
            ToTextFile toTextFileComponent = GameObject.Find("Scene Controller").GetComponent<ToTextFile>();
            Debug.Log("vr log in send email" + toTextFileComponent.GetLogDate());

            MailMessage mail = new MailMessage();
            SmtpClient smtpClient = new SmtpClient(smtpServer);

            mail.From = new MailAddress(mailAddress);
            mail.To.Add(mailAddress);
            mail.Subject = toTextFileComponent.GetLogDate();

            mail.Body = toTextFileComponent.GetVRLog();
            mail.IsBodyHtml = false;

            smtpClient.Port = smtpPort;
            smtpClient.Credentials = new NetworkCredential(mailAddress, smtpPass);
            smtpClient.EnableSsl = true;

            smtpClient.Send(mail);
            Debug.Log("Email sent successfully.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to send email: {ex.Message}");
        }
    }
}