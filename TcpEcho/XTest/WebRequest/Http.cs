using System;
using System.Collections.Generic;
using System.Text;
using Test.Requests;
using Xunit;

namespace XTest.WebRequest
{
    public class Http
    {
        [Theory]
        [InlineData("http://localhost:5094/Student/Create?value=123456789abcdefghijklmn", "Hello World!")]
        public void HttpWebRequestTest(string url, string value)
        {
            HttpReq.QuestPost(url, value);


        }

        [Theory]
        [InlineData("https://localhost:44338/Home/RedirectTo", true)]
        //[InlineData("https://localhost:44338/Home/RedirectTo", false)]
        public void HttpWebRequestRedirect(string url, bool allowAutoRedirect)
        {
            HttpReq.HttpRedirect(url, allowAutoRedirect);
        }
    }
}
