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
using System.Diagnostics;

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
        private const string REFERER_EXAM_SCORE = "http://zyfw.bnu.edu.cn/student/xscj.stuckcj.jsp?menucode=JW130706";

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

        private const string URL_EXAM_SCORE     // 成绩
            = "http://zyfw.bnu.edu.cn/student/xscj.stuckcj_data.jsp";

        private const string URL_DROPLIST       // 下拉列表，参见Droplists常量
            = "http://zyfw.bnu.edu.cn/frame/droplist/getDropLists.action";

        /// <summary>
        /// Droplists
        /// </summary>
        private const string DROP_EXAM_NAME = "Ms_KSSW_FBXNXQKSLC";

        /// <summary>
        /// Other fields
        /// </summary>
        private HtmlParser m_Parser = new HtmlParser();
        private CXSession m_Session = CXHttp.Session();

        private string m_Username = "", m_Password = "";
        public string Username { get => m_Username; set { m_Username = value; m_isLogined = false; m_StudentInfo = null; } }
        public string Password { get => m_Password; set { m_Password = value; m_isLogined = false; m_StudentInfo = null; } }

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

            var res = await m_Session.Req                       // GET URL_LOGIN with specific UA
               .Url(URL_LOGIN)
               .ClearCookie()
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
            if (m_StudentInfo != null)
            {
                return m_StudentInfo;
            }

            var res = await m_Session.Req
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
            var res = await m_Session.Req
                .Url(URL_GRADE_INFO)
                .Header(HEADER_USER_AGENT, USER_AGENT)
                .Header(HEADER_REFERER, REFERER)
                .Data("xh", studentId)
                .Post();

            string body = await res.Content("UTF-8");

            if (!UpdateLoginState(body))
                return null;

            RequestResult result = JsonConvert.DeserializeObject<RequestResult>(body);
            return new GradeInfo(JsonConvert.DeserializeObject<GradeInfo._GradeInfo>(result.Result));
        }

        /// <summary>
        /// Get student details
        /// </summary>
        /// <returns><see cref="StudentDetails"/></returns>
        public async Task<StudentDetails> GetStudentDetails()
        {
            var res = await m_Session.Req
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
                address:            ParserHelper.GetFirstElementText(info.GetElementsByTagName("txdz")),
                avatarId:           ParserHelper.GetFirstElementText(info.GetElementsByTagName("zpid")),
                birthday:           ParserHelper.GetFirstElementText(info.GetElementsByTagName("csrq")),
                className:          ParserHelper.GetFirstElementText(info.GetElementsByTagName("bjmc")),
                college:            ParserHelper.GetFirstElementText(info.GetElementsByTagName("yxb")),
                collegeWill:        ParserHelper.GetFirstElementText(info.GetElementsByTagName("zymc")),
                cultureStandard:    ParserHelper.GetFirstElementText(info.GetElementsByTagName("whcd")),
                educationLevel:     ParserHelper.GetFirstElementText(info.GetElementsByTagName("pycc")),
                email:              ParserHelper.GetFirstElementText(info.GetElementsByTagName("dzyx")),
                gaokaoId:           ParserHelper.GetFirstElementText(info.GetElementsByTagName("gkksh")),
                gender:             ParserHelper.GetFirstElementText(info.GetElementsByTagName("xb")),
                id:                 ParserHelper.GetFirstElementText(info.GetElementsByTagName("yhxh")),
                idNumber:           ParserHelper.GetFirstElementText(info.GetElementsByTagName("sfzjh")),
                middleSchool:       ParserHelper.GetFirstElementText(info.GetElementsByTagName("sydw")),
                mobile:             ParserHelper.GetFirstElementText(info.GetElementsByTagName("dh")),
                name:               ParserHelper.GetFirstElementText(info.GetElementsByTagName("xm")),
                nationality:        ParserHelper.GetFirstElementText(info.GetElementsByTagName("mz")),
                number:             ParserHelper.GetFirstElementText(info.GetElementsByTagName("xh")),
                pinyin:             ParserHelper.GetFirstElementText(info.GetElementsByTagName("xmpy")),
                registrationGrade:  ParserHelper.GetFirstElementText(info.GetElementsByTagName("rxnj")),
                registrationTime:   ParserHelper.GetFirstElementText(info.GetElementsByTagName("bdtime")),
                schoolSystem:       ParserHelper.GetFirstElementText(info.GetElementsByTagName("xz")),
                speciality:         ParserHelper.GetFirstElementText(info.GetElementsByTagName("lqzy"))
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

            var res = await m_Session.Req
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
            string body = "";
            body = res.Content("GBK").Result;

            var doc = m_Parser.Parse(body);

            // Init error message
            string error = "登录失败";

            // If no "KINGOSOFT高校数字校园综合管理平台" found, then there will be errors
            if (!body.Contains("KINGOSOFT高校数字校园综合管理平台"))
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

        /// <summary>
        /// Get exam rounds
        /// </summary>
        /// <returns>A list of <see cref="ExamRound"/></returns>
        public async Task<List<ExamRound>> GetExamRounds()
        {
            var res = await m_Session.Req
                .Url(URL_DROPLIST)
                .Header(HEADER_USER_AGENT, USER_AGENT)
                .Header(HEADER_REFERER, REFERER)
                .Data("comboBoxName", DROP_EXAM_NAME)
                .Post();

            string body = await res.Content("UTF-8");

            if (!UpdateLoginState(body))
                return null;

            var list = JsonConvert.DeserializeObject<List<ExamRound>>(body);
            return list.OrderByDescending(o => o.Code).ToList();
        }

        /// <summary>
        /// Get exam scores
        /// </summary>
        /// <param name="year">Specific which year to query. If 0 is given, all scores will be returned.</param>
        /// <param name="term">Specific which term to query.</param>
        /// <param name="isOnlyMajor">Specific whether scores of minor profession will be returned.</param>
        /// <returns>A list of <see cref="ExamScore"/></returns>
        public async Task<List<ExamScore>> GetExamScores(int year, int term, bool isOnlyMajor)
        {
            var req = m_Session.Req
                .Url(URL_EXAM_SCORE)
                .Header(HEADER_USER_AGENT, USER_AGENT)
                .Header(HEADER_REFERER, REFERER_EXAM_SCORE)
                .Data("ysyx", "yscj")
                .Data("userCode", (await FetchStudentInfo()).StudentId)
                .Data("zfx", isOnlyMajor ? "0" : "1")
                .Data("ysyxS", "on")
                .Data("sjxzS", "on")
                .Data("zfxS", "on");

            if (year == 0)
            {
                // If year == 0, Then get all exam scores
                req.Data("sjxz", "sjxz1");
            }
            else
            {
                // Else get scores for specific semester
                req.Data("sjxz", "sjxz3")
                    .Data("xn", year.ToString())
                    .Data("xn1", (year + 1).ToString())
                    .Data("xq", term.ToString());
            }
            var res = await req.Post();

            string body = await res.Content("GBK");

            if (!UpdateLoginState(body))
                return null;

            var doc = m_Parser.Parse(body);

            var scores = new List<ExamScore>();

            if (doc.GetElementsByTagName("tbody").Count() == 0)
            {
                // No result
                return scores;
            }

            var table = doc.GetElementsByTagName("tbody")[0];
            var rows = table.GetElementsByTagName("tr");
            string lastTerm = "";

            foreach (var tr in rows)
            {
                var cols = tr.GetElementsByTagName("td");
                string currentTerm = cols[0].TextContent.Trim();
                if (currentTerm == "")
                {
                    currentTerm = lastTerm;
                }
                else
                {
                    lastTerm = currentTerm;
                }

                if (cols.Count() < 9) continue;

                scores.Add(new ExamScore(
                        term:                   currentTerm,
                        courseName:             cols[1].TextContent,
                        courseCredit:           cols[2].TextContent,
                        classification:         cols[3].TextContent,
                        score1:                 cols[5].TextContent,
                        score2:                 cols[6].TextContent,
                        score:                  cols[7].TextContent,
                        doLearnForFirstTime:    "初修" == cols[4].TextContent.Trim(),
                        isMajor:                "主修" == cols[8].TextContent.Trim()
                ));
            }

            scores.OrderByDescending(o => o.Score);
            return scores;
        }

        public async Task<List<ExamScore>> GetExamScores(bool isOnlyMajor)
        {
            return await GetExamScores(0, 0, isOnlyMajor);
        }
    }

}
