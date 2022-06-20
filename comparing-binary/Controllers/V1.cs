using Microsoft.AspNetCore.Mvc;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace comparing_binary.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class V1 : ControllerBase
    {
        private static string left = "";
        private static string right = "";

        [HttpGet("diff/1")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            if (string.IsNullOrWhiteSpace(left) || string.IsNullOrWhiteSpace(right))
                return NotFound();

            if (left.Length != right.Length)
                return Ok(new Response() { diffResultType = ResultType.SizeDoNotMatch.ToString() });

            if (string.Equals(left, right))
                return Ok(new Response() { diffResultType = ResultType.Equals.ToString() });

            var response = calculateDiffs(left, right);

            return Ok(response);
        }

        [HttpPut("diff/1/left")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public StatusCodeResult PutLeft(string data)
        {
            if (string.IsNullOrEmpty(data))
                return StatusCode(400);

            var bin = EncodeToBinary(data);
            if (bin == null)
                return BadRequest();

            left = bin;

            return StatusCode(201);
        }

        [HttpPut("diff/1/right")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult PutRight(string data)
        {
            if (string.IsNullOrEmpty(data))
                return BadRequest();

            var bin = EncodeToBinary(data);
            if (bin == null)
                return BadRequest();

            right = bin;

            return StatusCode(201);
        }

        private string? EncodeToBinary(string data)
        {
            try
            {
                string decoded = Encoding.UTF8.GetString(Convert.FromBase64String(data));
                return GetBytesAsString(decoded);
            }
            catch
            {
                return null;
            }
        }

        private string GetBytesAsString(string s)
        {
            StringBuilder stringBuilder = new StringBuilder();

            var chars = s.ToCharArray();
            foreach (char c in chars)
            {
                stringBuilder.Append(((int)c).ToString("x"));
            }

            return stringBuilder.ToString();
        }

        private Response calculateDiffs(string left, string right)
        {
            var diffs = new List<Diff>();

            var diff = new Diff();

            for (int i = 0; i < left.Length; i++)
            {
                if (left[i] != right[i])
                {
                    if (diff.Offset == null)
                        diff.Offset = i;

                    if (diff.Length == null)
                        diff.Length = 1;
                    else
                        diff.Length++;
                }
                else
                {
                    if (diff.Offset != null && diff.Length != null)
                    {
                        diffs.Add(diff);

                        diff = new Diff();
                    }
                }
            }

            if (diff.Offset != null && diff.Length != null)
            {
                diffs.Add(diff);
            }

            return new Response() { diffResultType = ResultType.ContentDoNotMatch.ToString(), diffs = diffs };
        }
    }

    public class Response
    {
        public string diffResultType { get; set; }
        public List<Diff> diffs { get; set; }
    }

    public class Diff
    {
        public int? Offset { get; set; }
        public int? Length { get; set; }

        public Diff()
        {
            Offset = null;
            Length = null;
        }
    }

    public enum ResultType
    {
        Equals,
        SizeDoNotMatch,
        ContentDoNotMatch,
    }
}