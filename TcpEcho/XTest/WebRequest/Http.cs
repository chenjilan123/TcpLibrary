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
    }
}
