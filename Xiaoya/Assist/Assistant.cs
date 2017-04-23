using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using CXHttpNS;
using AngleSharp.Parser.Html;
using AngleSharp.Parser.Xml;
using Xiaoya.Helpers;
using Xiaoya.Assist.Model;
using Newtonsoft.Json;

namespace Xiaoya
{
    public class Assistant
    {
        /// <summary>
        /// Headers
        /// </summary>
        private const string HEADER_USER_AGENT = "User-Agent";
        private const string USER_AGENT =
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) " +
            "Chrome/55.0.2883.87 Safari/537.36";
        private const string HEADER_REFERER = "Referer";
        private const string REFERER = "http://zyfw.bnu.edu.cn";

        /// <summary>
        /// URLs
        /// </summary>
        
        private const string URL_LOGIN          // 登录
            = "http://cas.bnu.edu.cn/cas/login?service=http%3A%2F%2Fzyfw.bnu.edu.cn%2FMainFrm.html";

        private const string URL_STUDENT_INFO   // 学生信息
            = "http://zyfw.bnu.edu.cn/STU_DynamicInitDataAction.do?classPath=com.kingosoft.service" +
            ".jw.student.pyfa.CourseInfoService&xn=2015&xq_m=1";

        private const string URL_GRADE_INFO     // 年级专业信息
            = "http://zyfw.bnu.edu.cn/jw/common/getStuGradeSpeciatyInfo.action";

        private const string URL_STUDENT_DETAILS    // 学生详细资料
            = "http://zyfw.bnu.edu.cn/STU_BaseInfoAction.do?" +
            "hidOption=InitData&menucode_current=JW13020101";

        /// <summary>
        /// Other fields
        /// </summary>
        private HtmlParser m_Parser = new HtmlParser();
        private CXSession m_Session = CXHttp.Session("xiaoya");

        private string m_Username = "", m_Password = "";
        public string Username { get => m_Username; set { m_Username = value; m_isLogined = false; } }
        public string Password { get => m_Password; set { m_Password = value; m_isLogined = false; } }

        private bool m_isLogined = false;
        public bool IsLogin { get => m_isLogined; }

        private StudentInfo m_StudentInfo;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Assistant() { }

        /// <summary>
        /// Constructor with username and password
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        public Assistant(string username, string password)
        {
            m_Username = username;
            m_Password = password;
        }
        
        /// <summary>
        /// Get login parameters from the login page
        /// </summary>
        /// <returns>A 2-element array of <see cref="string"/>: [0] => lt, [1] => execution</returns>
        private async Task<string[]> FetchLoginParams()
        {
            string[] retParams = new string[2];                 // there'll be 2 params

            var res = await m_Session.req                       // GET URL_LOGIN with specific UA
               .Url(URL_LOGIN)
               .Header(HEADER_USER_AGENT, USER_AGENT)
               .Get();

            string body = await res.Content();                  // fetch the response body (html)

            // extract the value of param "lt"
            Match mc = Regex.Match(body, "input type=\"hidden\" name=\"lt\" value=\"(.*)\"");
            retParams[0] = mc.Groups[1].Value;

            // extract the value of param "execution"
            mc = Regex.Match(body, "input type=\"hidden\" name=\"execution\" value=\"(.*)\"");
            retParams[1] = mc.Groups[1].Value;

            return retParams;
        }

        /// <summary>
        /// Update and return IsLogin state according to the HTML body
        /// </summary>
        /// <param name="body">HTML body</param>
        /// <returns>A boolean value indicating login state</returns>
        private bool UpdateLoginState(string body)
        {
            if (body.Contains("统一身份认证平台"))
            {
                m_isLogined = false;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Fetch student info
        /// <list type="bullet">
        ///     <item>student_id: xh</item>
        ///     <item>grade: nj</item>
        ///     <item>major: zymc</item>
        ///     <item>major_id: zydm</item>
        ///     <item>school_year: xn</item>
        ///     <item>semester: xq_m</item>
        /// </list>
        /// </summary>
        /// <returns><see cref="StudentInfo"/><returns>
        public async Task<StudentInfo> FetchStudentInfo()
        {
            var res = await m_Session.req
                .Url(URL_STUDENT_INFO)
                .Header(HEADER_USER_AGENT, USER_AGENT)
                .Header(HEADER_REFERER, REFERER)
                .Post();

            string body = await res.Content();

            // Update login state to avoid session expiration
            if (!UpdateLoginState(body))
                return null;

            // Parse the fetched XML body
            var xmlParser = new XmlParser();
            var doc = xmlParser.Parse(body);

            string studentId, grade, major, majorId, schoolYear, semester;

            studentId = ParserHelper.GetFirstElementText(doc.GetElementsByTagName("xh"));
            grade = ParserHelper.GetFirstElementText(doc.GetElementsByTagName("nj"));
            major = ParserHelper.GetFirstElementText(doc.GetElementsByTagName("zymc"));
            majorId = ParserHelper.GetFirstElementText(doc.GetElementsByTagName("zydm"));
            schoolYear = ParserHelper.GetFirstElementText(doc.GetElementsByTagName("xn"));
            semester = ParserHelper.GetFirstElementText(doc.GetElementsByTagName("xq_m"));

            // If no major or grade info
            if (majorId == null || grade == null || majorId == "" || grade == "")
            {
                // Fetch them by another way
                GradeInfo gradeInfo = await FetchGradeInfo(studentId);
                major = gradeInfo.Major;
                majorId = gradeInfo.MajorId;
                grade = gradeInfo.Grade;
            }

            m_StudentInfo = new StudentInfo(studentId, grade, major, majorId, schoolYear, semester);
            return m_StudentInfo;
        }

        /// <summary>
        /// Fetch grade and major info
        /// </summary>
        /// <param name="studentId">Student Id</param>
        /// <returns><see cref="GradeInfo"/></returns>
        public async Task<GradeInfo> FetchGradeInfo(string studentId)
        {
            var res = await m_Session.req
                .Url(URL_GRADE_INFO)
                .Header(HEADER_USER_AGENT, USER_AGENT)
                .Header(HEADER_REFERER, REFERER)
                .Data("xh", studentId)
                .Post();

            string body = await res.Content("UTF-8");

            if (!UpdateLoginState(body))
                return null;

            Result result = JsonConvert.DeserializeObject<Result>(body);
            return new GradeInfo(JsonConvert.DeserializeObject<GradeInfo._GradeInfo>(result.result));
        }

        /// <summary>
        /// Get student details
        /// </summary>
        /// <returns><see cref="StudentDetails"/></returns>
        public async Task<StudentDetails> GetStudentDetails()
        {
            var res = await m_Session.req
                .Url(URL_STUDENT_DETAILS)
                .Header(HEADER_USER_AGENT, USER_AGENT)
                .Header(HEADER_REFERER, REFERER)
                .Post();

            string body = await res.Content();

            if (!UpdateLoginState(body))
                return null;

            var xmlParser = new XmlParser();
            var doc = xmlParser.Parse(body);

            var info = ParserHelper.GetFirstElement(doc.GetElementsByTagName("info"));
            if (info == null)
            {
                return null;
            }
            StudentDetails details = new StudentDetails(
                ParserHelper.GetFirstElementText(info.GetElementsByTagName("txdz")),
                ParserHelper.GetFirstElementText(info.GetElementsByTagName("zpid")),
                ParserHelper.GetFirstElementText(info.GetElementsByTagName("csrq")),
                ParserHelper.GetFirstElementText(info.GetElementsByTagName("bjmc")),
                ParserHelper.GetFirstElementText(info.GetElementsByTagName("yxb")),
                ParserHelper.GetFirstElementText(info.GetElementsByTagName("zymc")),
                ParserHelper.GetFirstElementText(info.GetElementsByTagName("whcd")),
                ParserHelper.GetFirstElementText(info.GetElementsByTagName("pycc")),
                ParserHelper.GetFirstElementText(info.GetElementsByTagName("dzyx")),
                ParserHelper.GetFirstElementText(info.GetElementsByTagName("gkksh")),
                ParserHelper.GetFirstElementText(info.GetElementsByTagName("xb")),
                ParserHelper.GetFirstElementText(info.GetElementsByTagName("yhxh")),
                ParserHelper.GetFirstElementText(info.GetElementsByTagName("sfzjh")),
                ParserHelper.GetFirstElementText(info.GetElementsByTagName("sydw")),
                ParserHelper.GetFirstElementText(info.GetElementsByTagName("dh")),
                ParserHelper.GetFirstElementText(info.GetElementsByTagName("xm")),
                ParserHelper.GetFirstElementText(info.GetElementsByTagName("mz")),
                ParserHelper.GetFirstElementText(info.GetElementsByTagName("xh")),
                ParserHelper.GetFirstElementText(info.GetElementsByTagName("xmpy")),
                ParserHelper.GetFirstElementText(info.GetElementsByTagName("rxnj")),
                ParserHelper.GetFirstElementText(info.GetElementsByTagName("bdtime")),
                ParserHelper.GetFirstElementText(info.GetElementsByTagName("xz")),
                ParserHelper.GetFirstElementText(info.GetElementsByTagName("lqzy"))
                );
            return details;
        }

        /// <summary>
        /// Login to the BNU universal authentication platform
        /// </summary>
        /// <returns>Error message. Returns <c>null</c> if succeed.</returns>
        public async Task<string> Login()
        {
            if (m_isLogined) return null;
            // Fetch login params needed
            string[] loginParams = await FetchLoginParams();

            var res = await m_Session.req
                .Url(URL_LOGIN)
                .Header(HEADER_USER_AGENT, USER_AGENT)
                .Data("username", m_Username)
                .Data("password", m_Password)
                .Data("code", "code")
                .Data("lt", loginParams[0])
                .Data("execution", loginParams[1])
                .Data("_eventId", "submit")
                .Post();

            // Decode html body by GBK
            string body = await res.Content("GBK");

            var doc = m_Parser.Parse(body);

            // Init error message
            string error = "登录失败";

            // If no "frmbody" found, then there will be errors
            if (doc.GetElementById("frmBody") == null)
            {
                // Get error message element: <span id="error_message_show">
                var msg = doc.GetElementById("error_message_show");
                // Element found, then assign error message
                if (msg != null)
                {
                    error = msg.TextContent;
                }
                return error;
            }
            // Otherwise, logined successfully.
            m_isLogined = true;
            return null;
        }
    }

    
}
