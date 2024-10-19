using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenEmailer : MonoBehaviour
{
    public void SendEmailRequest()
    {
        Emailer.SendEmail();
    }
}
