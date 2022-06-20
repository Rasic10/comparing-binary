using comparing_binary.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Tests
{
    public class V1Tests
    {
        [Fact]
        public void Equal_Data()
        {
            var left = "AAAAAA==";
            var right = "AAAAAA==";
            var controller = new V1();

            var leftResult = controller.PutLeft(left);
            var rightResult = controller.PutRight(right) as StatusCodeResult;
            var result = controller.Get() as OkObjectResult;
            var resultType = result?.Value as Response;

            Assert.Equal(leftResult.StatusCode, new StatusCodeResult(201).StatusCode);
            Assert.Equal(rightResult?.StatusCode, new StatusCodeResult(201).StatusCode);
            Assert.Equal(resultType?.diffResultType, ResultType.Equals.ToString());
        }

        [Fact]
        public void Size_Do_Not_Match()
        {
            var left = "AAAAAA==";
            var right = "AAA=";
            var controller = new V1();

            var leftResult = controller.PutLeft(left);
            var rightResult = controller.PutRight(right) as StatusCodeResult;
            var result = controller.Get() as OkObjectResult;
            var resultType = result?.Value as Response;

            Assert.Equal(leftResult.StatusCode, new StatusCodeResult(201).StatusCode);
            Assert.Equal(rightResult?.StatusCode, new StatusCodeResult(201).StatusCode);
            Assert.Equal(resultType?.diffResultType, ResultType.SizeDoNotMatch.ToString());
        }

        [Fact]
        public void Non_Equal_Data()
        {
            var left = "AAAAAA==";
            var right = "AQABAQ==";
            var controller = new V1();

            var leftResult = controller.PutLeft(left);
            var rightResult = controller.PutRight(right) as StatusCodeResult;
            var result = controller.Get() as OkObjectResult;
            var resultType = result?.Value as Response;

            Assert.Equal(leftResult.StatusCode, new StatusCodeResult(201).StatusCode);
            Assert.Equal(rightResult?.StatusCode, new StatusCodeResult(201).StatusCode);
            Assert.Equal(resultType?.diffResultType, ResultType.ContentDoNotMatch.ToString());
        }
    }
}